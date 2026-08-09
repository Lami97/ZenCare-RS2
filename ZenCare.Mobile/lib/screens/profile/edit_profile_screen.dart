import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';

import '../../models/update_profile_request.dart';
import '../../models/user.dart';
import '../../providers/auth_provider.dart';
import '../../utils/account_validators.dart';
import '../../utils/api_exception.dart';
import '../../widgets/error_dialog.dart';
import '../../widgets/loading_button.dart';

class EditProfileScreen extends StatefulWidget {
  const EditProfileScreen({super.key, required this.user});

  final User user;

  @override
  State<EditProfileScreen> createState() => _EditProfileScreenState();
}

class _EditProfileScreenState extends State<EditProfileScreen> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _firstNameController;
  late final TextEditingController _lastNameController;
  late final TextEditingController _usernameController;
  late final TextEditingController _emailController;
  late final TextEditingController _phoneController;
  String? _emailServerError;
  String? _phoneServerError;

  @override
  void initState() {
    super.initState();
    _firstNameController = TextEditingController(text: widget.user.firstName);
    _lastNameController = TextEditingController(text: widget.user.lastName);
    _usernameController = TextEditingController(text: widget.user.username);
    _emailController = TextEditingController(text: widget.user.email);
    _phoneController = TextEditingController(text: widget.user.phoneNumber ?? '');
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _usernameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final authProvider = context.read<AuthProvider>();

    try {
      await authProvider.updateProfile(
        UpdateProfileRequest(
          firstName: _firstNameController.text.trim(),
          lastName: _lastNameController.text.trim(),
          email: _emailController.text.trim(),
          phoneNumber: _emptyToNull(_phoneController.text),
        ),
      );

      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Profile updated successfully.')),
      );
      Navigator.of(context).pop(true);
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
      await showErrorDialog(context, 'Unable to update profile. Please try again.');
    }
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = context.watch<AuthProvider>().isLoading;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Edit profile'),
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
                      controller: _firstNameController,
                      textInputAction: TextInputAction.next,
                      decoration: const InputDecoration(
                        labelText: 'First name',
                        prefixIcon: Icon(Icons.badge_outlined),
                      ),
                      validator: AccountValidators.firstName,
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _lastNameController,
                      textInputAction: TextInputAction.next,
                      decoration: const InputDecoration(
                        labelText: 'Last name',
                        prefixIcon: Icon(Icons.badge_outlined),
                      ),
                      validator: AccountValidators.lastName,
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _usernameController,
                      enabled: false,
                      decoration: const InputDecoration(
                        labelText: 'Username',
                        prefixIcon: Icon(Icons.person_outline),
                      ),
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      textInputAction: TextInputAction.next,
                      decoration: const InputDecoration(
                        labelText: 'Email',
                        prefixIcon: Icon(Icons.email_outlined),
                        helperText: AccountValidators.emailHelperText,
                        helperMaxLines: 2,
                        errorMaxLines: 2,
                      ),
                      onChanged: (_) => _clearEmailServerError(),
                      validator: (value) => _emailServerError ?? AccountValidators.email(value),
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _phoneController,
                      keyboardType: TextInputType.number,
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                        LengthLimitingTextInputFormatter(AccountValidators.phoneMaxLength),
                      ],
                      maxLength: AccountValidators.phoneMaxLength,
                      onFieldSubmitted: (_) => _save(),
                      decoration: const InputDecoration(
                        labelText: 'Phone number',
                        prefixIcon: Icon(Icons.phone_outlined),
                        helperText: AccountValidators.phoneHelperText,
                        helperMaxLines: 2,
                        counterText: '',
                        errorMaxLines: 2,
                      ),
                      onChanged: (_) => _clearPhoneServerError(),
                      validator: (value) => _phoneServerError ?? AccountValidators.phoneNumber(value),
                    ),
                    const SizedBox(height: 24),
                    LoadingButton(
                      isLoading: isLoading,
                      onPressed: _save,
                      child: const Text('Save'),
                    ),
                    const SizedBox(height: 12),
                    OutlinedButton(
                      onPressed: isLoading ? null : () => Navigator.of(context).pop(false),
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
    if (message.toLowerCase().contains('email')) {
      setState(() {
        _emailServerError = message;
      });
      _formKey.currentState?.validate();
      return true;
    }

    if (message.toLowerCase().contains('phone number')) {
      setState(() {
        _phoneServerError = message;
      });
      _formKey.currentState?.validate();
      return true;
    }

    return false;
  }

  void _clearEmailServerError() {
    if (_emailServerError != null) {
      setState(() {
        _emailServerError = null;
      });
    }
  }

  void _clearPhoneServerError() {
    if (_phoneServerError != null) {
      setState(() {
        _phoneServerError = null;
      });
    }
  }
}

String? _emptyToNull(String value) {
  final trimmed = value.trim();
  return trimmed.isEmpty ? null : trimmed;
}
