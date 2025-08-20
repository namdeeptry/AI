using DEMO.Model;
using DEMO.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DEMO.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db = new DatabaseService();
        private readonly ChatService _chat = new ChatService();

        public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();

        private string _currentMessage = string.Empty;
        public string CurrentMessage
        {
            get => _currentMessage;
            set { _currentMessage = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand ClearMessagesCommand { get; }

        public MainViewModel()
        {
            LoadMessages();
            SendMessageCommand = new RelayCommand(async _ => await SendMessage(), _ => !string.IsNullOrWhiteSpace(CurrentMessage) && !IsLoading);
            ClearMessagesCommand = new RelayCommand(_ => ClearMessages());
        }

        private void LoadMessages()
        {
            try
            {
                foreach (var msg in _db.LoadMessages())
                    Messages.Add(msg);
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage
                {
                    Text = $"Lỗi tải tin nhắn: {ex.Message}",
                    IsUser = false
                });
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessage) || IsLoading)
                return;

            var userMsg = new ChatMessage
            {
                Text = CurrentMessage.Trim(),
                IsUser = true,
                Time = DateTime.Now
            };

            Messages.Add(userMsg);
            _db.SaveMessage(userMsg);

            var userInput = CurrentMessage.Trim();
            CurrentMessage = string.Empty;
            IsLoading = true;

            // Thêm tin nhắn "Thinking..." tạm thời
            var thinkingMsg = new ChatMessage
            {
                Text = "Thinking...",
                IsUser = false,
                Time = DateTime.Now
            };
            Messages.Add(thinkingMsg);

            try
            {
                var replyText = await _chat.AskAsync(userInput);

                // Xóa tin nhắn "Thinking..."
                Messages.Remove(thinkingMsg);

                var botMsg = new ChatMessage
                {
                    Text = replyText,
                    IsUser = false,
                    Time = DateTime.Now
                };

                Messages.Add(botMsg);
                _db.SaveMessage(botMsg);
            }
            catch (Exception ex)
            {
                // Xóa tin nhắn "Thinking..." khi có lỗi
                Messages.Remove(thinkingMsg);

                var errorMsg = new ChatMessage
                {
                    Text = $"Lỗi: {ex.Message}",
                    IsUser = false,
                    Time = DateTime.Now
                };
                Messages.Add(errorMsg);
                _db.SaveMessage(errorMsg);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearMessages()
        {
            try
            {
                Messages.Clear();
                _db.ClearMessages();
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage
                {
                    Text = $"Lỗi xóa tin nhắn: {ex.Message}",
                    IsUser = false,
                    Time = DateTime.Now
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}