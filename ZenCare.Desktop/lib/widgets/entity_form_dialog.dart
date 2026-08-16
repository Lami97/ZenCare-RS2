import 'package:flutter/material.dart';

import '../models/admin_models.dart';
import '../services/admin_repository.dart';
import '../utils/api_exception.dart';

class EntityFormDialog extends StatefulWidget {
  const EntityFormDialog({
    super.key,
    required this.module,
    required this.repository,
    this.initialData,
  });

  final AdminModule module;
  final AdminRepository repository;
  final Map<String, dynamic>? initialData;

  @override
  State<EntityFormDialog> createState() => _EntityFormDialogState();
}

class _EntityFormDialogState extends State<EntityFormDialog> {
  final _formKey = GlobalKey<FormState>();
  final _controllers = <String, TextEditingController>{};
  final _values = <String, dynamic>{};
  final _lookups = <String, List<LookupOption>>{};
  String? _loadError;
  String? _saveError;
  bool _loadingLookups = true;
  bool _saving = false;

  bool get _isEdit => widget.initialData != null;

  @override
  void initState() {
    super.initState();
    _initializeValues();
    _loadLookups();
  }

  @override
  void dispose() {
    for (final controller in _controllers.values) {
      controller.dispose();
    }
    super.dispose();
  }

  void _initializeValues() {
    for (final field in widget.module.fields) {
      if (_isEdit && field.createOnly) continue;
      final initial = widget.initialData?[field.key];
      switch (field.type) {
        case AdminFieldType.boolean:
          _values[field.key] = initial is bool
              ? initial
              : initial?.toString().toLowerCase() == 'true';
          break;
        case AdminFieldType.lookup:
        case AdminFieldType.status:
          _values[field.key] = initial is int
              ? initial
              : int.tryParse(initial?.toString() ?? '');
          break;
        case AdminFieldType.date:
          _controllers[field.key] = TextEditingController(
            text: _dateText(initial),
          );
          break;
        case AdminFieldType.time:
          _controllers[field.key] = TextEditingController(
            text: _timeText(initial),
          );
          break;
        default:
          _controllers[field.key] = TextEditingController(
            text: initial?.toString() ?? '',
          );
      }
    }

    if (!_isEdit && widget.module.endpoint == 'PurchaseItem') {
      _controllers['unitPrice']?.text = '0.01';
      _controllers['totalPrice']?.text = '0.01';
    }
  }

  Future<void> _loadLookups() async {
    try {
      final lookupFields = widget.module.fields
          .where((field) => field.lookup != null)
          .toList();
      for (final field in lookupFields) {
        _lookups[field.key] = await _loadLookup(field);
      }
    } on ApiException catch (error) {
      _loadError = error.message;
    } catch (_) {
      _loadError = 'Reference data could not be loaded.';
    } finally {
      if (mounted) {
        setState(() => _loadingLookups = false);
      }
    }
  }

  Future<List<LookupOption>> _loadLookup(AdminField field) {
    final dependencyValue = field.dependsOn == null
        ? null
        : _values[field.dependsOn];
    if (field.dependsOn != null && dependencyValue == null) {
      return Future.value(const []);
    }

    final lookup = field.lookup!;
    final queryParameters = <String, dynamic>{...lookup.queryParameters};
    if (field.dependencyQueryKey != null) {
      queryParameters[field.dependencyQueryKey!] = dependencyValue;
    }

    return widget.repository.lookup(
      LookupConfig(
        endpoint: lookup.endpoint,
        valueKey: lookup.valueKey,
        labelBuilder: lookup.labelBuilder,
        queryParameters: queryParameters,
      ),
    );
  }

  Future<void> _changeLookup(AdminField field, int? value) async {
    final dependentFields = widget.module.fields
        .where((candidate) => candidate.dependsOn == field.key)
        .toList();

    setState(() {
      _values[field.key] = value;
      for (final dependent in dependentFields) {
        _values[dependent.key] = null;
        _lookups[dependent.key] = const [];
      }
    });

    for (final dependent in dependentFields) {
      try {
        final options = await _loadLookup(dependent);
        if (!mounted || _values[field.key] != value) return;
        setState(() => _lookups[dependent.key] = options);
      } on ApiException catch (error) {
        if (mounted) setState(() => _saveError = error.message);
      } catch (_) {
        if (mounted) {
          setState(() => _saveError = 'Reference data could not be loaded.');
        }
      }
    }
  }

  Future<void> _save() async {
    setState(() => _saveError = null);
    if (!_formKey.currentState!.validate()) return;
    final request = <String, dynamic>{};
    for (final field in widget.module.fields) {
      if (_isEdit && field.createOnly) continue;
      final value = _readValue(field);
      if (value == null || (value is String && value.trim().isEmpty)) continue;
      request[_toRequestKey(field.key)] = value;
    }

    setState(() => _saving = true);
    try {
      final id = widget.initialData?['id'] is int
          ? widget.initialData!['id'] as int
          : int.tryParse(widget.initialData?['id']?.toString() ?? '');
      if (_isEdit && id != null) {
        await widget.repository.update(widget.module, id, request);
      } else {
        await widget.repository.create(widget.module, request);
      }
      if (mounted) Navigator.of(context).pop(true);
    } on ApiException catch (error) {
      setState(() => _saveError = error.message);
    } catch (_) {
      setState(
        () => _saveError = '${widget.module.entityName} could not be saved.',
      );
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  dynamic _readValue(AdminField field) {
    switch (field.type) {
      case AdminFieldType.boolean:
      case AdminFieldType.lookup:
      case AdminFieldType.status:
        return _values[field.key];
      case AdminFieldType.integer:
        return int.tryParse(_controllers[field.key]?.text.trim() ?? '');
      case AdminFieldType.decimal:
        return double.tryParse(_controllers[field.key]?.text.trim() ?? '');
      case AdminFieldType.date:
        final text = _controllers[field.key]?.text.trim() ?? '';
        return text.isEmpty ? null : text;
      case AdminFieldType.time:
        final text = _controllers[field.key]?.text.trim() ?? '';
        return text.isEmpty ? null : (text.length == 5 ? '$text:00' : text);
      case AdminFieldType.text:
      case AdminFieldType.multiline:
        return _controllers[field.key]?.text.trim();
    }
  }

  String _toRequestKey(String key) {
    if (key.isEmpty) return key;
    return key.substring(0, 1).toUpperCase() + key.substring(1);
  }

  String? _validate(AdminField field, String? value) {
    final text = value?.trim() ?? '';
    if (field.required && text.isEmpty) return '${field.label} is required.';
    if (field.maxLength != null && text.length > field.maxLength!) {
      return '${field.label} can contain up to ${field.maxLength} characters.';
    }
    if (field.label.toLowerCase().contains('email') &&
        text.isNotEmpty &&
        !RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$').hasMatch(text)) {
      return 'Email must be in the format: user@example.com.';
    }
    if (field.key == 'phoneNumber' &&
        text.isNotEmpty &&
        !RegExp(r'^\d{9,10}$').hasMatch(text)) {
      return 'Phone number must contain 9 or 10 digits (numbers only).';
    }
    if (field.key == 'password' && text.length < 6) {
      return 'Password must contain at least 6 characters.';
    }
    if (field.key == 'passwordConfirm' &&
        text != (_controllers['password']?.text ?? '')) {
      return 'Passwords do not match.';
    }
    if (field.type == AdminFieldType.date &&
        field.disallowFutureDates &&
        text.isNotEmpty) {
      final date = DateTime.tryParse(text);
      final now = DateTime.now();
      final today = DateTime(now.year, now.month, now.day);
      if (date != null && date.isAfter(today)) {
        return '${field.label} cannot be in the future.';
      }
    }
    if ((field.type == AdminFieldType.integer ||
            field.type == AdminFieldType.decimal) &&
        text.isNotEmpty) {
      final number = num.tryParse(text);
      if (number == null) return '${field.label} must be a valid number.';
      if (field.min != null && number < field.min!) {
        return '${field.label} must be at least ${field.min}.';
      }
    }
    return null;
  }

  @override
  Widget build(BuildContext context) {
    final title = _isEdit
        ? 'Edit ${widget.module.entityName}'
        : 'Add ${widget.module.entityName}';
    return AlertDialog(
      scrollable: true,
      title: Text(title),
      content: SizedBox(
        width: 620,
        child: _loadingLookups
            ? const Padding(
                padding: EdgeInsets.all(32),
                child: Center(child: CircularProgressIndicator()),
              )
            : _loadError != null
            ? Text(
                _loadError!,
                style: TextStyle(color: Theme.of(context).colorScheme.error),
              )
            : Form(
                key: _formKey,
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    if (_saveError != null) ...[
                      Text(
                        _saveError!,
                        style: TextStyle(
                          color: Theme.of(context).colorScheme.error,
                        ),
                      ),
                      const SizedBox(height: 12),
                    ],
                    ...widget.module.fields
                        .where((field) => !(_isEdit && field.createOnly))
                        .map(_buildField),
                  ],
                ),
              ),
      ),
      actions: [
        TextButton(
          onPressed: _saving ? null : () => Navigator.of(context).pop(false),
          child: const Text('Cancel'),
        ),
        FilledButton.icon(
          onPressed: _saving || _loadError != null ? null : _save,
          icon: _saving
              ? const SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Icon(Icons.save),
          label: const Text('Save'),
        ),
      ],
    );
  }

  Widget _buildField(AdminField field) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 14),
      child: switch (field.type) {
        AdminFieldType.boolean => CheckboxListTile(
          value: _values[field.key] == true,
          onChanged: field.readOnly
              ? null
              : (value) => setState(() => _values[field.key] = value ?? false),
          title: Text(field.label),
          contentPadding: EdgeInsets.zero,
        ),
        AdminFieldType.lookup => DropdownButtonFormField<int>(
          key: ValueKey('${field.key}:${_values[field.key]}'),
          initialValue: _values[field.key] as int?,
          decoration: InputDecoration(
            labelText: field.label,
            helperText: field.helperText,
          ),
          items: [
            if (!field.required)
              const DropdownMenuItem<int>(value: null, child: Text('None')),
            ...(_lookups[field.key] ?? []).map(
              (item) => DropdownMenuItem(
                value: item.value,
                child: Text(item.label, overflow: TextOverflow.ellipsis),
              ),
            ),
          ],
          onChanged: field.readOnly
              ? null
              : (value) => _changeLookup(field, value),
          validator: (value) => field.required && value == null
              ? '${field.label} is required.'
              : null,
        ),
        AdminFieldType.status => DropdownButtonFormField<int>(
          initialValue:
              (_values[field.key] as int?) ??
              (field.statusOptions.isEmpty
                  ? null
                  : field.statusOptions.first.value),
          decoration: InputDecoration(
            labelText: field.label,
            helperText: field.helperText,
          ),
          items: field.statusOptions
              .map(
                (item) => DropdownMenuItem(
                  value: item.value,
                  child: Text(item.label),
                ),
              )
              .toList(),
          onChanged: field.readOnly
              ? null
              : (value) => setState(() => _values[field.key] = value),
          validator: (value) => field.required && value == null
              ? '${field.label} is required.'
              : null,
        ),
        AdminFieldType.date => TextFormField(
          controller: _controllers[field.key],
          readOnly: true,
          decoration: InputDecoration(
            labelText: field.label,
            helperText: field.helperText,
            suffixIcon: const Icon(Icons.calendar_today),
          ),
          validator: (value) => _validate(field, value),
          onTap: field.readOnly ? null : () => _pickDate(field),
        ),
        AdminFieldType.time => TextFormField(
          controller: _controllers[field.key],
          readOnly: true,
          decoration: InputDecoration(
            labelText: field.label,
            helperText: field.helperText,
            suffixIcon: const Icon(Icons.schedule),
          ),
          validator: (value) => _validate(field, value),
          onTap: field.readOnly ? null : () => _pickTime(field),
        ),
        _ => TextFormField(
          controller: _controllers[field.key],
          readOnly: field.readOnly,
          maxLines: field.type == AdminFieldType.multiline ? 4 : 1,
          keyboardType:
              field.type == AdminFieldType.integer ||
                  field.type == AdminFieldType.decimal
              ? TextInputType.number
              : TextInputType.text,
          obscureText: field.key.toLowerCase().contains('password'),
          decoration: InputDecoration(
            labelText: field.label,
            helperText: field.helperText,
          ),
          validator: (value) => _validate(field, value),
        ),
      },
    );
  }

  Future<void> _pickDate(AdminField field) async {
    final current =
        DateTime.tryParse(_controllers[field.key]?.text ?? '') ??
        DateTime.now();
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final lastDate = field.disallowFutureDates ? today : DateTime(2100);
    final selected = await showDatePicker(
      context: context,
      initialDate: current.isAfter(lastDate) ? lastDate : current,
      firstDate: DateTime(2020),
      lastDate: lastDate,
    );
    if (selected != null) {
      _controllers[field.key]?.text = selected.toIso8601String().substring(
        0,
        10,
      );
    }
  }

  Future<void> _pickTime(AdminField field) async {
    final selected = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );
    if (selected != null) {
      _controllers[field.key]?.text =
          '${selected.hour.toString().padLeft(2, '0')}:${selected.minute.toString().padLeft(2, '0')}';
    }
  }

  String _dateText(dynamic value) {
    if (value == null) return '';
    final text = value.toString();
    return text.length >= 10 ? text.substring(0, 10) : text;
  }

  String _timeText(dynamic value) {
    if (value == null) return '';
    final text = value.toString();
    return text.length >= 5 ? text.substring(0, 5) : text;
  }
}
