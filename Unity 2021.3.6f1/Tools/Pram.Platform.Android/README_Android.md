# Android


### Supported Application Formats
-------------------------------------------------------------------------------
* Android Application Package (APK)
* Gradle Project (With [Shadow Files](#shadow-files) Support)
* FolderWithExe (command line applications)


### Start Arguments
-------------------------------------------------------------------------------
Arguments for installed applications (all format except FolderWithExe) are passed as to the intent as an "extra string array" with key `cli-args`.


### Host Tool Dependencies
-------------------------------------------------------------------------------
See environment variable section. There are fallbacks in place to pickup system installations of java and adb. But that is not a recommended setup.

### Shadow Files
-------------------------------------------------------------------------------
Shadow Files let's you push file updates to your application without redeploying or rebuilding the APK.

How this works is that pram will put all asset files marked as shadow assets in the internal package folder under the
name `pram-shadow-files/assets`. Files will have the same relative path as they did in the original assets folder.
Marking files as potential shadow files is done by adding paths to a file named `pram-shadow-files` in project root
directory. A path can be a file or directory. If path is a directory that path will be scanned for any asset files.

To make an application aware that it's running in a Shadow Files setup pram will place a marker asset, again named
`pram-shadow-files`, inside the APK. An application should _ONLY_ use assets from the shadow files directory if that
 marker is present. This is crucial since not having that marker most likely means the APK was (re)deployed without
 Shadow Files support. Potentially by a tool other than pram. For ease of use the marker file will also contain the
 path where shadow files are located on the device. Here is a short example how to detect and open an asset file:

```java
try {
    InputStream in = getAssets().open("pram-shadow-files");
    String shadowFilesFolder = new BufferedReader(new InputStreamReader(in)).readLine();
    return new FileInputStream(new File(shadowFilesFolder + "/assets", assetPath))

} catch (IOException noShadowFilesSupport) {
    return context.getAssets().open(assetPath);
}
```

In a similar fashion native shadow libraries are also supported. Here is a short example of loading a native library:

```java
try {
    InputStream in = getAssets().open("pram-shadow-files");
    String shadowFilesFolder = new BufferedReader(new InputStreamReader(in)).readLine();
    System.load(shadowFilesFolder + "/lib/" + System.mapLibraryName(libName));

} catch (IOException noShadowFilesSupport) {
    System.loadLibrary(libName);
}
```

If application is uninstalled the package directory is deleted and so are all Shadow Files.

### File Pulling
-------------------------------------------------------------------------------

Command line applications (`FolderWithExe`) will check the install folder for `app-file-pull`
For any other application type, Pram will first check the internal files directory (see https://developer.android.com/reference/android/content/Context#getFilesDir()) and then the external.
