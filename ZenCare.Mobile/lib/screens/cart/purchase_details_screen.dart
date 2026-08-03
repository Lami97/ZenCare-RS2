import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/purchase.dart';
import '../../models/purchase_item.dart';
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
          return _PurchaseDetailsContent(purchase: purchase);
        },
      ),
    );
  }
}

class _PurchaseDetailsContent extends StatelessWidget {
  const _PurchaseDetailsContent({required this.purchase});

  final Purchase purchase;

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