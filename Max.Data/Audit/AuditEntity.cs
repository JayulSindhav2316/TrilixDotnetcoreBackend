using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Max.Data.Audit
{
    public class AuditEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public EntityState EntityState { get; set; }
        public string ByUser { get; set; }        

        public AuditMetaDataEntity AuditMetaData { get; set; }
    }
}
