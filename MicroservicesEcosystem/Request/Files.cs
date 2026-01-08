using MicroservicesEcosystem.Models;
using System.Text;

namespace MicroservicesEcosystem.Request
{
    public class Files
    {
        public static StringBuilder ReadData(string path)
        {
            StringBuilder content = new StringBuilder();
            content.Append(File.ReadAllText(path));
            return content;

        }      
    }
}
