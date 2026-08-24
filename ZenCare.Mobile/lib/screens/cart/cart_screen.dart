import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/cart_item.dart';
import '../../providers/cart_provider.dart';
import '../../utils/api_exception.dart';
import 'purchase_details_screen.dart';
import 'purchase_history_screen.dart';

class CartScreen extends StatefulWidget {
  const CartScreen({super.key});

  @override
  State<CartScreen> createState() => _CartScreenState();
}

class _CartScreenState extends State<CartScreen> {
  bool _loaded = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (!_loaded) {
      _loaded = true;
      final cartProvider = context.read<CartProvider>();
      Future.microtask(cartProvider.loadCart);
    }
  }

  Future<void> _checkout(CartProvider provider) async {
    final confirmed =
        await _confirmCheckout(provider.totalItemCount, provider.totalPrice);
    if (!confirmed || !mounted) {
      return;
    }

    try {
      final purchase = await provider.checkout();
      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Purchase ${purchase.displayNumber} created.')),
      );
      await Navigator.of(context).push(
        MaterialPageRoute<void>(
          builder: (_) => PurchaseDetailsScreen(
              purchaseId: purchase.id, initialPurchase: purchase),
        ),
      );
    } on ApiException catch (error) {
      if (!mounted) {
        return;
      }
      _showError(error.message);
    }
  }

  void _showError(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message)),
    );
  }

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<CartProvider>();

    return Scaffold(
      body: RefreshIndicator(
        onRefresh: provider.refresh,
        child: CustomScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
                child: Row(
                  children: [
                    Expanded(
                      child: Text(
                        'Cart',
                        style:
                            Theme.of(context).textTheme.headlineSmall?.copyWith(
                                  fontWeight: FontWeight.w700,
                                ),
                      ),
                    ),
                    TextButton.icon(
                      onPressed: () {
                        Navigator.of(context).push(
                          MaterialPageRoute<void>(
                              builder: (_) => const PurchaseHistoryScreen()),
                        );
                      },
                      icon: const Icon(Icons.receipt_long_outlined),
                      label: const Text('History'),
                    ),
                  ],
                ),
              ),
            ),
            if (provider.isLoading)
              const SliverFillRemaining(
                hasScrollBody: false,
                child: Center(child: CircularProgressIndicator()),
              )
            else if (provider.error != null)
              SliverFillRemaining(
                hasScrollBody: false,
                child: _StateMessage(
                  icon: Icons.error_outline,
                  title: 'Cart could not be loaded',
                  message: provider.error!,
                  actionLabel: 'Retry',
                  onAction: provider.retry,
                ),
              )
            else if (provider.isEmpty)
              const SliverFillRemaining(
                hasScrollBody: false,
                child: _StateMessage(
                  icon: Icons.shopping_cart_outlined,
                  title: 'Your cart is empty',
                  message: 'Add products to your cart before checkout.',
                ),
              )
            else ...[
              SliverPadding(
                padding: const EdgeInsets.all(16),
                sliver: SliverList.separated(
                  itemCount: provider.items.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final item = provider.items[index];
                    return _CartItemTile(
                      item: item,
                      isMutating: provider.isMutating,
                      onIncrease: () async {
                        try {
                          await provider.increaseQuantity(item);
                        } on ApiException catch (error) {
                          if (!mounted) {
                            return;
                          }
                          _showError(error.message);
                        }
                      },
                      onDecrease: () async {
                        try {
                          await provider.decreaseQuantity(item);
                        } on ApiException catch (error) {
                          if (!mounted) {
                            return;
                          }
                          _showError(error.message);
                        }
                      },
                      onRemove: () async {
                        final confirmed =
                            await _confirmRemove(item.productName);
                        if (!confirmed || !context.mounted) {
                          return;
                        }
                        try {
                          await provider.removeItem(item);
                        } on ApiException catch (error) {
                          if (!mounted) {
                            return;
                          }
                          _showError(error.message);
                        }
                      },
                    );
                  },
                ),
              ),
              SliverToBoxAdapter(
                child: _CartSummary(
                  totalItems: provider.totalItemCount,
                  totalPrice: provider.totalPrice,
                  isCheckingOut: provider.isMutating,
                  onCheckout:
                      provider.isMutating ? null : () => _checkout(provider),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Future<bool> _confirmRemove(String productName) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Remove item'),
        content: Text('Remove $productName from your cart?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Remove'),
          ),
        ],
      ),
    );

    return result ?? false;
  }

  Future<bool> _confirmCheckout(int itemCount, double totalPrice) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Confirm checkout'),
        content: Text(
          'Create a purchase for $itemCount item${itemCount == 1 ? '' : 's'} '
          'with a total of \$${totalPrice.toStringAsFixed(2)}?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Confirm checkout'),
          ),
        ],
      ),
    );

    return result ?? false;
  }
}

class _CartItemTile extends StatelessWidget {
  const _CartItemTile({
    required this.item,
    required this.isMutating,
    required this.onIncrease,
    required this.onDecrease,
    required this.onRemove,
  });

  final CartItem item;
  final bool isMutating;
  final VoidCallback onIncrease;
  final VoidCallback onDecrease;
  final VoidCallback onRemove;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Dismissible(
      key: ValueKey(item.id),
      direction: DismissDirection.endToStart,
      confirmDismiss: (_) async {
        onRemove();
        return false;
      },
      background: Container(
        alignment: Alignment.centerRight,
        padding: const EdgeInsets.only(right: 20),
        color: theme.colorScheme.error,
        child: const Icon(Icons.delete_outline, color: Colors.white),
      ),
      child: Card(
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      item.productName,
                      style: theme.textTheme.titleMedium
                          ?.copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  IconButton(
                    tooltip: 'Remove',
                    onPressed: isMutating ? null : onRemove,
                    icon: const Icon(Icons.delete_outline),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Text('Unit price: \$${item.unitPrice.toStringAsFixed(2)}'),
              const SizedBox(height: 8),
              Row(
                children: [
                  IconButton.filledTonal(
                    tooltip: 'Decrease quantity',
                    onPressed: isMutating ? null : onDecrease,
                    icon: const Icon(Icons.remove),
                  ),
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    child: Text(
                      item.quantity.toString(),
                      style: theme.textTheme.titleMedium
                          ?.copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  IconButton.filledTonal(
                    tooltip: 'Increase quantity',
                    onPressed: isMutating ? null : onIncrease,
                    icon: const Icon(Icons.add),
                  ),
                  const Spacer(),
                  Text(
                    '\$${item.subtotal.toStringAsFixed(2)}',
                    style: theme.textTheme.titleMedium?.copyWith(
                      color: theme.colorScheme.primary,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _CartSummary extends StatelessWidget {
  const _CartSummary({
    required this.totalItems,
    required this.totalPrice,
    required this.isCheckingOut,
    required this.onCheckout,
  });

  final int totalItems;
  final double totalPrice;
  final bool isCheckingOut;
  final VoidCallback? onCheckout;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 0, 16, 24),
      child: Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            children: [
              Row(
                children: [
                  const Expanded(child: Text('Items')),
                  Text(totalItems.toString()),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Text('Total',
                      style: theme.textTheme.titleMedium
                          ?.copyWith(fontWeight: FontWeight.w700)),
                  const Spacer(),
                  Text(
                    '\$${totalPrice.toStringAsFixed(2)}',
                    style: theme.textTheme.titleMedium?.copyWith(
                      color: theme.colorScheme.primary,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              SizedBox(
                width: double.infinity,
                child: FilledButton.icon(
                  onPressed: onCheckout,
                  icon: isCheckingOut
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                      : const Icon(Icons.lock_outline),
                  label: const Text('Checkout'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _StateMessage extends StatelessWidget {
  const _StateMessage({
    required this.icon,
    required this.title,
    required this.message,
    this.actionLabel,
    this.onAction,
  });

  final IconData icon;
  final String title;
  final String message;
  final String? actionLabel;
  final VoidCallback? onAction;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(icon, size: 48, color: theme.colorScheme.primary),
          const SizedBox(height: 16),
          Text(
            title,
            style: theme.textTheme.titleLarge
                ?.copyWith(fontWeight: FontWeight.w700),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(message, textAlign: TextAlign.center),
          if (actionLabel != null && onAction != null) ...[
            const SizedBox(height: 16),
            FilledButton(
              onPressed: onAction,
              child: Text(actionLabel!),
            ),
          ],
        ],
      ),
    );
  }
}
