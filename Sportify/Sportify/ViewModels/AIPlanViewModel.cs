namespace Sportify.Models
{
    public class AIPlanViewModel
    {
        // Kullanıcıdan alınacak veriler
        public string BodyType { get; set; } // Ektomorf, Mezomorf, Endomorf
        public int Height { get; set; } // cm
        public int Weight { get; set; } // kg
        public string Gender { get; set; } // Kadın/Erkek

        // AI'dan dönecek cevap
        public string AIResponse { get; set; }

        // Hata veya Yüklenme durumu için
        public bool IsLoading { get; set; } = false;
    }
}