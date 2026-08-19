class ResetPasswordRequest {
  ResetPasswordRequest({
    required this.token,
    required this.newPassword,
    required this.confirmNewPassword,
  });

  final String token;
  final String newPassword;
  final String confirmNewPassword;

  Map<String, dynamic> toJson() {
    return {
      'token': token,
      'newPassword': newPassword,
      'confirmNewPassword': confirmNewPassword,
    };
  }
}
