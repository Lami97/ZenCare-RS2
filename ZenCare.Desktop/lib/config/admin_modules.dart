import '../models/admin_models.dart';
import '../models/appointment_dtos.dart';
import '../models/employee_dtos.dart';
import '../models/faq_dtos.dart';
import '../models/product_dtos.dart';
import '../models/purchase_dtos.dart';
import '../models/reference_dtos.dart';
import '../models/review_dtos.dart';
import '../models/service_dtos.dart';
import '../models/supplier_dtos.dart';
import '../models/time_slot_dtos.dart';
import '../models/user_dtos.dart';
import '../utils/formatters.dart';

const activeFilter = FilterField(
  key: 'IsActive',
  label: 'Status',
  isBoolean: true,
  booleanTrueLabel: 'Active',
  booleanFalseLabel: 'Inactive',
);

String _userLabel(UserDto user) =>
    user.fullName.isEmpty || user.fullName == user.username
    ? user.username
    : '${user.fullName} (${user.username})';

final usersLookup = typedLookup<UserDto>(
  endpoint: 'User',
  decoder: UserDto.fromJson,
  labelBuilder: _userLabel,
);
final appointmentClientsLookup = typedLookup<UserDto>(
  endpoint: 'User',
  decoder: UserDto.fromJson,
  labelBuilder: _userLabel,
  queryParameters: const {'IsClient': true},
);
final rolesLookup = typedLookup<RoleDto>(
  endpoint: 'Role',
  decoder: RoleDto.fromJson,
  labelBuilder: (item) => item.name,
);
final productCategoriesLookup = typedLookup<ProductCategoryDto>(
  endpoint: 'ProductCategory',
  decoder: ProductCategoryDto.fromJson,
  labelBuilder: (item) => item.name,
);
final productTypesLookup = typedLookup<ProductTypeDto>(
  endpoint: 'ProductType',
  decoder: ProductTypeDto.fromJson,
  labelBuilder: (item) => item.name,
);
final unitsLookup = typedLookup<UnitOfMeasureDto>(
  endpoint: 'UnitOfMeasure',
  decoder: UnitOfMeasureDto.fromJson,
  labelBuilder: (item) => item.name,
);
final suppliersLookup = typedLookup<SupplierDto>(
  endpoint: 'Supplier',
  decoder: SupplierDto.fromJson,
  labelBuilder: (item) => item.name,
);
final serviceCategoriesLookup = typedLookup<ServiceCategoryDto>(
  endpoint: 'ServiceCategory',
  decoder: ServiceCategoryDto.fromJson,
  labelBuilder: (item) => item.name,
);
final servicesLookup = typedLookup<WellnessServiceDto>(
  endpoint: 'Service',
  decoder: WellnessServiceDto.fromJson,
  labelBuilder: (item) => item.name,
);
final employeesLookup = typedLookup<EmployeeDto>(
  endpoint: 'Employee',
  decoder: EmployeeDto.fromJson,
  labelBuilder: (item) =>
      item.employeeName.isEmpty ? item.userName : item.employeeName,
);
final productsLookup = typedLookup<ProductDto>(
  endpoint: 'Product',
  decoder: ProductDto.fromJson,
  labelBuilder: (item) => item.name,
);
final purchasesLookup = typedLookup<PurchaseDto>(
  endpoint: 'Purchase',
  decoder: PurchaseDto.fromJson,
  labelBuilder: (item) => item.purchaseNumber,
);
final appointmentsLookup = typedLookup<AppointmentDto>(
  endpoint: 'Appointment',
  decoder: AppointmentDto.fromJson,
  labelBuilder: (item) =>
      '${item.userName} - ${item.serviceName} - ${dateValue(item.appointmentDate)}',
);
final completedAppointmentsLookup = typedLookup<AppointmentDto>(
  endpoint: 'Appointment',
  decoder: AppointmentDto.fromJson,
  labelBuilder: appointmentsLookup.labelBuilder,
  queryParameters: const {'Status': 4},
);
final faqCategoriesLookup = typedLookup<FaqCategoryDto>(
  endpoint: 'FAQCategory',
  decoder: FaqCategoryDto.fromJson,
  labelBuilder: (item) => item.name,
);

const appointmentStatuses = [
  StatusOption(1, 'Pending'),
  StatusOption(2, 'Confirmed'),
  StatusOption(3, 'Paid'),
  StatusOption(4, 'Completed'),
  StatusOption(5, 'Cancelled'),
  StatusOption(6, 'No-show'),
];
const reviewStatuses = [
  StatusOption(1, 'Pending approval'),
  StatusOption(2, 'Approved'),
  StatusOption(3, 'Rejected'),
];
const purchaseStatuses = [
  StatusOption(1, 'Draft'),
  StatusOption(2, 'Pending payment'),
  StatusOption(3, 'Paid'),
  StatusOption(4, 'Processing'),
  StatusOption(5, 'Ready for pickup'),
  StatusOption(6, 'Shipped'),
  StatusOption(7, 'Completed'),
  StatusOption(8, 'Cancelled'),
  StatusOption(9, 'Refunded'),
  StatusOption(10, 'Failed'),
];
const paymentStatuses = [
  StatusOption(1, 'Pending'),
  StatusOption(2, 'Succeeded'),
  StatusOption(3, 'Failed'),
  StatusOption(4, 'Cancelled'),
  StatusOption(5, 'Refunded'),
];

Map<int, String> _labels(List<StatusOption> options) => {
  for (final option in options) option.value: option.label,
};

const nameDescActiveFields = [
  AdminField(
    key: 'name',
    label: 'Name',
    type: AdminFieldType.text,
    required: true,
    maxLength: 100,
  ),
  AdminField(
    key: 'description',
    label: 'Description',
    type: AdminFieldType.multiline,
    maxLength: 500,
  ),
  AdminField(key: 'isActive', label: 'Active', type: AdminFieldType.boolean),
];

List<AdminColumn> _referenceColumns<T extends NamedReferenceDto>() => [
  typedColumn<T>(label: 'Name', value: (item) => textValue(item.name)),
  typedColumn<T>(
    label: 'Description',
    value: (item) => textValue(item.description),
  ),
  typedColumn<T>(label: 'Active', value: (item) => textValue(item.isActive)),
];

AdminModule _referenceModule<T extends NamedReferenceDto>({
  required String title,
  required String endpoint,
  required String entityName,
  required T Function(JsonMap) decoder,
  required AdminWriteDto Function(AdminFormValues) insert,
  required AdminWriteDto Function(int, AdminFormValues) update,
}) => AdminModule(
  title: title,
  endpoint: endpoint,
  entityName: entityName,
  searchKey: 'Name',
  searchLabel: 'Search by name',
  filters: const [activeFilter],
  columns: _referenceColumns<T>(),
  fields: nameDescActiveFields,
  decoder: decoder,
  buildInsert: insert,
  buildUpdate: update,
);

T _referenceInsert<T extends AdminWriteDto>(
  AdminFormValues values,
  T Function(String, String?, bool) create,
) => create(
  values.requiredString('name'),
  values.string('description'),
  values.boolean('isActive'),
);

T _referenceUpdate<T extends AdminWriteDto>(
  int id,
  AdminFormValues values,
  T Function(int, String, String?, bool) create,
) => create(
  id,
  values.requiredString('name'),
  values.string('description'),
  values.boolean('isActive'),
);

final adminModules = <AdminModule>[
  _referenceModule<ProductCategoryDto>(
    title: 'Product Categories',
    endpoint: 'ProductCategory',
    entityName: 'Product category',
    decoder: ProductCategoryDto.fromJson,
    insert: (v) => _referenceInsert(
      v,
      (name, description, active) => ProductCategoryInsertDto(
        name: name,
        description: description,
        isActive: active,
      ),
    ),
    update: (id, v) => _referenceUpdate(
      id,
      v,
      (id, name, description, active) => ProductCategoryUpdateDto(
        id: id,
        name: name,
        description: description,
        isActive: active,
      ),
    ),
  ),
  _referenceModule<ProductTypeDto>(
    title: 'Product Types',
    endpoint: 'ProductType',
    entityName: 'Product type',
    decoder: ProductTypeDto.fromJson,
    insert: (v) => _referenceInsert(
      v,
      (name, description, active) => ProductTypeInsertDto(
        name: name,
        description: description,
        isActive: active,
      ),
    ),
    update: (id, v) => _referenceUpdate(
      id,
      v,
      (id, name, description, active) => ProductTypeUpdateDto(
        id: id,
        name: name,
        description: description,
        isActive: active,
      ),
    ),
  ),
  _referenceModule<UnitOfMeasureDto>(
    title: 'Units of Measure',
    endpoint: 'UnitOfMeasure',
    entityName: 'unit of measure',
    decoder: UnitOfMeasureDto.fromJson,
    insert: (v) => _referenceInsert(
      v,
      (name, description, active) => UnitOfMeasureInsertDto(
        name: name,
        description: description,
        isActive: active,
      ),
    ),
    update: (id, v) => _referenceUpdate(
      id,
      v,
      (id, name, description, active) => UnitOfMeasureUpdateDto(
        id: id,
        name: name,
        description: description,
        isActive: active,
      ),
    ),
  ),
  AdminModule(
    title: 'Products',
    endpoint: 'Product',
    entityName: 'product',
    decoder: ProductDto.fromJson,
    searchKey: 'Name',
    searchLabel: 'Search by product name',
    filters: [
      activeFilter,
      FilterField(
        key: 'ProductCategoryId',
        label: 'Category',
        lookup: productCategoriesLookup,
      ),
      FilterField(
        key: 'ProductTypeId',
        label: 'Type',
        lookup: productTypesLookup,
      ),
      FilterField(
        key: 'SupplierId',
        label: 'Supplier',
        lookup: suppliersLookup,
      ),
    ],
    columns: [
      typedColumn<ProductDto>(label: 'Name', value: (x) => textValue(x.name)),
      typedColumn<ProductDto>(
        label: 'Category',
        value: (x) => textValue(x.productCategoryName),
      ),
      typedColumn<ProductDto>(
        label: 'Type',
        value: (x) => textValue(x.productTypeName),
      ),
      typedColumn<ProductDto>(
        label: 'Unit',
        value: (x) => textValue(x.unitOfMeasureName),
      ),
      typedColumn<ProductDto>(
        label: 'Supplier',
        value: (x) => textValue(x.supplierName),
      ),
      typedColumn<ProductDto>(
        label: 'Price',
        value: (x) => moneyValue(x.price),
      ),
      typedColumn<ProductDto>(
        label: 'Stock',
        value: (x) => textValue(x.stockQuantity),
      ),
      typedColumn<ProductDto>(
        label: 'Active',
        value: (x) => textValue(x.isActive),
      ),
    ],
    fields: [
      const AdminField(
        key: 'name',
        label: 'Name',
        type: AdminFieldType.text,
        required: true,
        maxLength: 150,
      ),
      const AdminField(
        key: 'description',
        label: 'Description',
        type: AdminFieldType.multiline,
        maxLength: 1000,
      ),
      const AdminField(
        key: 'price',
        label: 'Price',
        type: AdminFieldType.decimal,
        required: true,
        min: 0.01,
      ),
      const AdminField(
        key: 'stockQuantity',
        label: 'Stock quantity',
        type: AdminFieldType.integer,
        required: true,
        min: 0,
      ),
      AdminField(
        key: 'productCategoryId',
        label: 'Product category',
        type: AdminFieldType.lookup,
        required: true,
        lookup: productCategoriesLookup,
      ),
      AdminField(
        key: 'productTypeId',
        label: 'Product type',
        type: AdminFieldType.lookup,
        required: true,
        lookup: productTypesLookup,
      ),
      AdminField(
        key: 'unitOfMeasureId',
        label: 'Unit of measure',
        type: AdminFieldType.lookup,
        required: true,
        lookup: unitsLookup,
      ),
      AdminField(
        key: 'supplierId',
        label: 'Supplier',
        type: AdminFieldType.lookup,
        required: true,
        lookup: suppliersLookup,
      ),
      const AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => ProductInsertDto(
      name: v.requiredString('name'),
      description: v.string('description'),
      price: v.requiredDecimal('price'),
      stockQuantity: v.requiredInteger('stockQuantity'),
      productCategoryId: v.requiredInteger('productCategoryId'),
      productTypeId: v.requiredInteger('productTypeId'),
      unitOfMeasureId: v.requiredInteger('unitOfMeasureId'),
      supplierId: v.requiredInteger('supplierId'),
      isActive: v.boolean('isActive'),
    ),
    buildUpdate: (id, v) => ProductUpdateDto(
      id: id,
      name: v.requiredString('name'),
      description: v.string('description'),
      price: v.requiredDecimal('price'),
      stockQuantity: v.requiredInteger('stockQuantity'),
      productCategoryId: v.requiredInteger('productCategoryId'),
      productTypeId: v.requiredInteger('productTypeId'),
      unitOfMeasureId: v.requiredInteger('unitOfMeasureId'),
      supplierId: v.requiredInteger('supplierId'),
      isActive: v.boolean('isActive'),
    ),
  ),
  _referenceModule<ServiceCategoryDto>(
    title: 'Service Categories',
    endpoint: 'ServiceCategory',
    entityName: 'service category',
    decoder: ServiceCategoryDto.fromJson,
    insert: (v) => _referenceInsert(
      v,
      (name, description, active) => ServiceCategoryInsertDto(
        name: name,
        description: description,
        isActive: active,
      ),
    ),
    update: (id, v) => _referenceUpdate(
      id,
      v,
      (id, name, description, active) => ServiceCategoryUpdateDto(
        id: id,
        name: name,
        description: description,
        isActive: active,
      ),
    ),
  ),
  AdminModule(
    title: 'Services',
    endpoint: 'Service',
    entityName: 'service',
    decoder: WellnessServiceDto.fromJson,
    searchKey: 'Name',
    searchLabel: 'Search by service name',
    filters: [
      activeFilter,
      FilterField(
        key: 'ServiceCategoryId',
        label: 'Category',
        lookup: serviceCategoriesLookup,
      ),
    ],
    columns: [
      typedColumn<WellnessServiceDto>(
        label: 'Name',
        value: (x) => textValue(x.name),
      ),
      typedColumn<WellnessServiceDto>(
        label: 'Category',
        value: (x) => textValue(x.serviceCategoryName),
      ),
      typedColumn<WellnessServiceDto>(
        label: 'Duration',
        value: (x) => '${x.durationMinutes} min',
      ),
      typedColumn<WellnessServiceDto>(
        label: 'Price',
        value: (x) => moneyValue(x.price),
      ),
      typedColumn<WellnessServiceDto>(
        label: 'Active',
        value: (x) => textValue(x.isActive),
      ),
    ],
    fields: [
      const AdminField(
        key: 'name',
        label: 'Name',
        type: AdminFieldType.text,
        required: true,
        maxLength: 150,
      ),
      const AdminField(
        key: 'description',
        label: 'Description',
        type: AdminFieldType.multiline,
        maxLength: 1000,
      ),
      const AdminField(
        key: 'durationMinutes',
        label: 'Duration minutes',
        type: AdminFieldType.integer,
        required: true,
        min: 1,
      ),
      const AdminField(
        key: 'price',
        label: 'Price',
        type: AdminFieldType.decimal,
        required: true,
        min: 0.01,
      ),
      AdminField(
        key: 'serviceCategoryId',
        label: 'Service category',
        type: AdminFieldType.lookup,
        required: true,
        lookup: serviceCategoriesLookup,
      ),
      const AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => WellnessServiceInsertDto(
      name: v.requiredString('name'),
      description: v.string('description'),
      durationMinutes: v.requiredInteger('durationMinutes'),
      price: v.requiredDecimal('price'),
      serviceCategoryId: v.requiredInteger('serviceCategoryId'),
      isActive: v.boolean('isActive'),
    ),
    buildUpdate: (id, v) => WellnessServiceUpdateDto(
      id: id,
      name: v.requiredString('name'),
      description: v.string('description'),
      durationMinutes: v.requiredInteger('durationMinutes'),
      price: v.requiredDecimal('price'),
      serviceCategoryId: v.requiredInteger('serviceCategoryId'),
      isActive: v.boolean('isActive'),
    ),
  ),
  _referenceModule<FaqCategoryDto>(
    title: 'FAQ Categories',
    endpoint: 'FAQCategory',
    entityName: 'FAQ category',
    decoder: FaqCategoryDto.fromJson,
    insert: (v) => _referenceInsert(
      v,
      (name, description, active) => FaqCategoryInsertDto(
        name: name,
        description: description,
        isActive: active,
      ),
    ),
    update: (id, v) => _referenceUpdate(
      id,
      v,
      (id, name, description, active) => FaqCategoryUpdateDto(
        id: id,
        name: name,
        description: description,
        isActive: active,
      ),
    ),
  ),
  AdminModule(
    title: 'FAQ',
    endpoint: 'FAQ',
    entityName: 'FAQ',
    decoder: FaqDto.fromJson,
    searchKey: 'Question',
    searchLabel: 'Search by question',
    filters: [
      activeFilter,
      FilterField(
        key: 'FAQCategoryId',
        label: 'Category',
        lookup: faqCategoriesLookup,
      ),
    ],
    columns: [
      typedColumn<FaqDto>(
        label: 'Question',
        value: (x) => textValue(x.question),
      ),
      typedColumn<FaqDto>(
        label: 'Category',
        value: (x) => textValue(x.faqCategoryName),
      ),
      typedColumn<FaqDto>(
        label: 'Display order',
        value: (x) => textValue(x.displayOrder),
      ),
      typedColumn<FaqDto>(label: 'Active', value: (x) => textValue(x.isActive)),
    ],
    fields: [
      const AdminField(
        key: 'question',
        label: 'Question',
        type: AdminFieldType.multiline,
        required: true,
        maxLength: 250,
      ),
      const AdminField(
        key: 'answer',
        label: 'Answer',
        type: AdminFieldType.multiline,
        required: true,
        maxLength: 2000,
      ),
      AdminField(
        key: 'faqCategoryId',
        label: 'FAQ category',
        type: AdminFieldType.lookup,
        required: true,
        lookup: faqCategoriesLookup,
      ),
      const AdminField(
        key: 'displayOrder',
        label: 'Display order',
        type: AdminFieldType.integer,
        min: 0,
      ),
      const AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => FaqInsertDto(
      question: v.requiredString('question'),
      answer: v.requiredString('answer'),
      faqCategoryId: v.requiredInteger('faqCategoryId'),
      displayOrder: v.integer('displayOrder') ?? 0,
      isActive: v.boolean('isActive'),
    ),
    buildUpdate: (id, v) => FaqUpdateDto(
      id: id,
      question: v.requiredString('question'),
      answer: v.requiredString('answer'),
      faqCategoryId: v.requiredInteger('faqCategoryId'),
      displayOrder: v.integer('displayOrder') ?? 0,
      isActive: v.boolean('isActive'),
    ),
  ),
  AdminModule(
    title: 'Users',
    endpoint: 'User',
    entityName: 'user',
    decoder: UserDto.fromJson,
    searchKey: 'Username',
    searchLabel: 'Search by username',
    filters: const [activeFilter],
    columns: [
      typedColumn<UserDto>(label: 'Name', value: (x) => textValue(x.fullName)),
      typedColumn<UserDto>(
        label: 'Username',
        value: (x) => textValue(x.username),
      ),
      typedColumn<UserDto>(label: 'Email', value: (x) => textValue(x.email)),
      typedColumn<UserDto>(
        label: 'Phone',
        value: (x) => textValue(x.phoneNumber),
      ),
      typedColumn<UserDto>(
        label: 'Active',
        value: (x) => textValue(x.isActive),
      ),
    ],
    fields: const [
      AdminField(
        key: 'firstName',
        label: 'First name',
        type: AdminFieldType.text,
        required: true,
        maxLength: 50,
      ),
      AdminField(
        key: 'lastName',
        label: 'Last name',
        type: AdminFieldType.text,
        required: true,
        maxLength: 50,
      ),
      AdminField(
        key: 'email',
        label: 'Email',
        type: AdminFieldType.text,
        required: true,
        maxLength: 100,
        helperText: 'Format: user@example.com',
      ),
      AdminField(
        key: 'username',
        label: 'Username',
        type: AdminFieldType.text,
        required: true,
        maxLength: 100,
      ),
      AdminField(
        key: 'phoneNumber',
        label: 'Phone number',
        type: AdminFieldType.text,
        maxLength: 20,
        helperText: 'Example: 061666999 (9 or 10 digits, numbers only).',
      ),
      AdminField(
        key: 'password',
        label: 'Password',
        type: AdminFieldType.text,
        required: true,
        helperText: 'Minimum 6 characters.',
        createOnly: true,
      ),
      AdminField(
        key: 'passwordConfirm',
        label: 'Confirm password',
        type: AdminFieldType.text,
        required: true,
        helperText: 'Must match the password.',
        createOnly: true,
      ),
      AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => UserInsertDto(
      firstName: v.requiredString('firstName'),
      lastName: v.requiredString('lastName'),
      email: v.requiredString('email'),
      username: v.requiredString('username'),
      password: v.requiredString('password'),
      passwordConfirm: v.requiredString('passwordConfirm'),
      phoneNumber: v.string('phoneNumber'),
      isActive: v.boolean('isActive'),
    ),
    buildUpdate: (id, v) => UserUpdateDto(
      id: id,
      firstName: v.requiredString('firstName'),
      lastName: v.requiredString('lastName'),
      email: v.requiredString('email'),
      username: v.requiredString('username'),
      phoneNumber: v.string('phoneNumber'),
      isActive: v.boolean('isActive'),
    ),
  ),
  AdminModule(
    title: 'User Roles',
    endpoint: 'UserRole',
    entityName: 'user role',
    decoder: UserRoleDto.fromJson,
    searchKey: 'Username',
    searchLabel: 'Search by username',
    filters: [FilterField(key: 'RoleId', label: 'Role', lookup: rolesLookup)],
    columns: [
      typedColumn<UserRoleDto>(
        label: 'Username',
        value: (x) => textValue(x.userName),
      ),
      typedColumn<UserRoleDto>(
        label: 'Role',
        value: (x) => textValue(x.roleName),
      ),
    ],
    fields: [
      AdminField(
        key: 'userId',
        label: 'User',
        type: AdminFieldType.lookup,
        required: true,
        lookup: usersLookup,
      ),
      AdminField(
        key: 'roleId',
        label: 'Role',
        type: AdminFieldType.lookup,
        required: true,
        lookup: rolesLookup,
      ),
    ],
    buildInsert: (v) => UserRoleInsertDto(
      userId: v.requiredInteger('userId'),
      roleId: v.requiredInteger('roleId'),
    ),
    buildUpdate: (id, v) => UserRoleUpdateDto(
      id: id,
      userId: v.requiredInteger('userId'),
      roleId: v.requiredInteger('roleId'),
    ),
  ),
  AdminModule(
    title: 'Employees',
    endpoint: 'Employee',
    entityName: 'employee',
    decoder: EmployeeDto.fromJson,
    searchKey: 'SearchTerm',
    searchLabel: 'Search by name / specialization',
    searchWidth: 340,
    filters: const [
      FilterField(
        key: 'IsAvailable',
        label: 'Availability',
        isBoolean: true,
        booleanTrueLabel: 'Available',
        booleanFalseLabel: 'Unavailable',
      ),
    ],
    columns: [
      typedColumn<EmployeeDto>(
        label: 'Name',
        value: (x) =>
            textValue(x.employeeName.isEmpty ? x.userName : x.employeeName),
      ),
      typedColumn<EmployeeDto>(
        label: 'Specialization',
        value: (x) => textValue(x.specialization),
      ),
      typedColumn<EmployeeDto>(label: 'Bio', value: (x) => textValue(x.bio)),
      typedColumn<EmployeeDto>(
        label: 'Hire date',
        value: (x) => dateValue(x.hireDate),
      ),
      typedColumn<EmployeeDto>(
        label: 'Available',
        value: (x) => textValue(x.isAvailable),
      ),
    ],
    fields: [
      AdminField(
        key: 'userId',
        label: 'User',
        type: AdminFieldType.lookup,
        required: true,
        lookup: usersLookup,
      ),
      const AdminField(
        key: 'specialization',
        label: 'Specialization',
        type: AdminFieldType.text,
        maxLength: 100,
      ),
      const AdminField(
        key: 'bio',
        label: 'Bio',
        type: AdminFieldType.multiline,
        maxLength: 1000,
      ),
      const AdminField(
        key: 'hireDate',
        label: 'Hire date',
        type: AdminFieldType.date,
        disallowFutureDates: true,
      ),
      const AdminField(
        key: 'isAvailable',
        label: 'Available',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => EmployeeInsertDto(
      userId: v.requiredInteger('userId'),
      specialization: v.string('specialization'),
      bio: v.string('bio'),
      hireDate: v.date('hireDate'),
      isAvailable: v.boolean('isAvailable'),
    ),
    buildUpdate: (id, v) => EmployeeUpdateDto(
      id: id,
      userId: v.requiredInteger('userId'),
      specialization: v.string('specialization'),
      bio: v.string('bio'),
      hireDate: v.date('hireDate'),
      isAvailable: v.boolean('isAvailable'),
    ),
  ),
  AdminModule(
    title: 'Service Assignments',
    endpoint: 'EmployeeService',
    entityName: 'service assignment',
    decoder: EmployeeServiceDto.fromJson,
    filters: [
      FilterField(
        key: 'EmployeeId',
        label: 'Employee',
        lookup: employeesLookup,
      ),
      FilterField(
        key: 'WellnessServiceId',
        label: 'Service',
        lookup: servicesLookup,
      ),
      const FilterField(key: 'IsActive', label: 'Active', isBoolean: true),
    ],
    columns: [
      typedColumn<EmployeeServiceDto>(
        label: 'Employee',
        value: (x) => textValue(x.employeeName),
      ),
      typedColumn<EmployeeServiceDto>(
        label: 'Service',
        value: (x) => textValue(x.serviceName),
      ),
      typedColumn<EmployeeServiceDto>(
        label: 'Active',
        value: (x) => textValue(x.isActive),
      ),
    ],
    fields: [
      AdminField(
        key: 'employeeId',
        label: 'Employee',
        type: AdminFieldType.lookup,
        required: true,
        lookup: employeesLookup,
      ),
      AdminField(
        key: 'wellnessServiceId',
        label: 'Service',
        type: AdminFieldType.lookup,
        required: true,
        lookup: servicesLookup,
      ),
      const AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => EmployeeServiceInsertDto(
      employeeId: v.requiredInteger('employeeId'),
      wellnessServiceId: v.requiredInteger('wellnessServiceId'),
      isActive: v.boolean('isActive'),
    ),
    buildUpdate: (id, v) => EmployeeServiceUpdateDto(
      id: id,
      employeeId: v.requiredInteger('employeeId'),
      wellnessServiceId: v.requiredInteger('wellnessServiceId'),
      isActive: v.boolean('isActive'),
    ),
  ),
  AdminModule(
    title: 'Schedule',
    endpoint: 'TimeSlot',
    entityName: 'schedule entry',
    decoder: TimeSlotDto.fromJson,
    filters: [
      FilterField(
        key: 'EmployeeId',
        label: 'Employee',
        lookup: employeesLookup,
      ),
      FilterField(
        key: 'WellnessServiceId',
        label: 'Service',
        lookup: servicesLookup,
      ),
      const FilterField(
        key: 'Status',
        label: 'Status',
        statusOptions: [
          StatusOption(1, 'Available'),
          StatusOption(2, 'Booked'),
          StatusOption(3, 'Inactive'),
          StatusOption(4, 'Expired'),
        ],
      ),
    ],
    columns: [
      typedColumn<TimeSlotDto>(
        label: 'Employee',
        value: (item) => textValue(item.employeeName),
      ),
      typedColumn<TimeSlotDto>(
        label: 'Service',
        value: (item) => textValue(item.serviceName),
      ),
      typedColumn<TimeSlotDto>(
        label: 'Date',
        value: (item) => dateValue(item.slotDate),
      ),
      typedColumn<TimeSlotDto>(
        label: 'Start',
        value: (item) => timeValue(item.startTime),
      ),
      typedColumn<TimeSlotDto>(
        label: 'End',
        value: (item) => timeValue(item.endTime),
      ),
      typedColumn<TimeSlotDto>(
        label: 'Status',
        value: (item) => textValue(item.status),
      ),
    ],
    fields: [
      AdminField(
        key: 'employeeId',
        label: 'Employee',
        type: AdminFieldType.lookup,
        required: true,
        lookup: employeesLookup,
      ),
      AdminField(
        key: 'wellnessServiceId',
        label: 'Service',
        type: AdminFieldType.lookup,
        required: true,
        lookup: servicesLookup,
      ),
      const AdminField(
        key: 'slotDate',
        label: 'Date',
        type: AdminFieldType.date,
        required: true,
      ),
      const AdminField(
        key: 'startTime',
        label: 'Start time',
        type: AdminFieldType.time,
        required: true,
      ),
      const AdminField(
        key: 'endTime',
        label: 'End time',
        type: AdminFieldType.time,
        required: true,
      ),
      const AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (values) => TimeSlotInsertDto(
      employeeId: values.requiredInteger('employeeId'),
      wellnessServiceId: values.requiredInteger('wellnessServiceId'),
      slotDate: values.date('slotDate')!,
      startTime: values.requiredString('startTime'),
      endTime: values.requiredString('endTime'),
      isActive: values.boolean('isActive'),
    ),
    buildUpdate: (id, values) => TimeSlotUpdateDto(
      id: id,
      employeeId: values.requiredInteger('employeeId'),
      wellnessServiceId: values.requiredInteger('wellnessServiceId'),
      slotDate: values.date('slotDate')!,
      startTime: values.requiredString('startTime'),
      endTime: values.requiredString('endTime'),
      isActive: values.boolean('isActive'),
    ),
  ),
  AdminModule(
    title: 'Reservations',
    endpoint: 'Appointment',
    entityName: 'reservation',
    decoder: AppointmentDto.fromJson,
    canAdd: false,
    canDelete: false,
    filters: [
      FilterField(
        key: 'UserId',
        label: 'Client',
        lookup: appointmentClientsLookup,
      ),
      FilterField(
        key: 'EmployeeId',
        label: 'Employee',
        lookup: employeesLookup,
      ),
      FilterField(
        key: 'WellnessServiceId',
        label: 'Service',
        lookup: servicesLookup,
      ),
      const FilterField(
        key: 'Status',
        label: 'Status',
        statusOptions: appointmentStatuses,
      ),
    ],
    columns: [
      typedColumn<AppointmentDto>(
        label: 'Client',
        value: (x) => textValue(x.userName),
      ),
      typedColumn<AppointmentDto>(
        label: 'Employee',
        value: (x) => textValue(x.employeeName),
      ),
      typedColumn<AppointmentDto>(
        label: 'Service',
        value: (x) => textValue(x.serviceName),
      ),
      typedColumn<AppointmentDto>(
        label: 'Category',
        value: (x) => textValue(x.serviceCategoryName),
      ),
      typedColumn<AppointmentDto>(
        label: 'Date',
        value: (x) => dateValue(x.appointmentDate),
      ),
      typedColumn<AppointmentDto>(
        label: 'Start',
        value: (x) => timeValue(x.startTime),
      ),
      typedColumn<AppointmentDto>(
        label: 'End',
        value: (x) => timeValue(x.endTime),
      ),
      typedColumn<AppointmentDto>(
        label: 'Status',
        value: (x) => enumLabel(_labels(appointmentStatuses), x.status),
      ),
      typedColumn<AppointmentDto>(
        label: 'Notes',
        value: (x) => textValue(x.notes),
      ),
    ],
    fields: [
      AdminField(
        key: 'userId',
        label: 'Client',
        type: AdminFieldType.lookup,
        required: true,
        lookup: appointmentClientsLookup,
        readOnly: true,
      ),
      AdminField(
        key: 'employeeId',
        label: 'Employee',
        type: AdminFieldType.lookup,
        required: true,
        lookup: employeesLookup,
        readOnly: true,
      ),
      AdminField(
        key: 'wellnessServiceId',
        label: 'Service',
        type: AdminFieldType.lookup,
        required: true,
        lookup: servicesLookup,
        readOnly: true,
      ),
      const AdminField(
        key: 'appointmentDate',
        label: 'Appointment date',
        type: AdminFieldType.date,
        required: true,
        readOnly: true,
      ),
      const AdminField(
        key: 'startTime',
        label: 'Start time',
        type: AdminFieldType.time,
        required: true,
        readOnly: true,
      ),
      const AdminField(
        key: 'endTime',
        label: 'End time',
        type: AdminFieldType.time,
        required: true,
        readOnly: true,
      ),
      const AdminField(
        key: 'status',
        label: 'Status',
        type: AdminFieldType.status,
        statusOptions: appointmentStatuses,
      ),
      const AdminField(
        key: 'notes',
        label: 'Notes',
        type: AdminFieldType.multiline,
        maxLength: 1000,
      ),
      const AdminField(
        key: 'cancellationReason',
        label: 'Cancellation reason',
        type: AdminFieldType.multiline,
        maxLength: 500,
      ),
    ],
    buildUpdate: (id, v) => AppointmentUpdateDto(
      id: id,
      userId: v.requiredInteger('userId'),
      employeeId: v.requiredInteger('employeeId'),
      wellnessServiceId: v.requiredInteger('wellnessServiceId'),
      appointmentDate: v.date('appointmentDate')!,
      startTime: v.requiredString('startTime'),
      endTime: v.requiredString('endTime'),
      status: v.integer('status') ?? 1,
      notes: v.string('notes'),
      cancellationReason: v.string('cancellationReason'),
    ),
  ),
  AdminModule(
    title: 'Reviews',
    endpoint: 'Review',
    entityName: 'review',
    decoder: ReviewDto.fromJson,
    filters: [
      FilterField(
        key: 'UserId',
        label: 'User',
        lookup: appointmentClientsLookup,
      ),
      const FilterField(
        key: 'Status',
        label: 'Status',
        statusOptions: reviewStatuses,
      ),
    ],
    columns: [
      typedColumn<ReviewDto>(
        label: 'User',
        value: (x) => textValue(x.userName),
      ),
      typedColumn<ReviewDto>(
        label: 'Target',
        value: (x) => textValue(x.targetName),
      ),
      typedColumn<ReviewDto>(
        label: 'Rating',
        value: (x) => textValue(x.rating),
      ),
      typedColumn<ReviewDto>(
        label: 'Comment',
        value: (x) => textValue(x.comment),
      ),
      typedColumn<ReviewDto>(
        label: 'Status',
        value: (x) => enumLabel(_labels(reviewStatuses), x.status),
      ),
    ],
    fields: [
      AdminField(
        key: 'userId',
        label: 'User',
        type: AdminFieldType.lookup,
        required: true,
        lookup: appointmentClientsLookup,
      ),
      AdminField(
        key: 'appointmentId',
        label: 'Completed appointment',
        type: AdminFieldType.lookup,
        lookup: completedAppointmentsLookup,
        dependsOn: 'userId',
        dependencyQueryKey: 'UserId',
        helperText: 'Use only for an eligible completed appointment.',
      ),
      AdminField(
        key: 'productId',
        label: 'Product',
        type: AdminFieldType.lookup,
        lookup: productsLookup,
      ),
      const AdminField(
        key: 'rating',
        label: 'Rating',
        type: AdminFieldType.integer,
        required: true,
        min: 1,
      ),
      const AdminField(
        key: 'comment',
        label: 'Comment',
        type: AdminFieldType.multiline,
        maxLength: 1000,
      ),
      const AdminField(
        key: 'status',
        label: 'Status',
        type: AdminFieldType.status,
        statusOptions: reviewStatuses,
      ),
    ],
    buildInsert: (v) => ReviewInsertDto(
      userId: v.requiredInteger('userId'),
      appointmentId: v.integer('appointmentId'),
      productId: v.integer('productId'),
      rating: v.requiredInteger('rating'),
      comment: v.string('comment'),
      status: v.integer('status') ?? 1,
    ),
    buildUpdate: (id, v) => ReviewUpdateDto(
      id: id,
      userId: v.requiredInteger('userId'),
      appointmentId: v.integer('appointmentId'),
      productId: v.integer('productId'),
      rating: v.requiredInteger('rating'),
      comment: v.string('comment'),
      status: v.integer('status') ?? 1,
    ),
  ),
  AdminModule(
    title: 'Purchases',
    endpoint: 'Purchase',
    entityName: 'purchase',
    decoder: PurchaseDto.fromJson,
    canAdd: false,
    canDelete: false,
    searchKey: 'PurchaseNumber',
    searchLabel: 'Search by purchase number',
    filters: [
      FilterField(key: 'UserId', label: 'Client', lookup: usersLookup),
      const FilterField(
        key: 'Status',
        label: 'Purchase status',
        statusOptions: purchaseStatuses,
      ),
      const FilterField(
        key: 'PaymentStatus',
        label: 'Payment status',
        statusOptions: paymentStatuses,
      ),
    ],
    columns: [
      typedColumn<PurchaseDto>(
        label: 'Client',
        value: (x) => textValue(x.userName),
      ),
      typedColumn<PurchaseDto>(
        label: 'Purchase number',
        value: (x) => textValue(x.purchaseNumber),
      ),
      typedColumn<PurchaseDto>(
        label: 'Total',
        value: (x) => moneyValue(x.totalAmount),
      ),
      typedColumn<PurchaseDto>(
        label: 'Purchase status',
        value: (x) => enumLabel(_labels(purchaseStatuses), x.status),
      ),
      typedColumn<PurchaseDto>(
        label: 'Payment status',
        value: (x) => enumLabel(_labels(paymentStatuses), x.paymentStatus),
      ),
      typedColumn<PurchaseDto>(
        label: 'Paid at',
        value: (x) => dateTimeValue(x.paidAt),
      ),
    ],
    fields: [
      AdminField(
        key: 'userId',
        label: 'Client',
        type: AdminFieldType.lookup,
        required: true,
        lookup: usersLookup,
      ),
      const AdminField(
        key: 'purchaseNumber',
        label: 'Purchase number',
        type: AdminFieldType.text,
        required: true,
        maxLength: 30,
      ),
      const AdminField(
        key: 'totalAmount',
        label: 'Total amount',
        type: AdminFieldType.decimal,
        required: true,
        min: 0,
        readOnly: true,
      ),
      const AdminField(
        key: 'status',
        label: 'Purchase status',
        type: AdminFieldType.status,
        statusOptions: purchaseStatuses,
      ),
      const AdminField(
        key: 'paymentStatus',
        label: 'Payment status',
        type: AdminFieldType.status,
        statusOptions: paymentStatuses,
        readOnly: true,
      ),
      const AdminField(
        key: 'paidAt',
        label: 'Paid at',
        type: AdminFieldType.date,
        readOnly: true,
      ),
    ],
    buildUpdate: (id, v) => PurchaseUpdateDto(
      id: id,
      userId: v.requiredInteger('userId'),
      purchaseNumber: v.requiredString('purchaseNumber'),
      status: v.integer('status') ?? 1,
      paymentStatus: v.integer('paymentStatus') ?? 1,
      totalAmount: v.requiredDecimal('totalAmount'),
      paidAt: v.date('paidAt'),
    ),
  ),
  AdminModule(
    title: 'Purchase Items',
    endpoint: 'PurchaseItem',
    entityName: 'purchase item',
    decoder: PurchaseItemDto.fromJson,
    filters: [
      FilterField(
        key: 'PurchaseId',
        label: 'Purchase',
        lookup: purchasesLookup,
      ),
      FilterField(key: 'ProductId', label: 'Product', lookup: productsLookup),
    ],
    columns: [
      typedColumn<PurchaseItemDto>(
        label: 'Purchase',
        value: (x) => textValue(x.purchaseNumber),
      ),
      typedColumn<PurchaseItemDto>(
        label: 'Product',
        value: (x) => textValue(x.productName),
      ),
      typedColumn<PurchaseItemDto>(
        label: 'Quantity',
        value: (x) => textValue(x.quantity),
      ),
      typedColumn<PurchaseItemDto>(
        label: 'Unit price',
        value: (x) => moneyValue(x.unitPrice),
      ),
      typedColumn<PurchaseItemDto>(
        label: 'Total',
        value: (x) => moneyValue(x.totalPrice),
      ),
    ],
    fields: [
      AdminField(
        key: 'purchaseId',
        label: 'Purchase',
        type: AdminFieldType.lookup,
        required: true,
        lookup: purchasesLookup,
      ),
      AdminField(
        key: 'productId',
        label: 'Product',
        type: AdminFieldType.lookup,
        required: true,
        lookup: productsLookup,
      ),
      const AdminField(
        key: 'quantity',
        label: 'Quantity',
        type: AdminFieldType.integer,
        required: true,
        min: 1,
      ),
      const AdminField(
        key: 'unitPrice',
        label: 'Unit price',
        type: AdminFieldType.decimal,
        required: true,
        min: 0.01,
        readOnly: true,
      ),
      const AdminField(
        key: 'totalPrice',
        label: 'Total price',
        type: AdminFieldType.decimal,
        required: true,
        min: 0.01,
        readOnly: true,
      ),
    ],
    buildInsert: (v) => PurchaseItemInsertDto(
      purchaseId: v.requiredInteger('purchaseId'),
      productId: v.requiredInteger('productId'),
      quantity: v.requiredInteger('quantity'),
      unitPrice: v.requiredDecimal('unitPrice'),
      totalPrice: v.requiredDecimal('totalPrice'),
    ),
    buildUpdate: (id, v) => PurchaseItemUpdateDto(
      id: id,
      purchaseId: v.requiredInteger('purchaseId'),
      productId: v.requiredInteger('productId'),
      quantity: v.requiredInteger('quantity'),
      unitPrice: v.requiredDecimal('unitPrice'),
      totalPrice: v.requiredDecimal('totalPrice'),
    ),
  ),
  AdminModule(
    title: 'Suppliers',
    endpoint: 'Supplier',
    entityName: 'supplier',
    decoder: SupplierDto.fromJson,
    searchKey: 'Name',
    searchLabel: 'Search by supplier name',
    filters: const [activeFilter],
    columns: [
      typedColumn<SupplierDto>(label: 'Name', value: (x) => textValue(x.name)),
      typedColumn<SupplierDto>(
        label: 'Email',
        value: (x) => textValue(x.contactEmail),
      ),
      typedColumn<SupplierDto>(
        label: 'Phone',
        value: (x) => textValue(x.phoneNumber),
      ),
      typedColumn<SupplierDto>(
        label: 'Address',
        value: (x) => textValue(x.address),
      ),
      typedColumn<SupplierDto>(
        label: 'Active',
        value: (x) => textValue(x.isActive),
      ),
    ],
    fields: const [
      AdminField(
        key: 'name',
        label: 'Name',
        type: AdminFieldType.text,
        required: true,
        maxLength: 100,
      ),
      AdminField(
        key: 'contactEmail',
        label: 'Contact email',
        type: AdminFieldType.text,
        maxLength: 150,
        helperText: 'Format: user@example.com',
      ),
      AdminField(
        key: 'phoneNumber',
        label: 'Phone number',
        type: AdminFieldType.text,
        maxLength: 30,
        helperText: 'Digits and common phone separators only.',
      ),
      AdminField(
        key: 'address',
        label: 'Address',
        type: AdminFieldType.multiline,
        maxLength: 500,
      ),
      AdminField(
        key: 'isActive',
        label: 'Active',
        type: AdminFieldType.boolean,
      ),
    ],
    buildInsert: (v) => SupplierInsertDto(
      name: v.requiredString('name'),
      contactEmail: v.string('contactEmail'),
      phoneNumber: v.string('phoneNumber'),
      address: v.string('address'),
      isActive: v.boolean('isActive'),
    ),
    buildUpdate: (id, v) => SupplierUpdateDto(
      id: id,
      name: v.requiredString('name'),
      contactEmail: v.string('contactEmail'),
      phoneNumber: v.string('phoneNumber'),
      address: v.string('address'),
      isActive: v.boolean('isActive'),
    ),
  ),
];
