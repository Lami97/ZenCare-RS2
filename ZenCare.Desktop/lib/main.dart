import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'providers/auth_provider.dart';
import 'screens/admin_shell.dart';
import 'screens/login_screen.dart';
import 'services/admin_repository.dart';
import 'services/api_service.dart';
import 'services/auth_service.dart';
import 'widgets/app_state_views.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  final apiService = ApiService();
  final authService = AuthService(apiService);
  final authProvider = AuthProvider(apiService, authService);
  final adminRepository = AdminRepository(apiService);

  runApp(
    MultiProvider(
      providers: [
        Provider<ApiService>.value(value: apiService),
        Provider<AuthService>.value(value: authService),
        Provider<AdminRepository>.value(value: adminRepository),
        ChangeNotifierProvider<AuthProvider>.value(
          value: authProvider..initialize(),
        ),
      ],
      child: const ZenCareDesktopApp(),
    ),
  );
}

class ZenCareDesktopApp extends StatelessWidget {
  const ZenCareDesktopApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'ZenCare Desktop Admin',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF2F6F73)),
        useMaterial3: true,
        inputDecorationTheme: const InputDecorationTheme(
          border: OutlineInputBorder(),
          helperMaxLines: 3,
          errorMaxLines: 3,
        ),
        dataTableTheme: const DataTableThemeData(
          headingRowHeight: 44,
          dataRowMinHeight: 46,
          dataRowMaxHeight: 58,
        ),
        cardTheme: const CardThemeData(margin: EdgeInsets.zero),
      ),
      home: Consumer<AuthProvider>(
        builder: (context, auth, _) {
          if (auth.isInitializing) {
            return const Scaffold(
              body: LoadingView(message: 'Preparing admin session...'),
            );
          }
          return auth.isAuthenticated
              ? const AdminShell()
              : const LoginScreen();
        },
      ),
    );
  }
}
