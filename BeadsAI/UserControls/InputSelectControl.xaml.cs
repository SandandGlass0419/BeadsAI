using System.Windows.Controls;

namespace BeadsAI.UserControls
{
    public partial class InputSelectControl : UserControl
    {
        public InputSelectControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static Dictionary<string, float[]> StrInputmap { get; } = new()
        {
            { "Input1" , [1,0,1,0,1] },
            { "Input2" , [0,1,1,0,1] },
            { "Input3" , [0,0,0,0,0] },
            { "Input4" , [1,1,1,1,1] }
        };

        public static List<string> StrInputs { get; } = StrInputmap.Keys.ToList();

        private string input = String.Empty;

        public string Input
        {
            get { return input; }
            set 
            { 
                input = value;
                MessageBus.UpdateInput(value);
            }
        }

    }
}
