using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace UserInterface
{
    class ImageProcessing
    {
        private static string solutionPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\";
        //trả về html Source
        public static string GetImageSoureHtml(string keyWord)
        {
            //tạo query tìm kiếm html, query này trả về chuỗi html từ máy chủ ứng với link url
            string stringHtml = "";
            string url = "https://www.google.com/search?q=" + @keyWord + "&tbm=isch";
            //tạo yêu cầu http theo link url
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            //nhận phản hồi từ httpweb
            var response = (HttpWebResponse)request.GetResponse();

            //ghi file html từ stream response
            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    stringHtml = sr.ReadToEnd();
                }
            }
            response.Close();
            // trả về chuỗi html

            //cắt hình ảnh từ file html theo định dạng bắt đầu bằng data:imag/jpeg và tận cùng bằng \
            Regex regex = new Regex("(?<=\"data:image/jpeg)[^\"]+?(?=\")");
            Match match = regex.Match(stringHtml);
            string imageSource = "";
            if (match.Success == true)
            {
                imageSource = match.Value;
                //nếu tồn tại kí tự \\ trong chuỗi thì xóa nó đi
                if (imageSource.Contains("\\"))
                {
                    imageSource = imageSource.Remove(imageSource.IndexOf('\\'), imageSource.Length - 1 - imageSource.IndexOf('\\'));
                }
                //kiểm tra định dạng base64 của hình ảnh có hợp lệ, định dạng hợp lệ thì phải chia hết cho 2^2
                //nếu không hợp lệ thì thêm dấu = vào cuối chuỗi cho hợp lệ
                //thêm chuỗi data:image/jpeg trước chuỗi string
                if (imageSource.Length % 4 == 0)
                    imageSource = "data:image/jpeg" + imageSource;
                else
                    imageSource = "data:image/jpeg" + imageSource + "=";

                return imageSource;
            }
            //nếu không tồn tại hình theo định dạng thì trả về hình ảnh màu trắng
            else return "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBxAPDw0PDw8NDw8NDQ0NDQ8PDQ8NDQ0NFREWFhURFRUYHSggGBolGxUVITEhJSkrLi4uFx8zODMtNygtLisBCgoKDg0NDw0NDysZFRk3KzctNys3LTc3NystLSsrKysrLSstKy0tKysrNysrKy0rLSsrKysrLSsrKysrKysrK//AABEIAKgBLAMBIgACEQEDEQH/xAAXAAEBAQEAAAAAAAAAAAAAAAAAAQIH/8QAFxABAQEBAAAAAAAAAAAAAAAAAAERAv/EABUBAQEAAAAAAAAAAAAAAAAAAAAB/8QAFBEBAAAAAAAAAAAAAAAAAAAAAP/aAAwDAQACEQMRAD8A4euBqC4mGkoJjUTCgqWJFAipiAtSLgBpqUkAaiYYCpUXASKmGg0zV1KCyKzFAwFgIKAmFUBmmNUBMURAxJDRRpDTQWFZlUCFMUENNKBFTAA0kUE0LEwCi4WAQQwBYlhAXENUEiooAAGmEXQTDVARFUEwxQEoVIDQSpQDE0AjSYYAUwBGkLAVmmNAzqmJgLqashgKlQgFWUwwAC0EipqygrNjVqaCGKoMqqApqGAaqGAqISApFgCaLUAxQQE0kNUTVXGaC6kq4AAgLamrgCiGgUTAGkAEqmgGFgoJAUE0VMBdEVBCGmqKIIFEoo0IUCkMMA1CrIBppiAsSxdXQZXSpIBqa1IWAQRZQKQwAUEGbDFpFDBQEEsWAVZSxkGhmVRSxUoIqKIJSLaiigiBYaWigoiBV1FUQsVAQ00gKiyqDK6WJgFIYAqpKAlFABdARUsIBSGJgNCLqDNgtFDDFNBmKtQExVATQqwEkUNBlpKAqLqaguM6uiiYY0mggWgBKpYArC6BYSCgmLIaSgqBAVNMMBRNNAqRSAGKAmqICauqAkVMMAMUBApoLASopQFQNVmguGIugmCmgkq6i4CLDEBpmxQElUsANIYYACaDTOLqaCyLiSqCaLUgGimgkVMIBooCaKkoLiYaaBioSAuiEAIYYAJaSgVcLTQTCKYBqsgGLE1ZQMSKoAlTQaTBAaQ00DDTQBMaASVUIBUaATQ00FMQ0CwDAUZpIC2iwBJVTAAhiYDSWGqDOmrQE1TAFTCAEoUBNWGFgCsroFiYupoLhi6AmprSYAYaugmmlpKBhgaBeSBgKM1QNTWgGdXSoCloAsABKkAFwgAKAMrgAmAA0AKJIAi4AipYkBUWGACUAFwwALCACgAlJABKoAQwAf/Z";
        }


        //Trả về BitmapImage lấy từ Html Source
        public static BitmapImage BitmapImageFromHtmlSource(string keyWord)
        {
            string path = GetImageSoureHtml(keyWord);
            //đổi chuỗi base64 sang dạng binary 
            var base64Data = Regex.Match(path, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);

            //nếu muốn save thì thêm hàng này
            //File.WriteAllBytes("E:\\keyWord.jpeg", binData);

            //khai báo BitmapImage để trả về

            BitmapImage bitmapImage = new BitmapImage();
            //Khởi tạo cho BitmapImage
            bitmapImage.BeginInit();
            using (var stream = new MemoryStream(binData))
            {
                {
                    //ghi bitmap từ chuỗi nhị phân binary vừa đổi được từ dạng base64
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
            }
            return bitmapImage;
        }


        public static BitmapImage GetBitmapImageFromFile(string keyWord)
        {
            BitmapImage bitmapImageWord = new BitmapImage();
            bitmapImageWord.BeginInit();

            if (File.Exists(@".\Resources\ImageSource\" + keyWord + ".jpeg") == true)
            {
                bitmapImageWord.UriSource = new Uri(solutionPath+ @"bin\Debug\Resources\ImageSource\" + keyWord + ".jpeg", UriKind.Absolute);
            }
            else
                return null;
            bitmapImageWord.EndInit();
            return bitmapImageWord;
        }
    }
}

