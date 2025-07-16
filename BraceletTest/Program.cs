using BeadsAI;

namespace BraceletTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetworkTest();

            //RecognitionTest();
        }

        static void NetworkTest()
        {
            BraceletNetwork NetworkTest = new();

            NetworkTest.AddLayers(["Red","Blue","Blue","Green","Red","Blue","Green","Blue",
                                   "Red","Red","Blue","Red","Green","Blue","Red","Red",
                                   "Blue","Green","Red","Red","Green","Blue","Blue","Green",
                                   "Red","Green","Blue","Blue","Red","Green","Green","Blue"]);

            var result = NetworkTest.RunModel([ 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0,1]);

            Console.WriteLine(string.Join(',',result));
        }

        static void RecognitionTest()
        {
            BraceletRecognition RecogTest = new();

            const string saved_dir = BraceletRecognitionConfig.saved_dir;

            //RecogTest.FindBraceletColors(saved_dir + "ao.jpg"); Exception test

            // Configuration is done on implementation class


            //var res1 = RecogTest.FindBraceletColors(saved_dir + "Red.jpg");
            //var res2 = RecogTest.FindBraceletColors(saved_dir + "Blue.jpg");

            //Console.WriteLine($"{res1}, {res2}");
        }
    }
}
