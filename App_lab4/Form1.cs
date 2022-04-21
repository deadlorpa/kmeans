namespace App_lab4
{
    public partial class Form1 : Form
    {
        private static string path = "";
        private static Image img= null;
        private static int iterCount = 100;
        private static int iterPeriod = 10;
        private static bool runMega = false;
        public Form1()
        {
            InitializeComponent();
        }

        private List<int> recalculate(List<int> centroids, List<Pixel> pixels)
        {
            Dictionary<int, List<int>> clusters = new Dictionary<int, List<int>>();
            foreach(int centroid in centroids)
            {
                clusters.Add(centroid, new List<int>());
            }
            foreach (Pixel pixel in pixels)
            {

                long dist = long.MaxValue;
                int newCluster = pixel.cluster;
                foreach (int centroid in centroids)
                {
                    long d = pixel.distance(pixels[centroid]);

                    if (d < dist)
                    {
                        dist = d;
                        newCluster = centroid;
                    }

                }
                pixel.cluster = newCluster;

            }
            foreach (Pixel pixel in pixels)
            {
                clusters[pixel.cluster].Add(pixel.i);
            }
            List<int> nexGeneration = new List<int>(clusters.Count);
            foreach (var vals in clusters)
            {
                List<KeyValuePair<long, int>> data = new List<KeyValuePair<long, int>>();
                foreach (int i in vals.Value)
                {
                    Pixel p = pixels[i];
                    data.Add(new KeyValuePair<long, int>(p.distance(pixels[p.cluster]), p.i));
                }
                data = data.OrderBy(x => x.Key).ToList();
                nexGeneration.Add(data[data.Count / 2].Value);
            }
            nexGeneration.Sort();
            return nexGeneration;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (runMega == true)
            {
                runMega = false;
                button1.Enabled = false;
                Thread.Sleep(1000);
                button1.Enabled = true;
            }
            else
            {
                runMega = true;
                if (img == null)
                {
                    MessageBox.Show("А можно картинку?");
                    return;
                }
                textBox2.Enabled = false;
                listView1.Items.Clear();
                imageList1.Images.Clear();

                int k = Int32.Parse(textBox1.Text);
                iterCount = Int32.Parse(textBox3.Text);
                iterPeriod = Int32.Parse(textBox5.Text);
                if (iterCount < iterPeriod)
                {
                    MessageBox.Show("Так не пойдёт");
                    return;
                }
                progressBar1.Maximum = iterCount;
                progressBar1.Value = 0;
                progressBar1.Step = iterPeriod;
                progressBar1.Invalidate();
                Bitmap image = new Bitmap(img);
                int height = image.Height;
                int width = image.Width;
                List<Pixel> pixels = new List<Pixel>();
                int n = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color color = image.GetPixel(x, y);
                        pixels.Add(new Pixel(color.R, color.G, color.B, x, y, n++));
                    }
                }
                List<int> clusters = new List<int>();
                int i = 0;
                while (clusters.Count != k)
                {
                    Random random = new Random();

                    int point = random.Next(i * width / k, width) + random.Next(0, height) * width;
                    bool exist = false;
                    foreach (var cluster in clusters)
                    {
                        if (pixels[point].r == pixels[cluster].r && pixels[point].g == pixels[cluster].g && pixels[point].b == pixels[cluster].b)
                        {
                            MessageBox.Show("И такое бывает - рандом выдал два одинаковых центроида!");
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        clusters.Add(point);
                        pixels[point].cluster = point;
                        Bitmap Bmp = new Bitmap(32, 32);
                        using (Graphics gfx = Graphics.FromImage(Bmp))
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(pixels[point].r, pixels[point].g, pixels[point].b)))
                        {
                            gfx.FillRectangle(brush, 0, 0, 32, 32);
                        }
                        imageList1.Images.Add(Bmp);
                        var item = new ListViewItem();
                        item.ImageIndex = imageList1.Images.Count - 1;
                        item.Text = "(" + pixels[point].r.ToString() + ";" + pixels[point].g.ToString() + ";" + pixels[point].b.ToString() + ")";
                        item.Tag = pixels[point].x.ToString() + ";" + pixels[point].y.ToString();
                        listView1.Items.Add(item);
                    }
                }
                listView1.Invalidate();

                int count = 0;
                bool run = true;
                var tr = new Thread(delegate ()
                {
                    while (run && runMega)
                    {

                        List<int> newClusters = recalculate(clusters, pixels);
                        run = false;
                        for (int i = 0; i < k; i++)
                        {
                            if (clusters[i] != newClusters[i])
                            {
                                run = true;
                            }
                        }
                        count = count + 1;
                        clusters = newClusters;
                        Invoke((MethodInvoker)delegate
                        {
                            textBox4.Text = count.ToString();
                        });
                        if (count == iterCount)
                        {
                            run = false;
                            Invoke((MethodInvoker)delegate
                            {
                                progressBar1.Value = progressBar1.Maximum;
                            });
                            break;
                        }
                        if (count % iterPeriod == 0)
                        {

                            Invoke((MethodInvoker)delegate
                            {
                                textBox4.Text = "не пугайтесь, я рисую:)";
                            });
                            var trr = new Thread(delegate ()
                                {
                                    Invoke((MethodInvoker)delegate
                                    {


                                        listView2.Items.Clear();
                                        imageList2.Images.Clear();

                                        foreach (var cluster in clusters)
                                        {
                                            int point = cluster;
                                            Bitmap Bmp = new Bitmap(32, 32);
                                            using (Graphics gfx = Graphics.FromImage(Bmp))
                                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(pixels[point].r, pixels[point].g, pixels[point].b)))
                                            {
                                                gfx.FillRectangle(brush, 0, 0, 32, 32);
                                            }

                                            imageList2.Images.Add(Bmp);

                                            var item = new ListViewItem();
                                            item.ImageIndex = imageList2.Images.Count - 1;
                                            item.Text = "(" + pixels[point].r.ToString() + ";" + pixels[point].g.ToString() + ";" + pixels[point].b.ToString() + ")";
                                            item.Tag = pixels[point].x.ToString() + ";" + pixels[point].y.ToString();

                                            listView2.Items.Add(item);

                                            Bitmap result = new Bitmap(width, height);
                                            foreach (Pixel pixel in pixels)
                                            {
                                                Pixel c = pixels[pixel.cluster];
                                                result.SetPixel(pixel.x, pixel.y, Color.FromArgb(c.r, c.g, c.b));

                                            }
                                            Image rest = result;
                                            rest.Save(path.Substring(0, path.Length - 4) + "_new.jpg");

                                            pictureBox2.Image = rest;

                                        }
                                        progressBar1.PerformStep();
                                    });
                                });
                            trr.Start();

                        }

                    }

                    var trr1 = new Thread(delegate ()
                    {
                        Invoke((MethodInvoker)delegate
                        {


                            listView2.Items.Clear();
                            imageList2.Images.Clear();

                            foreach (var cluster in clusters)
                            {
                                int point = cluster;
                                Bitmap Bmp = new Bitmap(32, 32);
                                using (Graphics gfx = Graphics.FromImage(Bmp))
                                using (SolidBrush brush = new SolidBrush(Color.FromArgb(pixels[point].r, pixels[point].g, pixels[point].b)))
                                {
                                    gfx.FillRectangle(brush, 0, 0, 32, 32);
                                }

                                imageList2.Images.Add(Bmp);

                                var item = new ListViewItem();
                                item.ImageIndex = imageList2.Images.Count - 1;
                                item.Text = "(" + pixels[point].r.ToString() + ";" + pixels[point].g.ToString() + ";" + pixels[point].b.ToString() + ")";
                                item.Tag = pixels[point].x.ToString() + ";" + pixels[point].y.ToString();

                                listView2.Items.Add(item);

                                Bitmap result = new Bitmap(width, height);
                                foreach (Pixel pixel in pixels)
                                {
                                    Pixel c = pixels[pixel.cluster];
                                    result.SetPixel(pixel.x, pixel.y, Color.FromArgb(c.r, c.g, c.b));

                                }
                                Image rest = result;
                                rest.Save(path.Substring(0, path.Length - 4) + "_new.jpg");

                                pictureBox2.Image = rest;

                            }
                            
                        });

                    });
                        trr1.Start();
                        Bitmap resul = new Bitmap(width, height);
                    foreach (Pixel pixel in pixels)
                    {
                        Pixel c = pixels[pixel.cluster];
                        resul.SetPixel(pixel.x, pixel.y, Color.FromArgb(c.r, c.g, c.b));

                    }
                    Image res = resul;
                    res.Save(path.Substring(0, path.Length - 4) + "_new.jpg");
                    Invoke((MethodInvoker)delegate
                    {
                        textBox4.Text = count.ToString();
                        pictureBox2.Image = res;
                        textBox2.Enabled = true;
                    });
                });
                tr.Start();
            }
        }


        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog chooseFile = new OpenFileDialog();
            if (chooseFile.ShowDialog() == DialogResult.Cancel)
                return;
            path = chooseFile.FileName;
            try
            {
                img = Image.FromFile(path);
                pictureBox1.Image = img;
                textBox2.Text = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Меня где-то обманули");
            }
        }
    }
}