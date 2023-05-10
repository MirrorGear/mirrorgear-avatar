using System;

namespace MirrorGearSDK
{
    namespace Utils
    {
        public static class Logger
        {
            public delegate void LogHandler(int level, string file_name, string function_name, int line_number, string message);
            public static readonly LogHandler DefaultLogHandler = LogMessage;

            public static void SetLogger(LogHandler logHandler)
            {
                NativeMethods.SetLogger(logHandler);
            }

            [AOT.MonoPInvokeCallback(typeof(LogHandler))]
            private static void LogMessage(int level, string file_name, string function_name, int line_number, string message)
            {
                Priority priority = (Priority)level;
                switch (priority)
                {
                    case Priority.FATAL:
                        {
                            UnityEngine.Debug.LogError($"[{NativeMethods.Import} FATAL {file_name}:{function_name}:{line_number}] {message}");
                            return;
                        }
                    case Priority.ERROR:
                        {
                            UnityEngine.Debug.LogError($"[{NativeMethods.Import} ERROR {file_name}:{function_name}:{line_number}] {message}");
                            return;
                        }
                    case Priority.WARN:
                        {
                            UnityEngine.Debug.LogWarning($"[{NativeMethods.Import} WARNING {file_name}:{function_name}:{line_number}] {message}");
                            return;
                        }
                    case Priority.INFO:
                        {
                            UnityEngine.Debug.Log($"[{NativeMethods.Import} INFO {file_name}:{function_name}:{line_number}] {message}");
                            return;
                        }
                    case Priority.DEBUG:
                        {
                            UnityEngine.Debug.Log($"[{NativeMethods.Import} DEBUG {file_name}:{function_name}:{line_number}] {message}");
                            return;
                        }
                    default:
                        {
                            UnityEngine.Debug.Log($"[{NativeMethods.Import} TRACE {file_name}:{function_name}:{line_number}] {message}");
                            return;
                        }
                }
            }

            [Serializable]
            private enum Priority
            {
                FATAL = 0,
                ERROR = 1,
                WARN = 2,
                INFO = 3,
                DEBUG = 4,
                TRACE = 5,
            }
        }
    }
}


