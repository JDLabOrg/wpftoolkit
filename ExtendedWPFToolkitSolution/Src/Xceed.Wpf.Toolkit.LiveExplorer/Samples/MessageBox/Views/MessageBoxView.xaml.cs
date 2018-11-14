using System.Windows;
using System.Windows.Controls;

namespace Xceed.Wpf.Toolkit.LiveExplorer.Samples.MessageBox.Views
{
    /// <summary>
    /// Interaction logic for MessageBoxView.xaml
    /// </summary>
    public partial class MessageBoxView : DemoView
    {
        public MessageBoxView()
        {
            InitializeComponent();
        }

        private void ErrorButton_Click(object sender, RoutedEventArgs e)
        {
            Toolkit.MessageBox.Show("Thanks for clicking me!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void WarnButton_Click(object sender, RoutedEventArgs e)
        {
            Toolkit.MessageBox.Show("Thanks for clicking me!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

        }
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Toolkit.MessageBox.Show("Thanks for clicking me!", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information);
        }
        private void QuestionButton_Click(object sender, RoutedEventArgs e)
        {
            Toolkit.MessageBox.Show("Thanks for clicking me!", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

        }
    }
}
