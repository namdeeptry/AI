using DEMO.Model;
using DEMO.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DEMO.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db = new DatabaseService();
        private readonly ChatService _chat = new ChatService();

        public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();

        #region -- Properties --

        private string _titleText = "New chat";
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; OnPropertyChanged(); }
        }
        private string _currentMessage = string.Empty;
        public string   CurrentMessage
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

        #endregion

        #region -- Commands --
        public ICommand SendMessageCommand { get; }
        public ICommand ClearMessagesCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand NewChatCommand { get; }
        public ICommand CopyMessageCommand { get; }

        #endregion

        public MainViewModel()
        {
            // LoadMessages();
            SendMessageCommand = new RelayCommand(async _ => await SendMessage(), _ => !string.IsNullOrWhiteSpace(CurrentMessage) && !IsLoading);
            ClearMessagesCommand = new RelayCommand(_ => ClearMessages());
            MinimizeCommand = new RelayCommand(_ => System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized);
            MaximizeCommand = new RelayCommand(_ =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow;
                mainWindow.WindowState = mainWindow.WindowState == System.Windows.WindowState.Maximized ?
                                         System.Windows.WindowState.Normal :
                                         System.Windows.WindowState.Maximized;
            });
            CloseCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());
            NewChatCommand = new RelayCommand(_ => NewChat());
            CopyMessageCommand = new RelayCommand(msg => CopyMessage(msg as ChatMessage));

        }

        private void NewChat()
        {
            // Chỉ clear trong ObservableCollection, không động đến DatabaseService
            Messages.Clear();

            // Reset title về mặc định
            TitleText = "New chat";
        }

        private void CopyMessage(ChatMessage msg)
        {
            if (msg != null)
            {
                Clipboard.SetText(msg.Text);
            }
        }

        //private void LoadMessages()
        //{
        //    try
        //    {
        //        foreach (var msg in _db.LoadMessages())
        //            Messages.Add(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        Messages.Add(new ChatMessage
        //        {
        //            Text = $"Lỗi tải tin nhắn: {ex.Message}",
        //            IsUser = false
        //        });
        //    }
        //}

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
                Text = " Thinking...",
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