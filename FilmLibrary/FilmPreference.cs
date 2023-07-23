using System;
using System.Windows.Forms;

namespace FilmLibrary
{
    public partial class FilmPreference : Form
    {
        private readonly MainWindow _oldForm;

        public FilmPreference(MainWindow form)
        {
            _oldForm = form;
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox1.Text))
                button1.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(comboBox1.Text))
                button1.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(comboBox1.Text))
                button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var newbie = new Film(textBox2.Text, textBox1.Text, comboBox1.Text);

            _oldForm.films.addNew(newbie);

            Close();
            _oldForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            _oldForm.Show();
        }
    }
}