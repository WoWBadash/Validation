using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Falcon_Validation
{
    public partial class frmUpdateSerial : Form
    {
        public frmUpdateSerial()
        {
            InitializeComponent();
        }

        private void LoadSerialDetails(string SerialNo)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(Common.ASMConnectionString);
                SqlDataAdapter sqlAdap = null;
                DataSet ds = null;
                if (!string.IsNullOrEmpty(SerialNo))
                {
                    sqlAdap = new SqlDataAdapter("select Asm_Num, Container_Serial from " + Common.AsmTable + "  where Container_Serial='" + SerialNo + "'", sqlConn);
                }
                else
                {
                    MessageBox.Show("Please enter Container # to Load");
                    return;
                }

                ds = new DataSet();
                sqlAdap.Fill(ds, "Asm_Serial");

                flexLotControl.DataSource = ds.Tables["Asm_Serial"];

                if (flexLotControl.Rows.Count > 0)
                {
                    LoadSelectedSerialDetails(0);
                    gbSerialUpdate.Visible = true;
                }
                else
                {
                    gbSerialUpdate.Visible = false;
                }
            }
             catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void LoadSelectedSerialDetails(int pos)
        {
            try
            {
                DataGridViewRow dgvr = this.flexLotControl.Rows[pos];
                lblAsmNo.Text = dgvr.Cells["Asm_Num"].Value.ToString();
                txtSerialNo.Text = dgvr.Cells["Container_Serial"].Value.ToString();
            }
             catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }           
        }

        private void flexLotControl_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    LoadSelectedSerialDetails(e.RowIndex);
                    gbSerialUpdate.Visible = true;
                }
                else
                {
                    gbSerialUpdate.Visible = false;
                }
            }
             catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSerialDetails(txtSerialNoEntered.Text);
            }
             catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            gbSerialUpdate.Visible = false;
            txtSerialNo.Text = string.Empty;
        }

        private void frmUpdateSerial_Load(object sender, EventArgs e)
        {
            try
            {
                Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
                Common.M2MConnectionString = ConfigurationManager.AppSettings["M2M_CONNECTION_STRING"].ToString();
                Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

                gbSerialUpdate.Visible = false;
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try {
                if (string.IsNullOrEmpty(lblAsmNo.Text))
                {
                    MessageBox.Show("Assembly # is not available");
                    return;
                }
                UpdateSerialContainer(lblAsmNo.Text);
                LoadSerialDetails(txtSerialNoEntered.Text);
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }           
        }

        private void UpdateSerialContainer(string AsmNo)
        {
            try
            {
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();

                sql = "UPDATE " + Common.AsmTable + " SET Container_Serial = @ContainerNo WHERE Asm_Num = '" + AsmNo + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ContainerNo", txtSerialNo.Text);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            frmUpdateSerial frm = Application.OpenForms.OfType<frmUpdateSerial>().FirstOrDefault();
            frm.Close();
        }

       
    }
}
