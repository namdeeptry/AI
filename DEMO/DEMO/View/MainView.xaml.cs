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
            if (sender is ListBox listBox && listBox.Items.Count > 0)
            {
                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
            }
        }

        private void Composer_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter = gửi, Shift+Enter = xuống dòng
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (DataContext is MainViewModel vm && vm.SendMessageCommand.CanExecute(null))
                {
                    vm.SendMessageCommand.Execute(null);
                    e.Handled = true; // chặn xuống dòng mặc định
                }
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void StackPanel_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
