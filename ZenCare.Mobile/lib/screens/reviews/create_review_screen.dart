import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/review_create_request.dart';
import '../../services/review_service.dart';
import '../../utils/api_exception.dart';

class CreateReviewScreen extends StatefulWidget {
  const CreateReviewScreen({
    super.key,
    this.appointmentId,
    this.productId,
    required this.targetName,
  }) : assert(appointmentId != null || productId != null);

  final int? appointmentId;
  final int? productId;
  final String targetName;

  @override
  State<CreateReviewScreen> createState() => _CreateReviewScreenState();
}

class _CreateReviewScreenState extends State<CreateReviewScreen> {
  final _formKey = GlobalKey<FormState>();
  final _commentController = TextEditingController();
  int _rating = 5;
  bool _isSubmitting = false;

  @override
  void dispose() {
    _commentController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isSubmitting = true;
    });

    try {
      await context.read<ReviewService>().createMyReview(
            ReviewCreateRequest(
              appointmentId: widget.appointmentId,
              productId: widget.productId,
              rating: _rating,
              comment: _commentController.text.trim().isEmpty ? null : _commentController.text.trim(),
            ),
          );

      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Review submitted.')),
      );
      Navigator.of(context).pop(true);
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(error.message)),
      );
    } catch (_) {
      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Review could not be submitted.')),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isSubmitting = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Create review')),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(20),
          children: [
            Text(
              widget.targetName,
              style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 8),
            Text(
              widget.productId == null ? 'Service appointment review' : 'Product review',
              style: theme.textTheme.bodyMedium,
            ),
            const SizedBox(height: 24),
            Text('Rating', style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 8),
            SegmentedButton<int>(
              segments: const [
                ButtonSegment(value: 1, label: Text('1')),
                ButtonSegment(value: 2, label: Text('2')),
                ButtonSegment(value: 3, label: Text('3')),
                ButtonSegment(value: 4, label: Text('4')),
                ButtonSegment(value: 5, label: Text('5')),
              ],
              selected: {_rating},
              onSelectionChanged: _isSubmitting
                  ? null
                  : (values) {
                      setState(() {
                        _rating = values.first;
                      });
                    },
            ),
            const SizedBox(height: 20),
            TextFormField(
              controller: _commentController,
              enabled: !_isSubmitting,
              maxLines: 5,
              maxLength: 1000,
              decoration: const InputDecoration(
                labelText: 'Comment',
                alignLabelWithHint: true,
              ),
              validator: (value) {
                if ((value ?? '').length > 1000) {
                  return 'Comment cannot be longer than 1000 characters.';
                }

                return null;
              },
            ),
            const SizedBox(height: 20),
            SizedBox(
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: _isSubmitting ? null : _submit,
                icon: _isSubmitting
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.rate_review_outlined),
                label: const Text('Submit review'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}