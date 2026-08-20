class BusinessAnalytics {
  const BusinessAnalytics({
    required this.dateFrom,
    required this.dateTo,
    required this.totalRevenue,
    required this.completedPurchases,
    required this.completedAppointments,
    required this.uniqueClients,
    required this.totalUsers,
    required this.totalEmployees,
    required this.totalAppointments,
    required this.totalServices,
    required this.totalProducts,
    required this.totalPurchases,
    required this.bestSellingProducts,
    required this.serviceUsage,
    required this.weeklyAttendance,
    required this.appointmentStatuses,
    required this.employeeWorkload,
    required this.clientActivity,
  });

  final DateTime? dateFrom;
  final DateTime? dateTo;
  final double totalRevenue;
  final int completedPurchases;
  final int completedAppointments;
  final int uniqueClients;
  final int totalUsers;
  final int totalEmployees;
  final int totalAppointments;
  final int totalServices;
  final int totalProducts;
  final int totalPurchases;
  final List<ProductAnalytics> bestSellingProducts;
  final List<ServiceAnalytics> serviceUsage;
  final List<WeeklyAttendance> weeklyAttendance;
  final List<NamedCount> appointmentStatuses;
  final List<NamedCount> employeeWorkload;
  final List<NamedCount> clientActivity;

  factory BusinessAnalytics.fromJson(Map<String, dynamic> json) {
    return BusinessAnalytics(
      dateFrom: DateTime.tryParse(json['dateFrom']?.toString() ?? ''),
      dateTo: DateTime.tryParse(json['dateTo']?.toString() ?? ''),
      totalRevenue: _double(json['totalRevenue']),
      completedPurchases: _integer(json['completedPurchases']),
      completedAppointments: _integer(json['completedAppointments']),
      uniqueClients: _integer(json['uniqueClients']),
      totalUsers: _integer(json['totalUsers']),
      totalEmployees: _integer(json['totalEmployees']),
      totalAppointments: _integer(json['totalAppointments']),
      totalServices: _integer(json['totalServices']),
      totalProducts: _integer(json['totalProducts']),
      totalPurchases: _integer(json['totalPurchases']),
      bestSellingProducts: _list(
        json['bestSellingProducts'],
        ProductAnalytics.fromJson,
      ),
      serviceUsage: _list(json['serviceUsage'], ServiceAnalytics.fromJson),
      weeklyAttendance: _list(
        json['weeklyAttendance'],
        WeeklyAttendance.fromJson,
      ),
      appointmentStatuses: _list(
        json['appointmentStatuses'],
        NamedCount.fromStatusJson,
      ),
      employeeWorkload: _list(json['employeeWorkload'], NamedCount.fromJson),
      clientActivity: _list(json['clientActivity'], NamedCount.fromJson),
    );
  }
}

class ProductAnalytics {
  const ProductAnalytics({
    required this.productId,
    required this.productName,
    required this.quantitySold,
    required this.revenue,
  });

  final int productId;
  final String productName;
  final int quantitySold;
  final double revenue;

  factory ProductAnalytics.fromJson(Map<String, dynamic> json) =>
      ProductAnalytics(
        productId: _integer(json['productId']),
        productName: json['productName']?.toString() ?? 'Unknown product',
        quantitySold: _integer(json['quantitySold']),
        revenue: _double(json['revenue']),
      );
}

class ServiceAnalytics {
  const ServiceAnalytics({
    required this.serviceId,
    required this.serviceName,
    required this.appointmentCount,
  });

  final int serviceId;
  final String serviceName;
  final int appointmentCount;

  factory ServiceAnalytics.fromJson(Map<String, dynamic> json) =>
      ServiceAnalytics(
        serviceId: _integer(json['serviceId']),
        serviceName: json['serviceName']?.toString() ?? 'Unknown service',
        appointmentCount: _integer(json['appointmentCount']),
      );
}

class WeeklyAttendance {
  const WeeklyAttendance({
    required this.weekStart,
    required this.attendanceCount,
  });

  final DateTime weekStart;
  final int attendanceCount;

  factory WeeklyAttendance.fromJson(Map<String, dynamic> json) =>
      WeeklyAttendance(
        weekStart:
            DateTime.tryParse(json['weekStart']?.toString() ?? '') ??
            DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
        attendanceCount: _integer(json['attendanceCount']),
      );
}

class NamedCount {
  const NamedCount({required this.name, required this.count});

  final String name;
  final int count;

  factory NamedCount.fromJson(Map<String, dynamic> json) => NamedCount(
    name: json['name']?.toString() ?? 'Unknown',
    count: _integer(json['count']),
  );

  factory NamedCount.fromStatusJson(Map<String, dynamic> json) => NamedCount(
    name: json['status']?.toString() ?? 'Unknown',
    count: _integer(json['count']),
  );
}

List<T> _list<T>(dynamic value, T Function(Map<String, dynamic>) converter) {
  if (value is! List) return const [];
  return value
      .whereType<Map>()
      .map((item) => converter(Map<String, dynamic>.from(item)))
      .toList();
}

int _integer(dynamic value) =>
    value is num ? value.toInt() : int.tryParse(value?.toString() ?? '') ?? 0;

double _double(dynamic value) => value is num
    ? value.toDouble()
    : double.tryParse(value?.toString() ?? '') ?? 0;
