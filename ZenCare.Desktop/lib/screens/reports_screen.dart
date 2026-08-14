import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:printing/printing.dart';
import 'package:provider/provider.dart';

import '../config/admin_modules.dart';
import '../services/admin_repository.dart';
import '../utils/api_exception.dart';
import '../utils/formatters.dart';

class ReportsScreen extends StatefulWidget {
  const ReportsScreen({super.key});

  @override
  State<ReportsScreen> createState() => _ReportsScreenState();
}

class _ReportsScreenState extends State<ReportsScreen> {
  bool _loading = true;
  String? _error;
  _ReportData? _data;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final repository = context.read<AdminRepository>();
      final users = await _loadAll(repository, 'User');
      final employees = await _loadAll(repository, 'Employee');
      final appointments = await _loadAll(repository, 'Appointment');
      final services = await _loadAll(repository, 'Service');
      final products = await _loadAll(repository, 'Product');
      final purchases = await _loadAll(repository, 'Purchase');
      if (!mounted) return;
      setState(
        () => _data = _ReportData.from(
          users: users,
          employees: employees,
          appointments: appointments,
          services: services,
          products: products,
          purchases: purchases,
        ),
      );
    } on ApiException catch (error) {
      if (!mounted) return;
      setState(() => _error = error.message);
    } catch (_) {
      if (!mounted) return;
      setState(() => _error = 'Reports could not be loaded.');
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<List<Map<String, dynamic>>> _loadAll(
    AdminRepository repository,
    String endpoint,
  ) async {
    final module = adminModules.firstWhere(
      (module) => module.endpoint == endpoint,
    );
    final items = <Map<String, dynamic>>[];
    var page = 1;
    const pageSize = 100;
    while (true) {
      final result = await repository.list(
        module,
        page: page,
        pageSize: pageSize,
        filters: const {},
      );
      items.addAll(result.items);
      final total = result.totalCount;
      if (result.items.length < pageSize ||
          (total != null && items.length >= total)) {
        break;
      }
      page++;
    }
    return items;
  }

  Future<void> _shareAppointmentSummary() async {
    final bytes = await _buildAppointmentPdf(await _currentData());
    await _export('zencare-appointment-summary.pdf', bytes);
  }

  Future<void> _shareBusinessStatistics() async {
    final bytes = await _buildBusinessPdf(await _currentData());
    await _export('zencare-business-statistics.pdf', bytes);
  }

  Future<void> _printAppointmentSummary() async => Printing.layoutPdf(
    onLayout: (_) async => await _buildAppointmentPdf(await _currentData()),
  );
  Future<void> _printBusinessStatistics() async => Printing.layoutPdf(
    onLayout: (_) async => await _buildBusinessPdf(await _currentData()),
  );

  Future<_ReportData> _currentData() async {
    final data = _data;
    if (data == null) throw StateError('Report data is not loaded.');
    return data;
  }

  Future<void> _export(String filename, Uint8List bytes) async {
    await Printing.sharePdf(bytes: bytes, filename: filename);
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('$filename was generated successfully.')),
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  'Reports',
                  style: theme.textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
              OutlinedButton.icon(
                onPressed: _loading ? null : _shareAppointmentSummary,
                icon: const Icon(Icons.picture_as_pdf),
                label: const Text('Export Appointment Summary PDF'),
              ),
              const SizedBox(width: 8),
              OutlinedButton.icon(
                onPressed: _loading ? null : _shareBusinessStatistics,
                icon: const Icon(Icons.picture_as_pdf),
                label: const Text('Export Business Statistics PDF'),
              ),
              const SizedBox(width: 8),
              IconButton(
                onPressed: _loading ? null : _printAppointmentSummary,
                tooltip: 'Print appointment summary',
                icon: const Icon(Icons.print_outlined),
              ),
              IconButton(
                onPressed: _loading ? null : _printBusinessStatistics,
                tooltip: 'Print business statistics',
                icon: const Icon(Icons.print),
              ),
              IconButton(
                onPressed: _loading ? null : _load,
                tooltip: 'Refresh',
                icon: const Icon(Icons.refresh),
              ),
            ],
          ),
          const SizedBox(height: 18),
          Expanded(child: _body()),
        ],
      ),
    );
  }

  Widget _body() {
    final data = _data;
    if (_loading && data == null) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null && data == null) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(_error!),
            const SizedBox(height: 12),
            FilledButton(onPressed: _load, child: const Text('Retry')),
          ],
        ),
      );
    }
    if (data == null) {
      return const Center(child: Text('No report data available.'));
    }
    return RefreshIndicator(
      onRefresh: _load,
      child: ListView(
        children: [
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              _StatCard(
                label: 'Total users',
                value: data.totalUsers.toString(),
              ),
              _StatCard(
                label: 'Total employees',
                value: data.totalEmployees.toString(),
              ),
              _StatCard(
                label: 'Total appointments',
                value: data.totalAppointments.toString(),
              ),
              _StatCard(
                label: 'Total services',
                value: data.totalServices.toString(),
              ),
              _StatCard(
                label: 'Total products',
                value: data.totalProducts.toString(),
              ),
              _StatCard(
                label: 'Total purchases',
                value: data.totalPurchases.toString(),
              ),
            ],
          ),
          const SizedBox(height: 22),
          _StatusCards(counts: data.appointmentStatuses),
          const SizedBox(height: 22),
          _ReportTable(
            title: 'Most popular services',
            columns: const ['Service', 'Appointments'],
            rows: data.popularServices,
          ),
          const SizedBox(height: 18),
          _ReportTable(
            title: 'Employee workload',
            columns: const ['Employee', 'Appointments'],
            rows: data.employeeWorkload,
          ),
          const SizedBox(height: 18),
          _ReportTable(
            title: 'User activity',
            columns: const ['User', 'Appointments'],
            rows: data.userActivity,
          ),
        ],
      ),
    );
  }
}

class _StatCard extends StatelessWidget {
  const _StatCard({required this.label, required this.value});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 190,
      child: Card(
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
          side: BorderSide(color: Theme.of(context).dividerColor),
        ),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(label),
              const SizedBox(height: 8),
              Text(
                value,
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _StatusCards extends StatelessWidget {
  const _StatusCards({required this.counts});
  final Map<String, int> counts;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Appointment statistics',
          style: Theme.of(
            context,
          ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
        ),
        const SizedBox(height: 10),
        Wrap(
          spacing: 12,
          runSpacing: 12,
          children: counts.entries
              .map(
                (entry) =>
                    _StatCard(label: entry.key, value: entry.value.toString()),
              )
              .toList(),
        ),
      ],
    );
  }
}

class _ReportTable extends StatelessWidget {
  const _ReportTable({
    required this.title,
    required this.columns,
    required this.rows,
  });
  final String title;
  final List<String> columns;
  final List<List<String>> rows;

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 0,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: BorderSide(color: Theme.of(context).dividerColor),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              title,
              style: Theme.of(
                context,
              ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 10),
            if (rows.isEmpty)
              const Text('No data available.')
            else
              SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: DataTable(
                  columns: columns
                      .map((column) => DataColumn(label: Text(column)))
                      .toList(),
                  rows: rows
                      .map(
                        (row) => DataRow(
                          cells: row
                              .map((cell) => DataCell(Text(cell)))
                              .toList(),
                        ),
                      )
                      .toList(),
                ),
              ),
          ],
        ),
      ),
    );
  }
}

class _ReportData {
  _ReportData({
    required this.totalUsers,
    required this.totalEmployees,
    required this.totalAppointments,
    required this.totalServices,
    required this.totalProducts,
    required this.totalPurchases,
    required this.appointmentStatuses,
    required this.popularServices,
    required this.employeeWorkload,
    required this.userActivity,
  });

  final int totalUsers;
  final int totalEmployees;
  final int totalAppointments;
  final int totalServices;
  final int totalProducts;
  final int totalPurchases;
  final Map<String, int> appointmentStatuses;
  final List<List<String>> popularServices;
  final List<List<String>> employeeWorkload;
  final List<List<String>> userActivity;

  factory _ReportData.from({
    required List<Map<String, dynamic>> users,
    required List<Map<String, dynamic>> employees,
    required List<Map<String, dynamic>> appointments,
    required List<Map<String, dynamic>> services,
    required List<Map<String, dynamic>> products,
    required List<Map<String, dynamic>> purchases,
  }) {
    final statusMap = {
      1: 'Pending',
      2: 'Confirmed',
      3: 'Paid',
      4: 'Completed',
      5: 'Cancelled',
      6: 'No-show',
    };
    final counts = {for (final label in statusMap.values) label: 0};
    for (final appointment in appointments) {
      final status = appointment['status'] is int
          ? appointment['status'] as int
          : int.tryParse(appointment['status']?.toString() ?? '');
      final label = statusMap[status] ?? 'Other';
      counts[label] = (counts[label] ?? 0) + 1;
    }
    return _ReportData(
      totalUsers: users.length,
      totalEmployees: employees.length,
      totalAppointments: appointments.length,
      totalServices: services.length,
      totalProducts: products.length,
      totalPurchases: purchases.length,
      appointmentStatuses: counts,
      popularServices: _group(appointments, (x) => textValue(x['serviceName'])),
      employeeWorkload: _group(
        appointments,
        (x) => textValue(x['employeeName']),
      ),
      userActivity: _group(appointments, (x) => textValue(x['userName'])),
    );
  }

  static List<List<String>> _group(
    List<Map<String, dynamic>> items,
    String Function(Map<String, dynamic>) keySelector,
  ) {
    final map = <String, int>{};
    for (final item in items) {
      final key = keySelector(item);
      if (key == 'Not available') continue;
      map[key] = (map[key] ?? 0) + 1;
    }
    final entries = map.entries.toList()
      ..sort((a, b) => b.value.compareTo(a.value));
    return entries.map((entry) => [entry.key, entry.value.toString()]).toList();
  }
}

Future<Uint8List> _buildAppointmentPdf(_ReportData data) {
  final doc = pw.Document();
  doc.addPage(
    pw.MultiPage(
      pageFormat: PdfPageFormat.a4,
      build: (_) => [
        pw.Header(level: 0, text: 'Appointment Summary'),
        pw.Text('Generated: ${DateTime.now().toLocal()}'),
        pw.SizedBox(height: 16),
        pw.Text(
          'Appointment status counts',
          style: pw.TextStyle(fontWeight: pw.FontWeight.bold),
        ),
        pw.TableHelper.fromTextArray(
          headers: const ['Status', 'Count'],
          data: data.appointmentStatuses.entries
              .map((e) => [e.key, e.value.toString()])
              .toList(),
        ),
        pw.SizedBox(height: 16),
        pw.Text(
          'Most popular services',
          style: pw.TextStyle(fontWeight: pw.FontWeight.bold),
        ),
        pw.TableHelper.fromTextArray(
          headers: const ['Service', 'Appointments'],
          data: data.popularServices,
        ),
        pw.SizedBox(height: 16),
        pw.Text(
          'Employee workload',
          style: pw.TextStyle(fontWeight: pw.FontWeight.bold),
        ),
        pw.TableHelper.fromTextArray(
          headers: const ['Employee', 'Appointments'],
          data: data.employeeWorkload,
        ),
      ],
    ),
  );
  return doc.save();
}

Future<Uint8List> _buildBusinessPdf(_ReportData data) {
  final doc = pw.Document();
  doc.addPage(
    pw.MultiPage(
      pageFormat: PdfPageFormat.a4,
      build: (_) => [
        pw.Header(level: 0, text: 'Business Statistics'),
        pw.Text('Generated: ${DateTime.now().toLocal()}'),
        pw.SizedBox(height: 16),
        pw.TableHelper.fromTextArray(
          headers: const ['Metric', 'Value'],
          data: [
            ['Total users', data.totalUsers.toString()],
            ['Total employees', data.totalEmployees.toString()],
            ['Total appointments', data.totalAppointments.toString()],
            ['Total services', data.totalServices.toString()],
            ['Total products', data.totalProducts.toString()],
            ['Total purchases', data.totalPurchases.toString()],
          ],
        ),
        pw.SizedBox(height: 16),
        pw.Text(
          'User activity',
          style: pw.TextStyle(fontWeight: pw.FontWeight.bold),
        ),
        pw.TableHelper.fromTextArray(
          headers: const ['User', 'Appointments'],
          data: data.userActivity,
        ),
      ],
    ),
  );
  return doc.save();
}
