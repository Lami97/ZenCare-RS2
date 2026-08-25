import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/wellness_service.dart';
import '../../services/wellness_service_service.dart';
import '../../utils/api_exception.dart';
import '../appointments/create_appointment_screen.dart';

class ServiceDetailsScreen extends StatefulWidget {
  const ServiceDetailsScreen({
    super.key,
    required this.serviceId,
    required this.onReservationCreated,
    this.onBack,
  });

  final int serviceId;
  final VoidCallback? onBack;
  final VoidCallback onReservationCreated;

  @override
  State<ServiceDetailsScreen> createState() => _ServiceDetailsScreenState();
}

class _ServiceDetailsScreenState extends State<ServiceDetailsScreen> {
  late Future<WellnessService> _serviceFuture;

  @override
  void initState() {
    super.initState();
    _serviceFuture = _loadService();
  }

  Future<WellnessService> _loadService() {
    return context
        .read<WellnessServiceService>()
        .getServiceById(widget.serviceId);
  }

  void _retry() {
    setState(() {
      _serviceFuture = _loadService();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Service details'),
        leading: widget.onBack == null
            ? null
            : IconButton(
                tooltip: 'Back to services',
                onPressed: widget.onBack,
                icon: const Icon(Icons.arrow_back),
              ),
      ),
      body: FutureBuilder<WellnessService>(
        future: _serviceFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            final message = snapshot.error is ApiException
                ? (snapshot.error! as ApiException).message
                : 'Unable to load service details.';

            return _DetailsError(message: message, onRetry: _retry);
          }

          final service = snapshot.data;
          if (service == null || service.id == 0) {
            return _DetailsError(
              message: 'Service details were not found.',
              onRetry: _retry,
            );
          }

          return _ServiceDetailsContent(
            service: service,
            onReservationCreated: widget.onReservationCreated,
          );
        },
      ),
    );
  }
}

class _ServiceDetailsContent extends StatelessWidget {
  const _ServiceDetailsContent({
    required this.service,
    required this.onReservationCreated,
  });

  final WellnessService service;
  final VoidCallback onReservationCreated;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Center(
          child: Icon(
            Icons.spa_outlined,
            size: 88,
            color: theme.colorScheme.primary,
          ),
        ),
        const SizedBox(height: 20),
        Text(
          service.name,
          style: theme.textTheme.headlineSmall
              ?.copyWith(fontWeight: FontWeight.w700),
        ),
        const SizedBox(height: 8),
        Text(
          '\$${service.price.toStringAsFixed(2)}',
          style: theme.textTheme.headlineSmall?.copyWith(
            color: theme.colorScheme.primary,
            fontWeight: FontWeight.w700,
          ),
        ),
        if ((service.description ?? '').trim().isNotEmpty) ...[
          const SizedBox(height: 16),
          Text(
            service.description!.trim(),
            style: theme.textTheme.bodyLarge,
          ),
        ],
        const SizedBox(height: 24),
        _DetailsSection(
          children: [
            _DetailsRow(label: 'Category', value: service.serviceCategoryName),
            _DetailsRow(
                label: 'Duration', value: '${service.durationMinutes} minutes'),
            _DetailsRow(
                label: 'Status',
                value: service.isActive ? 'Active' : 'Inactive'),
          ],
        ),
        if (service.isActive) ...[
          const SizedBox(height: 24),
          FilledButton.icon(
            onPressed: () async {
              final created = await Navigator.of(context).push<bool>(
                MaterialPageRoute<bool>(
                  builder: (_) => CreateAppointmentScreen(
                    initialServiceId: service.id,
                  ),
                ),
              );

              if (created == true && context.mounted) {
                onReservationCreated();
              }
            },
            icon: const Icon(Icons.calendar_month_outlined),
            label: const Text('Book appointment'),
          ),
        ],
      ],
    );
  }
}

class _DetailsSection extends StatelessWidget {
  const _DetailsSection({required this.children});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(children: children),
      ),
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
            width: 130,
            child: Text(
              label,
              style: const TextStyle(fontWeight: FontWeight.w700),
            ),
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
            Icon(Icons.error_outline,
                size: 48, color: theme.colorScheme.primary),
            const SizedBox(height: 16),
            Text(
              'Service details could not be loaded',
              style: theme.textTheme.titleLarge
                  ?.copyWith(fontWeight: FontWeight.w700),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 8),
            Text(message, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: onRetry,
              child: const Text('Retry'),
            ),
          ],
        ),
      ),
    );
  }
}
