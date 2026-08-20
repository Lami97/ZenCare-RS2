import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:printing/printing.dart';
import 'package:provider/provider.dart';

import '../models/business_analytics.dart';
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
  BusinessAnalytics? _data;
  DateTime? _dateFrom;
  DateTime? _dateTo;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    if (_dateFrom != null && _dateTo != null && _dateFrom!.isAfter(_dateTo!)) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('From date must be before or equal to To date.'),
        ),
      );
      return;
    }

    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final data = await context.read<AdminRepository>().getBusinessAnalytics(
        dateFrom: _dateFrom,
        dateTo: _dateTo,
      );
      if (!mounted) return;
      setState(() => _data = data);
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

  Future<void> _pickDate({required bool from}) async {
    final selected = await showDatePicker(
      context: context,
      initialDate: (from ? _dateFrom : _dateTo) ?? DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (!mounted || selected == null) return;
    setState(() {
      if (from) {
        _dateFrom = selected;
      } else {
        _dateTo = selected;
      }
    });
  }

  void _clearDates() {
    setState(() {
      _dateFrom = null;
      _dateTo = null;
    });
    _load();
  }

  Future<void> _shareAppointmentSummary() async {
    final bytes = await _buildAppointmentPdf(_currentData());
    await _export('zencare-appointment-summary.pdf', bytes);
  }

  Future<void> _shareBusinessStatistics() async {
    final bytes = await _buildBusinessPdf(_currentData());
    await _export('zencare-business-statistics.pdf', bytes);
  }

  Future<void> _printAppointmentSummary() async => Printing.layoutPdf(
    onLayout: (_) async => _buildAppointmentPdf(_currentData()),
  );

  Future<void> _printBusinessStatistics() async => Printing.layoutPdf(
    onLayout: (_) async => _buildBusinessPdf(_currentData()),
  );

  BusinessAnalytics _currentData() {
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
          Wrap(
            crossAxisAlignment: WrapCrossAlignment.center,
            spacing: 8,
            runSpacing: 8,
            children: [
              SizedBox(
                width: 220,
                child: Text(
                  'Reports',
                  style: theme.textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
              OutlinedButton.icon(
                onPressed: _loading || _data == null
                    ? null
                    : _shareAppointmentSummary,
                icon: const Icon(Icons.picture_as_pdf),
                label: const Text('Appointment PDF'),
              ),
              OutlinedButton.icon(
                onPressed: _loading || _data == null
                    ? null
                    : _shareBusinessStatistics,
                icon: const Icon(Icons.picture_as_pdf),
                label: const Text('Business PDF'),
              ),
              IconButton(
                onPressed: _loading || _data == null
                    ? null
                    : _printAppointmentSummary,
                tooltip: 'Print appointment summary',
                icon: const Icon(Icons.print_outlined),
              ),
              IconButton(
                onPressed: _loading || _data == null
                    ? null
                    : _printBusinessStatistics,
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
          const SizedBox(height: 16),
          _DateFilter(
            dateFrom: _dateFrom,
            dateTo: _dateTo,
            loading: _loading,
            onPickFrom: () => _pickDate(from: true),
            onPickTo: () => _pickDate(from: false),
            onApply: _load,
            onClear: _clearDates,
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
      return _ErrorState(message: _error!, onRetry: _load);
    }
    if (data == null) {
      return const Center(child: Text('No report data available.'));
    }

    return RefreshIndicator(
      onRefresh: _load,
      child: ListView(
        children: [
          if (_loading) const LinearProgressIndicator(),
          if (_error != null) ...[
            MaterialBanner(
              content: Text(_error!),
              actions: [
                TextButton(onPressed: _load, child: const Text('Retry')),
              ],
            ),
            const SizedBox(height: 12),
          ],
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              _StatCard(label: 'Revenue', value: moneyValue(data.totalRevenue)),
              _StatCard(
                label: 'Completed purchases',
                value: data.completedPurchases.toString(),
              ),
              _StatCard(
                label: 'Completed appointments',
                value: data.completedAppointments.toString(),
              ),
              _StatCard(
                label: 'Unique clients',
                value: data.uniqueClients.toString(),
              ),
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
          _HorizontalBarChart(
            title: 'Best-selling products',
            valueLabel: 'units sold',
            entries: data.bestSellingProducts
                .map((item) => _ChartEntry(item.productName, item.quantitySold))
                .toList(),
          ),
          const SizedBox(height: 18),
          _HorizontalBarChart(
            title: 'Service usage',
            valueLabel: 'completed appointments',
            entries: data.serviceUsage
                .map(
                  (item) =>
                      _ChartEntry(item.serviceName, item.appointmentCount),
                )
                .toList(),
          ),
          const SizedBox(height: 18),
          _WeeklyBarChart(entries: data.weeklyAttendance),
          const SizedBox(height: 18),
          _ReportTable(
            title: 'Appointment status summary',
            columns: const ['Status', 'Count'],
            rows: data.appointmentStatuses
                .map((item) => [item.name, item.count.toString()])
                .toList(),
          ),
          const SizedBox(height: 18),
          _ReportTable(
            title: 'Product sales detail',
            columns: const ['Product', 'Units sold', 'Revenue'],
            rows: data.bestSellingProducts
                .map(
                  (item) => [
                    item.productName,
                    item.quantitySold.toString(),
                    moneyValue(item.revenue),
                  ],
                )
                .toList(),
          ),
          const SizedBox(height: 18),
          _ReportTable(
            title: 'Employee workload',
            columns: const ['Employee', 'Completed appointments'],
            rows: data.employeeWorkload
                .map((item) => [item.name, item.count.toString()])
                .toList(),
          ),
          const SizedBox(height: 18),
          _ReportTable(
            title: 'Client attendance',
            columns: const ['Client', 'Completed appointments'],
            rows: data.clientActivity
                .map((item) => [item.name, item.count.toString()])
                .toList(),
          ),
        ],
      ),
    );
  }
}

class _DateFilter extends StatelessWidget {
  const _DateFilter({
    required this.dateFrom,
    required this.dateTo,
    required this.loading,
    required this.onPickFrom,
    required this.onPickTo,
    required this.onApply,
    required this.onClear,
  });

  final DateTime? dateFrom;
  final DateTime? dateTo;
  final bool loading;
  final VoidCallback onPickFrom;
  final VoidCallback onPickTo;
  final VoidCallback onApply;
  final VoidCallback onClear;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        border: Border.all(color: Theme.of(context).dividerColor),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Wrap(
        spacing: 10,
        runSpacing: 10,
        crossAxisAlignment: WrapCrossAlignment.center,
        children: [
          _DateButton(label: 'From', value: dateFrom, onPressed: onPickFrom),
          _DateButton(label: 'To', value: dateTo, onPressed: onPickTo),
          FilledButton.icon(
            onPressed: loading ? null : onApply,
            icon: const Icon(Icons.filter_alt),
            label: const Text('Apply'),
          ),
          TextButton.icon(
            onPressed: loading || (dateFrom == null && dateTo == null)
                ? null
                : onClear,
            icon: const Icon(Icons.clear),
            label: const Text('All time'),
          ),
        ],
      ),
    );
  }
}

class _DateButton extends StatelessWidget {
  const _DateButton({
    required this.label,
    required this.value,
    required this.onPressed,
  });

  final String label;
  final DateTime? value;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    return OutlinedButton.icon(
      onPressed: onPressed,
      icon: const Icon(Icons.calendar_today_outlined),
      label: Text('$label: ${value == null ? 'All time' : dateValue(value)}'),
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

class _HorizontalBarChart extends StatelessWidget {
  const _HorizontalBarChart({
    required this.title,
    required this.valueLabel,
    required this.entries,
  });

  final String title;
  final String valueLabel;
  final List<_ChartEntry> entries;

  @override
  Widget build(BuildContext context) {
    final maxValue = entries.fold<int>(
      0,
      (max, item) => item.value > max ? item.value : max,
    );
    return _ReportPanel(
      title: title,
      child: entries.isEmpty
          ? const Text('No completed business data is available.')
          : Column(
              children: entries
                  .map(
                    (entry) => Padding(
                      padding: const EdgeInsets.only(bottom: 10),
                      child: LayoutBuilder(
                        builder: (context, constraints) {
                          final bar = _Bar(
                            entry: entry,
                            maxValue: maxValue,
                            valueLabel: valueLabel,
                          );
                          if (constraints.maxWidth < 560) {
                            return Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(entry.label),
                                const SizedBox(height: 4),
                                bar,
                              ],
                            );
                          }
                          return Row(
                            children: [
                              SizedBox(
                                width: 210,
                                child: Text(
                                  entry.label,
                                  maxLines: 2,
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(child: bar),
                            ],
                          );
                        },
                      ),
                    ),
                  )
                  .toList(),
            ),
    );
  }
}

class _Bar extends StatelessWidget {
  const _Bar({
    required this.entry,
    required this.maxValue,
    required this.valueLabel,
  });

  final _ChartEntry entry;
  final int maxValue;
  final String valueLabel;

  @override
  Widget build(BuildContext context) {
    final ratio = maxValue == 0 ? 0.0 : entry.value / maxValue;
    return Tooltip(
      message: '${entry.label}: ${entry.value} $valueLabel',
      child: Row(
        children: [
          Expanded(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: LinearProgressIndicator(
                minHeight: 18,
                value: ratio,
                backgroundColor: Theme.of(
                  context,
                ).colorScheme.surfaceContainerHighest,
              ),
            ),
          ),
          const SizedBox(width: 10),
          SizedBox(
            width: 42,
            child: Text(entry.value.toString(), textAlign: TextAlign.end),
          ),
        ],
      ),
    );
  }
}

class _WeeklyBarChart extends StatelessWidget {
  const _WeeklyBarChart({required this.entries});

  final List<WeeklyAttendance> entries;

  @override
  Widget build(BuildContext context) {
    final maxValue = entries.fold<int>(
      0,
      (max, item) => item.attendanceCount > max ? item.attendanceCount : max,
    );
    final visible = entries.length > 16
        ? entries.sublist(entries.length - 16)
        : entries;
    return _ReportPanel(
      title: 'Weekly attendance',
      child: visible.isEmpty
          ? const Text('No completed appointments are available.')
          : SizedBox(
              height: 230,
              child: SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: visible.map((entry) {
                    final ratio = maxValue == 0
                        ? 0.0
                        : entry.attendanceCount / maxValue;
                    return Tooltip(
                      message:
                          'Week of ${dateValue(entry.weekStart)}: ${entry.attendanceCount}',
                      child: SizedBox(
                        width: 72,
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            Text(entry.attendanceCount.toString()),
                            const SizedBox(height: 4),
                            Container(
                              width: 34,
                              height: 150 * ratio,
                              constraints: const BoxConstraints(minHeight: 2),
                              decoration: BoxDecoration(
                                color: Theme.of(context).colorScheme.primary,
                                borderRadius: const BorderRadius.vertical(
                                  top: Radius.circular(4),
                                ),
                              ),
                            ),
                            const SizedBox(height: 6),
                            Text(
                              DateFormat(
                                'MMM d',
                              ).format(entry.weekStart.toLocal()),
                              textAlign: TextAlign.center,
                              style: Theme.of(context).textTheme.bodySmall,
                            ),
                          ],
                        ),
                      ),
                    );
                  }).toList(),
                ),
              ),
            ),
    );
  }
}

class _ReportPanel extends StatelessWidget {
  const _ReportPanel({required this.title, required this.child});

  final String title;
  final Widget child;

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
            const SizedBox(height: 14),
            child,
          ],
        ),
      ),
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
    return _ReportPanel(
      title: title,
      child: rows.isEmpty
          ? const Text('No data available.')
          : SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: DataTable(
                columns: columns
                    .map((column) => DataColumn(label: Text(column)))
                    .toList(),
                rows: rows
                    .map(
                      (row) => DataRow(
                        cells: row.map((cell) => DataCell(Text(cell))).toList(),
                      ),
                    )
                    .toList(),
              ),
            ),
    );
  }
}

class _ErrorState extends StatelessWidget {
  const _ErrorState({required this.message, required this.onRetry});

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(message),
          const SizedBox(height: 12),
          FilledButton(onPressed: onRetry, child: const Text('Retry')),
        ],
      ),
    );
  }
}

class _ChartEntry {
  const _ChartEntry(this.label, this.value);

  final String label;
  final int value;
}

Future<Uint8List> _buildAppointmentPdf(BusinessAnalytics data) {
  final doc = pw.Document();
  doc.addPage(
    pw.MultiPage(
      pageFormat: PdfPageFormat.a4,
      build: (_) => [
        pw.Header(level: 0, text: 'Appointment Summary'),
        pw.Text('Generated: ${DateTime.now().toLocal()}'),
        pw.Text(_dateRangeLabel(data)),
        pw.SizedBox(height: 16),
        pw.Text('Completed appointments: ${data.completedAppointments}'),
        pw.SizedBox(height: 12),
        _pdfTable(
          'Appointment status counts',
          const ['Status', 'Count'],
          data.appointmentStatuses
              .map((item) => [item.name, item.count.toString()])
              .toList(),
        ),
        pw.SizedBox(height: 16),
        _pdfTable(
          'Service usage',
          const ['Service', 'Completed appointments'],
          data.serviceUsage
              .map(
                (item) => [item.serviceName, item.appointmentCount.toString()],
              )
              .toList(),
        ),
        pw.SizedBox(height: 16),
        _pdfTable(
          'Weekly attendance',
          const ['Week starting', 'Attendance'],
          data.weeklyAttendance
              .map(
                (item) => [
                  dateValue(item.weekStart),
                  item.attendanceCount.toString(),
                ],
              )
              .toList(),
        ),
        pw.SizedBox(height: 16),
        _pdfTable(
          'Employee workload',
          const ['Employee', 'Completed appointments'],
          data.employeeWorkload
              .map((item) => [item.name, item.count.toString()])
              .toList(),
        ),
      ],
    ),
  );
  return doc.save();
}

Future<Uint8List> _buildBusinessPdf(BusinessAnalytics data) {
  final doc = pw.Document();
  doc.addPage(
    pw.MultiPage(
      pageFormat: PdfPageFormat.a4,
      build: (_) => [
        pw.Header(level: 0, text: 'Business Statistics'),
        pw.Text('Generated: ${DateTime.now().toLocal()}'),
        pw.Text(_dateRangeLabel(data)),
        pw.SizedBox(height: 16),
        pw.TableHelper.fromTextArray(
          headers: const ['Metric', 'Value'],
          data: [
            ['Revenue', moneyValue(data.totalRevenue)],
            ['Completed purchases', data.completedPurchases.toString()],
            ['Completed appointments', data.completedAppointments.toString()],
            ['Unique clients', data.uniqueClients.toString()],
            ['Total users', data.totalUsers.toString()],
            ['Total employees', data.totalEmployees.toString()],
            ['Total appointments', data.totalAppointments.toString()],
            ['Total services', data.totalServices.toString()],
            ['Total products', data.totalProducts.toString()],
            ['Total purchases', data.totalPurchases.toString()],
          ],
        ),
        pw.SizedBox(height: 16),
        _pdfTable(
          'Best-selling products',
          const ['Product', 'Units sold', 'Revenue'],
          data.bestSellingProducts
              .map(
                (item) => [
                  item.productName,
                  item.quantitySold.toString(),
                  moneyValue(item.revenue),
                ],
              )
              .toList(),
        ),
        pw.SizedBox(height: 16),
        _pdfTable(
          'Client attendance',
          const ['Client', 'Completed appointments'],
          data.clientActivity
              .map((item) => [item.name, item.count.toString()])
              .toList(),
        ),
      ],
    ),
  );
  return doc.save();
}

pw.Widget _pdfTable(
  String title,
  List<String> headers,
  List<List<String>> rows,
) {
  return pw.Column(
    crossAxisAlignment: pw.CrossAxisAlignment.start,
    children: [
      pw.Text(title, style: pw.TextStyle(fontWeight: pw.FontWeight.bold)),
      pw.SizedBox(height: 6),
      if (rows.isEmpty)
        pw.Text('No data available.')
      else
        pw.TableHelper.fromTextArray(headers: headers, data: rows),
    ],
  );
}

String _dateRangeLabel(BusinessAnalytics data) {
  if (data.dateFrom == null && data.dateTo == null) {
    return 'Date range: All time';
  }
  return 'Date range: ${data.dateFrom == null ? 'Beginning' : dateValue(data.dateFrom)}'
      ' to ${data.dateTo == null ? 'Present' : dateValue(data.dateTo)}';
}
