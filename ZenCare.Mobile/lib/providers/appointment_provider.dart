import 'package:flutter/foundation.dart';

import '../models/appointment.dart';
import '../services/appointment_service.dart';
import '../utils/api_exception.dart';

class AppointmentProvider extends ChangeNotifier {
  AppointmentProvider(this._appointmentService);

  final AppointmentService _appointmentService;

  final List<Appointment> _appointments = [];
  bool _isLoading = false;
  String? _error;
  int? _totalCount;

  List<Appointment> get appointments => List.unmodifiable(_appointments);
  bool get isLoading => _isLoading;
  String? get error => _error;
  int get totalCount => _totalCount ?? _appointments.length;
  bool get isEmpty => !_isLoading && _error == null && _appointments.isEmpty;

  Future<void> loadAppointments() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final result = await _appointmentService.getMyAppointments();
      _appointments
        ..clear()
        ..addAll(result.items);
      _appointments.sort((a, b) {
        final dateComparison = b.appointmentDate.compareTo(a.appointmentDate);
        if (dateComparison != 0) {
          return dateComparison;
        }
        return b.startTime.compareTo(a.startTime);
      });
      _totalCount = result.totalCount ?? _appointments.length;
      _error = null;
    } on ApiException catch (error) {
      _error = error.message;
    } catch (_) {
      _error = 'Unable to load appointments.';
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> refresh() async {
    await loadAppointments();
  }

  Future<void> retry() async {
    await loadAppointments();
  }
}
