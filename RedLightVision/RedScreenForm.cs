using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedLightVision
{
    public class RedScreenForm : Form
    {
        private readonly int durationMinutes;
        private readonly System.Windows.Forms.Timer circleTimer;
        private readonly List<Circle> circles = new();
        private readonly Random rand = new();
        private DateTime sessionStartTime;


        private Color baseCircleColor = Color.FromArgb(200, 0, 0); ///6700; // roșu închis
            //Color.FromArgb(255, 100, 100); // roșu pal

        public RedScreenForm(int durationMinutes)
        {
            this.durationMinutes = durationMinutes;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.FromArgb(255, 0, 0); // roșu complet
            TopMost = true;
            ShowInTaskbar = false;
            Opacity = 1.0;
            DoubleBuffered = true;

            Cursor.Hide();

            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            circleTimer = new System.Windows.Forms.Timer { Interval = 1500 }; // la 1 secunde
            circleTimer.Tick += (s, e) =>GenerateCircles();
            circleTimer.Start();
            //

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var now = DateTime.Now;

            foreach (var circle in circles.ToList())
            {
                double age = (now - circle.CreatedAt).TotalSeconds;
                if (age > 10)
                {
                    circles.Remove(circle);
                    continue;
                }

                // Fade-in/out logic
                double opacityFactor = 1.0;
                if (age < 2)
                    opacityFactor = age / 2.0;
                else if (age > 8)
                    opacityFactor = (10 - age) / 2.0;

                int alpha = (int)(circle.BaseOpacity * opacityFactor * 255) / 2;

                // Roșu mai deschis pentru cercuri
                Color fillColor = Color.FromArgb(alpha, 255, 100, 100); // cerc

                using var brush = new SolidBrush(fillColor);
                e.Graphics.FillEllipse(brush, circle.X, circle.Y, circle.Diameter, circle.Diameter);
                //
                ShowTimer(e.Graphics); // timer 

                // ShowText(e.Graphics);
            }

        }
        private void ShowTimer(Graphics g)
        {
            if (sessionStartTime == DateTime.MinValue)
                return;

            TimeSpan elapsed = DateTime.Now - sessionStartTime;
            TimeSpan remaining = TimeSpan.FromMinutes(durationMinutes) - elapsed;

            string timeText;
            int alpha;

            if (remaining <= TimeSpan.Zero)
            {
                timeText = "00:00";
                alpha = 60; // 15% opacitate când sesiunea s-a încheiat
            }
            else
            {
                timeText = $"{(int)remaining.TotalMinutes:D2}:{remaining.Seconds:D2}";
                alpha = 60; 
            }

            using var font = new Font("Segoe UI", 12, FontStyle.Regular);
            using var brush = new SolidBrush(Color.FromArgb(alpha, baseCircleColor.R, baseCircleColor.G, baseCircleColor.B));

            SizeF textSize = g.MeasureString(timeText, font);
            float x = 10f;
            float y = ClientSize.Height - textSize.Height - 10f;

            g.DrawString(timeText, font, brush, new PointF(x, y));
        }


        //
        private void GenerateCircles()
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;

            int attempts = 0;
            int maxAttempts = 10;

            while (attempts < maxAttempts)
            {
                int size = rand.Next(40, 250);
                int x = rand.Next(0, width - size);
                int y = rand.Next(0, height - size);
                double opacity = rand.NextDouble() * 0.4 + 0.4;

                var newCircle = new Circle
                {
                    X = x,
                    Y = y,
                    Diameter = size,
                    BaseOpacity = opacity,
                    CreatedAt = DateTime.Now
                };

                bool overlaps = circles.Any(existing =>
                {
                    var dx = (existing.X + existing.Diameter / 2) - (newCircle.X + newCircle.Diameter / 2);
                    var dy = (existing.Y + existing.Diameter / 2) - (newCircle.Y + newCircle.Diameter / 2);
                    var distance = Math.Sqrt(dx * dx + dy * dy);
                    return distance < (existing.Diameter + newCircle.Diameter) / 2;
                });

                if (!overlaps)
                {
                    circles.Add(newCircle);
                    break;
                }

                attempts++;
            }

            Invalidate(); // redesenează
        }
 
        public async Task RunSessionAsync()
        {
            int totalMillis = durationMinutes * 60 * 1000;
            int interval = 100;
            int elapsed = 0;

            sessionStartTime = DateTime.Now;

            while (elapsed < totalMillis)
            {
                await Task.Delay(interval);
                elapsed += interval;
                Invalidate();
            }

            Close();
        }

        private class Circle
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Diameter { get; set; }
            public double BaseOpacity { get; set; }
            public DateTime CreatedAt { get; set; }
        }


    }
}
