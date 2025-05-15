using System.ComponentModel.DataAnnotations;

namespace logika2025.Models
{
    public class Pytania
    {
        public int Id { get; set; }
        public string Tresc { get; set; }
        public string? OdpowiedzA { get; set; }
        public string? OdpowiedzB { get; set; }
        public string? OdpowiedzC { get; set; }
        public string? OdpowiedzD { get; set; }
        public string? OdpowiedzE { get; set; }

        public string PoprawnaOdpowiedz { get; set; }
        public string TypOdpowiedzi { get; set; } 


        public string? Obraz { get; set; }  
    }

}
