import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/user_notification.dart';
import '../../providers/notification_provider.dart';
import '../../services/notification_service.dart';
import '../../utils/api_exception.dart';

class NotificationsScreen extends StatelessWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<NotificationProvider>(
      create: (context) => NotificationProvider(context.read<NotificationService>())..start(),
      child: const _NotificationsView(),
    );
  }
}

class _NotificationsView extends StatelessWidget {
  const _NotificationsView();

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<NotificationProvider>();

    return Scaffold(
      appBar: AppBar(title: const Text('Notifications')),
      body: RefreshIndicator(
        onRefresh: provider.refresh,
        child: CustomScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          slivers: [
            if (provider.isLoading)
              const SliverFillRemaining(
                hasScrollBody: false,
                child: Center(child: CircularProgressIndicator()),
              )
            else if (provider.error != null)
              SliverFillRemaining(
                hasScrollBody: false,
                child: _StateMessage(
                  icon: Icons.notifications_off_outlined,
                  title: 'Notifications could not be loaded',
                  message: provider.error!,
                  actionLabel: 'Retry',
                  onAction: provider.retry,
                ),
              )
            else if (provider.isEmpty)
              const SliverFillRemaining(
                hasScrollBody: false,
                child: _StateMessage(
                  icon: Icons.notifications_none_outlined,
                  title: 'No notifications',
                  message: 'Updates about your appointments and purchases will appear here.',
                ),
              )
            else
              SliverPadding(
                padding: const EdgeInsets.all(16),
                sliver: SliverList.separated(
                  itemCount: provider.notifications.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 10),
                  itemBuilder: (context, index) {
                    final notification = provider.notifications[index];
                    return _NotificationCard(
                      notification: notification,
                      isMarkingAsRead: provider.isMarkingAsRead(notification.id),
                      onTap: notification.isRead ? null : () => _markAsRead(context, provider, notification.id),
                    );
                  },
                ),
              ),
          ],
        ),
      ),
    );
  }

  Future<void> _markAsRead(
    BuildContext context,
    NotificationProvider provider,
    int id,
  ) async {
    final messenger = ScaffoldMessenger.of(context);

    try {
      await provider.markAsRead(id);
    } on ApiException catch (error) {
      messenger.showSnackBar(SnackBar(content: Text(error.message)));
    } catch (_) {
      messenger.showSnackBar(const SnackBar(content: Text('Notification could not be marked as read.')));
    }
  }
}

class _NotificationCard extends StatelessWidget {
  const _NotificationCard({
    required this.notification,
    required this.isMarkingAsRead,
    required this.onTap,
  });

  final UserNotification notification;
  final bool isMarkingAsRead;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final unread = !notification.isRead;

    return Card(
      color: unread ? theme.colorScheme.primaryContainer.withValues(alpha: 0.45) : null,
      child: InkWell(
        onTap: isMarkingAsRead ? null : onTap,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(
                unread ? Icons.notifications_active_outlined : Icons.notifications_none_outlined,
                color: unread ? theme.colorScheme.primary : theme.colorScheme.onSurfaceVariant,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      notification.title,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: unread ? FontWeight.w700 : FontWeight.w500,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(notification.message),
                    const SizedBox(height: 8),
                    Text(
                      _formatDateTime(notification.createdAt),
                      style: theme.textTheme.bodySmall,
                    ),
                  ],
                ),
              ),
              if (isMarkingAsRead) ...[
                const SizedBox(width: 12),
                const SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(strokeWidth: 2),
                ),
              ] else if (unread) ...[
                const SizedBox(width: 12),
                Tooltip(
                  message: 'Mark as read',
                  child: Icon(Icons.circle, size: 10, color: theme.colorScheme.primary),
                ),
              ],
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
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(icon, size: 48, color: Theme.of(context).colorScheme.primary),
          const SizedBox(height: 16),
          Text(
            title,
            style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(message, textAlign: TextAlign.center),
          if (actionLabel != null && onAction != null) ...[
            const SizedBox(height: 16),
            FilledButton(onPressed: onAction, child: Text(actionLabel!)),
          ],
        ],
      ),
    );
  }
}

String _formatDateTime(DateTime value) {
  final local = value.toLocal();
  final date =
      '${local.year.toString().padLeft(4, '0')}-${local.month.toString().padLeft(2, '0')}-${local.day.toString().padLeft(2, '0')}';
  final time = '${local.hour.toString().padLeft(2, '0')}:${local.minute.toString().padLeft(2, '0')}';
  return '$date $time';
}
