using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;






namespace ShadesOfGray
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // кнопка Открыть
        private void openButton_Click(object sender, EventArgs e)
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



        // кнопка Ч/Б
        private void grayButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) // если изображение в pictureBox1 имеется
            {
               
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(pictureBox1.Image);

                Bitmap output1 = new Bitmap(input.Width, input.Height);
                Bitmap output2 = new Bitmap(input.Width, input.Height);
                Bitmap output3 = new Bitmap(input.Width, input.Height);
                Bitmap output4 = new Bitmap(512, pictureBox5.Height);

                int[] gist = new int[256];

                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий

                        float R1, R2, G1, G2, B1, B2;
                        R1 = G1 = B1 = (R + G + B) / 3.0f;
                        R2 = G2 = B2 = 0.2126f * R + 0.7152f * G + 0.0722f * B;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel1 = 0xFF000000 | ((UInt32)R1 << 16) | ((UInt32)G1 << 8) | ((UInt32)B1);
                        // добавляем его в Bitmap нового изображения
                        output1.SetPixel(i, j, Color.FromArgb((int)newPixel1));

                        UInt32 newPixel2 = 0xFF000000 | ((UInt32)R2 << 16) | ((UInt32)G2 << 8) | ((UInt32)B2);

                        output2.SetPixel(i, j, Color.FromArgb((int)newPixel2));

                        gist[(int)R2]++;

                        output3.SetPixel(i, j, Color.FromArgb((int)(newPixel1 - newPixel2)));
                    }
     
                pictureBox2.Image = output1;
                pictureBox3.Image = output2;
                pictureBox4.Image = output3;


                double point = (double)gist.Max() / pictureBox5.Height;
                for (int i = 0; i < 512; i += 2)
                {
                    for (int j = pictureBox5.Height - 1; (j > pictureBox5.Height - (gist[i / 2] / point)) && (j >= 0); --j)
                    {
                        output4.SetPixel(i, j, Color.Gray);
                        output4.SetPixel(i + 1, j, Color.Gray);
                    }
                }

                pictureBox5.Image = output4;


            }
        }

    }
}