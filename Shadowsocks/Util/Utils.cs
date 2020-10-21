using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;

using NLog;

using Shadowsocks.Model;

namespace Shadowsocks.Util
{
    public struct BandwidthScaleInfo
    {
        public float value;
        public string unitName;
        public long unit;

        public BandwidthScaleInfo(float value, string unitName, long unit)
        {
            this.value = value;
            this.unitName = unitName;
            this.unit = unit;
        }
    }

    public static class PathUtil
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // https://github.com/dotnet/runtime/issues/13051#issuecomment-510267727
        public static readonly string ExecutablePath = Process.GetCurrentProcess().MainModule?.FileName;
        public static readonly string WorkingDirectory = Path.GetDirectoryName(ExecutablePath);

        private static string _tempPath = null;

        // return path to store temporary files
        public static string GetTempPath()
        {
            if (_tempPath == null)
            {
                bool isPortableMode = Configuration.Load().portableMode;
                try
                {
                    if (isPortableMode)
                    {
                        _tempPath = Directory.CreateDirectory("ss_win_temp").FullName;
                        // don't use "/", it will fail when we call explorer /select xxx/ss_win_temp\xxx.log
                    }
                    else
                    {
                        _tempPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), @"Shadowsocks\ss_win_temp_" + ExecutablePath.GetHashCode())).FullName;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw;
                }
            }
            return _tempPath;
        }

        // return a full path with filename combined which pointed to the temporary directory
        public static string GetTempPath(string filename) =>
            Path.Combine(GetTempPath(), filename);
    }

    public static class Utils
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static HttpClient HttpClient { get; } = new HttpClient();

        //public enum WindowsThemeMode { Dark, Light }

        //// Support on Windows 10 1903+
        //public static WindowsThemeMode GetWindows10SystemThemeSetting()
        //{
        //    WindowsThemeMode themeMode = WindowsThemeMode.Dark;
        //    try
        //    {
        //        RegistryKey reg_ThemesPersonalize = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
        //        if (reg_ThemesPersonalize.GetValue("SystemUsesLightTheme") != null)
        //        {
        //            if ((int)(reg_ThemesPersonalize.GetValue("SystemUsesLightTheme")) == 0) // 0:dark mode, 1:light mode
        //                themeMode = WindowsThemeMode.Dark;
        //            else
        //                themeMode = WindowsThemeMode.Light;
        //        }
        //        else
        //        {
        //            throw new Exception("Reg-Value SystemUsesLightTheme not found.");
        //        }
        //    }
        //    catch
        //    {

        //        logger.Debug(
        //                $"Cannot get Windows 10 system theme mode, return default value 0 (dark mode).");

        //    }
        //    return themeMode;
        //}

        //public static string FormatBandwidth(long n)
        //{
        //    var result = GetBandwidthScale(n);
        //    return $"{result.value:0.##}{result.unitName}";
        //}

        //public static string FormatBytes(long bytes)
        //{
        //    const long K = 1024L;
        //    const long M = K * 1024L;
        //    const long G = M * 1024L;
        //    const long T = G * 1024L;
        //    const long P = T * 1024L;
        //    const long E = P * 1024L;

        //    if (bytes >= P * 990)
        //        return (bytes / (double)E).ToString("F5") + "EiB";
        //    if (bytes >= T * 990)
        //        return (bytes / (double)P).ToString("F5") + "PiB";
        //    if (bytes >= G * 990)
        //        return (bytes / (double)T).ToString("F5") + "TiB";
        //    if (bytes >= M * 990)
        //    {
        //        return (bytes / (double)G).ToString("F4") + "GiB";
        //    }
        //    if (bytes >= M * 100)
        //    {
        //        return (bytes / (double)M).ToString("F1") + "MiB";
        //    }
        //    if (bytes >= M * 10)
        //    {
        //        return (bytes / (double)M).ToString("F2") + "MiB";
        //    }
        //    if (bytes >= K * 990)
        //    {
        //        return (bytes / (double)M).ToString("F3") + "MiB";
        //    }
        //    if (bytes > K * 2)
        //    {
        //        return (bytes / (double)K).ToString("F1") + "KiB";
        //    }
        //    return bytes.ToString() + "B";
        //}

        ///// <summary>
        ///// Return scaled bandwidth
        ///// </summary>
        ///// <param name="n">Raw bandwidth</param>
        ///// <returns>
        ///// The BandwidthScaleInfo struct
        ///// </returns>
        //public static BandwidthScaleInfo GetBandwidthScale(long n)
        //{
        //    long scale = 1;
        //    float f = n;
        //    string unit = "B";
        //    if (f > 1024)
        //    {
        //        f = f / 1024;
        //        scale <<= 10;
        //        unit = "KiB";
        //    }
        //    if (f > 1024)
        //    {
        //        f = f / 1024;
        //        scale <<= 10;
        //        unit = "MiB";
        //    }
        //    if (f > 1024)
        //    {
        //        f = f / 1024;
        //        scale <<= 10;
        //        unit = "GiB";
        //    }
        //    if (f > 1024)
        //    {
        //        f = f / 1024;
        //        scale <<= 10;
        //        unit = "TiB";
        //    }
        //    return new BandwidthScaleInfo(f, unit, scale);
        //}

        //public static RegistryKey OpenRegKey(string name, bool writable, RegistryHive hive = RegistryHive.CurrentUser)
        //{
        //    // we are building x86 binary for both x86 and x64, which will
        //    // cause problem when opening registry key
        //    // detect operating system instead of CPU
        //    if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
        //    try
        //    {
        //        RegistryKey userKey = RegistryKey.OpenBaseKey(hive,
        //                Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)
        //            .OpenSubKey(name, writable);
        //        return userKey;
        //    }
        //    catch (ArgumentException ae)
        //    {
        //        MessageBox.Show("OpenRegKey: " + ae.ToString());
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.LogUsefulException(e);
        //        return null;
        //    }
        //}

        //public static bool IsWinVistaOrHigher()
        //{
        //    return Environment.OSVersion.Version.Major > 5;
        //}

        //public static string ScanQRCodeFromScreen()
        //{
        //    foreach (Screen screen in Screen.AllScreens)
        //    {
        //        using (Bitmap fullImage = new Bitmap(screen.Bounds.Width,
        //                                        screen.Bounds.Height))
        //        {
        //            using (Graphics g = Graphics.FromImage(fullImage))
        //            {
        //                g.CopyFromScreen(screen.Bounds.X,
        //                                 screen.Bounds.Y,
        //                                 0, 0,
        //                                 fullImage.Size,
        //                                 CopyPixelOperation.SourceCopy);
        //            }
        //            int maxTry = 10;
        //            for (int i = 0; i < maxTry; i++)
        //            {
        //                int marginLeft = (int)((double)fullImage.Width * i / 2.5 / maxTry);
        //                int marginTop = (int)((double)fullImage.Height * i / 2.5 / maxTry);
        //                Rectangle cropRect = new Rectangle(marginLeft, marginTop, fullImage.Width - marginLeft * 2, fullImage.Height - marginTop * 2);
        //                Bitmap target = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);

        //                double imageScale = (double)screen.Bounds.Width / (double)cropRect.Width;
        //                using (Graphics g = Graphics.FromImage(target))
        //                {
        //                    g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
        //                                    cropRect,
        //                                    GraphicsUnit.Pixel);
        //                }
        //                var source = new BitmapLuminanceSource(target);
        //                var bitmap = new BinaryBitmap(new HybridBinarizer(source));
        //                QRCodeReader reader = new QRCodeReader();
        //                var result = reader.decode(bitmap);
        //                if (result != null)
        //                    return result.Text;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //public static void OpenInBrowser(string url)
        //{
        //    try
        //    {
        //        Process.Start(url);
        //    }
        //    catch
        //    {
        //        // hack because of this: https://github.com/dotnet/corefx/issues/10361
        //        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //        {
        //            Process.Start(new ProcessStartInfo(url)
        //            {
        //                UseShellExecute = true,
        //                Verb = "open"
        //            });
        //        }
        //        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //        {
        //            Process.Start("xdg-open", url);
        //        }
        //        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //        {
        //            Process.Start("open", url);
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}
    }
}
