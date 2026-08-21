import '../models/admin_models.dart';
import '../utils/formatters.dart';

const activeFilter = FilterField(
  key: 'IsActive',
  label: 'Status',
  isBoolean: true,
  booleanTrueLabel: 'Active',
  booleanFalseLabel: 'Inactive',
);

String _userLookupLabel(Map<String, dynamic> item) {
  final name = displayName(item);
  final username = item['username']?.toString();
  return username == null || username.isEmpty || username == name
      ? name
      : '$name ($username)';
}

final usersLookup = LookupConfig(
  endpoint: 'User',
  valueKey: 'id',
  labelBuilder: _userLookupLabel,
);

final appointmentClientsLookup = LookupConfig(
  endpoint: 'User',
  valueKey: 'id',
  labelBuilder: _userLookupLabel,
  queryParameters: const {'IsClient': true},
);

final rolesLookup = LookupConfig(
  endpoint: 'Role',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final productCategoriesLookup = LookupConfig(
  endpoint: 'ProductCategory',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final productTypesLookup = LookupConfig(
  endpoint: 'ProductType',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final unitsLookup = LookupConfig(
  endpoint: 'UnitOfMeasure',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final suppliersLookup = LookupConfig(
  endpoint: 'Supplier',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final serviceCategoriesLookup = LookupConfig(
  endpoint: 'ServiceCategory',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final servicesLookup = LookupConfig(
  endpoint: 'Service',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final employeesLookup = LookupConfig(
  endpoint: 'Employee',
  valueKey: 'id',
  labelBuilder: (item) {
    final employeeName = item['employeeName']?.toString().trim();
    if (employeeName != null && employeeName.isNotEmpty) return employeeName;
    return textValue(item['userName']);
  },
);
final productsLookup = LookupConfig(
  endpoint: 'Product',
  valueKey: 'id',
  labelBuilder: itemLabel,
);
final purchasesLookup = LookupConfig(
  endpoint: 'Purchase',
  valueKey: 'id',
  labelBuilder: (item) => textValue(item['purchaseNumber'] ?? item['id']),
);
final appointmentsLookup = LookupConfig(
  endpoint: 'Appointment',
  valueKey: 'id',
  labelBuilder: (item) {
    final client = textValue(item['userName']);
    final service = textValue(item['serviceName']);
    final date = dateValue(item['appointmentDate']);
    return '$client - $service - $date';
  },
);
final completedAppointmentsLookup = LookupConfig(
  endpoint: 'Appointment',
  valueKey: 'id',
  labelBuilder: appointmentsLookup.labelBuilder,
  queryParameters: const {'Status': 4},
);
final faqCategoriesLookup = LookupConfig(
  endpoint: 'FAQCategory',
  valueKey: 'id',
  labelBuilder: itemLabel,
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

List<AdminColumn> nameDescActiveColumns() => [
  AdminColumn(label: 'Name', value: (x) => textValue(x['name'])),
  AdminColumn(
    label: 'Description',
    value: (x) => textValue(x['description'] ?? x['abbreviation']),
  ),
  AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
];

final adminModules = <AdminModule>[
  AdminModule(
    title: 'Product Categories',
    endpoint: 'ProductCategory',
    entityName: 'Product category',
    searchKey: 'Name',
    searchLabel: 'Search by name',
    filters: const [activeFilter],
    columns: nameDescActiveColumns(),
    fields: nameDescActiveFields,
  ),
  AdminModule(
    title: 'Product Types',
    endpoint: 'ProductType',
    entityName: 'Product type',
    searchKey: 'Name',
    searchLabel: 'Search by name',
    filters: const [activeFilter],
    columns: nameDescActiveColumns(),
    fields: nameDescActiveFields,
  ),
  AdminModule(
    title: 'Units of Measure',
    endpoint: 'UnitOfMeasure',
    entityName: 'unit of measure',
    searchKey: 'Name',
    searchLabel: 'Search by name',
    filters: const [activeFilter],
    columns: nameDescActiveColumns(),
    fields: nameDescActiveFields,
  ),
  AdminModule(
    title: 'Products',
    endpoint: 'Product',
    entityName: 'product',
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
      AdminColumn(label: 'Name', value: (x) => textValue(x['name'])),
      AdminColumn(
        label: 'Category',
        value: (x) => textValue(x['productCategoryName']),
      ),
      AdminColumn(label: 'Type', value: (x) => textValue(x['productTypeName'])),
      AdminColumn(
        label: 'Unit',
        value: (x) => textValue(x['unitOfMeasureName']),
      ),
      AdminColumn(
        label: 'Supplier',
        value: (x) => textValue(x['supplierName']),
      ),
      AdminColumn(label: 'Price', value: (x) => moneyValue(x['price'])),
      AdminColumn(label: 'Stock', value: (x) => textValue(x['stockQuantity'])),
      AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
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
  ),
  AdminModule(
    title: 'Service Categories',
    endpoint: 'ServiceCategory',
    entityName: 'service category',
    searchKey: 'Name',
    searchLabel: 'Search by name',
    filters: const [activeFilter],
    columns: nameDescActiveColumns(),
    fields: nameDescActiveFields,
  ),
  AdminModule(
    title: 'Services',
    endpoint: 'Service',
    entityName: 'service',
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
      AdminColumn(label: 'Name', value: (x) => textValue(x['name'])),
      AdminColumn(
        label: 'Category',
        value: (x) => textValue(x['serviceCategoryName']),
      ),
      AdminColumn(
        label: 'Duration',
        value: (x) => '${textValue(x['durationMinutes'])} min',
      ),
      AdminColumn(label: 'Price', value: (x) => moneyValue(x['price'])),
      AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
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
  ),
  AdminModule(
    title: 'FAQ Categories',
    endpoint: 'FAQCategory',
    entityName: 'FAQ category',
    searchKey: 'Name',
    searchLabel: 'Search by name',
    filters: const [activeFilter],
    columns: nameDescActiveColumns(),
    fields: nameDescActiveFields,
  ),
  AdminModule(
    title: 'FAQ',
    endpoint: 'FAQ',
    entityName: 'FAQ',
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
      AdminColumn(label: 'Question', value: (x) => textValue(x['question'])),
      AdminColumn(
        label: 'Category',
        value: (x) => textValue(x['faqCategoryName']),
      ),
      AdminColumn(
        label: 'Display order',
        value: (x) => textValue(x['displayOrder']),
      ),
      AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
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
  ),
  AdminModule(
    title: 'Users',
    endpoint: 'User',
    entityName: 'user',
    searchKey: 'Username',
    searchLabel: 'Search by username',
    filters: const [activeFilter],
    columns: [
      AdminColumn(label: 'Name', value: (x) => displayName(x)),
      AdminColumn(label: 'Username', value: (x) => textValue(x['username'])),
      AdminColumn(label: 'Email', value: (x) => textValue(x['email'])),
      AdminColumn(label: 'Phone', value: (x) => textValue(x['phoneNumber'])),
      AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
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
  ),
  AdminModule(
    title: 'User Roles',
    endpoint: 'UserRole',
    entityName: 'user role',
    searchKey: 'Username',
    searchLabel: 'Search by username',
    filters: [FilterField(key: 'RoleId', label: 'Role', lookup: rolesLookup)],
    columns: [
      AdminColumn(
        label: 'Username',
        value: (x) => textValue(x['userName'] ?? x['username']),
      ),
      AdminColumn(label: 'Role', value: (x) => textValue(x['roleName'])),
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
  ),
  AdminModule(
    title: 'Employees',
    endpoint: 'Employee',
    entityName: 'employee',
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
      AdminColumn(
        label: 'Name',
        value: (x) =>
            textValue(x['employeeName'] ?? x['userName'] ?? x['username']),
      ),
      AdminColumn(
        label: 'Specialization',
        value: (x) => textValue(x['specialization']),
      ),
      AdminColumn(label: 'Bio', value: (x) => textValue(x['bio'])),
      AdminColumn(label: 'Hire date', value: (x) => dateValue(x['hireDate'])),
      AdminColumn(
        label: 'Available',
        value: (x) => textValue(x['isAvailable']),
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
  ),
  AdminModule(
    title: 'Service Assignments',
    endpoint: 'EmployeeService',
    entityName: 'service assignment',
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
      AdminColumn(
        label: 'Employee',
        value: (x) => textValue(x['employeeName']),
      ),
      AdminColumn(label: 'Service', value: (x) => textValue(x['serviceName'])),
      AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
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
  ),
  AdminModule(
    title: 'Appointments',
    endpoint: 'Appointment',
    entityName: 'appointment',
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
      AdminColumn(label: 'Client', value: (x) => textValue(x['userName'])),
      AdminColumn(
        label: 'Employee',
        value: (x) => textValue(x['employeeName']),
      ),
      AdminColumn(label: 'Service', value: (x) => textValue(x['serviceName'])),
      AdminColumn(
        label: 'Category',
        value: (x) => textValue(x['serviceCategoryName']),
      ),
      AdminColumn(label: 'Date', value: (x) => dateValue(x['appointmentDate'])),
      AdminColumn(label: 'Start', value: (x) => timeValue(x['startTime'])),
      AdminColumn(label: 'End', value: (x) => timeValue(x['endTime'])),
      AdminColumn(
        label: 'Status',
        value: (x) => enumLabel(_labels(appointmentStatuses), x['status']),
      ),
      AdminColumn(label: 'Notes', value: (x) => textValue(x['notes'])),
    ],
    fields: [
      AdminField(
        key: 'userId',
        label: 'Client',
        type: AdminFieldType.lookup,
        required: true,
        lookup: appointmentClientsLookup,
      ),
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
        key: 'appointmentDate',
        label: 'Appointment date',
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
  ),
  AdminModule(
    title: 'Reviews',
    endpoint: 'Review',
    entityName: 'review',
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
      AdminColumn(label: 'User', value: (x) => textValue(x['userName'])),
      AdminColumn(
        label: 'Target',
        value: (x) => textValue(
          x['productName'] ?? x['serviceName'] ?? x['appointmentDisplay'],
        ),
      ),
      AdminColumn(label: 'Rating', value: (x) => textValue(x['rating'])),
      AdminColumn(label: 'Comment', value: (x) => textValue(x['comment'])),
      AdminColumn(
        label: 'Status',
        value: (x) => enumLabel(_labels(reviewStatuses), x['status']),
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
  ),
  AdminModule(
    title: 'Purchases',
    endpoint: 'Purchase',
    entityName: 'purchase',
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
      AdminColumn(label: 'Client', value: (x) => textValue(x['userName'])),
      AdminColumn(
        label: 'Purchase number',
        value: (x) => textValue(x['purchaseNumber']),
      ),
      AdminColumn(label: 'Total', value: (x) => moneyValue(x['totalAmount'])),
      AdminColumn(
        label: 'Purchase status',
        value: (x) => enumLabel(_labels(purchaseStatuses), x['status']),
      ),
      AdminColumn(
        label: 'Payment status',
        value: (x) => enumLabel(_labels(paymentStatuses), x['paymentStatus']),
      ),
      AdminColumn(label: 'Paid at', value: (x) => dateTimeValue(x['paidAt'])),
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
  ),
  AdminModule(
    title: 'Purchase Items',
    endpoint: 'PurchaseItem',
    entityName: 'purchase item',
    filters: [
      FilterField(
        key: 'PurchaseId',
        label: 'Purchase',
        lookup: purchasesLookup,
      ),
      FilterField(key: 'ProductId', label: 'Product', lookup: productsLookup),
    ],
    columns: [
      AdminColumn(
        label: 'Purchase',
        value: (x) => textValue(x['purchaseNumber']),
      ),
      AdminColumn(label: 'Product', value: (x) => textValue(x['productName'])),
      AdminColumn(label: 'Quantity', value: (x) => textValue(x['quantity'])),
      AdminColumn(
        label: 'Unit price',
        value: (x) => moneyValue(x['unitPrice']),
      ),
      AdminColumn(label: 'Total', value: (x) => moneyValue(x['totalPrice'])),
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
  ),
  AdminModule(
    title: 'Suppliers',
    endpoint: 'Supplier',
    entityName: 'supplier',
    searchKey: 'Name',
    searchLabel: 'Search by supplier name',
    filters: const [activeFilter],
    columns: [
      AdminColumn(label: 'Name', value: (x) => textValue(x['name'])),
      AdminColumn(label: 'Email', value: (x) => textValue(x['contactEmail'])),
      AdminColumn(label: 'Phone', value: (x) => textValue(x['phoneNumber'])),
      AdminColumn(label: 'Address', value: (x) => textValue(x['address'])),
      AdminColumn(label: 'Active', value: (x) => textValue(x['isActive'])),
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
  ),
];
