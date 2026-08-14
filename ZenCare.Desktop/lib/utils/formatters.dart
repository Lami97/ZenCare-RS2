import 'package:intl/intl.dart';

String textValue(dynamic value) {
  if (value == null) return '-';
  if (value is bool) return value ? 'Yes' : 'No';
  if (value is num) return value.toString();
  final text = value.toString().trim();
  return text.isEmpty ? '-' : text;
}

String moneyValue(dynamic value) {
  final number = value is num
      ? value.toDouble()
      : double.tryParse(value?.toString() ?? '');
  if (number == null) return '-';
  return NumberFormat.currency(symbol: r'$', decimalDigits: 2).format(number);
}

String dateValue(dynamic value) {
  if (value == null) return '-';
  final date = value is DateTime ? value : DateTime.tryParse(value.toString());
  if (date == null) return textValue(value);
  return DateFormat('yyyy-MM-dd').format(date.toLocal());
}

String dateTimeValue(dynamic value) {
  if (value == null) return '-';
  final date = value is DateTime ? value : DateTime.tryParse(value.toString());
  if (date == null) return textValue(value);
  return DateFormat('yyyy-MM-dd HH:mm').format(date.toLocal());
}

String timeValue(dynamic value) {
  if (value == null) return '-';
  final text = value.toString();
  return text.length >= 5 ? text.substring(0, 5) : text;
}

String enumLabel(Map<int, String> labels, dynamic value) {
  final number = value is int ? value : int.tryParse(value?.toString() ?? '');
  return number == null
      ? textValue(value)
      : labels[number] ?? number.toString();
}

String displayName(Map<String, dynamic> item) {
  final fullName = item['fullName']?.toString();
  if (fullName != null && fullName.trim().isNotEmpty) return fullName;
  final first = item['firstName']?.toString() ?? '';
  final last = item['lastName']?.toString() ?? '';
  final combined = '$first $last'.trim();
  if (combined.isNotEmpty) return combined;
  for (final key in [
    'name',
    'username',
    'email',
    'purchaseNumber',
    'question',
    'title',
  ]) {
    final value = item[key]?.toString();
    if (value != null && value.trim().isNotEmpty) return value;
  }
  return 'Record ${item['id'] ?? ''}'.trim();
}
