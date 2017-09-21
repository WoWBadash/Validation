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
    public partial class frmCountrySetup : Form
    {
        public frmCountrySetup()
        {
            InitializeComponent();
        }

        private void frmCountrySetup_Load(object sender, EventArgs e)
        {
            try
            {
                Common.LineNum = ConfigurationManager.AppSettings["Line Number"].ToString();
                if (Common.LineNum == "227")
                {
                    Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
                    Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

                    var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                    var tsql = "SELECT distinct Customer FROM ShippingCustomer ;";
                    var cmd = new SqlCommand();
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = tsql;

                    var daAsm = new SqlDataAdapter(cmd);
                    var dtAsm = new DataTable();
                    daAsm.Fill(dtAsm);
                    sqlConnection.Close();
                    Common.LogMessage("Location Count.." + dtAsm.Rows.Count);
                    if (dtAsm.Rows.Count > 0)
                    {
                        cmbCountry.DataSource = dtAsm.DefaultView;
                        cmbCountry.DisplayMember = "Customer";
                    }
                }                    
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }                
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            try
            {
                if (cmbCountry.SelectedIndex != -1)
                {
                    this.Hide();
                    Common.CustomerCode = cmbCountry.Text;
                    frmValidation frm = Application.OpenForms.OfType<frmValidation>().FirstOrDefault();
                    frm.lblCustomerLocation.Text = cmbCountry.Text;
                }                
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }
    }
}
