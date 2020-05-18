using Common;
using Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;

namespace CpuUsageApplication
{
    class Program
    {
        private static readonly FileInfo _applicationDataFile = null;
        private static readonly System.Timers.Timer _latestProcessTimer = null;
        private static readonly System.Timers.Timer _profilingProcessTimer = null;
        private static readonly RunningProcessesHelper _processesHelper = null;
        private static readonly object _objLock = null;
        static Program()
        {
            _objLock = new object();
            DirectoryInfo dirInfo = new DirectoryInfo(Assembly.GetExecutingAssembly().Location);
            _applicationDataFile = new FileInfo(Path.Combine(dirInfo.Parent.FullName, "Data", Guid.NewGuid().ToString() + ".txt"));
            _processesHelper = new RunningProcessesHelper();
            _latestProcessTimer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = TimeSpan.FromMinutes(5).TotalMilliseconds
            };
            _latestProcessTimer.Elapsed += CollectLatestRunningProcess;

            _profilingProcessTimer = new System.Timers.Timer
            {
                Enabled = false,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };
            _profilingProcessTimer.Elapsed += ProfilingRunningProcess;
        }

        private static void ProfilingRunningProcess(object sender, ElapsedEventArgs e)
        {
            lock (_objLock)
            {
                try
                {
                    ProcessSnap processSnap1 = ProcessSnapHandler.GetCurrentProcessSnap();
                    _profilingProcessTimer.Enabled = false;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DateTime.Now} : Profiling Data Collection.....");
                    SamplesInfo samples = _processesHelper.ProfilingRunningProcesses();
                    if (samples != null)
                    {
                        ConsoleWriteText(samples.ConsoleContent());
                        WriteAllText(samples.ConvertToJson());
                        SelfProfiling(processSnap1, samples.Processes);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Profiling Processes Error : {ex}");
                }
                finally
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    _profilingProcessTimer.Enabled = true;
                }
            }
        }

        private static void CollectLatestRunningProcess(object sender, ElapsedEventArgs e)
        {
            lock (_objLock)
            {
                try
                {
                    _profilingProcessTimer.Enabled = false;
                    _latestProcessTimer.Enabled = false;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{DateTime.Now} : Collect latest Data.....!!");
                    ProcessSnap processSnap1 = ProcessSnapHandler.GetCurrentProcessSnap();
                    SamplesInfo samples = _processesHelper.CollectCurrentRunningProcesses();
                    if (samples != null)
                    {
                        ConsoleWriteText(samples.ConsoleContent());
                        WriteAllText(samples.ConvertToJson());
                        SelfProfiling(processSnap1, samples.Processes);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Collection Latest Processes Error : {ex}");
                }
                finally
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    _latestProcessTimer.Enabled = true;
                    _profilingProcessTimer.Enabled = true;
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now} : Collecting sample..........");
                ProcessSnap processSnap1 = ProcessSnapHandler.GetCurrentProcessSnap();
                _latestProcessTimer.Enabled = false;
                Console.ForegroundColor = ConsoleColor.Yellow;
                SamplesInfo samples = _processesHelper.CollectCurrentRunningProcesses();
                if (samples != null)
                {
                    ConsoleWriteText(samples.ConsoleContent());
                    WriteAllText(samples.ConvertToJson());
                    SelfProfiling(processSnap1, samples.Processes);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Start up Error : {ex}");
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
                _latestProcessTimer.Enabled = true;
                _profilingProcessTimer.Enabled = true;
                //Class1.Run();
            }
        }
        private static void ConsoleWriteText(string content)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(content);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private static void WriteAllText(string content)
        {
            if (!_applicationDataFile.Directory.Exists)
                _applicationDataFile.Directory.Create();
            using FileStream fStream = _applicationDataFile.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter contentStream = new StreamWriter(fStream);
            contentStream.WriteLine(content);
        }
        private static void SelfProfiling(ProcessSnap pSnap1, List<ProcessSnap> totalProcess)
        {
            ProcessSnap pSnap2 = ProcessSnapHandler.GetCurrentProcessSnap();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now}");
            Console.WriteLine($"{pSnap1.Name} total cpu% calculated : {ProcessSnapHandler.CalculateCpuPer(pSnap1, pSnap2)}");
            Console.WriteLine($"Total execution time : {pSnap2.RecordDateTime - pSnap1.RecordDateTime}");
            if (totalProcess != null)
                Console.WriteLine($"Total Process Computed : {totalProcess.Count}");
        }
    }
}
