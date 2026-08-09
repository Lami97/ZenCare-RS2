import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../providers/profile_provider.dart';
import 'edit_profile_screen.dart';
import '../recommendations/recommendations_screen.dart';
import '../reviews/reviews_screen.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<ProfileProvider>(
      create: (context) => ProfileProvider(authProvider: context.read<AuthProvider>())..loadProfile(),
      child: const _ProfileView(),
    );
  }
}

class _ProfileView extends StatelessWidget {
  const _ProfileView();

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<ProfileProvider>();

    return RefreshIndicator(
      onRefresh: provider.refresh,
      child: CustomScrollView(
        physics: const AlwaysScrollableScrollPhysics(),
        slivers: [
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
              child: Text(
                'Profile',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
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
                title: 'Profile could not be loaded',
                message: provider.error!,
                actionLabel: 'Retry',
                onAction: provider.retry,
              ),
            )
          else if (provider.isEmpty)
            const SliverFillRemaining(
              hasScrollBody: false,
              child: _StateMessage(
                icon: Icons.person_outline,
                title: 'No profile data',
                message: 'Sign in again to refresh your profile information.',
              ),
            )
          else
            SliverPadding(
              padding: const EdgeInsets.all(16),
              sliver: SliverList(
                delegate: SliverChildListDelegate([
                  _ProfileCard(profile: provider.profile!),
                  const SizedBox(height: 16),
                  FilledButton.icon(
                    onPressed: () async {
                      final authProvider = context.read<AuthProvider>();
                      final user = authProvider.user;

                      if (user == null) {
                        return;
                      }

                      final updated = await Navigator.of(context).push<bool>(
                        MaterialPageRoute<bool>(
                          builder: (_) => EditProfileScreen(user: user),
                        ),
                      );

                      if (updated == true && context.mounted) {
                        await context.read<ProfileProvider>().updateUser(authProvider.user);
                      }
                    },
                    icon: const Icon(Icons.edit_outlined),
                    label: const Text('Edit profile'),
                  ),
                  const SizedBox(height: 12),
                  OutlinedButton.icon(
                    onPressed: () {
                      Navigator.of(context).push(
                        MaterialPageRoute<void>(builder: (_) => const ReviewsScreen()),
                      );
                    },
                    icon: const Icon(Icons.rate_review_outlined),
                    label: const Text('My reviews'),
                  ),
                  const SizedBox(height: 12),
                  OutlinedButton.icon(
                    onPressed: () {
                      Navigator.of(context).push(
                        MaterialPageRoute<void>(builder: (_) => const RecommendationsScreen()),
                      );
                    },
                    icon: const Icon(Icons.auto_awesome_outlined),
                    label: const Text('Recommendations'),
                  ),
                  const SizedBox(height: 12),
                  FilledButton.icon(
                    onPressed: () => context.read<AuthProvider>().logout(),
                    icon: const Icon(Icons.logout),
                    label: const Text('Logout'),
                  ),
                ]),
              ),
            ),
        ],
      ),
    );
  }
}

class _ProfileCard extends StatelessWidget {
  const _ProfileCard({required this.profile});

  final ProfileViewData profile;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final displayName = profile.fullName.trim().isEmpty ? profile.username : profile.fullName.trim();
    final roles = profile.roles.isEmpty ? profile.role : profile.roles.join(', ');

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 28,
                  child: Text(_initials(displayName)),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        displayName,
                        style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
                      ),
                      const SizedBox(height: 4),
                      Text(profile.username),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 20),
            _ProfileRow(label: 'Full name', value: displayName),
            _ProfileRow(label: 'Username', value: profile.username),
            _ProfileRow(label: 'Email', value: profile.email),
            _ProfileRow(label: 'Phone number', value: _nullableText(profile.phoneNumber)),
            _ProfileRow(label: 'Role', value: roles),
            _ProfileRow(label: 'Active status', value: _activeText(profile.isActive)),
          ],
        ),
      ),
    );
  }
}

class _ProfileRow extends StatelessWidget {
  const _ProfileRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 116,
            child: Text(
              label,
              style: theme.textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w700),
            ),
          ),
          Expanded(child: Text(value.trim().isEmpty ? '-' : value)),
        ],
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
            style: theme.textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w700),
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

String _nullableText(String? value) {
  if (value == null || value.trim().isEmpty) {
    return 'Not available';
  }

  return value.trim();
}

String _activeText(bool? value) {
  if (value == null) {
    return 'Not available';
  }

  return value ? 'Active' : 'Inactive';
}

String _initials(String value) {
  final words = value.trim().split(RegExp(r'\s+')).where((word) => word.isNotEmpty).toList();

  if (words.isEmpty) {
    return 'ZC';
  }

  if (words.length == 1) {
    return words.first.substring(0, 1).toUpperCase();
  }

  return '${words.first.substring(0, 1)}${words.last.substring(0, 1)}'.toUpperCase();
}
