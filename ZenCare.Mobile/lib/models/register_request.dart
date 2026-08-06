class RegisterRequest {
  RegisterRequest({
    required this.firstName,
    required this.lastName,
    required this.username,
    required this.email,
    this.phoneNumber,
    required this.password,
    required this.passwordConfirm,
  });

  final String firstName;
  final String lastName;
  final String username;
  final String email;
  final String? phoneNumber;
  final String password;
  final String passwordConfirm;

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'username': username,
      'email': email,
      'phoneNumber': phoneNumber,
      'password': password,
      'passwordConfirm': passwordConfirm,
    };
  }
}
