using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FilmLibrary
{
    public partial class MainWindow : Form
    {
        public FilmList films = new FilmList();

        public MainWindow()
        {
            films.Sync();

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Откроем форму для ввода данных
            var preferencesForm = new FilmPreference(this);
            preferencesForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Откроем форму для ввода данных
            var selectorForm = new FilmSelector(this);
            selectorForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FilmList.GetGenresAsync();
        }
    }
}