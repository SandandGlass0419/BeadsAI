using BeadsAI.ViewModel;
using System.Windows.Controls;

namespace BeadsAI.View
{
    public partial class BraceletDisplayView : UserControl
    {
        public BraceletDisplayView()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}
