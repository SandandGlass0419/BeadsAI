using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeadsAI.UserControl
{
    public partial class BraceletDisplayControl
    {
        public UIBead[] Bracelet {  get; protected set; } = Array.Empty<UIBead>();

        public BraceletDisplayControl()
        {
            InitializeComponent();

            string[] strBracelet = ["Red", "Green", "Blue", "Red"];
            Bracelet = UIBead.ToBeads(strBracelet);

            AddBracelet(Bracelet);
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
            foreach (UIBead bead in Bracelet)
            {
                var btn = CreateCustombtn(bead);

                BraceletDisplay.Children.Add(btn);
            }
        }

        private void EditBracelet(UIBead bead)
        {
            var btn = CreateCustombtn(bead);

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
            UIBead[] Bracelet = new UIBead[strBracelet.Length];

            for (int i = 0;i < strBracelet.Length;i++)
            {
                Bracelet[i] = new UIBead(i,strBracelet[i]);
            }

            return Bracelet;
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
    }
}
