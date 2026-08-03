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

          return _DetailsContent(appointment: snapshot.data!);
        },
      ),
    );
  }
}

class _DetailsContent extends StatelessWidget {
  const _DetailsContent({required this.appointment});

  final Appointment appointment;

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
