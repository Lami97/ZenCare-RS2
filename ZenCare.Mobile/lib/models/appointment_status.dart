enum AppointmentStatus {
  pending(1, 'Pending'),
  confirmed(2, 'Confirmed'),
  paid(3, 'Paid'),
  completed(4, 'Completed'),
  cancelled(5, 'Cancelled'),
  noShow(6, 'No show');

  const AppointmentStatus(this.value, this.label);

  final int value;
  final String label;

  static AppointmentStatus fromValue(int value) {
    return AppointmentStatus.values.firstWhere(
      (status) => status.value == value,
      orElse: () => AppointmentStatus.pending,
    );
  }
}
