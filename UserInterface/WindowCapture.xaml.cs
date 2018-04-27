using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Diagnostics;
namespace UserInterface
{
    /// <summary>
    /// Interaction logic for WindowCapture.xaml
    /// </summary>
    public partial class WindowCapture : Window
    {
        public WindowCapture()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.Background = new SolidColorBrush(Colors.White) { Opacity = 0.1 };
            this.Topmost = true;
            this.AllowsTransparency = true;
            this.Cursor = Cursors.Cross;
        }
        #region declare some value
        System.Windows.Point startPoint = new System.Windows.Point();
        System.Windows.Point nowPoint = new System.Windows.Point();
        System.Windows.Point currentTopLeft = new System.Windows.Point();
        System.Windows.Point currentBottonRight = new System.Windows.Point();

        enum CheckMouseAction : int
        {
            NOTHING = 0,
            CLICKED = 1,
            RELEASE = 2
        }

        bool boolStart = false;
        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        CheckMouseAction checkMouseAction = CheckMouseAction.NOTHING;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            boolStart = true;
            startPoint = e.GetPosition(this);
            currentTopLeft = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && boolStart == true)
            {

                checkMouseAction = CheckMouseAction.CLICKED;
                nowPoint = e.GetPosition(this);
                CanvasCapture.Children.Clear();

                if (startPoint.X >= nowPoint.X && startPoint.Y >= nowPoint.Y)
                {
                    currentTopLeft.X = nowPoint.X;
                    currentTopLeft.Y = nowPoint.Y;
                    currentBottonRight.X = startPoint.X;
                    currentBottonRight.Y = startPoint.Y;

                }
                if (startPoint.X <= nowPoint.X && startPoint.Y >= nowPoint.Y)
                {
                    currentTopLeft.X = startPoint.X;
                    currentTopLeft.Y = nowPoint.Y;
                    currentBottonRight.X = nowPoint.X;
                    currentBottonRight.Y = startPoint.Y;
                }
                if (startPoint.X >= nowPoint.X && startPoint.Y <= nowPoint.Y)
                {
                    currentTopLeft.X = nowPoint.X;
                    currentTopLeft.Y = startPoint.Y;
                    currentBottonRight.X = startPoint.X;
                    currentBottonRight.Y = nowPoint.Y;
                }
                if (startPoint.X <= nowPoint.X && startPoint.Y <= nowPoint.Y)
                {
                    currentTopLeft.X = startPoint.X;
                    currentTopLeft.Y = startPoint.Y;
                    currentBottonRight.X = nowPoint.X;
                    currentBottonRight.Y = nowPoint.Y;
                }

                //DrawRectangle
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                {
                    Width = Math.Abs(currentBottonRight.X - currentTopLeft.X),
                    Height = Math.Abs(currentBottonRight.Y - currentTopLeft.Y),
                    Stroke = new SolidColorBrush(Colors.Green),

                    StrokeDashArray = new DoubleCollection() { 5, 1 },
                    StrokeThickness = 2
                };

                Canvas.SetLeft(rect, currentTopLeft.X);
                Canvas.SetTop(rect, currentTopLeft.Y);

                CanvasCapture.Children.Add(rect);
            }
            if (e.LeftButton == MouseButtonState.Released && checkMouseAction == CheckMouseAction.CLICKED)
            {
                checkMouseAction = CheckMouseAction.RELEASE;
                int Width = Convert.ToInt32(currentBottonRight.X - currentTopLeft.X - 10);
                int Height = Convert.ToInt32(currentBottonRight.Y - currentTopLeft.Y - 10);
                Bitmap bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(Convert.ToInt32(currentTopLeft.X), Convert.ToInt32(currentTopLeft.Y), 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                bitmap.Save(@".\Resources\WindowImageToText\bin\Debug\capture.png", System.Drawing.Imaging.ImageFormat.Png);
                //System.Diagnostics.Process.Start(@"C:\Users\NAM\Documents\Visual Studio 2017\Projects\NewDictionary\WindowImageToText\bin\Debug\WindowImageToText.exe");

                //ProcessStartInfo start = new ProcessStartInfo();

                //start.FileName = @"C:\Users\NAM\Documents\Visual Studio 2017\Projects\NewDictionary\WindowImageToText\bin\Debug\WindowImageToText.exe";

                string programPath = @".\Resources\WindowImageToText\bin\Debug\";
                Process proc = new Process();
                proc.StartInfo.FileName = "WindowImageToText.exe";
                proc.StartInfo.WorkingDirectory = programPath;
                proc.Start();

                this.Close();
            }
        }
    }
}
