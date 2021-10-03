using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenCV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_VisORB_Click(object sender, EventArgs e)
        {
            Form2 form2Form = new Form2();
            this.Hide();
            form2Form.ShowDialog();
            form2Form.Close();
            this.Show();
        }

        private void btn_DetectArucoInLowLight_Click(object sender, EventArgs e)
        {
            Form3 form3Form = new Form3();
            this.Hide();
            form3Form.ShowDialog();
            form3Form.Close();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4Form = new Form4();
            this.Hide();
            form4Form.ShowDialog();
            form4Form.Close();
            this.Show();
        }
    }
}
