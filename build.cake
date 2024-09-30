#addin nuget:?package=Cake.Incubator&version=8.0.0

var target = Argument("target", "Build");

var workflow = BuildSystem.GitHubActions.Environment.Workflow;
var buildId = workflow.RunNumber;
var tag = workflow.RefType == GitHubActionsRefType.Tag ? workflow.RefName : null;

var rootDir = Directory(".");
var buildDir = rootDir + Directory("build");
var binDir = buildDir + Directory("bin");
var packagesDir = buildDir + Directory("packages");
var archiveDir = buildDir + Directory("archives");
var cacheDir = buildDir + Directory("cache");
var tempDir = buildDir + Directory("temp");
var outputDir = tempDir + Directory("output");

EnsureDirectoryExists(buildDir);
EnsureDirectoryExists(binDir);
EnsureDirectoryExists(packagesDir);
EnsureDirectoryExists(archiveDir);
EnsureDirectoryExists(cacheDir);
EnsureDirectoryExists(tempDir);

const string ffmpegZipUrl =
    "https://github.com/BtbN/FFmpeg-Builds/releases/download/autobuild-2024-09-28-13-00/ffmpeg-N-117234-g8a951ef5e1-win64-lgpl.zip";

Task("Build")
    .Does(ctx =>
    {
        CleanDirectory(binDir);
        CleanDirectory(packagesDir);
        CleanDirectory(archiveDir);
        CleanDirectory(tempDir);
        EnsureDirectoryExists(outputDir);

        var settings = new DotNetBuildSettings
        {
            Configuration = "Release",
            MSBuildSettings = new DotNetMSBuildSettings(),
            OutputDirectory = outputDir
        };

        if (tag != null) 
        {
            settings.MSBuildSettings.Version = tag.TrimStart('v');
        }
        else if (buildId != 0)
        {
            settings.MSBuildSettings.VersionSuffix = "ci." + buildId;
        }

        DotNetBuild(".", settings);

        RunTarget("DownloadFFmpegBinary");

        foreach (var filePath in GetFiles(System.IO.Path.Combine(outputDir, "*.dll")))
        {
            var assemblyName = filePath.GetFilenameWithoutExtension().ToString();
            var assemblyOutputDir = System.IO.Path.Combine(binDir, assemblyName);
            var assemblyOutputPath = System.IO.Path.Combine(assemblyOutputDir, $"{assemblyName}.dll");
            EnsureDirectoryExists(assemblyOutputDir);
            CopyFile(filePath, assemblyOutputPath);

            Zip(assemblyOutputDir, System.IO.Path.Combine(archiveDir, $"{assemblyName}.zip"));
        }

        Zip(binDir, System.IO.Path.Combine(archiveDir, "Mitochondria.zip"));

        CopyFiles(System.IO.Path.Combine(outputDir, "*.nupkg"), packagesDir);

        DeleteDirectory(tempDir, new DeleteDirectorySettings
        {
            Recursive = true
        });

        const string AmongUsEnvVarName = "AmongUs";
        if (HasEnvironmentVariable(AmongUsEnvVarName))
        {
            var amongUsDir = EnvironmentVariable(AmongUsEnvVarName);
            var pluginsDir = System.IO.Path.Combine(amongUsDir, "BepInEx", "plugins", "Mitochondria");

            CopyDirectory(binDir, pluginsDir);
        }
    });

Task("DownloadFFmpegBinary")
    .Does(() =>
    {
        if (!FileExists(System.IO.Path.Combine(outputDir, "Mitochondria.Resources.FFmpeg.dll")))
        {
            return;
        }

        var ffmpegZipFileName = System.IO.Path.GetFileNameWithoutExtension(new Uri(ffmpegZipUrl).LocalPath);
        var ffmpegCachePath = cacheDir + Directory(ffmpegZipFileName);

        string ffmpegBinaryPath;
        string ffprobeBinaryPath;
        if (DirectoryExists(ffmpegCachePath))
        {
            ffmpegBinaryPath = System.IO.Path.Combine(ffmpegCachePath, "ffmpeg.exe");
            ffprobeBinaryPath = System.IO.Path.Combine(ffmpegCachePath, "ffprobe.exe");
        }
        else
        {
            foreach (var directoryPath in GetSubDirectories(cacheDir))
            {
                if (directoryPath.Segments[^1].StartsWith("ffmpeg"))
                {
                    DeleteDirectory(directoryPath, new DeleteDirectorySettings());
                }
            }

            var ffmpegZipPath = System.IO.Path.Combine(tempDir, $"{ffmpegZipFileName}.zip");
            var ffmpegBinPath = System.IO.Path.Combine(tempDir, ffmpegZipFileName, "bin");
            ffmpegBinaryPath = System.IO.Path.Combine(ffmpegBinPath, "ffmpeg.exe");
            ffprobeBinaryPath = System.IO.Path.Combine(ffmpegBinPath, "ffprobe.exe");

            DownloadFile(ffmpegZipUrl, ffmpegZipPath);
            Unzip(ffmpegZipPath, tempDir, true);

            EnsureDirectoryExists(ffmpegCachePath);
            CopyFile(ffmpegBinaryPath, System.IO.Path.Combine(ffmpegCachePath, "ffmpeg.exe"));
            CopyFile(ffprobeBinaryPath, System.IO.Path.Combine(ffmpegCachePath, "ffprobe.exe"));
        }

        var ffmpegProjectDir = System.IO.Path.Combine(binDir, "Mitochondria.Resources.FFmpeg");
        EnsureDirectoryExists(ffmpegProjectDir);

        Context.FileSystem.GetFile(ffmpegBinaryPath).Copy(System.IO.Path.Combine(ffmpegProjectDir, "ffmpeg.exe"), true);

        Context.FileSystem
            .GetFile(ffprobeBinaryPath)
            .Copy(System.IO.Path.Combine(ffmpegProjectDir, "ffprobe.exe"), true);
    });

RunTarget(target);
