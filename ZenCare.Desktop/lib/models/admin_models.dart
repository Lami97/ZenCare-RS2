import '../utils/formatters.dart';

enum AdminFieldType {
  text,
  multiline,
  integer,
  decimal,
  boolean,
  date,
  time,
  lookup,
  status,
}

class StatusOption {
  const StatusOption(this.value, this.label);

  final int value;
  final String label;
}

class LookupConfig {
  const LookupConfig({
    required this.endpoint,
    required this.valueKey,
    required this.labelBuilder,
    this.queryParameters = const {},
  });

  final String endpoint;
  final String valueKey;
  final String Function(Map<String, dynamic> item) labelBuilder;
  final Map<String, dynamic> queryParameters;
}

class LookupOption {
  const LookupOption({
    required this.value,
    required this.label,
    required this.raw,
  });

  final int value;
  final String label;
  final Map<String, dynamic> raw;
}

class AdminField {
  const AdminField({
    required this.key,
    required this.label,
    required this.type,
    this.required = false,
    this.maxLength,
    this.min,
    this.lookup,
    this.statusOptions = const [],
    this.helperText,
    this.createOnly = false,
    this.readOnly = false,
    this.disallowFutureDates = false,
    this.dependsOn,
    this.dependencyQueryKey,
  });

  final String key;
  final String label;
  final AdminFieldType type;
  final bool required;
  final int? maxLength;
  final num? min;
  final LookupConfig? lookup;
  final List<StatusOption> statusOptions;
  final String? helperText;
  final bool createOnly;
  final bool readOnly;
  final bool disallowFutureDates;
  final String? dependsOn;
  final String? dependencyQueryKey;
}

class AdminColumn {
  const AdminColumn({required this.label, required this.value});

  final String label;
  final String Function(Map<String, dynamic> item) value;
}

class FilterField {
  const FilterField({
    required this.key,
    required this.label,
    this.lookup,
    this.statusOptions = const [],
    this.isBoolean = false,
    this.booleanTrueLabel = 'Active',
    this.booleanFalseLabel = 'Inactive',
  });

  final String key;
  final String label;
  final LookupConfig? lookup;
  final List<StatusOption> statusOptions;
  final bool isBoolean;
  final String booleanTrueLabel;
  final String booleanFalseLabel;
}

class AdminModule {
  const AdminModule({
    required this.title,
    required this.endpoint,
    required this.entityName,
    required this.columns,
    required this.fields,
    this.filters = const [],
    this.searchKey,
    this.searchLabel = 'Search',
    this.searchWidth = 280,
    this.canAdd = true,
    this.canEdit = true,
    this.canDelete = true,
    this.detailsHint,
  });

  final String title;
  final String endpoint;
  final String entityName;
  final List<AdminColumn> columns;
  final List<AdminField> fields;
  final List<FilterField> filters;
  final String? searchKey;
  final String searchLabel;
  final double searchWidth;
  final bool canAdd;
  final bool canEdit;
  final bool canDelete;
  final String? detailsHint;
}

String itemLabel(Map<String, dynamic> item) => displayName(item);
