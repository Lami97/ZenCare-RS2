import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/recommendation.dart';
import '../../providers/recommendation_provider.dart';
import '../../services/product_service.dart';
import '../../services/recommendation_service.dart';
import '../../utils/api_exception.dart';
import '../products/product_details_screen.dart';
import '../services/service_details_screen.dart';

class RecommendationsScreen extends StatelessWidget {
  const RecommendationsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<RecommendationProvider>(
      create: (context) => RecommendationProvider(context.read<RecommendationService>())..loadRecommendations(),
      child: const _RecommendationsView(),
    );
  }
}

class _RecommendationsView extends StatefulWidget {
  const _RecommendationsView();

  @override
  State<_RecommendationsView> createState() => _RecommendationsViewState();
}

class _RecommendationsViewState extends State<_RecommendationsView> {
  bool _isOpeningProduct = false;

  Future<void> _openProductRecommendation(Recommendation recommendation) async {
    setState(() {
      _isOpeningProduct = true;
    });

    try {
      final product = await context.read<ProductService>().getProductById(recommendation.id);

      if (!mounted) {
        return;
      }

      await Navigator.of(context).push(
        MaterialPageRoute<void>(
          builder: (_) => ProductDetailsScreen(product: product),
        ),
      );
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
        const SnackBar(content: Text('Product details could not be opened.')),
      );
    } finally {
      if (mounted) {
        setState(() {
          _isOpeningProduct = false;
        });
      }
    }
  }

  void _openServiceRecommendation(Recommendation recommendation) {
    Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => ServiceDetailsScreen(serviceId: recommendation.id),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<RecommendationProvider>();

    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Recommendations'),
          bottom: const TabBar(
            tabs: [
              Tab(text: 'Products'),
              Tab(text: 'Services'),
            ],
          ),
        ),
        body: provider.isLoading
            ? const Center(child: CircularProgressIndicator())
            : provider.error != null
                ? _StateMessage(
                    icon: Icons.error_outline,
                    title: 'Recommendations could not be loaded',
                    message: provider.error!,
                    actionLabel: 'Retry',
                    onAction: provider.retry,
                  )
                : TabBarView(
                    children: [
                      _RecommendationTab(
                        recommendations: provider.productRecommendations,
                        onRefresh: provider.refresh,
                        onOpenRecommendation: _isOpeningProduct ? null : _openProductRecommendation,
                      ),
                      _RecommendationTab(
                        recommendations: provider.serviceRecommendations,
                        onRefresh: provider.refresh,
                        onOpenRecommendation: _openServiceRecommendation,
                      ),
                    ],
                  ),
      ),
    );
  }
}

class _RecommendationTab extends StatelessWidget {
  const _RecommendationTab({
    required this.recommendations,
    required this.onRefresh,
    this.onOpenRecommendation,
  });

  final List<Recommendation> recommendations;
  final Future<void> Function() onRefresh;
  final ValueChanged<Recommendation>? onOpenRecommendation;

  @override
  Widget build(BuildContext context) {
    return RefreshIndicator(
      onRefresh: onRefresh,
      child: recommendations.isEmpty
          ? const CustomScrollView(
              physics: AlwaysScrollableScrollPhysics(),
              slivers: [
                SliverFillRemaining(
                  hasScrollBody: false,
                  child: _StateMessage(
                    icon: Icons.auto_awesome_outlined,
                    title: 'No recommendations available.',
                    message: 'Check again after more appointments or purchases.',
                  ),
                ),
              ],
            )
          : ListView.separated(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              itemCount: recommendations.length,
              separatorBuilder: (_, __) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final recommendation = recommendations[index];
                return _RecommendationCard(
                  recommendation: recommendation,
                  onTap: onOpenRecommendation == null ? null : () => onOpenRecommendation!(recommendation),
                );
              },
            ),
    );
  }
}

class _RecommendationCard extends StatelessWidget {
  const _RecommendationCard({
    required this.recommendation,
    this.onTap,
  });

  final Recommendation recommendation;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Text(
                      recommendation.name,
                      style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700),
                    ),
                  ),
                  Chip(label: Text(recommendation.type)),
                ],
              ),
              const SizedBox(height: 8),
              Text('Score: ${recommendation.score.toStringAsFixed(2)}'),
              const SizedBox(height: 8),
              Text(
                recommendation.reason.isEmpty ? 'No recommendation reason was provided.' : recommendation.reason,
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
