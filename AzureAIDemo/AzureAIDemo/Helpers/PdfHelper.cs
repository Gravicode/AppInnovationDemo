using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Helpers
{
    public class PdfHelper
    {
        public static string ConvertToBase64(byte[] data)
        {
            string base64String = Convert.ToBase64String(data, 0, data.Length);
            var img = "data:application/pdf;base64," + base64String;
            return img;
        }
    }
}
