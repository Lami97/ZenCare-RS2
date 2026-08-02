import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/appointment_create_request.dart';
import '../../models/employee.dart';
import '../../models/wellness_service.dart';
import '../../providers/auth_provider.dart';
import '../../services/appointment_service.dart';
import '../../utils/api_exception.dart';

class CreateAppointmentScreen extends StatefulWidget {
  const CreateAppointmentScreen({super.key});

  @override
  State<CreateAppointmentScreen> createState() => _CreateAppointmentScreenState();
}

class _CreateAppointmentScreenState extends State<CreateAppointmentScreen> {
  final _formKey = GlobalKey<FormState>();
  final _notesController = TextEditingController();

  bool _isLoadingLookups = true;
  bool _isSubmitting = false;
  String? _lookupError;
  String? _submitError;
  List<Employee> _employees = [];
  List<WellnessService> _services = [];
  Employee? _selectedEmployee;
  WellnessService? _selectedService;
  DateTime? _selectedDate;
  TimeOfDay? _startTime;
  TimeOfDay? _endTime;

  @override
  void initState() {
    super.initState();
    _loadLookups();
  }

  @override
  void dispose() {
    _notesController.dispose();
    super.dispose();
  }

  Future<void> _loadLookups() async {
    setState(() {
      _isLoadingLookups = true;
      _lookupError = null;
    });

    try {
      final service = context.read<AppointmentService>();
      final employeesResult = await service.getAvailableEmployees();
      final servicesResult = await service.getActiveServices();

      _employees = employeesResult.items.where((employee) => employee.isAvailable).toList();
      _services = servicesResult.items.where((wellnessService) => wellnessService.isActive).toList();
    } on ApiException catch (error) {
      _lookupError = error.statusCode == 403
          ? 'Employee selection is not available yet because the current API exposes employee lookup to Admin users only. A Client-accessible employee lookup endpoint is required to create appointments.'
          : error.message;
    } catch (_) {
      _lookupError = 'Unable to load appointment options.';
    } finally {
      if (mounted) {
        setState(() {
          _isLoadingLookups = false;
        });
      }
    }
  }

  Future<void> _pickDate() async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: _selectedDate ?? now,
      firstDate: DateTime(now.year, now.month, now.day),
      lastDate: now.add(const Duration(days: 365)),
    );

    if (picked != null) {
      setState(() {
        _selectedDate = picked;
      });
    }
  }

  Future<void> _pickStartTime() async {
    final picked = await showTimePicker(
      context: context,
      initialTime: _startTime ?? const TimeOfDay(hour: 9, minute: 0),
    );

    if (picked != null) {
      setState(() {
        _startTime = picked;
        if (_selectedService != null) {
          _endTime = _addMinutes(picked, _selectedService!.durationMinutes);
        }
      });
    }
  }

  Future<void> _pickEndTime() async {
    final picked = await showTimePicker(
      context: context,
      initialTime: _endTime ?? const TimeOfDay(hour: 10, minute: 0),
    );

    if (picked != null) {
      setState(() {
        _endTime = picked;
      });
    }
  }

  Future<void> _submit() async {
    setState(() {
      _submitError = null;
    });

    if (!_formKey.currentState!.validate()) {
      return;
    }

    final user = context.read<AuthProvider>().user;
    if (user == null) {
      setState(() {
        _submitError = 'You must be logged in to create an appointment.';
      });
      return;
    }

    final start = _toDuration(_startTime!);
    final end = _toDuration(_endTime!);
    if (end <= start) {
      setState(() {
        _submitError = 'End time must be after start time.';
      });
      return;
    }

    setState(() {
      _isSubmitting = true;
    });

    try {
      final notes = _notesController.text.trim();
      final request = AppointmentCreateRequest(
        userId: user.id,
        employeeId: _selectedEmployee!.id,
        wellnessServiceId: _selectedService!.id,
        appointmentDate: _selectedDate!,
        startTime: start,
        endTime: end,
        notes: notes.isEmpty ? null : notes,
      );

      await context.read<AppointmentService>().createMyAppointment(request);

      if (mounted) {
        Navigator.of(context).pop(true);
      }
    } on ApiException catch (error) {
      setState(() {
        _submitError = error.message;
      });
    } catch (_) {
      setState(() {
        _submitError = 'Unable to create appointment.';
      });
    } finally {
      if (mounted) {
        setState(() {
          _isSubmitting = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Create appointment')),
      body: _isLoadingLookups
          ? const Center(child: CircularProgressIndicator())
          : _lookupError != null
              ? _LookupError(message: _lookupError!, onRetry: _loadLookups)
              : Form(
                  key: _formKey,
                  child: ListView(
                    padding: const EdgeInsets.all(20),
                    children: [
                      DropdownButtonFormField<Employee>(
                        initialValue: _selectedEmployee,
                        decoration: const InputDecoration(labelText: 'Employee'),
                        items: _employees
                            .map(
                              (employee) => DropdownMenuItem<Employee>(
                                value: employee,
                                child: Text(employee.displayName),
                              ),
                            )
                            .toList(),
                        onChanged: (value) => setState(() => _selectedEmployee = value),
                        validator: (value) => value == null ? 'Employee is required.' : null,
                      ),
                      const SizedBox(height: 16),
                      DropdownButtonFormField<WellnessService>(
                        initialValue: _selectedService,
                        decoration: const InputDecoration(labelText: 'Service'),
                        items: _services
                            .map(
                              (service) => DropdownMenuItem<WellnessService>(
                                value: service,
                                child: Text('${service.name} (${service.durationMinutes} min)'),
                              ),
                            )
                            .toList(),
                        onChanged: (value) {
                          setState(() {
                            _selectedService = value;
                            if (_startTime != null && value != null) {
                              _endTime = _addMinutes(_startTime!, value.durationMinutes);
                            }
                          });
                        },
                        validator: (value) => value == null ? 'Service is required.' : null,
                      ),
                      const SizedBox(height: 16),
                      _PickerTile(
                        label: 'Date',
                        value: _selectedDate == null ? 'Select date' : _formatDate(_selectedDate!),
                        icon: Icons.event_outlined,
                        onTap: _pickDate,
                        validator: () => _selectedDate == null ? 'Date is required.' : null,
                      ),
                      const SizedBox(height: 16),
                      _PickerTile(
                        label: 'Start time',
                        value: _startTime == null ? 'Select start time' : _formatTimeOfDay(_startTime!),
                        icon: Icons.schedule_outlined,
                        onTap: _pickStartTime,
                        validator: () => _startTime == null ? 'Start time is required.' : null,
                      ),
                      const SizedBox(height: 16),
                      _PickerTile(
                        label: 'End time',
                        value: _endTime == null ? 'Select end time' : _formatTimeOfDay(_endTime!),
                        icon: Icons.schedule,
                        onTap: _pickEndTime,
                        validator: () => _endTime == null ? 'End time is required.' : null,
                      ),
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _notesController,
                        maxLines: 4,
                        maxLength: 1000,
                        decoration: const InputDecoration(labelText: 'Notes'),
                      ),
                      if (_submitError != null) ...[
                        const SizedBox(height: 8),
                        Text(
                          _submitError!,
                          style: TextStyle(color: Theme.of(context).colorScheme.error),
                        ),
                      ],
                      const SizedBox(height: 24),
                      FilledButton(
                        onPressed: _isSubmitting ? null : _submit,
                        child: _isSubmitting
                            ? const SizedBox(
                                width: 20,
                                height: 20,
                                child: CircularProgressIndicator(strokeWidth: 2),
                              )
                            : const Text('Create appointment'),
                      ),
                    ],
                  ),
                ),
    );
  }
}

class _PickerTile extends FormField<String> {
  _PickerTile({
    required String label,
    required String value,
    required IconData icon,
    required VoidCallback onTap,
    required String? Function() validator,
  }) : super(
          validator: (_) => validator(),
          builder: (field) {
            return InkWell(
              onTap: onTap,
              child: InputDecorator(
                decoration: InputDecoration(
                  labelText: label,
                  prefixIcon: Icon(icon),
                  errorText: field.errorText,
                ),
                child: Text(value),
              ),
            );
          },
        );
}

class _LookupError extends StatelessWidget {
  const _LookupError({required this.message, required this.onRetry});

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.error_outline, size: 48, color: theme.colorScheme.primary),
            const SizedBox(height: 16),
            Text(
              'Appointment options could not be loaded',
              style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 8),
            Text(message, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(onPressed: onRetry, child: const Text('Retry')),
          ],
        ),
      ),
    );
  }
}

Duration _toDuration(TimeOfDay value) {
  return Duration(hours: value.hour, minutes: value.minute);
}

TimeOfDay _addMinutes(TimeOfDay value, int minutes) {
  final totalMinutes = value.hour * 60 + value.minute + minutes;
  return TimeOfDay(hour: (totalMinutes ~/ 60) % 24, minute: totalMinutes % 60);
}

String _formatDate(DateTime value) {
  return '${value.day.toString().padLeft(2, '0')}.${value.month.toString().padLeft(2, '0')}.${value.year}';
}

String _formatTimeOfDay(TimeOfDay value) {
  return '${value.hour.toString().padLeft(2, '0')}:${value.minute.toString().padLeft(2, '0')}';
}


