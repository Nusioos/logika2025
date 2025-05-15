using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace logika2025.Models
{
    public class Testmain
    {
        [Key]
        public int Id { get; set; }

        public int Odp { get; set; }
        public TimeSpan CzasTrwania { get; set; }

        public string UserName { get; set; }
        [Required]
        public string UserId { get; set; }

       
        [ForeignKey("UserId")]
        public User User { get; set; }



    }
}
