import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';

import '../../models/register_request.dart';
import '../../providers/auth_provider.dart';
import '../../utils/account_validators.dart';
import '../../utils/api_exception.dart';
import '../../widgets/error_dialog.dart';
import '../../widgets/loading_button.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _usernameController = TextEditingController();
  final _emailController = TextEditingController();
  final _phoneController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  String? _usernameServerError;
  String? _emailServerError;
  String? _phoneServerError;
  bool _obscurePassword = true;
  bool _obscureConfirmPassword = true;

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _usernameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final authProvider = context.read<AuthProvider>();

    try {
      await authProvider.register(
        RegisterRequest(
          firstName: _firstNameController.text.trim(),
          lastName: _lastNameController.text.trim(),
          username: _usernameController.text.trim(),
          email: _emailController.text.trim(),
          phoneNumber: _emptyToNull(_phoneController),
          password: _passwordController.text,
          passwordConfirm: _confirmPasswordController.text,
        ),
      );

      if (!mounted) {
        return;
      }

      await showDialog<void>(
        context: context,
        builder: (context) {
          return AlertDialog(
            title: const Text('Registration successful'),
            content: const Text('Account created successfully. Please sign in.'),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: const Text('OK'),
              ),
            ],
          );
        },
      );

      if (!mounted) {
        return;
      }

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
      await showErrorDialog(context, 'Unable to connect to ZenCare API.');
    }
  }

  String? _emptyToNull(TextEditingController controller) {
    final value = controller.text.trim();
    return value.isEmpty ? null : value;
  }

  @override
  Widget build(BuildContext context) {
    final isRegistering = context.watch<AuthProvider>().isRegistering;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Create account'),
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
                    Text(
                      'Join ZenCare',
                      style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                            fontWeight: FontWeight.w700,
                          ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Create your client account',
                      style: Theme.of(context).textTheme.bodyLarge,
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 32),
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
                      textInputAction: TextInputAction.next,
                      decoration: const InputDecoration(
                        labelText: 'Username',
                        prefixIcon: Icon(Icons.person_outline),
                      ),
                      onChanged: (_) => _clearUsernameServerError(),
                      validator: (value) => _usernameServerError ?? AccountValidators.username(value),
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
                      textInputAction: TextInputAction.next,
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
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _passwordController,
                      obscureText: _obscurePassword,
                      textInputAction: TextInputAction.next,
                      decoration: InputDecoration(
                        labelText: 'Password',
                        prefixIcon: const Icon(Icons.lock_outline),
                        helperText: AccountValidators.passwordHelperText,
                        helperMaxLines: 2,
                        errorMaxLines: 2,
                        suffixIcon: IconButton(
                          onPressed: () {
                            setState(() {
                              _obscurePassword = !_obscurePassword;
                            });
                          },
                          icon: Icon(
                            _obscurePassword ? Icons.visibility_outlined : Icons.visibility_off_outlined,
                          ),
                        ),
                      ),
                      validator: AccountValidators.password,
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _confirmPasswordController,
                      obscureText: _obscureConfirmPassword,
                      onFieldSubmitted: (_) => _submit(),
                      decoration: InputDecoration(
                        labelText: 'Confirm password',
                        prefixIcon: const Icon(Icons.lock_outline),
                        helperText: AccountValidators.confirmPasswordHelperText,
                        helperMaxLines: 2,
                        errorMaxLines: 2,
                        suffixIcon: IconButton(
                          onPressed: () {
                            setState(() {
                              _obscureConfirmPassword = !_obscureConfirmPassword;
                            });
                          },
                          icon: Icon(
                            _obscureConfirmPassword ? Icons.visibility_outlined : Icons.visibility_off_outlined,
                          ),
                        ),
                      ),
                      validator: (value) => AccountValidators.confirmPassword(value, _passwordController.text),
                    ),
                    const SizedBox(height: 24),
                    LoadingButton(
                      isLoading: isRegistering,
                      onPressed: _submit,
                      child: const Text('Create account'),
                    ),
                    const SizedBox(height: 12),
                    TextButton(
                      onPressed: isRegistering ? null : () => Navigator.of(context).pop(),
                      child: const Text('Already have an account? Sign in'),
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
    if (message.toLowerCase().contains('username')) {
      setState(() {
        _usernameServerError = message;
      });
      _formKey.currentState?.validate();
      return true;
    }

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

  void _clearUsernameServerError() {
    if (_usernameServerError != null) {
      setState(() {
        _usernameServerError = null;
      });
    }
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
