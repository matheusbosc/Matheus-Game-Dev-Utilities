using System;
using System.IO;
using UnityEngine;

namespace com.matheusbosc.utilities
{
    
    // To write a log, use:
    //
    // LogManager.Info(message);
    // LogManager.Warn(message);
    // LogManager.Error(message);
    // LogManager.Debug(message);     Only appears if printDebug is true
    
    public class LogManager : MonoBehaviour
    {
        public static LogManager instance;

        private static string curPath = "";

        public static bool printDebug = true;

        public static bool enableLogger = true;

        private void Start()
        {
            instance = this;
            
            if (!enableLogger) return;
            
            // Starting Message
            
            // Use the following website to generate big ascii letters: https://patorjk.com/software/taag/#p=display&f=Big&t=Type+Something+&x=none&v=4&h=4&w=80&we=false

            string entry = "  _______ _ _   _      " +
                           "\n |__   __(_) | | |     " +
                           "\n    | |   _| |_| | ___ " +
                           "\n    | |  | | __| |/ _ \\" +
                           "\n    | |  | | |_| |  __/" +
                           "\n    |_|  |_|\\__|_|\\___|" +
                           "\n                       " +
                           "\n                       " + 
                           "\n --- Log Starts at " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                           "\n --- GAME NAME - By AUTHOR" +
                           "\n ----------------------------------------------------------- " +
                           "\n";
            
            string path = Path.Combine(Application.persistentDataPath, "logs", "log-" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".log");

            print(path);

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            File.WriteAllText(path, entry + Environment.NewLine);

            curPath = path;
        }

        public static void Log(string level, string message)
        {
            if (!enableLogger) return;

            if (level == "DEBUG" && printDebug == false) return;
            
            string entry = string.Format(
                "[{0}] [{1}] {2}",
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                level.ToUpper(),
                message
            );

            // Write to file
            System.IO.File.AppendAllText(curPath, entry + Environment.NewLine);
            
            Console.WriteLine("[" + level + "] " + entry);
        }

        public static void Info(string msg) => Log("INFO", msg);
        public static void Warn(string msg) => Log("WARN", msg);
        public static void Error(string msg) => Log("ERROR", msg);
        public static void Debug(string msg) => Log("DEBUG", msg);
    }
}