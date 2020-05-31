using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Media;

namespace Курсовая_2
{
    public partial class HeadForm : Form
    {
        bool IsOpen = false;
        SqlConnection SqlCon = new SqlConnection();
        private static string password;
        public static string GetPassword()
        {
            return password;
        }

        private int childFormNumber = 0;
        SoundPlayer sp = new SoundPlayer(@Environment.CurrentDirectory + "\\bruh.wav");
        public bool CheckData(string CourseName, string CourseNumber, string Faculty, string Hours, string TeacherName, string StudentName)
        {
            bool IsOkay = false;
            try
            {
                if (CourseName.Length < 3 || CourseName.Length > 35 || string.IsNullOrEmpty(CourseName) == true)
                    throw new Exception("Неправильный ввод названия курса");
                if (Convert.ToInt32(CourseNumber) > 999 || Convert.ToInt32(CourseNumber) < 0)
                    throw new Exception("Неправильный ввод шифра");
                if (Faculty.Length < 3 || Faculty.Length > 20)
                    throw new Exception("Неправильный ввод факультетов");
                Faculty.Trim(' ');
                string[] Check = Faculty.Split(',');
                foreach (string i in Check)
                {
                    if (i.Length != 3)
                        throw new Exception("Неправильный ввод факультетов");
                }
                int[] HoursInt = new int[3];
                Hours.Trim(' ');
                string[] HoursArray = Hours.Split(',');
                if (HoursArray.Length != 3)
                    throw new Exception("Неправильный ввод количества часов");
                if ((TeacherName.Length < 5 || TeacherName.Length > 50) && TeacherName != "")
                    throw new Exception("Неправильный ввод имени преподователя");
                if ((StudentName.Length < 5 || StudentName.Length > 50) && StudentName != "")
                    throw new Exception("Неправильный ввод имени помошника");
                IsOkay = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }

            return IsOkay;
        }

        public HeadForm()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Окно " + childFormNumber++;
            childForm.Show();
        }

        public void BuildDB(SqlCommand command)
        {
            SqlDataReader Readsql = null;
            try
            {
                Readsql = command.ExecuteReader();
                int i = 0;
                while (Readsql.Read())
                {
                    dataGridView1.Rows.Add(Readsql["Навзание курса"], Readsql["Шифр курса"], Readsql["Факультет"], Readsql["Количество часов"], Readsql["ФИО преподователя"], Readsql["ФИО студента"]);
                    i++;
                }
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Ошибка БД", "Бабах",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Readsql.Close();
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Файл MDF (*.MDF)|*.MDF";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
                SqlCon = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename=" + FileName + @";Integrated Security = True");
                SqlCon.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM [table]", SqlCon);
                tabControl1.SelectedTab = tabControl1.TabPages[1];
                BuildDB(command);
                StatusLabel.Text = "Просмотр БД";
                IsOpen = true;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

  

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            CurrentTimeBox.Text = Convert.ToString(DateTime.Now);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void PasswordCreateButton_Click_1(object sender, EventArgs e)
        {
            password = null;
            PasswordCreateTextBox.Clear();
            Random random = new Random();
            char[] ForPassword = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@_-$%!*&".ToCharArray();
            for (int i = 1; i < 10; i++)
            {
                password += ForPassword[random.Next(0, ForPassword.Length)];
            }
            PasswordCreateTextBox.Text = password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sp.Play();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (IsOpen == false)
            {
                SearchBox.Clear();
                sp.Play();
                MessageBox.Show("Сначала открой БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dataGridView1.Rows.Clear();
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand("SELECT * FROM [Table] WHERE [Шифр курса] = " + Convert.ToInt32(SearchBox.Text) + "or [Количество часов] LIKE \'%" + SearchBox.Text + "%\'", SqlCon);
            }
            catch (System.FormatException)
            {
                command = new SqlCommand("SELECT * FROM [Table] WHERE [ФИО преподователя] LIKE \'%" + SearchBox.Text + "%\' or [Навзание курса] LIKE \'%" + SearchBox.Text + "%\' or [ФИО студента] LIKE \'%" + SearchBox.Text + "%\' or [Количество часов] LIKE \'%" + SearchBox.Text + "%\' or [ФИО студента] LIKE \'%" + SearchBox.Text + "%\' or [Факультет] LIKE \'%" + SearchBox.Text + "%\'", SqlCon);
            }
            BuildDB(command);
        }

        private void HeadForm_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsOpen == false)
            {
                sp.Play();
                MessageBox.Show("Сначала открой БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dataGridView1.Rows.Clear();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand("SELECT * FROM [Table] WHERE [ФИО преподователя] IS NULL OR [ФИО преподователя] LIKE ''", SqlCon);
            BuildDB(command);
        }

        private void RefreshBD_Click(object sender, EventArgs e)
        {
            if (IsOpen == false)
            {
                sp.Play();
                MessageBox.Show("Сначала открой БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dataGridView1.Rows.Clear();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand("SELECT * FROM [Table]", SqlCon);
            BuildDB(command);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (IsOpen == false)
            {
                textBox1.Clear();
                sp.Play();
                MessageBox.Show("Сначала открой БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dataGridView1.Rows.Clear();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand("SELECT * FROM [Table]WHERE [ФИО преподователя] LIKE \'%" + textBox1.Text + "%\'", SqlCon);
            BuildDB(command);
        }

        private void GiveHours_Click(object sender, EventArgs e)
        {
            if (IsOpen == false)
            {
                sp.Play();
                MessageBox.Show("Сначала открой БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SqlCommand command = new SqlCommand();
            command = new SqlCommand("SELECT * FROM [Table] WHERE [ФИО преподователя] LIKE \'%" + textBox1.Text + "%\'", SqlCon);
            SqlDataReader Readsql = null;
            int HoursL = 0;
            int HoursP = 0;
            int HoursS = 0;
            try
            {
                Readsql = command.ExecuteReader();
                while (Readsql.Read())
                {
                    string[] ArrayHours = new String[3];
                    ArrayHours = Convert.ToString(Readsql["Количество часов"]).Split(',');
                    HoursL += Convert.ToInt32(ArrayHours[0]);
                    HoursP += Convert.ToInt32(ArrayHours[1]);
                    HoursS += Convert.ToInt32(ArrayHours[2]);
                }
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Ошибка БД", "Бабах",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Readsql.Close();
            }
            MessageBox.Show($"Лекций {HoursL}, Практических {HoursP}, Лабораторных {HoursS}", "Успех!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (IsOpen == false)
            {
                sp.Play();
                MessageBox.Show("Сначала открой БД", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            PasswordInput input = new PasswordInput();
            input.ShowDialog();
            if (input.Dialog == true)
            {
                label2.Visible = false;
                CourseNameInput.Visible = true;
                CourseNumberInput.Visible = true;
                FacultyInput.Visible = true;
                HoursInput.Visible = true;
                DeleteButton.Visible = true;
                TeacherNameInput.Visible = true;
                StudentNameInput.Visible = true;
                StatusLabel.Text = "Просмотр и редактирование БД";
                AddButton.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                ClearButton.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (CheckData(CourseNameInput.Text, CourseNumberInput.Text, FacultyInput.Text, HoursInput.Text, TeacherNameInput.Text, StudentNameInput.Text) == false)
                return;
            try
            {
                if (String.IsNullOrWhiteSpace(TeacherNameInput.Text))
                    TeacherNameInput.Text = null;
                SqlCommand addcom = new SqlCommand("INSERT INTO [Table] ([Навзание курса],[Шифр курса],[Факультет],[Количество часов], [ФИО преподователя], [ФИО студента])VALUES(@Course,@Number,@Faculty,@Hours,@Teacher,@Student)", SqlCon);
                addcom.Parameters.AddWithValue("Course", CourseNameInput.Text);
                addcom.Parameters.AddWithValue("Number", Convert.ToInt32(CourseNumberInput.Text));
                addcom.Parameters.AddWithValue("Faculty", FacultyInput.Text);
                addcom.Parameters.AddWithValue("Hours", HoursInput.Text);
                addcom.Parameters.AddWithValue("Teacher", TeacherNameInput.Text);
                addcom.Parameters.AddWithValue("Student", StudentNameInput.Text);
                addcom.ExecuteNonQuery();
                CourseNameInput.Clear();
                CourseNumberInput.Clear();
                FacultyInput.Clear();
                HoursInput.Clear();
                TeacherNameInput.Clear();
                StudentNameInput.Clear();
                SearchBox.Clear();
                SqlCommand command = new SqlCommand();
                command = new SqlCommand("SELECT * FROM [Table]", SqlCon);
                BuildDB(command);
                MessageBox.Show($"Успешное добавление в БД", "Успех!",
              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                DialogResult result = MessageBox.Show($"Предмет с таким шифром уже существует. Желаете обновить его в БД?", "Непорядок",
              MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    if (CheckData(CourseNameInput.Text, CourseNumberInput.Text, FacultyInput.Text, HoursInput.Text, TeacherNameInput.Text, StudentNameInput.Text) == false)
                        return;
                    try
                    {
                        if (String.IsNullOrWhiteSpace(TeacherNameInput.Text))
                            TeacherNameInput.Text = null;
                        SqlCommand addcom = new SqlCommand("UPDATE [Table] SET [Навзание курса] = @Course,[Шифр курса] = @Number,[Факультет] = @Faculty,[Количество часов] = @Hours, [ФИО преподователя] = @Teacher, [ФИО студента] = @Student WHERE [Шифр курса] = " + CourseNumberInput.Text, SqlCon);
                        addcom.Parameters.AddWithValue("Course", CourseNameInput.Text);
                        addcom.Parameters.AddWithValue("Number", Convert.ToInt32(CourseNumberInput.Text));
                        addcom.Parameters.AddWithValue("Faculty", FacultyInput.Text);
                        addcom.Parameters.AddWithValue("Hours", HoursInput.Text);
                        addcom.Parameters.AddWithValue("Teacher", TeacherNameInput.Text);
                        addcom.Parameters.AddWithValue("Student", StudentNameInput.Text);
                        addcom.ExecuteNonQuery();
                        CourseNameInput.Clear();
                        CourseNumberInput.Clear();
                        FacultyInput.Clear();
                        HoursInput.Clear();
                        TeacherNameInput.Clear();
                        StudentNameInput.Clear();
                        SqlCommand command = new SqlCommand();
                        command = new SqlCommand("SELECT * FROM [Table]", SqlCon);
                        BuildDB(command);
                        SearchBox.Clear();
                        MessageBox.Show($"Успешное обновление БД", "Успех!",
                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
}

        private void CourseNumberInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SearchBox.TextLength < 3)
                    throw new Exception("Введите предмет полностью");
                using (SqlCommand cmd = new SqlCommand(("DELETE FROM [Table] WHERE [Навзание курса] LIKE \'%" + SearchBox.Text + "%\'"), SqlCon))
                    {
                            cmd.ExecuteNonQuery();
                    }
                SearchBox.Clear();
                MessageBox.Show($"Предмет успешно удален", "Успех!",
              MessageBoxButtons.OK, MessageBoxIcon.Information);
                SqlCommand command = new SqlCommand();
                command = new SqlCommand("SELECT * FROM [Table]", SqlCon);
                BuildDB(command);
                SearchBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            CourseNameInput.Text = Convert.ToString(dataGridView1[0, e.RowIndex].Value);
            CourseNumberInput.Text = Convert.ToString(dataGridView1[1, e.RowIndex].Value);
            FacultyInput.Text = Convert.ToString(dataGridView1[2, e.RowIndex].Value);
            HoursInput.Text = Convert.ToString(dataGridView1[3, e.RowIndex].Value);
            TeacherNameInput.Text = Convert.ToString(dataGridView1[4, e.RowIndex].Value);
            StudentNameInput.Text = Convert.ToString(dataGridView1[5, e.RowIndex].Value);
        }

        private void CourseNameInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((((e.KeyChar < 'A') || (e.KeyChar > 'z')) && !(e.KeyChar == ' ') && !(e.KeyChar == '.') && !(Char.IsControl(e.KeyChar))) || (textBox1.Text.Length > 35)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                CourseNameInput.Focus();
            }

        }

        private void TeacherNameInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((((e.KeyChar < 'A') || (e.KeyChar > 'z')) && !(e.KeyChar == ' ') && !(e.KeyChar == '.') && !(Char.IsControl(e.KeyChar))) || (textBox1.Text.Length > 50)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                TeacherNameInput.Focus();
            }
        }

        private void StudentNameInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((((e.KeyChar < 'A') || (e.KeyChar > 'z')) && !(e.KeyChar == ' ') && !(e.KeyChar == '.') && !(Char.IsControl(e.KeyChar))) || (textBox1.Text.Length > 35)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                StudentNameInput.Focus();
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            CourseNameInput.Clear();
            CourseNumberInput.Clear();
            FacultyInput.Clear();
            HoursInput.Clear();
            TeacherNameInput.Clear();
            StudentNameInput.Clear();
        }
    } 
}
