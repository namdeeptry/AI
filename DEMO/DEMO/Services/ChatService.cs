using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OllamaSharp;
using OllamaSharp.Models;

namespace DEMO.Services
{
    public class ChatService
    {
        private readonly OllamaApiClient _client;
        private readonly string _defaultModel = "deepseek-r1:1.5b";

        public ChatService()
        {
            _client = new OllamaApiClient("http://localhost:11434");
        }

        public async Task<string> AskAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return "Vui lòng nhập tin nhắn!";

            try
            {
                var request = new GenerateRequest
                {
                    Model = _defaultModel,
                    Prompt = prompt,
                    Stream = true
                };

                var sb = new StringBuilder();
                var enumerator = _client.GenerateAsync(request).GetAsyncEnumerator();

                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        var chunk = enumerator.Current;
                        if (chunk?.Response != null)
                        {
                            sb.Append(chunk.Response);
                        }
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }

                var result = sb.ToString().Trim();
                return string.IsNullOrEmpty(result) ? "Xin lỗi, tôi không thể trả lời câu hỏi này." : result;
            }
            catch (Exception ex)
            {
                return $"Lỗi kết nối với AI: {ex.Message}. Vui lòng kiểm tra Ollama server.";
            }
        }

        public async Task<bool> IsModelAvailableAsync()
        {
            try
            {
                var models = await _client.ListLocalModelsAsync();
                return models?.Any(m => m.Name.Contains(_defaultModel)) ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}