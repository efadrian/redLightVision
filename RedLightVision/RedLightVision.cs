using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedLightVision
{
    public class MainForm : Form
    {
        private Button startButton;
        private NumericUpDown durationSelector;

        public MainForm()
        {
            Text = "Red Light Vision";
            Size = new Size(300, 150);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            Label label = new Label
            {
                Text = "Durata sesiunii (minute):",
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(label);

            durationSelector = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 3,
                Value = 1,
                Location = new Point(180, 18),
                Width = 50
            };
            Controls.Add(durationSelector);

            startButton = new Button
            {
                Text = "Start sesiune",
                Location = new Point(80, 60),
                Width = 120
            };
            startButton.Click += StartButton_Click;
            Controls.Add(startButton);
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            int durationMinutes = (int)durationSelector.Value;

            using (var redForm = new RedScreenForm(durationMinutes))
            {
                redForm.Show();
                await redForm.RunSessionAsync();
            }
        }

        private void InitializeComponent()
        {

        }
    }
}
