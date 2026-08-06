import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/appointment.dart';
import '../../models/appointment_status.dart';
import '../../services/appointment_service.dart';
import '../../utils/api_exception.dart';
import '../reviews/create_review_screen.dart';

class AppointmentDetailsScreen extends StatefulWidget {
  const AppointmentDetailsScreen({super.key, required this.appointmentId});

  final int appointmentId;

  @override
  State<AppointmentDetailsScreen> createState() => _AppointmentDetailsScreenState();
}

class _AppointmentDetailsScreenState extends State<AppointmentDetailsScreen> {
  late Future<Appointment> _appointmentFuture;
  bool _isCancelling = false;

  @override
  void initState() {
    super.initState();
    _appointmentFuture = _loadAppointment();
  }

  Future<Appointment> _loadAppointment() {
    return context.read<AppointmentService>().getMyAppointmentById(widget.appointmentId);
  }

  void _retry() {
    setState(() {
      _appointmentFuture = _loadAppointment();
    });
  }

  void _reloadAppointment() {
    setState(() {
      _appointmentFuture = _loadAppointment();
    });
  }

  Future<void> _cancelAppointment(Appointment appointment) async {
    final reason = await showDialog<String>(
      context: context,
      builder: (context) => const _CancelAppointmentDialog(),
    );

    if (!mounted || reason == null) {
      return;
    }

    final service = context.read<AppointmentService>();
    final messenger = ScaffoldMessenger.of(context);

    setState(() {
      _isCancelling = true;
    });

    try {
      await service.cancelMyAppointment(
        id: appointment.id,
        cancellationReason: reason,
      );

      if (!mounted) {
        return;
      }

      messenger.showSnackBar(
        const SnackBar(content: Text('Appointment was cancelled successfully.')),
      );
      _reloadAppointment();
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }

      messenger.showSnackBar(SnackBar(content: Text(error.message)));
    } catch (_) {
      if (!mounted) {
        return;
      }

      messenger.showSnackBar(
        const SnackBar(content: Text('Appointment could not be cancelled.')),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isCancelling = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Appointment details')),
      body: FutureBuilder<Appointment>(
        future: _appointmentFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            final message = snapshot.error is ApiException
                ? (snapshot.error! as ApiException).message
                : 'Unable to load appointment details.';
            return _DetailsError(message: message, onRetry: _retry);
          }

          return _DetailsContent(
            appointment: snapshot.data!,
            isCancelling: _isCancelling,
            onCancel: _isCancelling ? null : () => _cancelAppointment(snapshot.data!),
          );
        },
      ),
    );
  }
}

class _DetailsContent extends StatelessWidget {
  const _DetailsContent({
    required this.appointment,
    required this.isCancelling,
    this.onCancel,
  });

  final Appointment appointment;
  final bool isCancelling;
  final VoidCallback? onCancel;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Icon(Icons.calendar_month_outlined, size: 72, color: theme.colorScheme.primary),
        const SizedBox(height: 16),
        Text(
          appointment.serviceName,
          style: theme.textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
        ),
        const SizedBox(height: 8),
        Chip(label: Text(appointment.status.label)),
        const SizedBox(height: 20),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                _DetailsRow(label: 'Client', value: appointment.userName),
                _DetailsRow(label: 'Employee', value: appointment.employeeName),
                _DetailsRow(label: 'Service', value: appointment.serviceName),
                _DetailsRow(label: 'Date', value: _formatDate(appointment.appointmentDate)),
                _DetailsRow(label: 'Start', value: _formatDuration(appointment.startTime)),
                _DetailsRow(label: 'End', value: _formatDuration(appointment.endTime)),
                _DetailsRow(label: 'Status', value: appointment.status.label),
                _DetailsRow(label: 'Notes', value: appointment.notes ?? '-'),
                _DetailsRow(label: 'Cancellation reason', value: appointment.cancellationReason ?? '-'),
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),
        if (appointment.canCancel) ...[
          SizedBox(
            width: double.infinity,
            child: FilledButton.icon(
              onPressed: onCancel,
              icon: isCancelling
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.cancel_outlined),
              label: const Text('Cancel appointment'),
            ),
          ),
          const SizedBox(height: 12),
        ],
        SizedBox(
          width: double.infinity,
          child: OutlinedButton.icon(
            onPressed: appointment.status == AppointmentStatus.completed
                ? () {
                    Navigator.of(context).push(
                      MaterialPageRoute<bool>(
                        builder: (_) => CreateReviewScreen(
                          appointmentId: appointment.id,
                          targetName: appointment.serviceName,
                        ),
                      ),
                    );
                  }
                : null,
            icon: const Icon(Icons.rate_review_outlined),
            label: Text(
              appointment.status == AppointmentStatus.completed
                  ? 'Review appointment'
                  : 'Review available after completion',
            ),
          ),
        ),
      ],
    );
  }
}

class _CancelAppointmentDialog extends StatefulWidget {
  const _CancelAppointmentDialog();

  @override
  State<_CancelAppointmentDialog> createState() => _CancelAppointmentDialogState();
}

class _CancelAppointmentDialogState extends State<_CancelAppointmentDialog> {
  final _formKey = GlobalKey<FormState>();
  final _reasonController = TextEditingController();

  @override
  void dispose() {
    _reasonController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Cancel appointment'),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Are you sure you want to cancel this appointment?'),
            const SizedBox(height: 16),
            TextFormField(
              controller: _reasonController,
              maxLength: 500,
              maxLines: 3,
              decoration: const InputDecoration(labelText: 'Cancellation reason'),
              validator: (value) => value == null || value.trim().isEmpty
                  ? 'Cancellation reason is required.'
                  : null,
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        FilledButton(
          onPressed: () {
            if (!_formKey.currentState!.validate()) {
              return;
            }

            Navigator.of(context).pop(_reasonController.text.trim());
          },
          child: const Text('Confirm cancellation'),
        ),
      ],
    );
  }
}

class _DetailsRow extends StatelessWidget {
  const _DetailsRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 140,
            child: Text(label, style: const TextStyle(fontWeight: FontWeight.w700)),
          ),
          Expanded(child: Text(value.isEmpty ? '-' : value)),
        ],
      ),
    );
  }
}

class _DetailsError extends StatelessWidget {
  const _DetailsError({required this.message, required this.onRetry});

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
              'Appointment details could not be loaded',
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

String _formatDate(DateTime value) {
  return '${value.day.toString().padLeft(2, '0')}.${value.month.toString().padLeft(2, '0')}.${value.year}';
}

String _formatDuration(Duration value) {
  final hours = value.inHours.toString().padLeft(2, '0');
  final minutes = value.inMinutes.remainder(60).toString().padLeft(2, '0');
  return '$hours:$minutes';
}
