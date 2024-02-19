using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Max.Data.Audit
{
    internal class AuditEntry
    {
        private string _readablePrimaryKey;
        public string ReadablePrimaryKey
        {
            get
            {
                if (string.IsNullOrEmpty(_readablePrimaryKey))
                    _readablePrimaryKey = Entry.ToReadablePrimaryKey();
                return _readablePrimaryKey;
            }
            set
            {
                _readablePrimaryKey = value;
            }
        }
        public Guid HashReferenceId { get; set; }
        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public string SchemaName { get; set; }
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public EntityState EntityState { get; set; }
        public string ByUser { get; set; }
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public AuditEntry(EntityEntry entry, string userName, IModel model)
        {
            Entry = entry;
            ReadablePrimaryKey = Entry.ToReadablePrimaryKey();
            HashReferenceId = ReadablePrimaryKey.ToGuidHash();
            TableName = entry.Metadata.GetTableName();            
            DisplayName = entry.Metadata.DisplayName();
            EntityState = entry.State;
            ByUser = userName;                

            foreach (PropertyEntry property in entry.Properties)
            {
                if (property.IsAuditable())
                {
                    if (property.IsTemporary)
                    {
                        TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:

                            OldValues[propertyName] = property.OriginalValue;
                            NewValues[propertyName] = property.CurrentValue;
                            break;
                    }
                }
            }
        }

        public void Update()
        {
            // Get the final value of the temporary properties
            foreach (var prop in TemporaryProperties)
            {
                NewValues[prop.Metadata.Name] = prop.CurrentValue;
            }

            if (TemporaryProperties != default && TemporaryProperties.Any(x => x.Metadata.IsKey()))
            {
                ReadablePrimaryKey = Entry.ToReadablePrimaryKey();
                HashReferenceId = ReadablePrimaryKey.ToGuidHash();
            }
        }

        public AuditMetaDataEntity ToAuditMetaDataEntity()
        {
            AuditMetaDataEntity auditMetaData = new AuditMetaDataEntity
            {
                DisplayName = DisplayName,
                Table = TableName,
                SchemaTable = $"{(!string.IsNullOrEmpty(SchemaName) ? SchemaName + "." : "")}{TableName}",
                ReadablePrimaryKey = ReadablePrimaryKey,
                HashPrimaryKey = HashReferenceId
            };

            return auditMetaData;
        }

        public AuditEntity ToAuditEntity(AuditMetaDataEntity auditMetaData)
        {
            AuditEntity audit = new AuditEntity
            {
                OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
                EntityState = EntityState,
                DateTimeOffset = DateTimeOffset.UtcNow,
                ByUser = ByUser,
                AuditMetaData = auditMetaData
            };

            return audit;
        }

        public AuditEntity ToAuditEntity()
        {
            AuditEntity audit = new AuditEntity
            {
                OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
                EntityState = EntityState,
                DateTimeOffset = DateTimeOffset.UtcNow,
                ByUser = ByUser
            };

            return audit;
        }
    }
}
