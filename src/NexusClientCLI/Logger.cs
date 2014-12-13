/* This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. */

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Nexus.Client.CLI
{
    class Logger
    {
        public enum Level
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
            Debug = 4,
            Fine = 5,
            // Place finer levels here if needed
            All = 9
        }


        private const string s_logfileName = "NexusClientCLI.log";
        private static string s_logfilePath = null;
        private static Level s_maxVerbosity = Level.None;

        public static void SetVerbosity(Level verbostity)
        {
            s_maxVerbosity = verbostity;
        }

        public static void SetLogDestination(string path)
        {
            s_logfilePath = Path.Combine(path, s_logfileName);
        }

        public static void Error(String msg)
        {
            if (IsLevelLoggable(Level.Error))
                WriteMessage(Level.Error, msg);
        }

        public static void Error(String fmt, Object arg)
        {
            if (IsLevelLoggable(Level.Error))
                WriteMessage(Level.Error, String.Format(fmt, arg));
        }

        public static void Warning(String msg)
        {
            if (IsLevelLoggable(Level.Warning))
                WriteMessage(Level.Warning, msg);
        }

        public static void Warning(String fmt, Object arg)
        {
            if (IsLevelLoggable(Level.Warning))
                WriteMessage(Level.Warning, String.Format(fmt, arg));
        }

        public static void Info(String msg)
        {
            if (IsLevelLoggable(Level.Info))
                WriteMessage(Level.Info, msg);
        }

        public static void Info(String fmt, Object arg)
        {
            if (IsLevelLoggable(Level.Info))
                WriteMessage(Level.Info, String.Format(fmt, arg));
        }

        public static void Debug(String msg)
        {
            if (IsLevelLoggable(Level.Debug))
                WriteMessage(Level.Debug, msg);
        }

        public static void Debug(String fmt, Object arg)
        {
            if (IsLevelLoggable(Level.Debug))
                WriteMessage(Level.Debug, String.Format(fmt, arg));
        }

        public static void Fine(String msg)
        {
            if (IsLevelLoggable(Level.Fine))
                WriteMessage(Level.Fine, msg);
        }

        public static void Fine(String fmt, Object arg)
        {
            if (IsLevelLoggable(Level.Fine))
                WriteMessage(Level.Fine, String.Format(fmt, arg));
        }

        public static bool IsLevelLoggable(Level level)
        {
            return level <= s_maxVerbosity;
        }

        private static void WriteMessage(Level level, string msg)
        {
            try
            {
                string output = string.Format("[{0}] {1}{2}", level, msg, Environment.NewLine);
                if (s_logfilePath != null) {
                    File.AppendAllText(s_logfilePath, output);
                }
                else
                {
                    Console.WriteLine(output);
                }

            }
            catch (Exception)
            {
                // Ignore exception
            }
        }

    }
}
