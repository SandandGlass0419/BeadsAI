using System.Windows;

namespace BeadsAI.UserControl
{
    public partial class ColorSelectWindow : Window
    {
        public ColorSelectWindow(UIBead CurrentBead)
        {
            InitializeComponent();
            DataContext = this;

            this.CurrentBead = CurrentBead;

            Title = $"Pos: {CurrentBead.Position}";
            ColorSelector.SelectedItem = CurrentBead.ColorName;
        }

        public static List<string> ColorList { get; } = UIBead.strColorMap.Keys.ToList();
        public UIBead CurrentBead { get; protected set; }

        private void OK_btn_Click(object sender, RoutedEventArgs e)
        {
            CurrentBead = new UIBead(CurrentBead.Position, (string) ColorSelector.SelectedItem);

            Close();
        }
    }
}
