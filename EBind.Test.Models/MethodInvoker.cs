namespace EBind.Test.Models
{
    public class MethodInvoker
    {
        public static string ConvertStatic(int obj) => obj.ToString();

        public string Convert(int obj) => obj.ToString();
        public string Convert(int obj1, int obj2) => obj1.ToString();
        public string Convert(int obj1, int obj2, int obj3) => obj1.ToString();
    }
}
