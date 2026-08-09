import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/change_password_request.dart';
import '../../providers/auth_provider.dart';
import '../../utils/account_validators.dart';
import '../../utils/api_exception.dart';
import '../../widgets/error_dialog.dart';
import '../../widgets/loading_button.dart';

class ChangePasswordScreen extends StatefulWidget {
  const ChangePasswordScreen({super.key});

  @override
  State<ChangePasswordScreen> createState() => _ChangePasswordScreenState();
}

class _ChangePasswordScreenState extends State<ChangePasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  final _currentPasswordController = TextEditingController();
  final _newPasswordController = TextEditingController();
  final _confirmNewPasswordController = TextEditingController();
  String? _currentPasswordServerError;
  String? _newPasswordServerError;
  String? _confirmPasswordServerError;
  bool _obscureCurrentPassword = true;
  bool _obscureNewPassword = true;
  bool _obscureConfirmNewPassword = true;

  @override
  void dispose() {
    _currentPasswordController.dispose();
    _newPasswordController.dispose();
    _confirmNewPasswordController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final authProvider = context.read<AuthProvider>();

    try {
      await authProvider.changePassword(
        ChangePasswordRequest(
          currentPassword: _currentPasswordController.text,
          newPassword: _newPasswordController.text,
          confirmNewPassword: _confirmNewPasswordController.text,
        ),
      );

      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Password changed successfully.')),
      );
      Navigator.of(context).pop();
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }

      if (_applyFieldError(error.message)) {
        return;
      }

      await showErrorDialog(context, error.message);
    } catch (_) {
      if (!mounted) {
        return;
      }

      await showErrorDialog(context, 'Unable to change password. Please try again.');
    }
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = context.watch<AuthProvider>().isLoading;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Change password'),
      ),
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(24),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 420),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    TextFormField(
                      controller: _currentPasswordController,
                      obscureText: _obscureCurrentPassword,
                      textInputAction: TextInputAction.next,
                      decoration: InputDecoration(
                        labelText: 'Current password',
                        prefixIcon: const Icon(Icons.lock_outline),
                        errorMaxLines: 2,
                        suffixIcon: IconButton(
                          onPressed: () {
                            setState(() {
                              _obscureCurrentPassword = !_obscureCurrentPassword;
                            });
                          },
                          icon: Icon(
                            _obscureCurrentPassword ? Icons.visibility_outlined : Icons.visibility_off_outlined,
                          ),
                        ),
                      ),
                      onChanged: (_) => _clearCurrentPasswordServerError(),
                      validator: (value) => _currentPasswordServerError ?? AccountValidators.currentPassword(value),
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _newPasswordController,
                      obscureText: _obscureNewPassword,
                      textInputAction: TextInputAction.next,
                      decoration: InputDecoration(
                        labelText: 'New password',
                        prefixIcon: const Icon(Icons.lock_outline),
                        helperText: AccountValidators.passwordHelperText,
                        helperMaxLines: 2,
                        errorMaxLines: 2,
                        suffixIcon: IconButton(
                          onPressed: () {
                            setState(() {
                              _obscureNewPassword = !_obscureNewPassword;
                            });
                          },
                          icon: Icon(
                            _obscureNewPassword ? Icons.visibility_outlined : Icons.visibility_off_outlined,
                          ),
                        ),
                      ),
                      onChanged: (_) => _clearNewPasswordServerError(),
                      validator: (value) => _newPasswordServerError ?? AccountValidators.newPassword(value),
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _confirmNewPasswordController,
                      obscureText: _obscureConfirmNewPassword,
                      onFieldSubmitted: (_) => _save(),
                      decoration: InputDecoration(
                        labelText: 'Confirm new password',
                        prefixIcon: const Icon(Icons.lock_outline),
                        helperText: AccountValidators.confirmNewPasswordHelperText,
                        helperMaxLines: 2,
                        errorMaxLines: 2,
                        suffixIcon: IconButton(
                          onPressed: () {
                            setState(() {
                              _obscureConfirmNewPassword = !_obscureConfirmNewPassword;
                            });
                          },
                          icon: Icon(
                            _obscureConfirmNewPassword
                                ? Icons.visibility_outlined
                                : Icons.visibility_off_outlined,
                          ),
                        ),
                      ),
                      onChanged: (_) => _clearConfirmPasswordServerError(),
                      validator: (value) => _confirmPasswordServerError ??
                          AccountValidators.confirmNewPassword(value, _newPasswordController.text),
                    ),
                    const SizedBox(height: 24),
                    LoadingButton(
                      isLoading: isLoading,
                      onPressed: _save,
                      child: const Text('Save'),
                    ),
                    const SizedBox(height: 12),
                    OutlinedButton(
                      onPressed: isLoading ? null : () => Navigator.of(context).pop(),
                      child: const Text('Cancel'),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  bool _applyFieldError(String message) {
    final normalizedMessage = message.toLowerCase();

    if (normalizedMessage.contains('current password')) {
      setState(() {
        _currentPasswordServerError = message;
      });
      _formKey.currentState?.validate();
      return true;
    }

    if (normalizedMessage.contains('new password')) {
      setState(() {
        _newPasswordServerError = message;
      });
      _formKey.currentState?.validate();
      return true;
    }

    if (normalizedMessage.contains('confirm') || normalizedMessage.contains('match')) {
      setState(() {
        _confirmPasswordServerError = message;
      });
      _formKey.currentState?.validate();
      return true;
    }

    return false;
  }

  void _clearCurrentPasswordServerError() {
    if (_currentPasswordServerError != null) {
      setState(() {
        _currentPasswordServerError = null;
      });
    }
  }

  void _clearNewPasswordServerError() {
    if (_newPasswordServerError != null) {
      setState(() {
        _newPasswordServerError = null;
      });
    }
  }

  void _clearConfirmPasswordServerError() {
    if (_confirmPasswordServerError != null) {
      setState(() {
        _confirmPasswordServerError = null;
      });
    }
  }
}
