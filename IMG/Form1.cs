using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IMG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static string projPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        private static string sourceImgPath = projPath + "\\receipt.jpg";
        private static string finishImgPath = projPath + "\\finish.jpg";

        private void button1_Click(object sender, EventArgs e)
        {
            useImage();
        }

        private void useImage()
        {
            //Mat mat = CvInvoke.Imread(sourceImgPath, ImreadModes.Unchanged);
            //Matrix<Byte> matrix = new Matrix<Byte>(mat.Rows, mat.Cols, mat.NumberOfChannels);
            //mat.CopyTo(matrix);
            //CvInvoke.Imshow("IntPtr", matrix);

            //imageBox1.Image = destIMG;

            Image<Gray, Byte> img = removeBackground();
            //imageBox1.Image = img;
            //CvInvoke.Imshow("Image<Gray, Byte>", img);

            img = img.SmoothMedian(3);

            img = img.Erode(1);
            //img = img.Dilate(1);

            //img = img.SmoothMedian(5);

            img.Save(finishImgPath);
            //CvInvoke.Imwrite(finishImgPath, img);
        }

        private Image<Gray, Byte> removeBackground()
        {
            using (Image<Bgr, Byte> sourceIMG = new Image<Bgr, Byte>(sourceImgPath))
            {
                Image<Gray, Byte> destIMG = new Image<Gray, Byte>(sourceIMG.Width, sourceIMG.Height);
                for (int i = 0; i < sourceIMG.Height; i++)
                {
                    for (int j = 0; j < sourceIMG.Width; j++)
                    {
                        Bgr pixel = sourceIMG[i, j];
                        double T = Convert.ToDouble(textBox1.Text);
                        if (pixel.Red < T && pixel.Green < T && pixel.Blue < T)
                        {
                            destIMG.Data[i, j, 0] = 0;
                        }
                        else
                        {
                            destIMG.Data[i, j, 0] = 255;
                        }
                    }
                }
                return destIMG;
            }
        }

        private void version1()
        {
            String win1 = "Test Window"; //The name of the window
            CvInvoke.NamedWindow(win1); //Create the window using the specific name

            Mat img = new Mat(200, 400, DepthType.Cv8U, 3); //Create a 3 channel image of 400x200
            img.SetTo(new Bgr(255, 0, 0).MCvScalar); // set it to Blue color

            //Draw "Hello, world." on the image using the specific font
            CvInvoke.PutText(
               img,
               "Hello, worldm ABC",
               new System.Drawing.Point(10, 80),
               FontFace.HersheyComplex,
               1.0,
               new Bgr(0, 255, 0).MCvScalar);

            CvInvoke.Imshow(win1, img); //Show the image
            CvInvoke.WaitKey(0);  //Wait for the key pressing event
        }
        private void version2()
        {
            //Create a 3 channel image of 400x200
            using (Mat img = new Mat(200, 400, DepthType.Cv8U, 3))
            {
                img.SetTo(new Bgr(255, 0, 0).MCvScalar); // set it to Blue color

                //Draw "Hello, world." on the image using the specific font
                CvInvoke.PutText(
                   img,
                   "Hello, world",
                   new System.Drawing.Point(10, 80),
                   FontFace.HersheyComplex,
                   1.0,
                   new Bgr(0, 255, 0).MCvScalar);

                //Show the image using ImageViewer from Emgu.CV.UI
                ImageViewer.Show(img, "Test Window");
            }
        }

        private void netClass()
        {
            using (Bitmap sourceImg = new Bitmap(projPath + "\\receipt.jpg"))
            {
                using (Bitmap saveImg = new Bitmap(sourceImg.Width, sourceImg.Height))
                {
                    for (int i = 0; i < sourceImg.Width; i++)
                    {
                        for (int j = 0; j < sourceImg.Height; j++)
                        {
                            Color pixel = sourceImg.GetPixel(i, j);

                            saveImg.SetPixel(i, j, pixel);
                        }
                    }

                    saveImg.Save(finishImgPath);
                }
            }
        }
    }
}
