using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilmLibrary
{
    public partial class FilmSelector : Form
    {
        private readonly MainWindow _oldForm;

        public FilmSelector(MainWindow form)
        {
            _oldForm = form;
            InitializeComponent();
            backgroundWorker1.RunWorkerAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string year = textBox2.Text;
            string genre = comboBox1.Text;
            Film newElement = _oldForm.films.FindRandom(year, name, genre);
            if (newElement == null)
            {
                DialogResult dialogResult = MessageBox.Show("По введенным параметрам фильм не найден. Выполнить поиск на кинопоиске?", "Фильм не найден!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    return;
                //Поиск на кинопоиске
                newElement = ApiHelper.KinopoiskSearch(year, name, genre).Result;

                if (newElement == null)
                {
                    MessageBox.Show("Ничего не найдено!", "", MessageBoxButtons.OK);
                    return;
                }
            }
            button3.Hide();
            label1.Text = newElement.name;
            label2.Text = newElement.genre;
            label6.Text = newElement.year;
            button2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            _oldForm.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label6.Text = "";
            button2.Show();
            button3.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label6.Text = "";
            button2.Show();
            button3.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label6.Text = "";
            button2.Show();
            button3.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ApiHelper.FormGenres(backgroundWorker1);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            comboBox1.Items.AddRange(ApiHelper.genres.ToArray());
            comboBox1.Enabled = true;
            label5.Text = "Жанры:";
            label5.Refresh();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label5.Invoke(new Action(() => { label5.Text = "Прогресс: " + e.ProgressPercentage + "%"; }));
            label5.Invoke(new Action(() => { label5.Refresh(); }));
        }
    }
}
