using DEMO.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DEMO.Services
{
    public class DatabaseService
    {
        private readonly string _filePath = "chat.json"; // File JSON lưu trong thư mục exe

        /// <summary>
        /// Lưu tin nhắn mới vào file JSON - Chỉ giữ 10 tin nhắn gần nhất
        /// </summary>
        public void SaveMessage(ChatMessage msg)
        {
            try
            {
                // Bước 1: Load tất cả tin nhắn hiện tại
                var messages = LoadMessages();

                // Bước 2: Tạo ID mới (ID lớn nhất + 1)
                msg.Id = messages.Count > 0 ? messages[messages.Count - 1].Id + 1 : 1;

                // Bước 3: Thêm tin nhắn mới vào list
                messages.Add(msg);

                // Bước 4: Chỉ giữ 10 tin nhắn gần nhất
                const int maxMessages = 10;
                if (messages.Count > maxMessages)
                {
                    // Xóa những tin nhắn cũ, chỉ giữ 10 tin nhắn cuối
                    messages = messages.Skip(messages.Count - maxMessages).ToList();
                    System.Diagnostics.Debug.WriteLine($"Đã xóa tin nhắn cũ, chỉ giữ {maxMessages} tin nhắn gần nhất");
                }

                // Bước 5: Convert list thành JSON string (có format đẹp)
                var json = JsonConvert.SerializeObject(messages, Formatting.Indented);

                // Bước 6: Ghi JSON vào file
                File.WriteAllText(_filePath, json);

                System.Diagnostics.Debug.WriteLine($"Đã lưu tin nhắn ID: {msg.Id}");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi không thể lưu
                System.Diagnostics.Debug.WriteLine($"Lỗi lưu tin nhắn: {ex.Message}");
                throw; // Ném lỗi lên để ViewModel xử lý
            }
        }

        /// <summary>
        /// Đọc tất cả tin nhắn từ file JSON
        /// </summary>
        public List<ChatMessage> LoadMessages()
        {
            try
            {
                // Kiểm tra file có tồn tại không
                if (!File.Exists(_filePath))
                {
                    System.Diagnostics.Debug.WriteLine("File chat.json chưa tồn tại, tạo list rỗng");
                    return new List<ChatMessage>();
                }

                // Đọc nội dung file thành string
                var json = File.ReadAllText(_filePath);

                // Kiểm tra file có rỗng không
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<ChatMessage>();
                }

                // Convert JSON string thành List<ChatMessage>
                var messages = JsonConvert.DeserializeObject<List<ChatMessage>>(json);

                System.Diagnostics.Debug.WriteLine($"Đã load {messages?.Count ?? 0} tin nhắn");
                return messages ?? new List<ChatMessage>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load tin nhắn: {ex.Message}");
                // Trả về list rỗng thay vì crash app
                return new List<ChatMessage>();
            }
        }

        /// <summary>
        /// Xóa tất cả tin nhắn (xóa file JSON)
        /// </summary>
        public void ClearMessages()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                    System.Diagnostics.Debug.WriteLine("Đã xóa tất cả tin nhắn");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi xóa tin nhắn: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy số lượng tin nhắn (không cần load hết vào RAM)
        /// </summary>
        public int GetMessageCount()
        {
            return LoadMessages().Count;
        }

        /// <summary>
        /// Kiểm tra file database có tồn tại không
        /// </summary>
        public bool DatabaseExists()
        {
            return File.Exists(_filePath);
        }
    }
}