import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/admin_models.dart';
import '../providers/module_provider.dart';
import '../services/admin_repository.dart';
import '../widgets/app_state_views.dart';
import '../widgets/entity_form_dialog.dart';

class ModuleScreen extends StatelessWidget {
  const ModuleScreen({super.key, required this.module});

  final AdminModule module;

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      key: ValueKey(module.endpoint),
      create: (context) =>
          ModuleProvider(context.read<AdminRepository>(), module)
            ..load(filters: const {}, page: 1),
      child: _ModuleView(module: module),
    );
  }
}

class _ModuleView extends StatefulWidget {
  const _ModuleView({required this.module});

  final AdminModule module;

  @override
  State<_ModuleView> createState() => _ModuleViewState();
}

class _ModuleViewState extends State<_ModuleView> {
  final _searchController = TextEditingController();
  final _filterValues = <String, dynamic>{};
  int? _selectedId;
  Map<String, dynamic>? _selectedItem;

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Map<String, dynamic> _filters() {
    final filters = <String, dynamic>{};
    if (widget.module.searchKey != null &&
        _searchController.text.trim().isNotEmpty) {
      filters[widget.module.searchKey!] = _searchController.text.trim();
    }
    for (final entry in _filterValues.entries) {
      final value = entry.value;
      if (value == null) continue;
      filters[entry.key] = value;
    }
    return filters;
  }

  Future<void> _applySearch() async {
    _clearSelection();
    await context.read<ModuleProvider>().load(filters: _filters(), page: 1);
  }

  Future<void> _refresh() async {
    _searchController.clear();
    _filterValues.clear();
    _clearSelection();
    setState(() {});
    await context.read<ModuleProvider>().load(filters: const {}, page: 1);
  }

  void _clearSelection() {
    _selectedId = null;
    _selectedItem = null;
  }

  Future<void> _openForm({Map<String, dynamic>? item}) async {
    final repository = context.read<AdminRepository>();
    final result = await showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (_) => EntityFormDialog(
        module: widget.module,
        repository: repository,
        initialData: item,
      ),
    );
    if (!mounted || result != true) return;
    final action = item == null ? 'added' : 'updated';
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          '${_capitalize(widget.module.entityName)} was $action successfully.',
        ),
      ),
    );
    _clearSelection();
    await context.read<ModuleProvider>().load(filters: _filters());
  }

  Future<void> _deleteSelected() async {
    final id = _selectedId;
    if (id == null) return;
    final confirmed = await confirmAction(
      context,
      title: 'Delete ${widget.module.entityName}',
      message:
          'Are you sure you want to delete this ${widget.module.entityName}?',
      actionLabel: 'Delete',
    );
    if (!mounted || !confirmed) return;
    try {
      await context.read<ModuleProvider>().delete(id);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            '${_capitalize(widget.module.entityName)} was deleted successfully.',
          ),
        ),
      );
      setState(_clearSelection);
    } catch (error) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(error.toString()),
          backgroundColor: Theme.of(context).colorScheme.error,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  widget.module.title,
                  style: theme.textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ),
              FilledButton.icon(
                onPressed: widget.module.canAdd ? () => _openForm() : null,
                icon: const Icon(Icons.add),
                label: const Text('Add'),
              ),
              const SizedBox(width: 8),
              OutlinedButton.icon(
                onPressed: widget.module.canEdit && _selectedItem != null
                    ? () => _openForm(item: _selectedItem)
                    : null,
                icon: const Icon(Icons.edit),
                label: const Text('Edit'),
              ),
              const SizedBox(width: 8),
              OutlinedButton.icon(
                onPressed: widget.module.canDelete && _selectedId != null
                    ? _deleteSelected
                    : null,
                icon: const Icon(Icons.delete_outline),
                label: const Text('Delete'),
              ),
              const SizedBox(width: 8),
              IconButton(
                onPressed: _refresh,
                tooltip: 'Refresh',
                icon: const Icon(Icons.refresh),
              ),
            ],
          ),
          const SizedBox(height: 18),
          _FilterBar(
            module: widget.module,
            searchController: _searchController,
            filterValues: _filterValues,
            onChanged: () => setState(() {}),
            onSearch: _applySearch,
          ),
          const SizedBox(height: 18),
          Expanded(
            child: Consumer<ModuleProvider>(
              builder: (context, provider, _) {
                if (provider.isLoading && provider.items.isEmpty) {
                  return const LoadingView(message: 'Loading records...');
                }
                if (provider.error != null && provider.items.isEmpty) {
                  return ErrorStateView(
                    message: provider.error!,
                    onRetry: () => provider.load(filters: _filters()),
                  );
                }
                if (provider.items.isEmpty) {
                  return EmptyView(
                    title: 'No records found',
                    message: 'Adjust filters or add a new record.',
                  );
                }
                return Column(
                  children: [
                    if (provider.error != null)
                      Padding(
                        padding: const EdgeInsets.only(bottom: 8),
                        child: Text(
                          provider.error!,
                          style: TextStyle(color: theme.colorScheme.error),
                        ),
                      ),
                    Expanded(
                      child: _DataGrid(
                        module: widget.module,
                        items: provider.items,
                        selectedId: _selectedId,
                        onSelected: _selectItem,
                      ),
                    ),
                    const SizedBox(height: 12),
                    _Pagination(provider: provider),
                  ],
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  void _selectItem(Map<String, dynamic> item) {
    setState(() {
      _selectedItem = item;
      final rawId = item['id'];
      _selectedId = rawId is int
          ? rawId
          : int.tryParse(rawId?.toString() ?? '');
    });
  }

  String _capitalize(String value) => value.isEmpty
      ? value
      : value.substring(0, 1).toUpperCase() + value.substring(1);
}

class _FilterBar extends StatefulWidget {
  const _FilterBar({
    required this.module,
    required this.searchController,
    required this.filterValues,
    required this.onChanged,
    required this.onSearch,
  });

  final AdminModule module;
  final TextEditingController searchController;
  final Map<String, dynamic> filterValues;
  final VoidCallback onChanged;
  final Future<void> Function() onSearch;

  @override
  State<_FilterBar> createState() => _FilterBarState();
}

class _FilterBarState extends State<_FilterBar> {
  final _lookupCache = <String, List<LookupOption>>{};

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    for (final filter in widget.module.filters.where(
      (filter) => filter.lookup != null,
    )) {
      _loadLookup(filter);
    }
  }

  Future<void> _loadLookup(FilterField filter) async {
    if (_lookupCache.containsKey(filter.key)) return;
    try {
      final items = await context.read<AdminRepository>().lookup(
        filter.lookup!,
      );
      if (!mounted) return;
      setState(() => _lookupCache[filter.key] = items);
    } catch (_) {
      if (!mounted) return;
      setState(() => _lookupCache[filter.key] = const []);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: BorderSide(color: Theme.of(context).dividerColor),
      ),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Wrap(
          spacing: 12,
          runSpacing: 12,
          crossAxisAlignment: WrapCrossAlignment.center,
          children: [
            if (widget.module.searchKey != null)
              SizedBox(
                width: widget.module.searchWidth,
                child: TextField(
                  controller: widget.searchController,
                  decoration: InputDecoration(
                    labelText: widget.module.searchLabel,
                    prefixIcon: const Icon(Icons.search),
                  ),
                  onSubmitted: (_) => widget.onSearch(),
                ),
              ),
            ...widget.module.filters.map(_buildFilter),
            FilledButton.icon(
              onPressed: widget.onSearch,
              icon: const Icon(Icons.search),
              label: const Text('Search'),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFilter(FilterField filter) {
    if (filter.isBoolean) {
      return SizedBox(
        width: 190,
        child: DropdownButtonFormField<bool>(
          isExpanded: true,
          initialValue: widget.filterValues[filter.key] as bool?,
          decoration: InputDecoration(labelText: filter.label),
          items: [
            const DropdownMenuItem<bool>(value: null, child: Text('All')),
            DropdownMenuItem<bool>(
              value: true,
              child: Text(filter.booleanTrueLabel),
            ),
            DropdownMenuItem<bool>(
              value: false,
              child: Text(filter.booleanFalseLabel),
            ),
          ],
          onChanged: (value) {
            widget.filterValues[filter.key] = value;
            widget.onChanged();
          },
        ),
      );
    }
    if (filter.lookup != null) {
      final items = _lookupCache[filter.key] ?? const <LookupOption>[];
      return SizedBox(
        width: 230,
        child: DropdownButtonFormField<int>(
          isExpanded: true,
          initialValue: widget.filterValues[filter.key] as int?,
          decoration: InputDecoration(labelText: filter.label),
          items: [
            const DropdownMenuItem<int>(value: null, child: Text('All')),
            ...items.map(
              (item) => DropdownMenuItem(
                value: item.value,
                child: Text(item.label, overflow: TextOverflow.ellipsis),
              ),
            ),
          ],
          onChanged: (value) {
            widget.filterValues[filter.key] = value;
            widget.onChanged();
          },
        ),
      );
    }
    return SizedBox(
      width: 220,
      child: DropdownButtonFormField<int>(
        isExpanded: true,
        initialValue: widget.filterValues[filter.key] as int?,
        decoration: InputDecoration(labelText: filter.label),
        items: [
          const DropdownMenuItem<int>(value: null, child: Text('All')),
          ...filter.statusOptions.map(
            (item) =>
                DropdownMenuItem(value: item.value, child: Text(item.label)),
          ),
        ],
        onChanged: (value) {
          widget.filterValues[filter.key] = value;
          widget.onChanged();
        },
      ),
    );
  }
}

class _DataGrid extends StatefulWidget {
  const _DataGrid({
    required this.module,
    required this.items,
    required this.selectedId,
    required this.onSelected,
  });

  final AdminModule module;
  final List<Map<String, dynamic>> items;
  final int? selectedId;
  final ValueChanged<Map<String, dynamic>> onSelected;

  @override
  State<_DataGrid> createState() => _DataGridState();
}

class _DataGridState extends State<_DataGrid> {
  final _horizontalScrollController = ScrollController();

  @override
  void dispose() {
    _horizontalScrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 0,
      margin: EdgeInsets.zero,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: BorderSide(color: Theme.of(context).dividerColor),
      ),
      child: Scrollbar(
        controller: _horizontalScrollController,
        child: SingleChildScrollView(
          controller: _horizontalScrollController,
          scrollDirection: Axis.horizontal,
          child: SingleChildScrollView(
            child: DataTable(
              showCheckboxColumn: false,
              columns: widget.module.columns
                  .map((column) => DataColumn(label: Text(column.label)))
                  .toList(),
              rows: widget.items.map((item) {
                final rawId = item['id'];
                final id = rawId is int
                    ? rawId
                    : int.tryParse(rawId?.toString() ?? '');
                return DataRow(
                  selected: id != null && id == widget.selectedId,
                  onSelectChanged: (_) => widget.onSelected(item),
                  cells: widget.module.columns
                      .map(
                        (column) => DataCell(
                          ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 280),
                            child: Text(
                              column.value(item),
                              overflow: TextOverflow.ellipsis,
                            ),
                          ),
                        ),
                      )
                      .toList(),
                );
              }).toList(),
            ),
          ),
        ),
      ),
    );
  }
}

class _Pagination extends StatelessWidget {
  const _Pagination({required this.provider});

  final ModuleProvider provider;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Text(
          provider.totalCount == null
              ? 'Page ${provider.page}'
              : 'Page ${provider.page} of ${provider.totalPages} - ${provider.totalCount} records',
        ),
        const Spacer(),
        OutlinedButton.icon(
          onPressed: provider.canPrevious ? provider.previousPage : null,
          icon: const Icon(Icons.chevron_left),
          label: const Text('Previous'),
        ),
        const SizedBox(width: 8),
        OutlinedButton.icon(
          onPressed: provider.canNext ? provider.nextPage : null,
          icon: const Icon(Icons.chevron_right),
          label: const Text('Next'),
        ),
      ],
    );
  }
}
