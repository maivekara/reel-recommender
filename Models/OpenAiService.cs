using System.Text;
using System.Text.Json;

namespace FilmOneriProjesi.Models
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string OpenAiUrl = "https://api.openai.com/v1/chat/completions";

        public OpenAiService()
        {
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("OPENAI_API_KEY ortam değişkeni ayarlanmamış.");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetMovieSuggestionAsync(string userInput)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Kullanıcının girdilerine göre filmler öner." },
                    new { role = "user", content = $"Kullanıcı şu kelimeleri yazdı: {userInput}. Ne önerebilirim?" }
                },
                temperature = 0.7
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var response = await _httpClient.PostAsync(OpenAiUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var doc = JsonDocument.Parse(responseBody);

                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    return content.GetString() ?? "İçerik boş geldi.";
                }
                else
                {
                    return $"Beklenen formatta yanıt alınamadı. API cevabı: {responseBody}";
                }
            }
            catch (JsonException ex)
            {
                return $"JSON çözümleme hatası: {ex.Message}\nYanıt: {responseBody}";
            }
        }
    }
}
