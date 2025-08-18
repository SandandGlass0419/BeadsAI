using System.Windows;
using System.ComponentModel;

namespace BeadsAI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosingHandler;
        }

        public static event Action? NotifyClosing;
        
        private void OnClosingHandler(object? sender, CancelEventArgs e)
        {
            NotifyClosing?.Invoke();
        }
    }
}