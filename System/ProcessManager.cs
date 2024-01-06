using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexumOS.System
{
    public class ProcessManager
    {
        static List<Process> _processes = new List<Process>();
        static List<Process> _processesToCreate = new List<Process>();
        static List<Process> _processesToDelete = new List<Process>();

        public static void Run()
        {
            // delete all deleted processes
            foreach (Process process in _processesToDelete)
            {
                _processes.Remove(process);
            }
            _processesToDelete.Clear();

            // create all new processes
            foreach (Process process in _processesToCreate)
            {
                _processes.Add(process);
            }
            _processesToCreate.Clear();

            if (_processes.Count == 0) return;

            // true Execute
            foreach (Process process in _processes)
            {
                if (process.priority != Process.Priority.System) continue;
                process.Execute();
            }

            foreach (Process process in _processes)
            {
                if (process.priority != Process.Priority.High) continue;
                process.Execute();
            }

            foreach (Process process in _processes)
            {
                if (process.priority != Process.Priority.Mid) continue;
                process.Execute();
            }

            foreach (Process process in _processes)
            {
                if (process.priority != Process.Priority.Low) continue;
                process.Execute();
            }

            Console.SetCursorPosition(0, 10);
            Console.WriteLine("Number of processes: " + _processes.Count);
        }

        public static void AddProcess(Process process)
        { 
            _processesToCreate.Add(process);
        }

        public static void RemoveProcess(Process process)
        { 
            _processesToDelete.Add(process);
        }
    }
}
