using BeadsAI.MVVM;
using BeadsAI.Model;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace BeadsAI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<BeadItem> Bracelet { get; set; } = new();

        public MainWindowViewModel()
        {
            Bracelet.Add(new BeadItem(0, "Red"));
            Bracelet.Add(new BeadItem(1, "Blue"));
        }
    }
}
