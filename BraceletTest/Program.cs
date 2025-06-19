using BeadsAI;

namespace BraceletTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetworkTest();

            RecognitionTest();
        }

        static void NetworkTest()
        {

        }

        static void RecognitionTest()
        {
            BraceletRecognition RecogTest = new();

            const string saved_dir = BraceletRecognitionConfig.saved_dir;

            // Configuration is done on implementation class

            var res1 = RecogTest.FindBraceletColors(saved_dir + "Red.jpg");
            var res2 = RecogTest.FindBraceletColors(saved_dir + "Blue.jpg");

            Console.WriteLine($"{res1}, {res2}");
        }
    }
}
