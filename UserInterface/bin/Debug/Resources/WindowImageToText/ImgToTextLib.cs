//Lệnh nạp packgage cho soltution(hình như là đổi sang máy khác cũng không cần cài lại):
//Install-Package Nuget.Tessnet2
//Install-Package Tesseract


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

//using tessnet2;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Tesseract;


namespace ImgToTextLib
{
    public class ImageToTextLib
    {
        
        private static string solutionPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        private static string stringTranslate = "";

        public static string strConvertImageToText(string filename)
        {

            //Ảnh screenshot:
            var image = new Bitmap(filename);
            //Khai báo biến để thực hiện OCR(Optical Character Regconition) và khởi tạo:
            var ocr = new tessnet2.Tesseract();
            ocr.Init(solutionPath + @"\Tessdata", "eng", false);
            //Console.WriteLine(solutionPath);
            //Kết quả được trả về dưới dạng mảng các dữ liệu kiểu World của namespace tessnet2, để lấy dạng string của kiểu dữ liệu này thì truy cập thuộc tính Text
            var result = ocr.DoOCR(image, Rectangle.Empty);
            foreach (tessnet2.Word wrd in result)
            {
                //Hiện các chữ ra console (Dùng để debug):
                stringTranslate += wrd.Text + " ";
                //Viết thêm đoạn code cập nhật những từ detect được vào file notepad dưới này
            }
            return stringTranslate;
        }
        public static string strTranslateGoogle(string text, string lgPair)
        {
            // normalize the culture in case something like en-us was passed 
            // retrieve only en since Google doesn't support sub-locales
            text = text.Replace(" ", "+");

            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", text, lgPair);
            // Retrieve Translation with HTTP GET call
            string result = null;
            try
            {
                WebClient web = new WebClient();
                // MUST add a known browser user agent or else response encoding doen't return UTF-8 ?
                web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8,UTF-16,Unicode,ISO-8859-1,ASCII,UTF-32/UCS-4,windows-1258");

                // Make sure we have response encoding to UTF-8
                web.Encoding = System.Text.Encoding.UTF8;
                result = web.DownloadString(url);
            }
            catch { return "Noi dung khong hop le"; }
            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            return result.Trim();
        }
    }
}
