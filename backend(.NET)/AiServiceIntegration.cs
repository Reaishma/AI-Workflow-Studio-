using System.Text.Json;

namespace WorkflowApi.Services
{
    public interface IAiServiceIntegration
    {
        Task<string> ProcessTextAsync(string input);
        Task<string> AnalyzeImageAsync(byte[] imageData);
        Task<string> TranslateTextAsync(string text, string targetLanguage);
        Task<string> ExtractSentimentAsync(string text);
        Task<string> TranscribeAudioAsync(byte[] audioData);
    }
    
    public class AiServiceIntegration : IAiServiceIntegration
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AiServiceIntegration> _logger;
        
        public AiServiceIntegration(HttpClient httpClient, IConfiguration configuration, ILogger<AiServiceIntegration> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<string> ProcessTextAsync(string input)
        {
            try
            {
                // OpenAI integration example
                var openAiApiKey = _configuration["AI:OpenAI:ApiKey"];
                if (string.IsNullOrEmpty(openAiApiKey))
                {
                    _logger.LogWarning("OpenAI API key not configured");
                    return $"AI processing simulated for: {input}";
                }
                
                var request = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "user", content = input }
                    },
                    max_tokens = 150
                };
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");
                
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<dynamic>(responseContent);
                    return result?.choices?[0]?.message?.content?.ToString() ?? "No response generated";
                }
                else
                {
                    _logger.LogError("OpenAI API call failed with status: {StatusCode}", response.StatusCode);
                    return $"AI processing failed for: {input}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing text with AI service");
                return $"AI processing error for: {input}";
            }
        }
        
        public async Task<string> AnalyzeImageAsync(byte[] imageData)
        {
            try
            {
                // Azure Computer Vision integration example
                var azureApiKey = _configuration["AI:Azure:ComputerVision:ApiKey"];
                var azureEndpoint = _configuration["AI:Azure:ComputerVision:Endpoint"];
                
                if (string.IsNullOrEmpty(azureApiKey) || string.IsNullOrEmpty(azureEndpoint))
                {
                    _logger.LogWarning("Azure Computer Vision credentials not configured");
                    return "Image analysis simulated - no Azure credentials";
                }
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureApiKey);
                
                var content = new ByteArrayContent(imageData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                
                var response = await _httpClient.PostAsync($"{azureEndpoint}/vision/v3.2/analyze?visualFeatures=Description,Tags", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    _logger.LogError("Azure Computer Vision API call failed with status: {StatusCode}", response.StatusCode);
                    return "Image analysis failed";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing image with AI service");
                return "Image analysis error";
            }
        }
        
        public async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            try
            {
                // Google Translate integration example
                var googleApiKey = _configuration["AI:Google:Translate:ApiKey"];
                
                if (string.IsNullOrEmpty(googleApiKey))
                {
                    _logger.LogWarning("Google Translate API key not configured");
                    return $"Translation simulated: '{text}' to {targetLanguage}";
                }
                
                var url = $"https://translation.googleapis.com/language/translate/v2?key={googleApiKey}";
                var request = new
                {
                    q = text,
                    target = targetLanguage,
                    format = "text"
                };
                
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<dynamic>(responseContent);
                    return result?.data?.translations?[0]?.translatedText?.ToString() ?? text;
                }
                else
                {
                    _logger.LogError("Google Translate API call failed with status: {StatusCode}", response.StatusCode);
                    return $"Translation failed for: {text}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error translating text with AI service");
                return $"Translation error for: {text}";
            }
        }
        
        public async Task<string> ExtractSentimentAsync(string text)
        {
            try
            {
                // Azure Text Analytics integration example
                var azureApiKey = _configuration["AI:Azure:TextAnalytics:ApiKey"];
                var azureEndpoint = _configuration["AI:Azure:TextAnalytics:Endpoint"];
                
                if (string.IsNullOrEmpty(azureApiKey) || string.IsNullOrEmpty(azureEndpoint))
                {
                    _logger.LogWarning("Azure Text Analytics credentials not configured");
                    return "Sentiment analysis simulated - no Azure credentials";
                }
                
                var request = new
                {
                    documents = new[]
                    {
                        new { id = "1", text = text }
                    }
                };
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureApiKey);
                
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{azureEndpoint}/text/analytics/v3.1/sentiment", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    _logger.LogError("Azure Text Analytics API call failed with status: {StatusCode}", response.StatusCode);
                    return "Sentiment analysis failed";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting sentiment with AI service");
                return "Sentiment analysis error";
            }
        }
        
        public async Task<string> TranscribeAudioAsync(byte[] audioData)
        {
            try
            {
                // Azure Speech Services integration example
                var azureApiKey = _configuration["AI:Azure:Speech:ApiKey"];
                var azureRegion = _configuration["AI:Azure:Speech:Region"];
                
                if (string.IsNullOrEmpty(azureApiKey) || string.IsNullOrEmpty(azureRegion))
                {
                    _logger.LogWarning("Azure Speech Services credentials not configured");
                    return "Audio transcription simulated - no Azure credentials";
                }
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureApiKey);
                
                var content = new ByteArrayContent(audioData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
                
                var response = await _httpClient.PostAsync($"https://{azureRegion}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
                else
                {
                    _logger.LogError("Azure Speech Services API call failed with status: {StatusCode}", response.StatusCode);
                    return "Audio transcription failed";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transcribing audio with AI service");
                return "Audio transcription error";
            }
        }
    }
}