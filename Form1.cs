using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        static Random rnd = new Random();

        readonly GameScore score = new GameScore();

        public Form1()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            timer1.Enabled = false;
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                EndGame();
            }
            else
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            toolStripButton1.Text = "Stop";
            toolStripButton1.BackColor = Color.FromArgb(255, 128, 128);
            if (int.TryParse(toolStripTextBox1.Text, out int count) && count > 0)
            {
                PnlGameField.Controls.Clear();
                for (int i = 0; i < count; i++)
                {
                    AddRandomBox();
                }
                score.Reset();
                toolStripTextBox2.Text = score.ToString();
                timer1.Enabled = true;
            }
        }
        private void EndGame()
        {
            toolStripButton1.Text = "Start";
            toolStripButton1.BackColor = Color.FromArgb(128, 255, 128);
            timer1.Interval = 500;
            timer1.Enabled = false;
        }

        private bool AddRandomBox()
        {
            var boxes = PnlGameField.Controls.OfType<PictureBox>().ToList();
            if (CheckCollisions(boxes, out int indexA, out int indexB))
            {
                return false;
            }
            //Object Box is created with 20 Pixels height and width and a random color is generated for their background
            var picBox = new PictureBox();
            picBox.Size = new Size(20, 20);
            picBox.BackColor = Color.FromArgb(rnd.Next(10, 245), rnd.Next(10, 245), rnd.Next(10, 245));

            //Event handler will be added to click the box event 
            picBox.Click += PicBox_Click;

            //Box will be added to the game field with 30 Pixels distance to the edges
            boxes.Add(picBox);
            do
            {
                picBox.Location = new Point(rnd.Next(0, PnlGameField.Width - 30), rnd.Next(0, PnlGameField.Height - 30));
            } while (CheckCollisions(boxes, out _, out _));
            PnlGameField.Controls.Add(picBox);
            return true;
        }

        private void PicBox_Click(object sender, EventArgs e)
        {
            var target = (PictureBox)sender;
            if (timer1.Enabled)
            {
                RemoveBox(target);
            }
        }
        private void RemoveBox(PictureBox box)
        {
            score.Add(box.Width);
            box.Dispose();
            PnlGameField.Controls.Remove(box);
            if (!AddRandomBox())
            {
                EndGame();
            }
            toolStripTextBox2.Text = score.ToString();
            PnlGameField.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (rnd.Next(1000) < timer1.Interval-1)
            {
                timer1.Interval += 1;
            }
            var boxes = PnlGameField.Controls.OfType<PictureBox>().ToArray();
            foreach (var box in boxes)
            {
                box.Size = new Size(box.Size.Width + 2, box.Size.Height + 2);
                box.Location = new Point(box.Location.X - 1, box.Location.Y - 1);
            }
            if (CheckCollisions(boxes, out int indexA, out int indexB))
            {
                if (indexA >= 0)
                {
                    boxes[indexA].BorderStyle = BorderStyle.FixedSingle;
                }
                if (indexB >= 0)
                {
                    boxes[indexA].BorderStyle = BorderStyle.FixedSingle;
                }
                EndGame();
            }
            PnlGameField.Invalidate();
        }
        private bool CheckCollisions()
            => CheckCollisions(PnlGameField.Controls.OfType<PictureBox>().ToArray(), out _, out _);

        private bool CheckCollisions(IList<PictureBox> boxes, out int indexA, out int indexB)
        {
            
            for (int i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                indexA = i;
                if (box.Left < 0 || box.Right >= PnlGameField.Width
                    || box.Top < 0 || box.Bottom >= PnlGameField.Height)
                {
                    indexB = -1;
                    return true;
                }
                for (int j = i+1; j < boxes.Count; j++)
                {
                    indexB = j;
                    var other = boxes[j];
                    if (box.Bounds.IntersectsWith(other.Bounds))
                    {
                        return true;
                    }
                }
            }
            indexA = -1;
            indexB = -1;
            return false;
        }
    }

    public class GameScore
    {
        readonly List<float> pointsList;
        public GameScore()
        {
            pointsList = new List<float>();
        }
        public int BoxCount { get => pointsList.Count; }
        public float Average { get => pointsList.Count >0 ? pointsList.Average() : 0; }
        public float Maximum { get => pointsList.Count>0 ? pointsList.Max() : 0; }
        public float Total { get => pointsList.Count>0 ? pointsList.Sum() : 0; }

        public void Add(int points)
        {
            pointsList.Add(points);
        }
        public void Reset()
        {
            pointsList.Clear();
        }

        public override string ToString()
        {
            return $"Total={Total}, Ave={Average}, Max={Maximum}";
        }
    }
}
