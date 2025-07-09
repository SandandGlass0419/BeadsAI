using System.Windows.Controls;

namespace BeadsAI.UserControls
{
    public partial class InputSelectControl : UserControl
    {
        public InputSelectControl()
        {
            InitializeComponent();
            DataContext = this;

            StrInput = "Cross";
        }

        public static Dictionary<string, float[]> StrInputMap { get; } = new()
        {
            { "Cross", [0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0] },
            { "Face", [0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1] },
            { "Heart", [0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0] },
            { "House", [0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1] }
        };

        public static List<string> StrInputs { get; } = StrInputMap.Keys.ToList(); // used on binding

        private string strinput = String.Empty;

        public string StrInput
        {
            get { return strinput; }
            set 
            { 
                strinput = value;
                MessageBus.UpdateStrInput(value);
            }
        }

    }
}
