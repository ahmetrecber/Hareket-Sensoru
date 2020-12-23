using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.SqlClient;

namespace hareketsensoru
{
    public partial class Form1 : Form
    {
        string[] portlar = SerialPort.GetPortNames();
        public Form1()
        {
            InitializeComponent();
            serialPort1.BaudRate = 9600;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data;
            data = serialPort1.ReadLine();
            listBox1.Items.Add(data);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string port in portlar)
            {
                comboBox1.Items.Add(port);
                comboBox1.SelectedIndex = 0;
            }
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBox1.Text;
                try
                {
                    serialPort1.Open();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Write("1");
        }


        //-------------------------------------------------------------SQL KODLARI---------------------------------------------
        SqlConnection baglan = new SqlConnection("Data Source=DESKTOP-GMITMRC;Initial Catalog=hareketsensoru;Integrated Security=True");

        public void verilerigoster(string veriler)
        {
            SqlDataAdapter da = new SqlDataAdapter(veriler, baglan);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
        }
        private void button4_Click(object sender, EventArgs e)
        {
            verilerigoster("Select * from hareketverileri");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand();
            if (listBox1.Items.Count != 0)
            {
                string sql = "insert into hareketverileri(durum) values (@durum)";
                foreach (string sqlkayit in listBox1.Items)
                {
                    komut = new SqlCommand(sql, baglan);
                    komut.Parameters.AddWithValue("@durum", sqlkayit);
                    baglan.Open();
                    komut.ExecuteNonQuery();
                    baglan.Close();
                }
                MessageBox.Show("Kayit Eklendi");
            }
            else
            {
                MessageBox.Show("ListBox Bozuk");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("update hareketverileri set durum=@durum where id=@id", baglan);
            komut.Parameters.AddWithValue("@id", Convert.ToInt32(textBox1.Text));
            komut.Parameters.AddWithValue("@durum", textBox2.Text);
            komut.ExecuteNonQuery();
            verilerigoster("select * from hareketverileri");
            baglan.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("delete from hareketverileri where id=@idd", baglan);
            komut.Parameters.AddWithValue("@idd", textBox1.Text);
            komut.ExecuteNonQuery();
            verilerigoster("select * from hareketverileri");
            baglan.Close();
            textBox1.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("select * from hareketverileri where durum like '%" + textBox2.Text + "%'", baglan);
            SqlDataAdapter da = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            da.Fill(dt);
            baglan.Close();
            dataGridView1.DataSource = dt;
        }
    }
}
