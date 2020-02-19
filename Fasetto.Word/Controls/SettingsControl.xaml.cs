using Fasetto.Word.Core;
using System.Windows.Controls;

namespace Fasetto.Word
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();

            // Set data context to settings view model
            DataContext = IoC.Settings;
        }
    }
}
