import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/category.dart';
import '../../models/wellness_service.dart';
import '../../providers/wellness_service_provider.dart';
import '../../services/wellness_service_service.dart';
import 'service_details_screen.dart';

class ServicesScreen extends StatefulWidget {
  const ServicesScreen({
    super.key,
    required this.onReservationCreated,
  });

  final VoidCallback onReservationCreated;

  @override
  State<ServicesScreen> createState() => _ServicesScreenState();
}

class _ServicesScreenState extends State<ServicesScreen> {
  WellnessService? _selectedService;

  void _openDetails(WellnessService service) {
    setState(() {
      _selectedService = service;
    });
  }

  void _closeDetails() {
    setState(() {
      _selectedService = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<WellnessServiceProvider>(
      create: (context) =>
          WellnessServiceProvider(context.read<WellnessServiceService>())
            ..loadInitial(),
      child: PopScope(
        canPop: _selectedService == null,
        onPopInvokedWithResult: (didPop, result) {
          if (!didPop && _selectedService != null) {
            _closeDetails();
          }
        },
        child: IndexedStack(
          index: _selectedService == null ? 0 : 1,
          children: [
            _ServicesView(onServiceSelected: _openDetails),
            if (_selectedService == null)
              const SizedBox.shrink()
            else
              ServiceDetailsScreen(
                serviceId: _selectedService!.id,
                onBack: _closeDetails,
                onReservationCreated: widget.onReservationCreated,
              ),
          ],
        ),
      ),
    );
  }
}

class _ServicesView extends StatefulWidget {
  const _ServicesView({required this.onServiceSelected});

  final ValueChanged<WellnessService> onServiceSelected;

  @override
  State<_ServicesView> createState() => _ServicesViewState();
}

class _ServicesViewState extends State<_ServicesView> {
  final _searchController = TextEditingController();
  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController
      ..removeListener(_onScroll)
      ..dispose();
    _searchController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (!_scrollController.hasClients) {
      return;
    }

    final position = _scrollController.position;
    if (position.pixels >= position.maxScrollExtent - 240) {
      context.read<WellnessServiceProvider>().loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<WellnessServiceProvider>();

    return RefreshIndicator(
      onRefresh: provider.refresh,
      child: CustomScrollView(
        controller: _scrollController,
        physics: const AlwaysScrollableScrollPhysics(),
        slivers: [
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Services',
                    style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                  ),
                  const SizedBox(height: 12),
                  TextField(
                    controller: _searchController,
                    textInputAction: TextInputAction.search,
                    decoration: InputDecoration(
                      labelText: 'Search services',
                      prefixIcon: const Icon(Icons.search),
                      suffixIcon: _searchController.text.isEmpty
                          ? null
                          : IconButton(
                              tooltip: 'Clear search',
                              onPressed: () {
                                _searchController.clear();
                                provider.setSearchText('');
                                setState(() {});
                              },
                              icon: const Icon(Icons.close),
                            ),
                    ),
                    onChanged: (value) {
                      setState(() {});
                      provider.setSearchText(value);
                    },
                  ),
                  const SizedBox(height: 12),
                  _CategoryFilter(
                    categories: provider.categories,
                    selectedCategoryId: provider.selectedCategoryId,
                    onSelected: provider.setSelectedCategoryId,
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
                title: 'Services could not be loaded',
                message: provider.error!,
                actionLabel: 'Retry',
                onAction: provider.retry,
              ),
            )
          else if (provider.isEmpty)
            const SliverFillRemaining(
              hasScrollBody: false,
              child: _StateMessage(
                icon: Icons.spa_outlined,
                title: 'No services found',
                message: 'Try another search term or category.',
              ),
            )
          else ...[
            SliverPadding(
              padding: const EdgeInsets.all(16),
              sliver: SliverList.separated(
                itemCount: provider.services.length,
                separatorBuilder: (_, __) => const SizedBox(height: 10),
                itemBuilder: (context, index) {
                  final service = provider.services[index];
                  return _ServiceCard(
                    service: service,
                    onTap: () => widget.onServiceSelected(service),
                  );
                },
              ),
            ),
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(16, 0, 16, 24),
                child: Center(
                  child: provider.hasMore
                      ? provider.isLoadingMore
                          ? const CircularProgressIndicator()
                          : TextButton.icon(
                              onPressed: provider.loadMore,
                              icon: const Icon(Icons.expand_more),
                              label: const Text('Load more'),
                            )
                      : Text(
                          'Showing ${provider.services.length} of ${provider.totalCount} services',
                          style: Theme.of(context).textTheme.bodySmall,
                        ),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _CategoryFilter extends StatelessWidget {
  const _CategoryFilter({
    required this.categories,
    required this.selectedCategoryId,
    required this.onSelected,
  });

  final List<Category> categories;
  final int? selectedCategoryId;
  final ValueChanged<int?> onSelected;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 40,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: categories.length + 1,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          if (index == 0) {
            return ChoiceChip(
              label: const Text('All'),
              selected: selectedCategoryId == null,
              onSelected: (_) => onSelected(null),
            );
          }

          final category = categories[index - 1];
          return ChoiceChip(
            label: Text(category.name),
            selected: selectedCategoryId == category.id,
            onSelected: (_) => onSelected(category.id),
          );
        },
      ),
    );
  }
}

class _ServiceCard extends StatelessWidget {
  const _ServiceCard({required this.service, required this.onTap});

  final WellnessService service;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Icon(
                Icons.spa_outlined,
                size: 40,
                color: theme.colorScheme.primary,
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      service.name,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      service.serviceCategoryName,
                      style: theme.textTheme.bodySmall,
                    ),
                    const SizedBox(height: 8),
                    Wrap(
                      spacing: 16,
                      runSpacing: 4,
                      children: [
                        Text(
                          '\$${service.price.toStringAsFixed(2)}',
                          style: theme.textTheme.titleSmall?.copyWith(
                            color: theme.colorScheme.primary,
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                        Text('${service.durationMinutes} minutes'),
                      ],
                    ),
                    if ((service.description ?? '').trim().isNotEmpty) ...[
                      const SizedBox(height: 8),
                      Text(
                        service.description!.trim(),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodySmall,
                      ),
                    ],
                  ],
                ),
              ),
              const Icon(Icons.chevron_right),
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
            style: theme.textTheme.titleLarge?.copyWith(
              fontWeight: FontWeight.w700,
            ),
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
