namespace EntityFrameworkCore.AuditR
{
    public class AuditRConfiguration
    {
        public AuditRConfiguration(string schema = "dbo", string auditEntryTableName = "AuditEntry",
            string auditEntryPropertyTableName = "AuditEntryProperty", KeyDefaultValue keyDefaultValue = KeyDefaultValue.NewSequentialId)
        {
            KeyDefaultValue = keyDefaultValue;
            Schema = schema;
            AuditEntryTableName = auditEntryTableName;
            AuditEntryPropertyTableName = auditEntryPropertyTableName;
        }

        public KeyDefaultValue KeyDefaultValue { get; private set; }
        public string Schema { get; private set; }
        public string AuditEntryTableName { get; private set; }
        public string AuditEntryPropertyTableName { get; private set; }
    }
}
