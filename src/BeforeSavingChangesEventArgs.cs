using System;

namespace EntityFrameworkCore.AuditR
{
    public class BeforeSavingChangesEventArgs : EventArgs
    {
        public BeforeSavingChangesEventArgs(Guid correlationId)
        {
            CorrelationId = correlationId;
            Cancel = false;
        }

        public bool Cancel { get; set; }
        public Guid CorrelationId { get; }
    }
}
