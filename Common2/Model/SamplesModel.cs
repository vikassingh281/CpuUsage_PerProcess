using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model
{
    public class SamplesInfo
    {
        private static int _sampleID = 0;
        private readonly string _consoleContent = null;

        [JsonProperty("operationId")]
        public int SampleId { get; private set; }
        [JsonProperty("type")]
        public string CollectionType { get; private set; }
        [JsonProperty("samples")]
        public List<ProcessSnap> Processes { get; private set; }

        public SamplesInfo(List<ProcessSnap> processes) : this(processes, string.Empty, "Data Collection")
        {
        }
        public SamplesInfo(List<ProcessSnap> processes, string consoleContent, string collectionType)
        {
            SampleId = LastSampleId();
            Processes = processes;
            _consoleContent = consoleContent;
            CollectionType = collectionType;
        }

        public string ConvertToJson()
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            string jString = JsonConvert.SerializeObject(this, Formatting.None, serializerSettings);
            if (string.IsNullOrEmpty(jString))
                jString = string.Empty;
            return jString;
        }
        public int LastSampleId()
        {
            return ++_sampleID;
        }
        public string ConsoleContent()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            return _consoleContent ?? string.Empty;
        }
    }
}
