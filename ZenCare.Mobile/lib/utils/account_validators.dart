class AccountValidators {
  static const firstNameMaxLength = 50;
  static const lastNameMaxLength = 50;
  static const usernameMaxLength = 100;
  static const emailMaxLength = 100;
  static const phoneMaxLength = 10;
  static const passwordMinLength = 6;
  static const phoneMessage = 'Phone number must contain 9 or 10 digits (numbers only).';
  static const emailHelperText = 'Format: user@example.com';
  static const phoneHelperText = 'Format: 9 or 10 digits, numbers only.';
  static const passwordHelperText = 'Minimum 6 characters.';
  static const confirmPasswordHelperText = 'Must match the password.';

  static final _emailPattern = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$');
  static final _phonePattern = RegExp(r'^\d{9,10}$');

  static String? firstName(String? value) {
    return _requiredWithMaxLength(value, 'First name', firstNameMaxLength);
  }

  static String? lastName(String? value) {
    return _requiredWithMaxLength(value, 'Last name', lastNameMaxLength);
  }

  static String? username(String? value) {
    return _requiredWithMaxLength(value, 'Username', usernameMaxLength);
  }

  static String? email(String? value) {
    final requiredMessage = _requiredWithMaxLength(value, 'Email', emailMaxLength);
    if (requiredMessage != null) {
      return requiredMessage;
    }

    if (!_emailPattern.hasMatch(value!.trim())) {
      return 'Email must be in the format: user@example.com.';
    }

    return null;
  }

  static String? phoneNumber(String? value) {
    final phoneNumber = value?.trim() ?? '';
    if (phoneNumber.isEmpty) {
      return null;
    }

    if (!_phonePattern.hasMatch(phoneNumber)) {
      return phoneMessage;
    }

    return null;
  }

  static String? password(String? value) {
    if (value == null || value.isEmpty) {
      return 'Password is required.';
    }

    if (value.length < passwordMinLength) {
      return 'Password must contain at least 6 characters.';
    }

    return null;
  }

  static String? confirmPassword(String? value, String password) {
    if (value == null || value.isEmpty) {
      return 'Confirm password is required.';
    }

    if (value != password) {
      return 'Passwords do not match.';
    }

    return null;
  }

  static String? _requiredWithMaxLength(String? value, String fieldName, int maxLength) {
    final trimmedValue = value?.trim() ?? '';
    if (trimmedValue.isEmpty) {
      return '$fieldName is required.';
    }

    if (trimmedValue.length > maxLength) {
      return '$fieldName must not exceed $maxLength characters.';
    }

    return null;
  }
}
