using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace majdoee.app
{
    public partial class DeveloperDetails : Form
    {
        public DeveloperDetails()
        {
            InitializeComponent();
        }

        private void linkedIn_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://www.linkedin.com/in/ahmed-alhadab?utm_source=share&utm_campaign=share_via&utm_content=profile&utm_medium=ios_app");
            Process.Start(sInfo);
        }

        private void x_icon_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://x.com/a_7db_");
            Process.Start(sInfo);
        }

        private void DeveloperDetails_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.contact;
            pictureBox2.Image = Properties.Resources.name;
            pictureBox3.Image = Properties.Resources.call;
            pictureBox4.Image = Properties.Resources.email;

            x_icon.Image = Properties.Resources.x;
            linkedIn.Image = Properties.Resources.linkedin;
        }
    }
}
