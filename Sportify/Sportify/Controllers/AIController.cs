using Microsoft.AspNetCore.Mvc;
using Sportify.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sportify.Controllers
{
    public class AIController : Controller
    {
        // Google Gemini API Key'ini buraya yapıştır
        private const string ApiKey = "AIzaSyDHH2btOrCOSJfTKkZFbmJJtfvI_tSFhMs";
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AIPlanViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(AIPlanViewModel model)
        {
            if (string.IsNullOrEmpty(model.BodyType) || model.Height == 0 || model.Weight == 0)
            {
                ModelState.AddModelError("", "Lütfen tüm alanları doldurun.");
                return View(model);
            }

            try
            {
                // 1. Promptu (İstemi) Hazırla
                string prompt = @$"Sen dünyanın en iyi spor ve beslenme koçusun.
                                   Kullanıcı Bilgileri:
                                   - Cinsiyet: {model.Gender}
                                   - Vücut Tipi: {model.BodyType}
                                   - Boy: {model.Height} cm
                                   - Kilo: {model.Weight} kg

                                   Bu kişi için aşağıdakileri içeren markdown formatında profesyonel bir rapor oluştur:
                                   1. Vücut Kitle İndeksi (VKİ) analizi ve yorumu.
                                   2. Vücut tipine özel haftalık antrenman programı (Tablo formatında olsun).
                                   3. Vücut tipine ve hedefine uygun günlük örnek diyet listesi (Kalori hesaplı).
                                   4. Motivasyon notu.
                                   
                                   Lütfen başlıkları belirgin yap ve emojiler kullan.";

                // 2. HTTP İsteği Gönder
                using (var client = new HttpClient())
                {
                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = prompt } } }
                        }
                    };

                    var jsonContent = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await client.PostAsync($"{ApiUrl}?key={ApiKey}", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<GeminiResponse>(responseString);

                        // Cevabı modele ekle
                        model.AIResponse = result?.Candidates?[0]?.Content?.Parts?[0]?.Text;
                    }
                    else
                    {
                        model.AIResponse = "Üzgünüm, şu an yapay zeka servisine ulaşılamıyor. Lütfen API Key'inizi kontrol edin.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.AIResponse = $"Bir hata oluştu: {ex.Message}";
            }

            return View(model);
        }

        // Google API Cevap Modelleri (Yardımcı Sınıflar)
        public class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate> Candidates { get; set; }
        }
        public class Candidate
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; }
        }
        public class Content
        {
            [JsonPropertyName("parts")]
            public List<Part> Parts { get; set; }
        }
        public class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }
    }
}