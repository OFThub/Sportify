using Microsoft.AspNetCore.Mvc;
using Sportify.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

namespace Sportify.Controllers
{
    public class AIController : Controller
    {
        // 1. ADIM: https://console.groq.com adresinden aldığınız API Key'i buraya yapıştırın.
        private const string ApiKey = "gsk_BURAYA_GROQ_API_KEYINIZI_YAZIN";

        // Groq API URL'i (OpenAI standartlarını kullanır)
        private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AIPlanViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(AIPlanViewModel model)
        {
            if (model == null) model = new AIPlanViewModel();

            try
            {
                // Prompt (İstem)
                string prompt = $@"Sen profesyonel bir spor koçusun. 
                                   Kullanıcı Bilgileri -> Boy: {model.Height}, Kilo: {model.Weight}, Tip: {model.BodyType}. 
                                   Bu kişiye özel, Türkçe, maddeler halinde detaylı bir antrenman ve beslenme programı hazırla.";

                using (var client = new HttpClient())
                {
                    // Groq Authorization Header eklenmeli
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                    // Groq / OpenAI İstek Formatı
                    var requestBody = new
                    {
                        model = "llama-3.3-70b-versatile", // Veya "llama3-8b-8192" (daha hızlı)
                        messages = new[]
                        {
                            new { role = "system", content = "Sen yardımsever ve uzman bir antrenörsün." },
                            new { role = "user", content = prompt }
                        },
                        temperature = 0.7
                    };

                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(ApiUrl, jsonContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var result = JsonSerializer.Deserialize<GroqResponse>(responseString, options);

                        // Cevabı alıyoruz
                        model.AIResponse = result?.Choices?.FirstOrDefault()?.Message?.Content;
                    }
                    else
                    {
                        model.AIResponse = $"Hata: {response.StatusCode} - {responseString}";
                    }
                }
            }
            catch (Exception ex)
            {
                model.AIResponse = $"Sistemsel Hata: {ex.Message}";
            }

            return View(model);
        }

        // --- Groq / OpenAI Uyumlu Cevap Modelleri ---
        public class GroqResponse
        {
            public List<Choice> Choices { get; set; }
        }

        public class Choice
        {
            public Message Message { get; set; }
        }

        public class Message
        {
            public string Content { get; set; }
        }
    }
}