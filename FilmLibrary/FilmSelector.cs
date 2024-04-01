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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string year = textBox2.Text;
            string genre = comboBox1.Text;
            var newElement = _oldForm.films.FindRandom(year, name, genre);
            if (newElement == null)
            {
                button2.Hide();
                DialogResult dialogResult = MessageBox.Show("По введенным параметрам фильм не найден. Выполнить поиск на кинопоиске?", "Фильм не найден!", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    //Поиск на кинопоиске
                    newElement = _oldForm.films.KinopoiskSearch(year, name, genre).Result;



                    if (newElement == null)
                    {
                        MessageBox.Show("Ничего не найдено!", "", MessageBoxButtons.OK);
                        return;
                    }

                    button3.Hide();
                    label1.Text = newElement.name;
                    label2.Text = newElement.genre;
                    label6.Text = newElement.year;
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
                return;
            }
            button3.Hide();
            label1.Text = newElement.name;
            label2.Text = newElement.genre;
            label6.Text = newElement.year;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            _oldForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var newElement = _oldForm.films.FindRandom(textBox2.Text, textBox1.Text, comboBox1.Text);
            button3.Hide();
            label1.Text = newElement.name;
            label2.Text = newElement.genre;
            label6.Text = newElement.year;
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
    }
}
