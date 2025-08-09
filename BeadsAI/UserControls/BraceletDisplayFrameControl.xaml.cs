using System.ComponentModel;
using System.Windows.Controls;

namespace BeadsAI.UserControls
{
    public partial class BraceletDisplayFrameControl : UserControl , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BraceletDisplayFrameControl()
        {
            InitializeComponent();
            DataContext = this;

            MessageBus.BestScoreChanged += BestScoreUpdateHandler;
            MessageBus.StrBraceletChanged += StrBraceletUpdateHandler;
        }
        
        public float BestScore { get; protected set; }

        private void BestScoreUpdateHandler(float bestscore)
        {
            BestScore = bestscore;

            tb_BestScore.Text = $"BestScore: {bestscore.ToString()}";
        }

        private int[] btnparams = Enumerable.Repeat(0, 4).ToArray();

        public string BtnCopyStart
        {
            get { return btnparams[0].ToString(); }
            set
            {
                btnparams[0] = ParseParams(value);
                OnPropertyChanged(nameof(BtnCopyStart));
            }
        }

        public string BtnCopyEnd
        {
            get { return btnparams[1].ToString(); }
            set
            {
                btnparams[1] = ParseParams(value);
                OnPropertyChanged(nameof(BtnCopyEnd));
            }
        }

        public string BtnPasteStart
        {
            get { return btnparams[2].ToString(); }
            set
            {
                btnparams[2] = ParseParams(value);
                OnPropertyChanged(nameof(BtnPasteStart));
            }
        }

        public string BtnPasteEnd
        {
            get { return btnparams[3].ToString(); }
            set
            {
                btnparams[3] = ParseParams(value);
                OnPropertyChanged(nameof(BtnPasteEnd));
            }
        }

        private int ParseParams(string parameter)
        {
            int intparam = int.TryParse(parameter, out intparam) ? intparam : 0;

            if (intparam >= BraceletDisplayControl.StrBraceletLength)
            { return BraceletDisplayControl.StrBraceletLength - 1; }

            if (intparam < 0)
            { return 0; }

            return intparam;
        }

        private string[] strbracelet = BraceletDisplayControl.InitStrBracelet;

        public string[] StrBracelet
        {
            get { return strbracelet; }
            set
            { 
                strbracelet = value;
                MessageBus.UpdateStrBracelet(value);
            }
        }

        private void StrBraceletUpdateHandler(string[] strbracelet)
        {
            this.strbracelet = strbracelet;
        }

        private void Btn_Repeat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var copyarray = GetCopyArray(btnparams[0], btnparams[1]);
            var pastearray = GetPasteArray(btnparams[2], btnparams[3], copyarray);
            StrBracelet = InsertPasteArray(btnparams[2], btnparams[3], pastearray);
        }

        private string[] GetCopyArray(int param1, int param2)
        {
            bool ascending = param1 <= param2;

            int start = ascending ? param1 : param2;
            int margin = Math.Abs(param2 - param1) + 1;

            string[] copyarray = StrBracelet.Skip(start).Take(margin).ToArray();

            if (!ascending)
            {
                copyarray = copyarray.Reverse().ToArray();
            }

            return copyarray;
        }

        private string[] GetPasteArray(int param3, int param4, string[] copyarray)
        {
            bool ascending = param3 <= param4;
            bool copyempty = copyarray.Length == 0;

            int margin = Math.Abs(param4 - param3) + 1;

            int times = copyempty ? 0 : margin / copyarray.Length;
            int leftoversize = copyempty ? 0 : margin % copyarray.Length;

            string[] leftover = copyarray.Take(leftoversize).ToArray();

            string[] pastearray = Array.Empty<string>();

            for (int i = 0; i < times; i++)
            {
                pastearray = pastearray.Concat(copyarray).ToArray();
            }

            pastearray = pastearray.Concat(leftover).ToArray();

            if (!ascending)
            { 
                pastearray = pastearray.Reverse().ToArray();
            }

            return pastearray;
        }

        private string[] InsertPasteArray(int param3,int param4, string[] pastearray)
        {
            bool ascending = param3 <= param4;

            int start = ascending ? param3 : param4;
            int margin = Math.Abs(param4 - param3) + 1;

            List<string> source = StrBracelet.ToList();

            source.RemoveRange(start, margin);

            source.InsertRange(start, pastearray);

            return source.ToArray();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
