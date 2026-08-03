import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'core/app_theme.dart';
import 'providers/auth_provider.dart';
import 'providers/cart_provider.dart';
import 'providers/purchase_provider.dart';
import 'screens/auth/login_screen.dart';
import 'screens/home/home_screen.dart';
import 'screens/splash_screen.dart';
import 'services/api_service.dart';
import 'services/appointment_service.dart';
import 'services/cart_service.dart';
import 'services/auth_service.dart';
import 'services/product_service.dart';
import 'services/purchase_service.dart';
import 'services/recommendation_service.dart';
import 'services/review_service.dart';
import 'services/wellness_service_service.dart';

final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const ZenCareApp());
}

class ZenCareApp extends StatelessWidget {
  const ZenCareApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        Provider<ApiService>(
          create: (_) => ApiService(),
          dispose: (_, service) => service.dispose(),
        ),
        ProxyProvider<ApiService, AuthService>(
          update: (_, apiService, __) => AuthService(apiService),
        ),
        ProxyProvider<ApiService, ProductService>(
          update: (_, apiService, __) => ProductService(apiService),
        ),
        ProxyProvider<ApiService, AppointmentService>(
          update: (_, apiService, __) => AppointmentService(apiService),
        ),
        ProxyProvider<ApiService, CartService>(
          update: (_, apiService, __) => CartService(apiService),
        ),
        ProxyProvider<ApiService, PurchaseService>(
          update: (_, apiService, __) => PurchaseService(apiService),
        ),
        ProxyProvider<ApiService, ReviewService>(
          update: (_, apiService, __) => ReviewService(apiService),
        ),
        ProxyProvider<ApiService, RecommendationService>(
          update: (_, apiService, __) => RecommendationService(apiService),
        ),
        ProxyProvider<ApiService, WellnessServiceService>(
          update: (_, apiService, __) => WellnessServiceService(apiService),
        ),
        ChangeNotifierProxyProvider2<CartService, ProductService, CartProvider>(
          create: (context) => CartProvider(
            context.read<CartService>(),
            context.read<ProductService>(),
          ),
          update: (_, cartService, productService, cartProvider) =>
              cartProvider ?? CartProvider(cartService, productService),
        ),
        ChangeNotifierProxyProvider<PurchaseService, PurchaseProvider>(
          create: (context) => PurchaseProvider(context.read<PurchaseService>()),
          update: (_, purchaseService, purchaseProvider) => purchaseProvider ?? PurchaseProvider(purchaseService),
        ),
        ChangeNotifierProxyProvider2<AuthService, ApiService, AuthProvider>(
          create: (_) => AuthProvider(),
          update: (_, authService, apiService, authProvider) {
            final provider = authProvider ?? AuthProvider();
            provider.configure(authService, apiService);
            return provider;
          },
        ),
      ],
      child: Consumer<AuthProvider>(
        builder: (context, authProvider, _) {
          return MaterialApp(
            title: 'ZenCare',
            debugShowCheckedModeBanner: false,
            navigatorKey: navigatorKey,
            theme: AppTheme.lightTheme,
            home: _resolveHome(authProvider),
          );
        },
      ),
    );
  }

  Widget _resolveHome(AuthProvider authProvider) {
    if (authProvider.isInitializing) {
      return const SplashScreen();
    }

    return authProvider.isAuthenticated ? const HomeScreen() : const LoginScreen();
  }
}


