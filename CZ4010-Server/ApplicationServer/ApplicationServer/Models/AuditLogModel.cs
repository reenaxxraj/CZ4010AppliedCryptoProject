using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApplicationServer.Models
{
    public class AuditLogModel
    {
        public AuditLogModel(){}
        public AuditLogModel(string log, LogType type, string caller, DateTime timestamp, string url)
        {
            Log = log;
            Type = type;
            Caller = caller;
            Timestamp = timestamp;
            URL = url;
        }
        
        public AuditLogModel(string log, LogType type, string caller, string url)
        {
            Log = log;
            Type = type;
            Caller = caller;
            Timestamp = DateTime.Now;
            URL = url;
        }
        
        [Key]
        public Guid Id { get; set; }

        public string Log { get; set; }
        public LogType Type { get; set; }
        public string Caller { get; set; }
        public DateTime Timestamp { get; set; }
        public string URL { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LogType
    {
        GetFile, UploadFile, UpdateFile, DeleteFile, ShareFile, UnshareFile
    }
}