namespace EntityFrameworkCore.AuditR
{
    public class AuditRConfiguration
    {
        public AuditRConfiguration(string schema = "dbo", string auditEntryTableName = "AuditEntry",
            string auditEntryPropertyTableName = "AuditEntryProperty", bool addChangesetWhenInsert = false)
        {
            Schema = schema;
            AuditEntryTableName = auditEntryTableName;
            AuditEntryPropertyTableName = auditEntryPropertyTableName;
            AddChangesetWhenInsert = addChangesetWhenInsert;
        }

        public string Schema { get; private set; }
        public string AuditEntryTableName { get; private set; }
        public string AuditEntryPropertyTableName { get; private set; }
        public bool AddChangesetWhenInsert { get; private set; }
    }
}
