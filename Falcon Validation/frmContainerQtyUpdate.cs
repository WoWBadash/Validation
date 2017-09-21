using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Falcon_Validation
{
    public partial class frmContainerQtyUpdate : Form
    {
        public frmContainerQtyUpdate()
        {
            InitializeComponent();
        }

        private static TForm getForm<TForm>() where TForm : Form, new()
        {
            TForm frm = (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
            if (frm == null)
            {
                frm = new TForm();
            }
            return frm;
        }

        private void frmContainerQtyUpdate_Load(object sender, EventArgs e)
        {
            Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
            Common.M2MConnectionString = ConfigurationManager.AppSettings["M2M_CONNECTION_STRING"].ToString();
            Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

            gbContainerQty.Visible = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblLineNum.Text) || string.IsNullOrEmpty(lblJobNum.Text) || string.IsNullOrEmpty(lblContainerNum.Text))
            {
                MessageBox.Show("Line Num / Job Num / Container # Data Not Found");
                return;
            }
            UpdateSerialContainer(lblContainerNum.Text, lblJobNum.Text, lblLineNum.Text, txtQty.Text);
        }

        private void UpdateSerialContainer(string ContainerNum, string JobNum, string LineNum, string Qty)
        {
            try
            {
                Common.LogMessage("Conatiner  -->" + ContainerNum + ": JobNum--> " + JobNum);
                Common.LogMessage("LineNum  -->" + LineNum + ": Qty--> " + Qty);
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                var frmValidaton = getForm<frmValidation>();
                sql = "UPDATE ContainerQuantities SET CompletedQty = @CompletedQty WHERE JobNum = '" + JobNum + "' and LineNum ='" + LineNum +"' and ContainerNum = '" + ContainerNum + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@CompletedQty", txtQty.Text);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
                MessageBox.Show("Container# updated Successfully");
                frmContainerQtyUpdate frm = Application.OpenForms.OfType<frmContainerQtyUpdate>().FirstOrDefault();
                frm.Close();
                if(LineNum == Common.LineNum)
                    frmValidaton.lblContainerQtyVal.Text = txtQty.Text + " Of " + (Common.ContainerLayerParts * Common.ContainerLayers);
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lblContainerNum.Text = "";
            lblLineNum.Text = "";
            txtQty.Text = "";

            gbContainerQty.Visible = false;
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

        private void LoadSerialDetails(string ContainerNo)
        {
            try
            {
                var sqlConn = new SqlConnection(Common.ASMConnectionString);
                SqlDataAdapter sqlAdap = null;
                DataSet ds = null;
                string tsql  = string.Empty;

                if (string.IsNullOrEmpty(Common.LineNum) || string.IsNullOrEmpty(Common.JobNum))
                {
                    tsql = "SELECT JobNum, LineNum, ContainerNum, ISNULL(CompletedQty,0) AS CompletedQty FROM ContainerQuantities WHERE ContainerNum = '" + ContainerNo + "'";
                }
                else
                {
                    tsql = "SELECT JobNum, LineNum, ContainerNum, ISNULL(CompletedQty,0) AS CompletedQty FROM ContainerQuantities WHERE ContainerNum = '" + ContainerNo + "' and LineNum = '" + Common.LineNum + "' and JobNum = '" + Common.JobNum + "'";
                }                

                if (!string.IsNullOrEmpty(ContainerNo))
                {
                    sqlAdap = new SqlDataAdapter(tsql, sqlConn);
                }
                else
                {
                    MessageBox.Show("Please enter Container # to Load");
                    return;
                }

                ds = new DataSet();
                sqlAdap.Fill(ds, "Asm_Serial");

                if (ds.Tables["Asm_Serial"].Rows.Count > 1)
                {
                    gbContainerQty.Visible = false;
                    MessageBox.Show("Multiple Jobs found:");
                    return;
                }
                else if (ds.Tables["Asm_Serial"].Rows.Count == 0)
                {
                    gbContainerQty.Visible = false;
                    MessageBox.Show("Container Num Not Found");
                    return;
                }
                else
                {
                    gbContainerQty.Visible = true;
                    lblContainerNum.Text = ds.Tables["Asm_Serial"].Rows[0]["ContainerNum"].ToString();
                    lblJobNum.Text = ds.Tables["Asm_Serial"].Rows[0]["JobNum"].ToString();
                    lblLineNum.Text = ds.Tables["Asm_Serial"].Rows[0]["LineNum"].ToString();
                    txtQty.Text = ds.Tables["Asm_Serial"].Rows[0]["CompletedQty"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                frmContainerQtyUpdate frm = Application.OpenForms.OfType<frmContainerQtyUpdate>().FirstOrDefault();
                frm.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
        }
    }
}
