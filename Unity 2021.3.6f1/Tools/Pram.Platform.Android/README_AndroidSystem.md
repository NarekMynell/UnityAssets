# AndroidSystem

Allows to deploy APKs as privileged system applications targeting Android Automotive Development Kits with engineering or user-debug systems.

Only DeploymentSchemeHint.Retail will enable SELinux enforcement on deploy.

Minimum version requirement: Android 9.0

### Supported Application Formats
-------------------------------------------------------------------------------
* Android Application Package (APK)

### Host Tool Dependencies
-------------------------------------------------------------------------------
See environment variable section. There are fallbacks in place to pickup system installations of java and adb. But that is not a recommended setup.
