using System;

namespace EntityFrameworkCore.AuditR
{
    public class AfterSavingChangesEventArgs : EventArgs
    {
        public AfterSavingChangesEventArgs(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
}
