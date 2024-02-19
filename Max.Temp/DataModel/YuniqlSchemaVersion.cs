using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class YuniqlSchemaVersion
    {
        public int SequenceId { get; set; }
        public string Version { get; set; }
        public DateTime AppliedOnUtc { get; set; }
        public string AppliedByUser { get; set; }
        public string AppliedByTool { get; set; }
        public string AppliedByToolVersion { get; set; }
        public string Status { get; set; }
        public int DurationMs { get; set; }
        public string Checksum { get; set; }
        public string FailedScriptPath { get; set; }
        public string FailedScriptError { get; set; }
        public string AdditionalArtifacts { get; set; }
    }
}
