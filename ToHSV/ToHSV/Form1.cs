using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using System.Xml.Schema;

namespace ToHSV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // диалог для выбора файла
            OpenFileDialog ofd = new OpenFileDialog();
            // фильтр форматов файлов
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            // если в диалоге была нажата кнопка ОК
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // загружаем изображение
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                }
                catch // в случае ошибки выводим MessageBox
                {
                    MessageBox.Show("Невозможно открыть выбранный файл", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap input = new Bitmap(pictureBox1.Image);
                Bitmap output = new Bitmap(input.Width, input.Height);

                for (int i = 0; i < input.Height; i++)
                {
                    for (int j = 0; j < input.Width; j++)
                    {
                        Color pixel = input.GetPixel(j, i);

                        double R = (double)pixel.R / 255;
                        double G = (double)pixel.G / 255;
                        double B = (double)pixel.B / 255;
                        double max = Math.Max(Math.Max(R, G), B);
                        double min = Math.Min(Math.Min(R, G), B);

                        double hue = get_hue(max, min, R, G, B);
                        double saturation = get_saturation(max, min);
                        double v = get_value(max);

                        hue = update_hue(hue);
                        saturation = update_saturation(saturation);
                        v = update_value(v);

                        int newR = 0;
                        int newG = 0;
                        int newB = 0;
                        hsv_to_rgb(hue, saturation, v, ref newR, ref newG, ref newB);

                        output.SetPixel(j, i, Color.FromArgb(newR, newG, newB));
                    }
                }
                pictureBox2.Image = output;
            }
        }

        public void hsv_to_rgb(double hue, double saturation, double v, ref int R, ref int G, ref int B)
        {
            double h = Math.Floor(hue / 60) % 6;
            double f = (hue / 60) - Math.Floor(hue / 60);
            double p = v * (1 - saturation);
            double q = v * (1 - f * saturation);
            double t = v * (1 - (1 - f) * saturation);

            double updated_R = 0.0;
            double updated_G = 0.0;
            double updated_B = 0.0;

            switch (h)
            {
                case 0:
                    updated_R = v;
                    updated_G = t;
                    updated_B = p;
                    break;
                case 1:
                    updated_R = q;
                    updated_G = v;
                    updated_B = p;
                    break;
                case 2:
                    updated_R = p;
                    updated_G = v;
                    updated_B = t;
                    break;
                case 3:
                    updated_R = p;
                    updated_G = q;
                    updated_B = v;
                    break;
                case 4:
                    updated_R = t;
                    updated_G = p;
                    updated_B = v;
                    break;
                case 5:
                    updated_R = v;
                    updated_G = p;
                    updated_B = q;
                    break;
                default:
                    break;
            }

            R = (int)Math.Truncate(updated_R * 255);
            G = (int)Math.Truncate(updated_G * 255);
            B = (int)Math.Truncate(updated_B * 255);
        }

        public double get_hue(double max, double min, double R, double G, double B)
        {
            double hue = 0.0;
            if (max == min)
            {
                hue = 0.0;
            }
            else if (max == R && G >= B)
            {
                hue = 60 * ((G - B) / (max - min));
            }
            else if (max == R && G < B)
            {
                hue = 60 * ((G - B) / (max - min)) + 360;
            }
            else if (max == G)
            {
                hue = 60 * ((B - R) / (max - min)) + 120;
            }
            else if (max == B)
            {
                hue = 60 * ((R - G) / (max - min)) + 240;
            }

            return hue;
        }

        public double get_saturation(double max, double min)
        {
            double saturation = 0.0;
            if (max == 0)
            {
                saturation = 0.0;
            }
            else
            {
                saturation = 1 - (min / max);
            }

            return saturation;
        }

        public double get_value(double max)
        {
            return (double)max;
        }

        public double update_hue(double hue)
        {
            double updated_hue = hue + (double)numericUpDown1.Value;
            if (updated_hue > 360)
            {
                updated_hue -= 360;
            }
            else if (updated_hue < 0)
            {
                updated_hue += 360;
            }

            return updated_hue;
        }

        public double update_saturation(double saturation)
        {
            double updated_saturation = saturation + (double)numericUpDown2.Value;
            if (updated_saturation < 0)
            {
                updated_saturation = 0;
            }
            else if (updated_saturation > 1)
            {
                updated_saturation = 1;
            }

            return updated_saturation;
        }

        public double update_value(double v)
        {
            double updated_value = v + (double)numericUpDown3.Value;
            if (updated_value < 0)
            {
                updated_value = 0;
            }
            else if (updated_value > 1)
            {
                updated_value = 1;
            }

            return updated_value;
        }

    }
}
