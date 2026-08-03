import 'package:flutter/material.dart';
import 'package:flutter_stripe/flutter_stripe.dart' hide Card;
import 'package:provider/provider.dart';

import '../../models/purchase.dart';
import '../../models/purchase_item.dart';
import '../../services/payment_service.dart';
import '../../services/purchase_service.dart';
import '../../utils/api_exception.dart';

class PurchaseDetailsScreen extends StatefulWidget {
  const PurchaseDetailsScreen({
    super.key,
    required this.purchaseId,
    this.initialPurchase,
  });

  final int purchaseId;
  final Purchase? initialPurchase;

  @override
  State<PurchaseDetailsScreen> createState() => _PurchaseDetailsScreenState();
}

class _PurchaseDetailsScreenState extends State<PurchaseDetailsScreen> {
  late Future<Purchase> _purchaseFuture;
  bool _isPaying = false;
  bool _isRefunding = false;

  @override
  void initState() {
    super.initState();
    _purchaseFuture = _loadPurchase();
  }

  Future<Purchase> _loadPurchase() {
    return context.read<PurchaseService>().getMyPurchaseById(widget.purchaseId);
  }

  void _retry() {
    setState(() {
      _purchaseFuture = _loadPurchase();
    });
  }

  void _reloadPurchase() {
    setState(() {
      _purchaseFuture = _loadPurchase();
    });
  }

  Future<void> _pay(Purchase purchase) async {
    final paymentService = context.read<PaymentService>();
    final messenger = ScaffoldMessenger.of(context);

    setState(() {
      _isPaying = true;
    });

    try {
      final intent = await paymentService.createPaymentIntent(purchase.id);

      await Stripe.instance.initPaymentSheet(
        paymentSheetParameters: SetupPaymentSheetParameters(
          paymentIntentClientSecret: intent.clientSecret,
          merchantDisplayName: 'ZenCare',
        ),
      );

      await Stripe.instance.presentPaymentSheet();
      await paymentService.confirmPayment(purchase.id);

      if (!mounted) {
        return;
      }

      messenger.showSnackBar(
        const SnackBar(content: Text('Payment completed successfully.')),
      );
      _reloadPurchase();
    } on StripeException catch (error) {
      if (!mounted) {
        return;
      }

      final message = error.error.code == FailureCode.Canceled
          ? 'Payment was cancelled.'
          : 'Payment could not be completed.';
      messenger.showSnackBar(SnackBar(content: Text(message)));
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }

      messenger.showSnackBar(SnackBar(content: Text(error.message)));
    } catch (_) {
      if (!mounted) {
        return;
      }

      messenger.showSnackBar(
        const SnackBar(content: Text('Payment could not be completed.')),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isPaying = false;
        });
      }
    }
  }

  Future<void> _refund(Purchase purchase) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Refund payment'),
        content: const Text('Do you want to refund this payment?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Refund'),
          ),
        ],
      ),
    );

    if (!mounted || confirmed != true) {
      return;
    }

    final paymentService = context.read<PaymentService>();
    final messenger = ScaffoldMessenger.of(context);

    setState(() {
      _isRefunding = true;
    });

    try {
      await paymentService.refundPayment(purchase.id);

      if (!mounted) {
        return;
      }

      messenger.showSnackBar(
        const SnackBar(content: Text('Payment refunded successfully.')),
      );
      _reloadPurchase();
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }

      messenger.showSnackBar(SnackBar(content: Text(error.message)));
    } catch (_) {
      if (!mounted) {
        return;
      }

      messenger.showSnackBar(
        const SnackBar(content: Text('Refund could not be completed.')),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isRefunding = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Purchase details')),
      body: FutureBuilder<Purchase>(
        future: _purchaseFuture,
        initialData: widget.initialPurchase,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting && snapshot.data == null) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError && snapshot.data == null) {
            final message = snapshot.error is ApiException
                ? (snapshot.error! as ApiException).message
                : 'Unable to load purchase details.';

            return _DetailsError(message: message, onRetry: _retry);
          }

          final purchase = snapshot.data ?? widget.initialPurchase!;
          return _PurchaseDetailsContent(
            purchase: purchase,
            isPaying: _isPaying,
            isRefunding: _isRefunding,
            onPay: _isPaying || _isRefunding ? null : () => _pay(purchase),
            onRefund: _isPaying || _isRefunding ? null : () => _refund(purchase),
          );
        },
      ),
    );
  }
}

class _PurchaseDetailsContent extends StatelessWidget {
  const _PurchaseDetailsContent({
    required this.purchase,
    required this.isPaying,
    required this.isRefunding,
    this.onPay,
    this.onRefund,
  });

  final Purchase purchase;
  final bool isPaying;
  final bool isRefunding;
  final VoidCallback? onPay;
  final VoidCallback? onRefund;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Text(
          purchase.displayNumber,
          style: theme.textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
        ),
        const SizedBox(height: 8),
        Text('Created: ${_formatDate(purchase.createdAt)}'),
        const SizedBox(height: 16),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                _DetailsRow(label: 'Purchase status', value: purchase.statusText),
                _DetailsRow(label: 'Payment status', value: purchase.paymentStatusText),
                _DetailsRow(label: 'Paid at', value: purchase.paidAt == null ? '-' : _formatDate(purchase.paidAt!)),
                _DetailsRow(label: 'Total', value: '\$${purchase.totalAmount.toStringAsFixed(2)}'),
              ],
            ),
          ),
        ),
        if (purchase.canPay) ...[
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: FilledButton.icon(
              onPressed: onPay,
              icon: isPaying
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.payment),
              label: const Text('Pay now'),
            ),
          ),
        ] else if (purchase.canRefund) ...[
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: onRefund,
              icon: isRefunding
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.undo),
              label: const Text('Refund payment'),
            ),
          ),
        ],
        const SizedBox(height: 16),
        Text(
          'Items',
          style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
        ),
        const SizedBox(height: 8),
        if (purchase.purchaseItems.isEmpty)
          const Card(
            child: Padding(
              padding: EdgeInsets.all(16),
              child: Text('No purchase items were returned for this purchase.'),
            ),
          )
        else
          ...purchase.purchaseItems.map((item) => _PurchaseItemCard(item: item)),
      ],
    );
  }
}

class _PurchaseItemCard extends StatelessWidget {
  const _PurchaseItemCard({required this.item});

  final PurchaseItem item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              item.productName,
              style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 8),
            Text('Quantity: ${item.quantity}'),
            Text('Unit price: \$${item.unitPrice.toStringAsFixed(2)}'),
            const SizedBox(height: 8),
            Text(
              'Subtotal: \$${item.totalPrice.toStringAsFixed(2)}',
              style: theme.textTheme.titleMedium?.copyWith(
                color: theme.colorScheme.primary,
                fontWeight: FontWeight.w700,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _DetailsRow extends StatelessWidget {
  const _DetailsRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 130,
            child: Text(label, style: const TextStyle(fontWeight: FontWeight.w700)),
          ),
          Expanded(child: Text(value.isEmpty ? '-' : value)),
        ],
      ),
    );
  }
}

class _DetailsError extends StatelessWidget {
  const _DetailsError({required this.message, required this.onRetry});

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.error_outline, size: 48, color: theme.colorScheme.primary),
            const SizedBox(height: 16),
            Text(
              'Purchase details could not be loaded',
              style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 8),
            Text(message, textAlign: TextAlign.center),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: onRetry,
              child: const Text('Retry'),
            ),
          ],
        ),
      ),
    );
  }
}

String _formatDate(DateTime value) {
  return '${value.year.toString().padLeft(4, '0')}-${value.month.toString().padLeft(2, '0')}-${value.day.toString().padLeft(2, '0')}';
}
