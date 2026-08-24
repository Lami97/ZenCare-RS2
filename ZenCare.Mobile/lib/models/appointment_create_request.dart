class AppointmentCreateRequest {
  AppointmentCreateRequest({required this.timeSlotId, this.notes});

  final int timeSlotId;
  final String? notes;

  Map<String, dynamic> toJson() => {
        'timeSlotId': timeSlotId,
        'notes': notes,
      };
}
