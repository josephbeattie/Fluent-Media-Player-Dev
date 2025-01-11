using System;
using System.Diagnostics;

namespace Rise.Common.Extensions
{
    /// <summary>
    /// Extensions that help improve the debugging experience.
    /// </summary>
    public static class DiagnosticsExtensions
    {
        /// <summary>
        /// Whether the deployed build is a debug one.
        /// </summary>
#if DEBUG
        public static readonly bool IsDebugBuild = true;
#else
        public static readonly bool IsDebugBuild = false;
#endif

        /// <summary>
        /// Logs an exception's type, HRESULT, source, message, and
        /// stack trace to output.
        /// </summary>
        public static void WriteToOutput(this Exception ex)
        {
            if (Debugger.IsAttached && IsDebugBuild)
            {
                Debug.WriteLine("--- Exception ---");
                Debug.Write("Exception type: ");
                Debug.WriteLine(ex.GetType());

                Debug.Write("HRESULT: ");
                Debug.WriteLine(ex.HResult);
                Debug.Write("Source: ");
                Debug.WriteLine(ex.Source);

                Debug.WriteLine("");

                Debug.WriteLine("Message:");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("");
                Debug.WriteLine("Stack trace:");
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine("-----");
            }
        }

        /// <summary>
        /// Writes the amount of milliseconds that a stopwatch
        /// has measured.
        /// </summary>
        public static void WriteElapsedMilliseconds(this Stopwatch watch)
        {
            Debug.WriteLine("{0}ms", watch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Writes the amount of milliseconds that a stopwatch
        /// has measured.
        /// </summary>
        /// <param name="message">A custom message logged before
        /// the elapsed time.</param>
        public static void WriteElapsedMilliseconds(this Stopwatch watch, string message)
        {
            Debug.WriteLine("{0}: {1}ms", message, watch.ElapsedMilliseconds);
        }
    }
}
