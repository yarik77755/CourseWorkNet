using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Курсовая_2
{
    public partial class PasswordInput : Form
    {
        public bool Dialog = false;
        SoundPlayer sp = new SoundPlayer(@Environment.CurrentDirectory + "\\bruh.wav");
        public PasswordInput()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (InputBox.Text == HeadForm.GetPassword())
                {
                    Dialog = true;
                    Close();
                }
                else
                    throw new Exception ();
            }
            catch (Exception)
            {
                sp.Play();
                MessageBox.Show("Тебе всего-лишь требовалось скопировать пароль. Мне стыдно за тебя", "Осуждаю",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
