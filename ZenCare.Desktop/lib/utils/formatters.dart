import 'package:intl/intl.dart';

String textValue(Object? value) {
  if (value == null) return '-';
  if (value is bool) return value ? 'Yes' : 'No';
  if (value is num) return value.toString();
  final text = value.toString().trim();
  return text.isEmpty ? '-' : text;
}

String moneyValue(Object? value) {
  final number = value is num
      ? value.toDouble()
      : double.tryParse(value?.toString() ?? '');
  if (number == null) return '-';
  return NumberFormat.currency(symbol: r'$', decimalDigits: 2).format(number);
}

String dateValue(Object? value) {
  if (value == null) return '-';
  final date = value is DateTime ? value : DateTime.tryParse(value.toString());
  if (date == null) return textValue(value);
  return DateFormat('yyyy-MM-dd').format(date.toLocal());
}

String dateTimeValue(Object? value) {
  if (value == null) return '-';
  final date = value is DateTime ? value : DateTime.tryParse(value.toString());
  if (date == null) return textValue(value);
  return DateFormat('yyyy-MM-dd HH:mm').format(date.toLocal());
}

String timeValue(Object? value) {
  if (value == null) return '-';
  final text = value.toString();
  return text.length >= 5 ? text.substring(0, 5) : text;
}

String enumLabel(Map<int, String> labels, Object? value) {
  final number = value is int ? value : int.tryParse(value?.toString() ?? '');
  return number == null
      ? textValue(value)
      : labels[number] ?? number.toString();
}
