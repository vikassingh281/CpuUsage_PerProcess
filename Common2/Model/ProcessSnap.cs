using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model
{
    public class ProcessSnap
    {
        [JsonProperty("id")]
        public int Id { get; private set; } = 0;
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("proTime")]
        public TimeSpan TotalProcessorTime { get; private set; }
        [JsonProperty("type")]
        public string ProcessType { get; private set; }
        [JsonProperty("recordTime")]
        public DateTime RecordDateTime { get; private set; } = DateTime.Now;
        [JsonProperty("cpuPer")]
        public double CpuPer { get; set; }

        public ProcessSnap(int pId, string pName, TimeSpan processorTime, string pType)
        {
            Id = pId;
            Name = pName;
            TotalProcessorTime = processorTime;
            ProcessType = pType;
        }
    }
}
