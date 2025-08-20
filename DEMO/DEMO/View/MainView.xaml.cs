using DEMO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DEMO.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            // Focus vào TextBox khi mở app
            
        }

        private void MessageList_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (MessageList.Items.Count > 0)
            {
                // Scroll xuống item cuối cùng
                MessageList.ScrollIntoView(MessageList.Items[MessageList.Items.Count - 1]);

                // Đảm bảo scroll đến bottom hoàn toàn
                var border = VisualTreeHelper.GetChild(MessageList, 0) as Decorator;
                if (border != null)
                {
                    var scrollViewer = border.Child as ScrollViewer;
                    scrollViewer?.ScrollToEnd();
                }
            }
        }

        private void Composer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Kiểm tra có phải Shift+Enter không (cho xuống dòng)
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    // Shift+Enter: cho phép xuống dòng (không làm gì)
                    return;
                }

                // Enter: gửi tin nhắn
                e.Handled = true; // Ngăn Enter tạo line break

                var viewModel = DataContext as MainViewModel;
                if (viewModel?.SendMessageCommand.CanExecute(null) == true)
                {
                    viewModel.SendMessageCommand.Execute(null);
                }
            }
        }
    }
}
