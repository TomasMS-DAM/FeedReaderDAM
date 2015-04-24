﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CNegocio.Utils;
using CNegocio.WBManager;

namespace CVista
{
    public partial class RssContactsDialog : Form
    {
        private int rowIndex;
        WebServiciesManager dataConect;

        public RssContactsDialog()
        {
            this.rowIndex = -1;
            this.dataConect = null;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int interv = Convert.ToInt32(textBox2.Text);
                Properties.Settings.Default.intervalo = interv;
                Properties.Settings.Default.Save();
                this.textBox1.Text = Properties.Settings.Default.intervalo.ToString();
                this.textBox2.Text = String.Empty;
            }
            catch (OverflowException ex)
            {
                String msg = ex.Message + Environment.NewLine + " Debe de ser un número comprendido entre De 0 y 4294967295.";
                MessageBox.Show(msg);
            }
            catch (FormatException ex)
            {
                String msg = ex.Message + Environment.NewLine + " Debe de ser un número.";
                MessageBox.Show(msg);
            }
        }

        private void RssContactsDialog_Leave(object sender, EventArgs e)
        {
        }

        private void RssContactsDialog_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Properties.Settings.Default.intervalo.ToString();
            this.cargarTabla1();
            this.cargarTabla2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebServiciesManager dataConect = null;

            String textboton = button1.Text;
            if (textboton.Equals("ACTIVAR"))
            {
                this.button1.Text = "DESACTIVAR";
                this.button2.Enabled = false;
                this.textBox2.Enabled = false;
                this.textBox2.Text = String.Empty;
                this.dataGridView2.Enabled = false;
                this.button4.Enabled = false;
                this.label4.Text = "Edición desactivada";
                this.label4.ForeColor = Color.Red;


                this.dataConect = new WebServiciesManager(Properties.Settings.Default.intervalo);
                foreach (CheckUpdatedThread item in this.dataConect.listwork)
	            {
                    item.updatedFeed += event_updatedFeed;
	            }
                this.dataConect.LanzarHilos();
                

            }
            else
            {
                this.button1.Text = "ACTIVAR";
                this.button2.Enabled = true;
                this.textBox2.Enabled = true;
                this.textBox2.Text = String.Empty;
                this.dataGridView2.Enabled = true;
                this.button4.Enabled = true;
                this.label4.Text = "Edición activada";
                this.label4.ForeColor = Color.Green;

                if (dataConect != null)
                {
                    this.dataConect.DetenerHilos();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataConect != null)
            {
                this.dataConect.DetenerHilos();
            }
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ModifyFeedDialog mfDialog = new ModifyFeedDialog(Constants.CREATE_FEED);
            mfDialog.MdiParent = WrapWindow.ActiveForm;
            mfDialog.Show();
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewRow clickedRow = (sender as DataGridView).Rows[e.RowIndex];
                    this.rowIndex = e.RowIndex;
                    Point relativeMousePosition = dataGridView1.PointToClient(Cursor.Position);
                    this.dataGridView2.Rows[this.dataGridView2.SelectedRows[0].Index].Selected = false;
                    this.dataGridView2.Rows[e.RowIndex].Selected = true;
                    this.contextMenuStrip1.Show(dataGridView1, relativeMousePosition);
                }
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                int id = (int)this.dataGridView1.Rows[this.rowIndex].Cells[0].Value;

                if (MessageBox.Show("¿Estas seguro de que desea eliminar este Feed?", "Eliminar Feed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (Utils.deleteRssContact(id) == 1)
                    {
                        MessageBox.Show("Feed eliminado", "Eliminar", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo elimina el Feed", "Eliminar", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    }
                }
            }
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                //this.dataGridView1.Rows.RemoveAt(this.rowIndex);
                int id = (int)this.dataGridView2.Rows[this.rowIndex].Cells[0].Value;
                string savedate = ((DateTime)this.dataGridView2.Rows[this.rowIndex].Cells[1].Value).ToString("d");
                string url = (string)this.dataGridView2.Rows[this.rowIndex].Cells[2].Value;
                string comm = (string)this.dataGridView2.Rows[this.rowIndex].Cells[3].Value;
                string type = (string)this.dataGridView2.Rows[this.rowIndex].Cells[4].Value;
                string title = (string)this.dataGridView2.Rows[this.rowIndex].Cells[5].Value;
                ModifyFeedDialog mfDialog = new ModifyFeedDialog(Constants.MODIFY_FEED, id, savedate, url, comm, type, title);
                mfDialog.MdiParent = WrapWindow.ActiveForm;
                mfDialog.Show();
            }
        }

        protected void cargarTabla1()
        {
            DataTable dtfeeds = new DataTable();
            dtfeeds = Utils.retrieveRssContact();

            DataGridViewImageColumn col1 = new DataGridViewImageColumn();
            col1.Name = "estatus";
            col1.HeaderText = "Estatus";
            col1.Image = Properties.Resources.greydot;
            col1.DisplayIndex = 0;
            this.dataGridView1.Columns.Add(col1);

            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.Name = "id";
            col2.HeaderText = "ID";
            col2.DisplayIndex = 1;
            this.dataGridView1.Columns.Add(col2);

            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.Name = "titulo";
            col3.HeaderText = "Titulo";
            col3.DisplayIndex = 2;
            this.dataGridView1.Columns.Add(col3);

            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
            col4.Name = "tipo";
            col4.HeaderText = "Tipo";
            col4.DisplayIndex = 3;
            this.dataGridView1.Columns.Add(col4);

            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
            col5.Name = "url";
            col5.HeaderText = "URL";
            col5.DisplayIndex = 4;
            this.dataGridView1.Columns.Add(col5);

            DataGridViewTextBoxColumn col6 = new DataGridViewTextBoxColumn();
            col6.Name = "comentario";
            col6.HeaderText = "Comentario";
            col6.DisplayIndex = 5;
            this.dataGridView1.Columns.Add(col6);

            if (dtfeeds.Rows.Count > 0)
            {
                foreach (DataRow dtRow in dtfeeds.Rows)
                {
                    int idfeed = Convert.ToInt32(dtRow["ID"]);
                    string titulofeed = dtRow["Titulo"].ToString();
                    string tipofeed = dtRow["Tipo"].ToString();
                    string urlfeed = dtRow["URL"].ToString();
                    string commfeed = dtRow["Comentario"].ToString();

                    this.dataGridView1.Rows.Add(Properties.Resources.greydot, idfeed, titulofeed, tipofeed, urlfeed, commfeed);
                }
            }
        }

        protected void cargarTabla2()
        {
            this.dataGridView2.DataSource = Utils.retrieveRssContact();
        }

        public void event_updatedFeed(object sender, UpdatedEventArgs e)
        {
            int id = e.rss.id;
            foreach (DataGridViewRow itemRow in this.dataGridView1.Rows)
            {
                int iddgv = (int)itemRow.Cells[1].Value;
                if (id == iddgv)
                {
                    DataGridViewImageCell cell = (DataGridViewImageCell)this.dataGridView1.Rows[itemRow.Index].Cells[0];

                    // http://stackoverflow.com/questions/2359124/datagridview-throwing-invalidoperationexception-operation-is-not-valid-whe
                    this.dataGridView1.BeginEdit(false);
                    this.dataGridView1.NotifyCurrentCellDirty(true);

                    if (e.status == 1)
                    {
                        cell.Value = Properties.Resources.greendot;
                        cell.ToolTipText = "El feed ha sido actualizado";
                    }
                    if (e.status == -1)
                    {
                        cell.Value = Properties.Resources.reddot;
                        cell.ToolTipText = "No se pudo contactar con el feed";
                    }
                    
                    this.dataGridView1.EndEdit();
                    this.dataGridView1.NotifyCurrentCellDirty(false);
                }
            }
        }
    }
}
