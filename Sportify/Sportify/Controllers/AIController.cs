using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportify.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sportify.Controllers
{
    public class AIController : Controller
    {
        private const string ApiKey = "API_KEY";

        private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View(new AIPlanViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(AIPlanViewModel model)
        {
            if (model == null) model = new AIPlanViewModel();

            try
            {
                string prompt = $@"Sen profesyonel bir spor koçusun. 
                                   Kullanıcı Bilgileri -> Boy: {model.Height}, Kilo: {model.Weight}, Tip: {model.BodyType}. 
                                   Bu kişiye özel, Türkçe, maddeler halinde detaylı bir antrenman ve beslenme programı hazırla.";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                    var requestBody = new
                    {
                        model = "llama-3.3-70b-versatile", 
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