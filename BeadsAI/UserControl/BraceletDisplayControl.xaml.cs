using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeadsAI.UserControl
{
    public partial class BraceletDisplayControl
    {
        public BraceletDisplayControl()
        {
            InitializeComponent();

            MessageBus.strBraceletChanged += strBraceletUpdateHandler;

            strBraceletUpdateHandler(["Red", "Green", "Blue", "SkyBlue"]);
        }

        private string[] strbracelet = Array.Empty <string>();

        public string[] strBracelet
        {
            get { return strbracelet; }
            set // when setted by itself
            {
                strbracelet = value;
                MessageBus.UpdatestrBracelet(value);
            }
        }

        private void strBraceletUpdateHandler(string[] strBracelet) // when setted by others
        {
            strbracelet = strBracelet; // use private field to avoid stack overflow by messagebus

            AddBracelet(UIBead.ToBeads(strBracelet));
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
            strBracelet = strbracelet; // invoke setter

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

    public class UIBead
    {
        public int Position { get; protected set; }
        public string ColorName { get; protected set; }
        public SolidColorBrush Color { get; protected set; }

        public UIBead(int beadPosition, string beadColorName)
        {
            Position = beadPosition;
            ColorName = beadColorName;
            Color = new(ToColor(beadColorName));
        }

        public static Dictionary<string, byte[]> strColorMap { get; } = new()
        {
            {"Red",[255,0,0] },
            {"Green",[0,255,0] },
            {"Blue",[0,0,255] },
            {"SkyBlue",[135,206,235] }
        };

        public static UIBead[] ToBeads(string[] strBracelet)
        {
            return strBracelet.Select((name,i) => new UIBead(i,name)).ToArray();
        }

        private Color ToColor(string ColorName)
        {
            Color color = new();

            byte[]? rgb = [0, 0, 0];

            if (strColorMap.TryGetValue(ColorName, out rgb))
            {
                color.A = 255;
                color.R = rgb[0];
                color.G = rgb[1];
                color.B = rgb[2];
            }

            return color;
        }

        public static string[] ToArray(UIBead[] Bracelet)
        {
            return Bracelet.Select(bead => bead.ColorName).ToArray();
        }
    }
}
