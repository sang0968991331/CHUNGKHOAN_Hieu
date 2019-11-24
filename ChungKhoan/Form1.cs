﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChungKhoan
{
    public partial class Form1 : Form
    {
        public static int checkTangGiam;
        public Form1()
        {
            InitializeComponent();
            labelMinSub.Text = "0";
            addMultiRecord();
            radioButtonTang.Checked = true;
            addListView();
            maHoaItems();
        }
        private void addToDatabase(string MACP, int Ngay, int Thang, int Nam, int gia1, int gia2, int gia3)
        {

            SqlConnection conn = new SqlConnection(Program.connnectionString);
            conn.Open();



            String spName = "SP_INSERT";
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;



            SqlParameter param;
            param = cmd.Parameters.Add("@MACP", SqlDbType.NChar);
            param.Value = MACP;



            param = cmd.Parameters.Add("@NGAY", SqlDbType.Date);
            param.Value = Thang + "/" + Ngay + "/" + Nam;



            param = cmd.Parameters.Add("@GIAMOCUA", SqlDbType.Money);
            param.Value = gia1;



            param = cmd.Parameters.Add("@GIATC", SqlDbType.Money);
            param.Value = gia2;



            param = cmd.Parameters.Add("@GIADONGCUA", SqlDbType.Money);
            param.Value = gia3;



            SqlDataReader rdr = cmd.ExecuteReader();
            conn.Close();
        }

        private void addMultiRecord()
        {

            //dot 1 2019->2035
            //tang
            //string[] MACP = { "AGM", "ARB", "CIG" };
            //string[] MACP1 = { "CNG", "DAB", "DBC" };
            //giam
            // string[] MACP2 = { "DQC", "SCB", "VCB", "VTB"};



            //dot 2 2035->2052
            //giam
            //string[] MACP = { "AGM", "ARB", "CIG", "VTB" };
            ////tang
            ////string[] MACP1 = { "CNG", "DAB" };
            //string[] MACP2 = { "DQC", "SCB", "VCB" };
            ////giam
            //string[] MACP3 = { "DBC" };

            //2019-2035  2035-2052

            //  string[] MACP = { "AGM", "CIG" };
            //tang
            //string[] MACP1 = { "CNG", "DAB" };
            // string[] MACP2 = { "DQC", "VCB" };
            //giam
            //string[] MACP3 = { "VCB" };
            string[] MACP = { "ARB", "VTB", "DQC", "VCB" };

            string[] MACP2 = { "AGM", "CIG", "CNG", "SCB", "DBC", "DAB" };
            int Ngay = 1, Thang = 1, Nam = 2052, gia1 = 5000000, gia2 = 8000, gia3 = 4900000;
           // int Ngay = 1, Thang = 1, Nam = 2052, gia1 = 7000, gia2 = 8000, gia3 = 9000;
            foreach (string str in MACP2)
            {



                Nam = 2052;
                while (Nam != 2120)
                {
                    addToDatabase(str, Ngay, Thang, Nam, gia1, gia2, gia3);



                    gia1 = gia3;
                  //  gia3 += 100;

                    gia3 -= 10;

                    Ngay++;



                    if (Ngay > 28)
                    {
                        Ngay = 1;
                        Thang++;
                    }



                    if (Thang > 12)
                    {
                        Thang = 1;
                        Nam++;
                    }
                }
            }

        }
        public void addListView()
        {
            SqlConnection conn = new SqlConnection(Program.connnectionString);
            conn.Open();

            String spName = "SP_GIAOTAC";
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = cmd.Parameters.Add("@minsup", SqlDbType.Float);
            param.Value = Int32.Parse(labelMinSub.Text);
            param = cmd.Parameters.Add("@isinc", SqlDbType.Int);
            param.Value = getValueRadioButton();
            checkTangGiam = getValueRadioButton();

            int i = 0;
            SqlDataReader rdr = cmd.ExecuteReader();
            string[] date;
            DateTime dt;
            ArrayList mylist = new ArrayList();
            string temp;

            for (int j = 0; j < rdr.FieldCount; j++)
            {
                temp = rdr.GetName(j).ToString();
                listView1.Columns.Add(temp);
                mylist.Add(temp);
            }

            while (rdr.Read())
            {
                date = rdr["NGAY"].ToString().Split(' ');
                dt = Convert.ToDateTime(date[0]);
                listView1.Items.Add(dt.Day + "/" + dt.Month + "/" + dt.Year);
                foreach (string str in mylist)
                {
                    if (!str.Equals("NGAY"))
                        listView1.Items[i].SubItems.Add(rdr[str].ToString());
                }
                i++;
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            conn.Close();
        }

        public int getValueRadioButton()
        {
            if (radioButtonTang.Checked == true)
            {
                return 1;
            }
            else return 0;

        }
        public void maHoaItems()
        {
            listView2.Columns.Add("Mã Item");
            listView2.Columns.Add("Mã Cổ Phiếu");
            Program.listMahoa.Clear();
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView2.Items.Add((i).ToString());
                listView2.Items[i].SubItems.Add(listView1.Columns[i].Text);
                //add to list MaHoa
                Program.listMahoa.Add(new model.MaHoa(listView1.Columns[i].Text, i));
            }

            if (listView2.Items.Count != 0)
                listView2.Items.Remove(listView2.Items[0]);

            if (Program.listMahoa.Count > 0)
                Program.listMahoa.Remove(Program.listMahoa[0]);

            //Program.listMahoa.Remove(Program.listMahoa[0]);

            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            labelMinSub.Text = trackBar1.Value.ToString();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            listView2.Clear();
            addListView();
            maHoaItems();

            Program.minSup = (trackBar1.Value);
            //Console.WriteLine("Gia tri Min Sub: "+Program.minSup);
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("Không có tập D nào thỏa minSub!");
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2(listView1, listView2);
            frm2.ShowDialog();
        }
    }
}
