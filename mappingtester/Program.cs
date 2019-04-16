using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mappingtester
{
    static class Program
    {
        private static Thread testThread;
        private static Tester test;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Process.GetCurrentProcess().PriorityClass =
                    System.Diagnostics.ProcessPriorityClass.High;
            }
            catch { } // Ignore problems raising the priority.

            // Force Normal IO Priority
            IntPtr ioPrio = new IntPtr(2);
            Util.NtSetInformationProcess(Process.GetCurrentProcess().Handle,
                Util.PROCESS_INFORMATION_CLASS.ProcessIoPriority, ref ioPrio, 4);

            // Force Normal Page Priority
            IntPtr pagePrio = new IntPtr(5);
            Util.NtSetInformationProcess(Process.GetCurrentProcess().Handle,
                Util.PROCESS_INFORMATION_CLASS.ProcessPagePriority, ref pagePrio, 4);

            testThread = new Thread(() =>
            {
                test = new Tester();
                test.Start();
            });

            testThread.IsBackground = true;
            testThread.Start();
            testThread.Join();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            test.Stop();
        }
    }
}
