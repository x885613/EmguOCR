using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private readonly static string PROJ_PATH = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        private readonly static string SOURCE_IMG_PATH = PROJ_PATH + "\\receipt.jpg";
        private readonly static string FINISH_IMG_PATH = PROJ_PATH + "\\finish.bmp";
        private readonly static string OCR_FOLDER_PATH = PROJ_PATH + "\\SeparateIMG";

        private readonly static Gray DRAW_COLOR = new Gray(150); //畫ROI的線顏色

        private void button1_Click(object sender, EventArgs e)
        {
            getBackgroundIMG();

            separateIMG();
        }

        private void separateIMG()
        {
            //取得ROI區塊
            IDictionary<int, Image<Gray, Byte>> dcROI = new Dictionary<int, Image<Gray, Byte>>();
            using (Image<Gray, Byte> sourceIMG = new Image<Gray, Byte>(FINISH_IMG_PATH))
            {
                getROI(dcROI, sourceIMG);
                imageBox1.Image = sourceIMG;
            }

            try
            {
                if (!Directory.Exists(OCR_FOLDER_PATH))
                {
                    Directory.CreateDirectory(OCR_FOLDER_PATH);
                }
                else
                {
                    string[] files = Directory.GetFiles(OCR_FOLDER_PATH);
                    if (files.Length > 0)
                    {
                        foreach (string file in files)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch
            {
            }

            //對ROI區塊做分割
            foreach (KeyValuePair<int, Image<Gray, Byte>> kvp in dcROI)
            {
                CvInvoke.Imshow(kvp.Key.ToString(), kvp.Value);

                Image<Gray, Byte> img = kvp.Value;

                //直方圖統計
                int[] his = new int[img.Width];
                int count = 0;
                for (int i = 0; i < img.Width; i++)
                {
                    count = 0;
                    for (int j = 0; j < img.Height; j++)
                    {
                        if (img[j, i].Intensity == 0)
                        {
                            count++;
                        }
                    }
                    his[i] = count;
                }

                //根據直方圖做分割圖片
                count = 0;
                int idxLeft = 0; //左邊邊界
                int idxRight = 0; //右邊邊界
                int Threshold = 5; //過濾掉雜訊，將小型的黑色區塊視而不見
                for (int i = 0; i < his.Length; i++)
                {
                    if (his[i] > 0)
                    {
                        idxRight = i;
                        count++;
                    }
                    else
                    {
                        if (count > Threshold)
                        {
                            //分割
                            using (Image<Gray, Byte> separateIMG = new Image<Gray, Byte>(idxRight - idxLeft + 1, img.Height))
                            {
                                for (int x = 0; x < img.Height; x++)
                                {
                                    for (int y = idxLeft; y <= idxRight; y++)
                                    {
                                        byte color = img.Data[x, y, 0];
                                        if (DRAW_COLOR.Intensity == color) //DRAW_COLOR是畫ROI的線，要視而不見
                                        {
                                            separateIMG.Data[x, y - idxLeft, 0] = 255;
                                        }
                                        else
                                        {
                                            separateIMG.Data[x, y - idxLeft, 0] = color;
                                        }
                                    }
                                }
                                separateIMG.Save(OCR_FOLDER_PATH + "\\" + kvp.Key + "-" + i + ".jpg");
                            }
                        }
                        idxLeft = i;
                        count = 0;
                    }
                }

                //直方圖影像
                //using (Image<Gray, Byte> binaryIMG = new Image<Gray, byte>(img.Width, img.Height, new Gray(255)))
                //{
                //    for (int i = 0; i < his.Length; i++)
                //    {
                //        for (int j = 0; j < his[i]; j++)
                //        {
                //            binaryIMG.Data[j, i, 0] = 0;
                //        }
                //    }

                //    binaryIMG.Save(OCR_FOLDER_PATH + "\\" + kvp.Key + ".png");
                //}
            }
        }

        private void getROI(IDictionary<int, Image<Gray, Byte>> dcROI, Image<Gray, Byte> sourceIMG)
        {
            Size word2 = new Size(100, 50);
            Size word6 = new Size(300, 50);
            Size word10 = new Size(360, 50);
            Size word24 = new Size(800, 50);
            //左上角號碼
            Point p = new Point(250, 173);
            Rectangle rect = new Rectangle(p, word10);
            drawIMG(sourceIMG, rect);
            dcROI.Add(1, getSubIMG(p, word10, sourceIMG));

            p = new Point(250, 273);
            rect = new Rectangle(p, word10);
            drawIMG(sourceIMG, rect);
            dcROI.Add(2, getSubIMG(p, word10, sourceIMG));

            //右下角文字
            p = new Point(1909, 594);
            rect = new Rectangle(p, word6);
            drawIMG(sourceIMG, rect);
            dcROI.Add(3, getSubIMG(p, word6, sourceIMG));

            p = new Point(1350, 638);
            rect = new Rectangle(p, word10);
            drawIMG(sourceIMG, rect);
            dcROI.Add(4, getSubIMG(p, word10, sourceIMG));

            p = new Point(1350, 690);
            rect = new Rectangle(p, word10);
            drawIMG(sourceIMG, rect);
            dcROI.Add(5, getSubIMG(p, word10, sourceIMG));

            p = new Point(1350, 738);
            rect = new Rectangle(p, word10);
            drawIMG(sourceIMG, rect);
            dcROI.Add(6, getSubIMG(p, word10, sourceIMG));

            p = new Point(1350, 788);
            rect = new Rectangle(p, word24);
            drawIMG(sourceIMG, rect);
            dcROI.Add(7, getSubIMG(p, word24, sourceIMG));

            p = new Point(1350, 836);
            rect = new Rectangle(p, word10);
            drawIMG(sourceIMG, rect);
            dcROI.Add(8, getSubIMG(p, word10, sourceIMG));

            //中間文字
            p = new Point(120, 415);
            rect = new Rectangle(p, word6);
            drawIMG(sourceIMG, rect);
            dcROI.Add(9, getSubIMG(p, word6, sourceIMG));

            p = new Point(639, 425);
            rect = new Rectangle(p, word2);
            drawIMG(sourceIMG, rect);
            dcROI.Add(10, getSubIMG(p, word2, sourceIMG));

            p = new Point(1011, 425);
            rect = new Rectangle(p, word2);
            drawIMG(sourceIMG, rect);
            dcROI.Add(11, getSubIMG(p, word2, sourceIMG));

            p = new Point(991, 626);
            rect = new Rectangle(p, word2);
            drawIMG(sourceIMG, rect);
            dcROI.Add(12, getSubIMG(p, word2, sourceIMG));

            p = new Point(1033, 682);
            rect = new Rectangle(p, word2);
            drawIMG(sourceIMG, rect);
            dcROI.Add(13, getSubIMG(p, word2, sourceIMG));

            p = new Point(995, 728);
            rect = new Rectangle(p, word2);
            drawIMG(sourceIMG, rect);
            dcROI.Add(14, getSubIMG(p, word2, sourceIMG));

            p = new Point(160, 816);
            rect = new Rectangle(p, new Size(1000, 75));
            drawIMG(sourceIMG, rect);
            dcROI.Add(15, getSubIMG(p, new Size(1000, 75), sourceIMG));
        }

        private Image<Gray, Byte> getSubIMG(Point p, Size s, Image<Gray, Byte> sourceIMG)
        {
            Image<Gray, Byte> img = new Image<Gray, Byte>(s.Width, s.Height);
            for (int i = 0; i < s.Height; i++)
            {
                for (int j = 0; j < s.Width; j++)
                {
                    img[i, j] = sourceIMG[p.Y + i, p.X + j];
                }
            }
            return img;
        }

        private void drawIMG(Image<Gray, Byte> img, Rectangle rect)
        {
            img.Draw(rect, DRAW_COLOR, 2, LineType.FourConnected, 0);
        }

        private void getBackgroundIMG()
        {
            //Mat mat = CvInvoke.Imread(sourceImgPath, ImreadModes.Unchanged);
            //Matrix<Byte> matrix = new Matrix<Byte>(mat.Rows, mat.Cols, mat.NumberOfChannels);
            //mat.CopyTo(matrix);
            //CvInvoke.Imshow("IntPtr", matrix);

            Image<Gray, Byte> img = removeBackground();
            img = img.SmoothMedian(3);
            img = img.Erode(1);
            img.Save(FINISH_IMG_PATH);
            //CvInvoke.Imwrite(FINISH_IMG_PATH, img);
        }

        private Image<Gray, Byte> removeBackground()
        {
            using (Image<Bgr, Byte> sourceIMG = new Image<Bgr, Byte>(SOURCE_IMG_PATH))
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

        private void netClass()
        {
            using (Bitmap sourceImg = new Bitmap(SOURCE_IMG_PATH))
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

                    saveImg.Save(FINISH_IMG_PATH);
                }
            }
        }
    }
}
