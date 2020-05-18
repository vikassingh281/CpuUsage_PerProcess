using Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common
{
    public sealed class RunningProcessesHelper
    {
        private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(100);
        private List<Model.ProcessSnap> _runningProcessesSnap = null;
        private readonly Process[] _runningProcesses = null;
        public RunningProcessesHelper()
        {
            //_runningProcesses = new Process[] { Process.GetProcessById(7136) };
            _runningProcesses = Process.GetProcesses()?.OrderBy(item => item.ProcessName)?.ToArray();
        }

        public SamplesInfo CollectCurrentRunningProcesses()
        {
            StringBuilder consoleContent = new StringBuilder();
            consoleContent.AppendLine(string.Format("| {0,-10} | {1,-45} | {2,-10} | {3,-20} | {4,-20} |", "Id", "Name", "CpuPer", "TotalProcessorTime", "Type"));
            _runningProcessesSnap = new List<Model.ProcessSnap>();
            foreach (Process process in _runningProcesses ?? Enumerable.Empty<Process>())
            {
                try
                {
                    ProcessSnapHandler processSnapHandler = new ProcessSnapHandler(process, enableConsole: true);
                    Model.ProcessSnap processSanp1 = processSnapHandler.EvaluateCurrentProcess();
                    if (processSanp1 != null)
                    {
                        consoleContent.AppendLine(processSnapHandler.ConsoleContent);
                        _runningProcessesSnap.Add(processSanp1);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now} : CollectCurrentRunningProcesses => {ex}");
                }
                finally
                {

                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(_delay);
                }
            }
            return new SamplesInfo(_runningProcessesSnap, consoleContent?.ToString(), "Data Collection");
        }
        public SamplesInfo ProfilingRunningProcesses()
        {
            List<Model.ProcessSnap> _profilingProcesses = new List<ProcessSnap>();
            StringBuilder consoleContent = new StringBuilder();
            try
            {
                consoleContent.AppendLine(string.Format("| {0,-10} | {1,-45} | {2,-10} | {3,-20} | {4,-20} |", "Id", "Name", "CpuPer", "TotalProcessorTime", "Type"));
                foreach (var processSnap1 in _runningProcessesSnap ?? Enumerable.Empty<ProcessSnap>())
                {
                    try
                    {
                        ProcessSnapHandler processSnapHandler = new ProcessSnapHandler(Process.GetProcessById(processSnap1.Id), enableConsole: true);
                        ProcessSnap processSnap2 = processSnapHandler.ProfileCurrentProcess(processSnap1);
                        if (processSnapHandler != null)
                        {
                            consoleContent.AppendLine(processSnapHandler.ConsoleContent);
                            _profilingProcesses.Add(processSnap2);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now} : ProfilingRunningProcesses => {ex}");
                    }
                    finally
                    {
                        Thread.Sleep(Convert.ToInt32(_delay.TotalMilliseconds * 3));
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (_profilingProcesses != null && _profilingProcesses.Count > 0)
                    _runningProcessesSnap = _profilingProcesses;
            }
            return new SamplesInfo(_runningProcessesSnap, consoleContent?.ToString(), "Profiling Collection");
        }
    }
}
