using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.AuditR.Test
{
    public class FakeDbModel: IAuditable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double TestDouble { get; set; }
        public float TestFloat { get; set; }
        public byte TestByte { get; set; }
    }
}
