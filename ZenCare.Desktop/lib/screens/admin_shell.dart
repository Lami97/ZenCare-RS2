import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../config/admin_modules.dart';
import '../providers/auth_provider.dart';
import 'module_screen.dart';
import 'reports_screen.dart';

class AdminShell extends StatefulWidget {
  const AdminShell({super.key});

  @override
  State<AdminShell> createState() => _AdminShellState();
}

class _AdminShellState extends State<AdminShell> {
  int _selectedIndex = 0;

  @override
  Widget build(BuildContext context) {
    final items = [
      const _NavigationItem(
        label: 'Dashboard',
        icon: Icons.dashboard_outlined,
        selectedIcon: Icons.dashboard,
      ),
      const _NavigationItem(
        label: 'Reports',
        icon: Icons.summarize_outlined,
        selectedIcon: Icons.summarize,
      ),
      ...adminModules.map(
        (module) => _NavigationItem(
          label: module.title,
          icon: Icons.table_rows_outlined,
          selectedIcon: Icons.table_rows,
        ),
      ),
    ];

    return Scaffold(
      body: Row(
        children: [
          SizedBox(
            width: 244,
            child: SafeArea(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(18, 18, 18, 12),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          'ZenCare',
                          style: TextStyle(
                            fontSize: 22,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                        Consumer<AuthProvider>(
                          builder: (context, auth, _) => Text(
                            auth.displayName,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                      ],
                    ),
                  ),
                  Expanded(
                    child: ListView.builder(
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                      itemCount: items.length,
                      itemBuilder: (context, index) {
                        final item = items[index];
                        final selected = index == _selectedIndex;
                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 2),
                          child: _SidebarDestination(
                            item: item,
                            selected: selected,
                            onTap: () => setState(() => _selectedIndex = index),
                          ),
                        );
                      },
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 10, 8, 18),
                    child: TextButton.icon(
                      onPressed: () => context.read<AuthProvider>().logout(),
                      icon: const Icon(Icons.logout),
                      label: const Align(
                        alignment: Alignment.centerLeft,
                        child: Text('Logout'),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          const VerticalDivider(width: 1),
          Expanded(child: _buildContent()),
        ],
      ),
    );
  }

  Widget _buildContent() {
    if (_selectedIndex == 0) return const _DashboardHome();
    if (_selectedIndex == 1) return const ReportsScreen();
    return ModuleScreen(module: adminModules[_selectedIndex - 2]);
  }
}

class _NavigationItem {
  const _NavigationItem({
    required this.label,
    required this.icon,
    required this.selectedIcon,
  });

  final String label;
  final IconData icon;
  final IconData selectedIcon;
}

class _SidebarDestination extends StatelessWidget {
  const _SidebarDestination({
    required this.item,
    required this.selected,
    required this.onTap,
  });

  final _NavigationItem item;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final color = selected
        ? theme.colorScheme.onSecondaryContainer
        : theme.colorScheme.onSurfaceVariant;

    return Material(
      color: selected
          ? theme.colorScheme.secondaryContainer
          : Colors.transparent,
      borderRadius: BorderRadius.circular(8),
      child: InkWell(
        borderRadius: BorderRadius.circular(8),
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          child: Row(
            children: [
              Icon(selected ? item.selectedIcon : item.icon, color: color),
              const SizedBox(width: 12),
              Expanded(
                child: Text(
                  item.label,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    color: color,
                    fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _DashboardHome extends StatelessWidget {
  const _DashboardHome();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.all(28),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Administration dashboard',
            style: theme.textTheme.headlineMedium?.copyWith(
              fontWeight: FontWeight.w700,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Use the navigation menu to manage ZenCare records, review reports, and export PDF summaries.',
            style: theme.textTheme.bodyLarge,
          ),
          const SizedBox(height: 28),
          Wrap(
            spacing: 16,
            runSpacing: 16,
            children: const [
              _QuickInfo(
                icon: Icons.manage_accounts,
                title: 'CRUD modules',
                text:
                    'Manage users, services, products, appointments, purchases, reviews, FAQ and suppliers.',
              ),
              _QuickInfo(
                icon: Icons.search,
                title: 'Search and filters',
                text:
                    'List screens include search, status filters, pagination and refresh behavior.',
              ),
              _QuickInfo(
                icon: Icons.picture_as_pdf,
                title: 'PDF reports',
                text:
                    'Export appointment summaries and business statistics from Reports.',
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _QuickInfo extends StatelessWidget {
  const _QuickInfo({
    required this.icon,
    required this.title,
    required this.text,
  });

  final IconData icon;
  final String title;
  final String text;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return SizedBox(
      width: 300,
      child: Card(
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
          side: BorderSide(color: theme.dividerColor),
        ),
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(icon, color: theme.colorScheme.primary),
              const SizedBox(height: 12),
              Text(
                title,
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
              ),
              const SizedBox(height: 8),
              Text(text),
            ],
          ),
        ),
      ),
    );
  }
}
