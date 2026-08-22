typedef JsonMap = Map<String, dynamic>;

abstract interface class AdminEntity {
  int get id;

  Object? formValue(String key);
}

abstract interface class AdminWriteDto {
  JsonMap toJson();
}

int jsonInt(Object? value) =>
    value is num ? value.toInt() : int.tryParse(value?.toString() ?? '') ?? 0;

double jsonDouble(Object? value) => value is num
    ? value.toDouble()
    : double.tryParse(value?.toString() ?? '') ?? 0;

bool jsonBool(Object? value) =>
    value is bool ? value : value?.toString().toLowerCase() == 'true';

DateTime? jsonDateTime(Object? value) =>
    value == null ? null : DateTime.tryParse(value.toString());

class AdminFormValues {
  const AdminFormValues(this._values);

  final Map<String, Object?> _values;

  String? string(String key) {
    final text = _values[key]?.toString().trim();
    return text == null || text.isEmpty ? null : text;
  }

  String requiredString(String key) => string(key) ?? '';
  int? integer(String key) => _values[key] is int
      ? _values[key] as int
      : int.tryParse(_values[key]?.toString() ?? '');
  int requiredInteger(String key) => integer(key) ?? 0;
  double? decimal(String key) => _values[key] is num
      ? (_values[key] as num).toDouble()
      : double.tryParse(_values[key]?.toString() ?? '');
  double requiredDecimal(String key) => decimal(key) ?? 0;
  bool boolean(String key, {bool fallback = false}) =>
      _values[key] is bool ? _values[key] as bool : fallback;
  DateTime? date(String key) => jsonDateTime(_values[key]);
}

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
  const LookupConfig._({
    required this.endpoint,
    required this.decoder,
    required this.labelBuilder,
    this.queryParameters = const {},
  });

  final String endpoint;
  final AdminEntity Function(JsonMap json) decoder;
  final String Function(AdminEntity item) labelBuilder;
  final Map<String, Object?> queryParameters;
}

LookupConfig typedLookup<T extends AdminEntity>({
  required String endpoint,
  required T Function(JsonMap json) decoder,
  required String Function(T item) labelBuilder,
  Map<String, Object?> queryParameters = const {},
}) => LookupConfig._(
  endpoint: endpoint,
  decoder: decoder,
  labelBuilder: (item) => labelBuilder(item as T),
  queryParameters: queryParameters,
);

class LookupOption {
  const LookupOption({required this.value, required this.label});

  final int value;
  final String label;
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
  const AdminColumn._({required this.label, required this.value});

  final String label;
  final String Function(AdminEntity item) value;
}

AdminColumn typedColumn<T extends AdminEntity>({
  required String label,
  required String Function(T item) value,
}) => AdminColumn._(label: label, value: (item) => value(item as T));

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
    required this.decoder,
    this.buildInsert,
    required this.buildUpdate,
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
  final AdminEntity Function(JsonMap json) decoder;
  final AdminWriteDto Function(AdminFormValues values)? buildInsert;
  final AdminWriteDto Function(int id, AdminFormValues values) buildUpdate;
  final List<FilterField> filters;
  final String? searchKey;
  final String searchLabel;
  final double searchWidth;
  final bool canAdd;
  final bool canEdit;
  final bool canDelete;
  final String? detailsHint;
}
