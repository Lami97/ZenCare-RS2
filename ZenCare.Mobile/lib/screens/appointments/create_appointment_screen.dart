import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/appointment_create_request.dart';
import '../../models/category.dart';
import '../../models/time_slot.dart';
import '../../models/wellness_service.dart';
import '../../services/appointment_service.dart';
import '../../services/wellness_service_service.dart';
import '../../utils/api_exception.dart';

class CreateAppointmentScreen extends StatefulWidget {
  const CreateAppointmentScreen({super.key, this.initialServiceId});

  final int? initialServiceId;

  @override
  State<CreateAppointmentScreen> createState() =>
      _CreateAppointmentScreenState();
}

class _CreateAppointmentScreenState extends State<CreateAppointmentScreen>
    with WidgetsBindingObserver {
  final _formKey = GlobalKey<FormState>();
  final _notesController = TextEditingController();

  bool _isLoadingLookups = true;
  bool _isLoadingSlots = false;
  bool _isSubmitting = false;
  String? _lookupError;
  String? _slotMessage;
  String? _submitError;
  List<Category> _categories = [];
  List<WellnessService> _services = [];
  List<TimeSlot> _slots = [];
  Category? _selectedCategory;
  WellnessService? _selectedService;
  TimeSlot? _selectedSlot;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _loadLookups();
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _notesController.dispose();
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed &&
        _selectedService != null &&
        !_isLoadingSlots &&
        !_isSubmitting) {
      _loadSlots();
    }
  }

  Future<void> _loadLookups() async {
    setState(() {
      _isLoadingLookups = true;
      _lookupError = null;
      _slotMessage = null;
      _selectedCategory = null;
      _selectedService = null;
      _selectedSlot = null;
      _slots = [];
    });

    try {
      final service = context.read<WellnessServiceService>();
      final categoriesResult = await service.getCategories(isActive: true);
      final servicesResult = await service.getServices(
        isActive: true,
        page: 1,
        pageSize: 100,
      );

      _categories = categoriesResult.items
          .where((category) => category.isActive)
          .toList();
      _services = servicesResult.items.where((item) => item.isActive).toList();

      final initialServiceId = widget.initialServiceId;
      if (initialServiceId != null) {
        _selectedService = _firstWhereOrNull(
          _services,
          (item) => item.id == initialServiceId,
        );
        final serviceCategoryId = _selectedService?.serviceCategoryId;
        if (serviceCategoryId != null) {
          _selectedCategory = _firstWhereOrNull(
            _categories,
            (category) => category.id == serviceCategoryId,
          );
        }
      }
    } on ApiException catch (error) {
      _lookupError = error.message;
    } catch (_) {
      _lookupError = 'Unable to load reservation options.';
    } finally {
      if (mounted) {
        setState(() => _isLoadingLookups = false);
      }
    }

    if (mounted && _selectedService != null) {
      await _loadSlots();
    }
  }

  List<WellnessService> get _filteredServices {
    final category = _selectedCategory;
    if (category == null) return [];
    return _services
        .where((service) => service.serviceCategoryId == category.id)
        .toList();
  }

  void _selectCategory(Category? category) {
    setState(() {
      _selectedCategory = category;
      _selectedService = null;
      _selectedSlot = null;
      _slots = [];
      _slotMessage = null;
      _submitError = null;
    });
  }

  Future<void> _selectService(WellnessService? service) async {
    setState(() {
      _selectedService = service;
      _selectedSlot = null;
      _slots = [];
      _slotMessage = null;
      _submitError = null;
    });
    await _loadSlots();
  }

  Future<void> _loadSlots() async {
    final service = _selectedService;
    if (service == null) return;

    setState(() {
      _isLoadingSlots = true;
      _selectedSlot = null;
      _slots = [];
      _slotMessage = null;
    });

    try {
      final result = await context
          .read<AppointmentService>()
          .getAvailableTimeSlots(wellnessServiceId: service.id);

      if (!mounted) return;
      setState(() {
        _slots = result.items.where((slot) => slot.isAvailable).toList();
        _slotMessage = _slots.isEmpty
            ? 'No appointment times are currently available for this service.'
            : null;
      });
    } on ApiException catch (error) {
      if (!mounted) return;
      setState(() => _slotMessage = error.message);
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _slotMessage = 'Unable to load available appointment times.';
      });
    } finally {
      if (mounted) {
        setState(() => _isLoadingSlots = false);
      }
    }
  }

  Future<void> _submit() async {
    setState(() => _submitError = null);
    if (!_formKey.currentState!.validate()) return;

    final slot = _selectedSlot;
    if (slot == null) return;

    setState(() => _isSubmitting = true);
    try {
      final notes = _notesController.text.trim();
      await context.read<AppointmentService>().createMyAppointment(
            AppointmentCreateRequest(
              timeSlotId: slot.id,
              notes: notes.isEmpty ? null : notes,
            ),
          );

      if (mounted) Navigator.of(context).pop(true);
    } on ApiException catch (error) {
      if (mounted) setState(() => _submitError = error.message);
    } catch (_) {
      if (mounted) {
        setState(() => _submitError = 'Unable to create reservation.');
      }
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Create reservation')),
      body: _isLoadingLookups
          ? const Center(child: CircularProgressIndicator())
          : _lookupError != null
              ? _LookupError(message: _lookupError!, onRetry: _loadLookups)
              : Form(
                  key: _formKey,
                  child: ListView(
                    padding: const EdgeInsets.all(20),
                    children: [
                      Text(
                        'Book a wellness service',
                        style:
                            Theme.of(context).textTheme.headlineSmall?.copyWith(
                                  fontWeight: FontWeight.w700,
                                ),
                      ),
                      const SizedBox(height: 20),
                      DropdownButtonFormField<Category>(
                        initialValue: _selectedCategory,
                        isExpanded: true,
                        decoration: const InputDecoration(
                          labelText: 'Service category',
                        ),
                        items: _categories
                            .map(
                              (category) => DropdownMenuItem(
                                value: category,
                                child: Text(category.name),
                              ),
                            )
                            .toList(),
                        onChanged: _isSubmitting ? null : _selectCategory,
                        validator: (value) =>
                            value == null ? 'Select service category.' : null,
                      ),
                      const SizedBox(height: 16),
                      DropdownButtonFormField<WellnessService>(
                        key: ValueKey(_selectedCategory?.id),
                        initialValue: _selectedService,
                        isExpanded: true,
                        decoration: const InputDecoration(labelText: 'Service'),
                        items: _filteredServices
                            .map(
                              (service) => DropdownMenuItem(
                                value: service,
                                child: Text(service.name),
                              ),
                            )
                            .toList(),
                        onChanged: _selectedCategory == null || _isSubmitting
                            ? null
                            : _selectService,
                        validator: (value) =>
                            value == null ? 'Select service.' : null,
                      ),
                      const SizedBox(height: 16),
                      if (_isLoadingSlots) const LinearProgressIndicator(),
                      if (_isLoadingSlots) const SizedBox(height: 12),
                      Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Expanded(
                            child: DropdownButtonFormField<TimeSlot>(
                              key: ValueKey(_selectedService?.id),
                              initialValue: _selectedSlot,
                              isExpanded: true,
                              decoration: const InputDecoration(
                                labelText: 'Available appointment time',
                              ),
                              items: _slots
                                  .map(
                                    (slot) => DropdownMenuItem(
                                      value: slot,
                                      child: Text(
                                        slot.displayLabel,
                                        overflow: TextOverflow.ellipsis,
                                      ),
                                    ),
                                  )
                                  .toList(),
                              onChanged: _selectedService == null ||
                                      _isLoadingSlots ||
                                      _isSubmitting
                                  ? null
                                  : (slot) =>
                                      setState(() => _selectedSlot = slot),
                              validator: (value) => value == null
                                  ? 'Select an available appointment time.'
                                  : null,
                            ),
                          ),
                          const SizedBox(width: 8),
                          IconButton(
                            onPressed: _selectedService == null ||
                                    _isLoadingSlots ||
                                    _isSubmitting
                                ? null
                                : _loadSlots,
                            tooltip: 'Refresh availability',
                            icon: const Icon(Icons.refresh),
                          ),
                        ],
                      ),
                      if (_slotMessage != null) ...[
                        const SizedBox(height: 8),
                        Text(
                          _slotMessage!,
                          style: TextStyle(
                            color:
                                Theme.of(context).colorScheme.onSurfaceVariant,
                          ),
                        ),
                      ],
                      if (_selectedSlot != null) ...[
                        const SizedBox(height: 16),
                        _SlotSummary(slot: _selectedSlot!),
                      ],
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _notesController,
                        enabled: !_isSubmitting,
                        maxLines: 4,
                        maxLength: 1000,
                        decoration: const InputDecoration(labelText: 'Notes'),
                      ),
                      if (_submitError != null) ...[
                        const SizedBox(height: 8),
                        Text(
                          _submitError!,
                          style: TextStyle(
                            color: Theme.of(context).colorScheme.error,
                          ),
                        ),
                      ],
                      const SizedBox(height: 20),
                      FilledButton.icon(
                        onPressed: _isSubmitting ? null : _submit,
                        icon: _isSubmitting
                            ? const SizedBox(
                                width: 18,
                                height: 18,
                                child:
                                    CircularProgressIndicator(strokeWidth: 2),
                              )
                            : const Icon(Icons.event_available),
                        label: const Text('Confirm reservation'),
                      ),
                    ],
                  ),
                ),
    );
  }
}

class _SlotSummary extends StatelessWidget {
  const _SlotSummary({required this.slot});

  final TimeSlot slot;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              slot.serviceName,
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
            ),
            const SizedBox(height: 8),
            Text('Employee: ${slot.employeeName}'),
            const SizedBox(height: 4),
            Text('Time: ${slot.displayLabel.split('  ').take(2).join('  ')}'),
          ],
        ),
      ),
    );
  }
}

class _LookupError extends StatelessWidget {
  const _LookupError({required this.message, required this.onRetry});

  final String message;
  final Future<void> Function() onRetry;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.error_outline, size: 48),
            const SizedBox(height: 16),
            Text(message, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(onPressed: onRetry, child: const Text('Retry')),
          ],
        ),
      ),
    );
  }
}

T? _firstWhereOrNull<T>(Iterable<T> values, bool Function(T) predicate) {
  for (final value in values) {
    if (predicate(value)) return value;
  }
  return null;
}
