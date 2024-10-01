using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test
{
    [Table("A")]
    public class A
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
