class UpdateProfileRequest {
  UpdateProfileRequest({
    required this.firstName,
    required this.lastName,
    required this.email,
    this.phoneNumber,
  });

  final String firstName;
  final String lastName;
  final String email;
  final String? phoneNumber;

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'email': email,
      'phoneNumber': phoneNumber,
    };
  }
}
