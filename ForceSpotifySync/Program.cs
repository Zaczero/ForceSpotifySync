using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ForceSpotifySync
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            OutLine();
            OutLine();

            Out("\tForceSpotifySync ", ConsoleColor.Cyan);
            OutLine("by Zaczero", ConsoleColor.Blue);

            OutLine("\t( https://github.com/Zaczero/ForceSpotifySync )", ConsoleColor.Gray);
            OutLine();
            
            Out("\tKilling Spotify .. ", ConsoleColor.Yellow);
            var spotifyPaths = KillSpotifyProcesses().ToArray();
            OutLine("OK", ConsoleColor.DarkGreen);

            foreach (var spotifyDir in GetDirsToProcess())
            {
                OutLine($"\tProcessing {spotifyDir} ..", ConsoleColor.Yellow);

                var usersDir = Path.Combine(spotifyDir, "Users");
                var usersDi = new DirectoryInfo(usersDir);

                foreach (var di in usersDi.GetDirectories("*-user"))
                {
                    OutLine($"\t--> Found User: {di.Name}", ConsoleColor.DarkYellow);

                    var userDir = di.FullName;
                    var userDi = new DirectoryInfo(userDir);

                    foreach (var fi in userDi.GetFiles("collection-*"))
                    {
                        Out($"\t--> Deleting: {fi.Name} .. ", ConsoleColor.DarkYellow);

                        try
                        {
                            fi.Delete();
                            OutLine("OK", ConsoleColor.DarkGreen);
                        }
                        catch (Exception ex)
                        {
                            OutLine($"FAIL ({ex.Message})", ConsoleColor.DarkRed);
                        }
                    }
                }
            }

            if (spotifyPaths.Length > 0)
            {
                Out("\tStarting Spotify .. ", ConsoleColor.Yellow);
                Process.Start(spotifyPaths[0]);
                OutLine("OK", ConsoleColor.DarkGreen);
            }
            
            OutLine();
            Out("\tPress any key to exit .. ", ConsoleColor.Magenta);
            Console.ReadKey();
        }

        private static void Out(object o = null, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Write(o);
        }

        private static void OutLine(object o = null, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.WriteLine(o);
        }

        private static IEnumerable<string> KillSpotifyProcesses()
        {
            foreach (var process in Process.GetProcessesByName("Spotify"))
            {
                yield return process.MainModule.FileName;

                if (process.CloseMainWindow())
                {
                    Thread.Sleep(150);

                    if (process.HasExited)
                    {
                        continue;
                    }
                }

                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private static List<string> GetDirsToProcess()
        {
            var dirsToProcess = new List<string>(2);

            var spotifyDir = GetSpotifyDirectory();
            if (spotifyDir != null)
            {
                dirsToProcess.Add(spotifyDir);
            }
            
            var spotifyWSDir = GetSpotifyWSDirectory();
            if (spotifyWSDir != null)
            {
                dirsToProcess.Add(spotifyWSDir);
            }

            return dirsToProcess;
        }

        private static string GetSpotifyDirectory()
        {
            var appData = Environment.GetEnvironmentVariable("LocalAppData");
            var baseDir = Path.Combine(appData, "Spotify");
            if (!Directory.Exists(baseDir))
            {
                return null;
            }

            return baseDir;
        }

        private static string GetSpotifyWSDirectory()
        {
            var appData = Environment.GetEnvironmentVariable("LocalAppData");
            var packagesDir = Path.Combine(appData, "Packages");
            if (!Directory.Exists(packagesDir))
            {
                return null;
            }

            var pdi = new DirectoryInfo(packagesDir);
            var packagesSearch = pdi.GetDirectories("SpotifyAB.SpotifyMusic_*");
            if (packagesSearch.Length == 0)
            {
                return null;
            }

            var packageDir = packagesSearch[0].FullName;
            var baseDir = Path.Combine(packageDir, "LocalState", "Spotify");
            if (!Directory.Exists(baseDir))
            {
                return null;
            }

            return baseDir;
        }
    }
}
