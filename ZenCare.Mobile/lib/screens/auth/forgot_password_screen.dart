import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../utils/account_validators.dart';
import '../../utils/api_exception.dart';
import '../../widgets/error_dialog.dart';
import '../../widgets/loading_button.dart';
import 'reset_password_screen.dart';

class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  String? _emailServerError;

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final authProvider = context.read<AuthProvider>();

    try {
      final message = await authProvider.forgotPassword(
        _emailController.text.trim(),
      );

      if (!mounted) {
        return;
      }

      await showDialog<void>(
        context: context,
        builder: (context) => AlertDialog(
          title: const Text('Check your email'),
          content: Text(message),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Continue'),
            ),
          ],
        ),
      );

      if (!mounted) {
        return;
      }

      await Navigator.of(context).push(
        MaterialPageRoute<void>(
          builder: (_) => const ResetPasswordScreen(),
        ),
      );
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }

      if (error.message.toLowerCase().contains('email')) {
        setState(() {
          _emailServerError = error.message;
        });
        _formKey.currentState?.validate();
        return;
      }

      await showErrorDialog(context, error.message);
    } catch (_) {
      if (!mounted) {
        return;
      }

      await showErrorDialog(
          context, 'Unable to request a password reset. Please try again.');
    }
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = context.watch<AuthProvider>().isResettingPassword;

    return Scaffold(
      appBar: AppBar(title: const Text('Forgot password')),
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
                      'Reset your password',
                      style:
                          Theme.of(context).textTheme.headlineSmall?.copyWith(
                                fontWeight: FontWeight.w700,
                              ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Enter your account email to receive a single-use reset token.',
                      style: Theme.of(context).textTheme.bodyLarge,
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 32),
                    TextFormField(
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      textInputAction: TextInputAction.done,
                      onFieldSubmitted: (_) => _submit(),
                      decoration: const InputDecoration(
                        labelText: 'Email',
                        prefixIcon: Icon(Icons.email_outlined),
                        helperText: AccountValidators.emailHelperText,
                        helperMaxLines: 2,
                        errorMaxLines: 2,
                      ),
                      onChanged: (_) {
                        if (_emailServerError != null) {
                          setState(() {
                            _emailServerError = null;
                          });
                        }
                      },
                      validator: (value) =>
                          _emailServerError ?? AccountValidators.email(value),
                    ),
                    const SizedBox(height: 24),
                    LoadingButton(
                      isLoading: isLoading,
                      onPressed: _submit,
                      child: const Text('Send reset token'),
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
}
