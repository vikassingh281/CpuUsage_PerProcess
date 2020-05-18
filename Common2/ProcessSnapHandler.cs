using Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Common
{
    public sealed class ProcessSnapHandler
    {
        private readonly Process _process = null;
        private readonly bool _enableConsole = false;
        public string ConsoleContent { get; private set; } = string.Empty;
        public ProcessSnapHandler(Process process) : this(process, false)
        {
        }
        public ProcessSnapHandler(Process process, bool enableConsole)
        {
            _process = process;
            _enableConsole = enableConsole;
        }

        public static ProcessSnap GetCurrentProcessSnap()
        {
            Process process = Process.GetCurrentProcess();
            return new Model.ProcessSnap(process.Id, process.ProcessName, process.TotalProcessorTime, string.Empty);
        }
        public static double CalculateCpuPer(ProcessSnap pSnapShot1, ProcessSnap pSnapShot2)
        {
            double cpuUsage = Math.Round((pSnapShot2.TotalProcessorTime.Subtract(pSnapShot1.TotalProcessorTime).TotalMilliseconds) * 100 /
                                          (Environment.ProcessorCount * pSnapShot2.RecordDateTime.Subtract(pSnapShot1.RecordDateTime).TotalMilliseconds), 2);
            return ((cpuUsage < 0)
                   || (double.IsNaN(cpuUsage))
                   || (double.IsInfinity(cpuUsage))
                   || (double.IsNegativeInfinity(cpuUsage))
                   || (double.IsPositiveInfinity(cpuUsage))
                   ) ? 0 : cpuUsage;
        }

        public ProcessSnap EvaluateCurrentProcess()
        {
            ProcessSnap processModel = null;
            int id = _process.Id;
            if (id > 4)
            {
                processModel = new ProcessSnap(id,
                                              _process.ProcessName,
                                              _process.TotalProcessorTime,
                                              GetProcessType(_process));
                ConsoleContent = ConsoleWriteLine(processModel);
            }
            return processModel;
        }

        public ProcessSnap ProfileCurrentProcess(ProcessSnap processSnap1)
        {
            ProcessSnap processSnap2 = null;
            if (!_process.HasExited)
            {
                int id = processSnap1.Id;
                if (id > 4)
                {
                    processSnap2 = new ProcessSnap(id,
                                                  processSnap1.Name,
                                                  _process.TotalProcessorTime,
                                                  processSnap1.ProcessType);
                    double cpuUsage = CalculateCpuPer(processSnap1, processSnap2);
                    processSnap2.CpuPer = cpuUsage;
                    ConsoleContent = ConsoleWriteLine(processSnap2);
                }
            }
            return processSnap2;
        }

        private string GetProcessType(Process process)
        {
            if (process.SessionId == 0 || process.MainWindowHandle == IntPtr.Zero)
                return "Background Process";
            else
                return "App Process";
        }
        private string ConsoleWriteLine(ProcessSnap p)
        {
            if (_enableConsole)
            {
                return string.Format("| {0,-10} | {1,-45} | {2,-10} | {3,-20} | {4,-20} |", p.Id, p.Name, p.CpuPer, p.TotalProcessorTime, p.ProcessType);
            }
            else
                return string.Empty;
        }
    }
}
