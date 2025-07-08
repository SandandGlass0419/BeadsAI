using System.Windows;

namespace BeadsAI.Core
{
    public class ExceptionThrower
    {
        public static void Throw(string message)
        {
            MessageBox.Show(message,"Exception Thrown",MessageBoxButton.OK,MessageBoxImage.Error);

            throw new Exception(message);
        }
    }
}
