using BeadsAI.Core.NeuralNetwork;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeadsAI.UserControls
{
    public partial class BraceletDisplayControl : UserControl
    {
        private static readonly string[] InitStrBracelet = Enumerable.Repeat("Red", 32).ToArray();  // set to 32 to prevent exception

        public BraceletDisplayControl()
        {
            InitializeComponent();

            MessageBus.StrBraceletChanged += StrBraceletUpdateHandler;

            //SetStrBracelet(InitStrBracelet);

            StrBracelet = InitStrBracelet;
        }

        private string[] strbracelet = Array.Empty <string>();

        public string[] StrBracelet
        {
            get { return strbracelet; }
            set // when setted by itself
            {
                strbracelet = value;
                MessageBus.UpdateStrBracelet(value);
            }
        }

        private void SetStrBracelet(string[] strbracelet)
        {
            StrBraceletUpdateHandler(strbracelet);
            StrBracelet = strbracelet;
        }

        private void StrBraceletUpdateHandler(string[] strbracelet) // when setted by others
        {
            this.strbracelet = strbracelet;

            AddBracelet(UIBead.ToBeads(StrBracelet));
        }

        private Button CreateCustombtn(UIBead bead)
        {
            Button beadbtn = new Button
            {
                Width = 40,
                Height = 40,
                BorderThickness = new Thickness(1),

                Tag = bead,
                Content = bead.Position,
                ToolTip = bead.ColorName,
                Background = bead.Color,
            };

            beadbtn.Click += Beadbtn_Click;

            return beadbtn;
        }

        private void AddBracelet(UIBead[] Bracelet)
        {
            BraceletDisplay.Children.Clear();

            foreach (UIBead bead in Bracelet)
            {
                var btn = CreateCustombtn(bead);

                BraceletDisplay.Children.Add(btn);
            }
        }

        private void EditBracelet(UIBead bead)
        {
            var btn = CreateCustombtn(bead);

            strbracelet[bead.Position] = bead.ColorName;
            StrBracelet = strbracelet; // invoke setter

            BraceletDisplay.Children.RemoveAt(bead.Position);
            BraceletDisplay.Children.Insert(bead.Position, btn);
        }

        private void Beadbtn_Click(object sender, EventArgs e)
        {
            var beadbtn  = (Button) sender;
            var CurrentBead = (UIBead) beadbtn.Tag;

            ColorSelectWindow SelectWindow = new(CurrentBead);

            SelectWindow.ShowDialog();

            EditBracelet(SelectWindow.CurrentBead);
        }

        private void BeadDebugMsgBox(object sender, EventArgs e)
        {
            var clked = (Button) sender;

            MessageBox.Show($"{clked.ToolTip}", $"Pos: {clked.Content}");
        }
    }
}
