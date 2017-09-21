using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Globalization;

namespace Falcon_Validation
{
    public partial class frmValidation : Form
    {
        System.IO.Ports.SerialPort _serialPort;

        public frmValidation()
        {
            InitializeComponent();
        }

        private DataTable getAssemblyDetails(string AsmNum)
        {
            DataTable dt = null;
            try
            {
                Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
                Common.M2MConnectionString = ConfigurationManager.AppSettings["M2M_CONNECTION_STRING"].ToString();
                Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

                using (var sqlConn = new SqlConnection(Common.ASMConnectionString))
                {
                    var sql = "SELECT * FROM " + Common.AsmTable + " WHERE Asm_Num = '" + AsmNum + "'";
                    sqlConn.Open();

                    var cmd = new SqlCommand
                    {
                        Connection = sqlConn,
                        CommandType = CommandType.Text,
                        CommandText = sql
                    };

                    var da = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);
                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ":" + ex.StackTrace.ToString(CultureInfo.InvariantCulture));
            }
            return dt;
        }

        private DataTable getAsmProcessMatrixDetails(string PartNum)
        {
            DataTable dtProcessMatrix = null;
            try
            {
                Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
                Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

                using (var sqlConn = new SqlConnection(Common.ASMConnectionString))
                {
                    string sql = string.Empty;
                    if (Common.LineNum == "ALP001")
                    {
                        sql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE NewPartNum = '" + PartNum + "'";
                    }
                    else
                    {
                        sql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE Part_Num = '" + PartNum + "'";
                    } 
                    //var sql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE OriginalPartNum = '" + PartNum + "'";
                    sqlConn.Open();

                    var cmd = new SqlCommand
                    {
                        Connection = sqlConn,
                        CommandType = CommandType.Text,
                        CommandText = sql
                    };

                    var da = new SqlDataAdapter(cmd);
                    dtProcessMatrix = new DataTable();
                    da.Fill(dtProcessMatrix);
                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ":" + ex.StackTrace.ToString(CultureInfo.InvariantCulture));
            }
            return dtProcessMatrix;
        }

        private void LoadSerialData()
        {
            int ContainerPartsCount;
            string ContainerNo;
            try
            {
                var tsql = "SELECT * FROM " + Common.AsmTable + " WHERE Asm_Num = '" + Common.SerialNo + "'";
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daAsm = new SqlDataAdapter(cmd);
                var dtAsm = new DataTable();
                daAsm.Fill(dtAsm);
                sqlConnection.Close();

                if (dtAsm.Rows.Count > 0)
                {
                    if (Convert.ToString(dtAsm.Rows[0]["Result"]) == "Pass")
                    {
                        try
                        {
                            Common.LineNum = ConfigurationManager.AppSettings["Line Number"].ToString();
                            if (Common.LineNum == "227")
                            {
                                if (string.IsNullOrEmpty(Common.JobCustomerCheck))
                                {
                                    frmCountrySetup frmCountry = new frmCountrySetup();
                                    frmCountry.ShowDialog();
                                }

                                lblCustomerLocation.Visible = true;
                                lblCustomerLocationLbl.Visible = true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        lblStatusVal.Text = "Validation Passed";
                        this.BackColor = System.Drawing.Color.Lime;
                        lblSerialNoVal.Text = Common.SerialNo;
                        Common.PartNum = dtAsm.Rows[0]["Part_Num"].ToString();
                        lblPartNoVal.Text = Common.PartNum;
                        Common.JobNum = dtAsm.Rows[0]["Job_Num"].ToString();
                        lblJobNoVal.Text = Common.JobNum;
                        Common.JobCustomerCheck = Common.JobNum;

                        Common.LogMessage("ASM NO --> " + Common.SerialNo + " : Part Num --> " + Common.PartNum + " : Job No --> " + Common.JobNum);

                        if (!Common.IsNewContainer)
                        {
                            ContainerNo = fetchContainerNumber(Common.LineNum);
                            var count = VerifyContainerNumberExist(Common.LineNum + Convert.ToInt32(ContainerNo).ToString("00000"));
                            if (count > 0)
                            {
                                ContainerNo = (Convert.ToInt32(ContainerNo) + 1).ToString();
                            }
                            ContainerPartsCount = getContainerPartsCount(Common.LineNum + Convert.ToInt32(ContainerNo).ToString("00000"));
                            Common.LogMessage("ContainerPartsCount: " + ContainerPartsCount.ToString());
                            getContainerLayerDetails(Common.PartNum);
                            Common.LogMessage("ContainerLayerParts: " + Common.ContainerLayerParts.ToString());
                            Common.LogMessage("ContainerLayers: " + Common.ContainerLayers.ToString());
                            if (ContainerPartsCount >= (Common.ContainerLayerParts * Common.ContainerLayers))
                            {
                                ContainerNo = (Convert.ToInt32(ContainerNo) + 1).ToString();
                                ContainerPartsCount = 0;
                            }
                            Common.LogMessage("ContainerNo: " + ContainerNo);

                            if (!string.IsNullOrEmpty(ContainerNo))
                            {
                                Common.ContainerNum = Common.LineNum + Convert.ToInt32(ContainerNo).ToString("00000");
                                lblContainerVal.Text = Common.ContainerNum;
                            }

                            ContainerPartsCount = ContainerPartsCount + 1;

                            lblContainerQtyVal.Text = ContainerPartsCount.ToString() + " Of " + (Common.ContainerLayerParts * Common.ContainerLayers);

                            if (ContainerPartsCount == (Common.ContainerLayerParts * Common.ContainerLayers))
                            {
                                PrintContainerLabel(Common.PartNum, ContainerPartsCount);
                            }
                            Common.IsNewContainer = false;
                        }

                        Common.ContainerQtyCompleted = GetContainerCompletedQty(Common.ContainerNum, Common.JobNum, Common.LineNum);

                        if (string.IsNullOrEmpty(Common.ContainerQtyCompleted))
                        {
                            Common.ContainerQtyCompleted = "1";
                        }
                        else
                        {
                            Common.ContainerQtyCompleted = (Convert.ToInt32(Common.ContainerQtyCompleted) + 1).ToString();
                        }

                        lblContainerQtyVal.Text = Common.ContainerQtyCompleted + " Of " + (Common.ContainerLayerParts * Common.ContainerLayers);

                        UpdateContainerCompletedQty(Common.ContainerNum, Common.JobNum, Common.LineNum);

                        if (Common.LineNum == "227")
                        {
                            ProcessToteContainer();
                        }

                        GetJobQuantities(Common.JobNum, ref Common.JobCompleted, ref Common.JobTotalQty);

                        if (string.IsNullOrEmpty(Common.JobCompleted) || Common.JobCompleted == "0" || Common.JobCompleted == "")
                            Common.JobCompleted = "1";
                        else
                            Common.JobCompleted = (Convert.ToInt32(Common.JobCompleted) + 1).ToString();

                        UpdateJobQuantities();

                        UpdateSerialContainer(Common.ContainerNum);

                        Common.LogMessage("Job Completed: " + Common.JobCompleted);

                        lblJobQtyVal.Text = Common.JobCompleted + " Of " + Common.JobTotalQty;

                        UpdateAsmStatusComplete();

                        if (Common.LineNum == "227")
                        {
                            UpdateCustomerCode();
                            UpdateToteSerialContainer(Common.ToteContainerNum);
                        }

                        if ((Common.LineNum == "227") && (Common.JobCompleted == Common.JobTotalQty))
                        {
                            Common.JobCustomerCheck = string.Empty;
                        }
                        if ((Common.JobCompleted == Common.JobTotalQty) && (Convert.ToInt32(Common.ContainerQtyCompleted) != (Common.ContainerLayerParts * Common.ContainerLayers)))
                        {
                            PrintContainerLabel(Common.PartNum, Convert.ToInt32(Common.ContainerQtyCompleted));
                            if (Common.LineNum == "227")
                            {
                                PrintToteContainerLabel(Common.PartNum, Common.ToteContainerQtyCompleted);
                            }
                        }
                        if (Common.JobCompleted == Common.JobTotalQty)
                        {
                            UpdateJobCompletedM2M();
                        }
                    }
                    else
                    {
                        if (Common.LineNum == "221")
                        {
                            Common.OrginalPartNum = string.Empty;
                            Common.NewPartNum = string.Empty;
                            Common.SerialNo = string.Empty;
                            lblValidationTitleVal.Text = "Part# has not completed ASM1";
                            return;
                        }
                    }
                }
                if (Common.LineNum == "ALP001" || Common.LineNum == "221")
                {
                    Common.OrginalPartNum = string.Empty;
                    Common.NewPartNum = string.Empty;
                    Common.SerialNo = string.Empty;
                    lblValidationTitleVal.Text = "Scan Finished Part#"; 
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void UpdateJobCompletedM2M()
        {
            try
            {
                var sqlConnection1 = new SqlConnection(Common.M2MConnectionString);
                var cmd1 = new SqlCommand();

                if (sqlConnection1.State != ConnectionState.Open)
                {
                    sqlConnection1.Open();
                }
                cmd1.CommandText = "update jomast set fstatus= 'CLOSED' where fjobno = @Job ;";
                cmd1.Parameters.AddWithValue("@Job", Common.JobNum);
                cmd1.Connection = sqlConnection1;
                cmd1.ExecuteNonQuery();
                sqlConnection1.Close();
                Common.LogMessage("status updated for jomast");
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void ProcessToteContainer()
        {
            try
            {
                var ToteContainerNo = fetchToteContainerNumber(Common.LineNum);
                Common.LogMessage("ToteContainerNo: " + ToteContainerNo);
                var count = VerifyToteContainerNumberExist(ToteContainerNo);
                if (count > 0)
                {
                    ToteContainerNo = (Convert.ToInt32(ToteContainerNo) + 1).ToString();
                    Common.LogMessage("Updated ToteContainerNo: " + ToteContainerNo);
                }
                var ToteContainerPartsCount = getToteContainerPartsCount(ToteContainerNo);
                Common.LogMessage("ToteContainerPartsCount: " + ToteContainerPartsCount);
                
                if (ToteContainerPartsCount >= Common.ContainerLayerParts)
                {
                    ToteContainerNo = (Convert.ToInt32(ToteContainerNo) + 1).ToString();
                    ToteContainerPartsCount = 0;
                }

                if (!string.IsNullOrEmpty(ToteContainerNo))
                {
                    Common.ToteContainerNum = ToteContainerNo;                    
                }               
                ToteContainerPartsCount = ToteContainerPartsCount + 1;
                Common.ToteContainerQtyCompleted = ToteContainerPartsCount;
                if (ToteContainerPartsCount == Common.ContainerLayerParts)
                {
                    PrintToteContainerLabel(Common.PartNum, Common.ContainerLayerParts);
                    Common.LogMessage(" Common.ToteContainerQtyCompleted : " + Common.ToteContainerQtyCompleted);                    
                }
                UpdateToteContainerCompletedQty(Common.ToteContainerNum, Common.JobNum, Common.LineNum);
            }
            catch(Exception ex)
            {
                 Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            } 
        }

        private string fetchToteContainerNumber(string LineNum)
        {
            string ToteContainerNumber = string.Empty;

            try
            {
                ToteContainerNumber = GetMaxToteContainerNumber(LineNum);

                if (ToteContainerNumber == "0")
                {
                    var tsql = "SELECT * FROM ToteSerialNumbers;";
                    Common.LogMessage("fetchToteContainerNumber: " + tsql);
                    var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                    var cmd = new SqlCommand();
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = tsql;

                    var daContainerNumbers = new SqlDataAdapter(cmd);
                    var dtContainerNumbers = new DataTable();
                    daContainerNumbers.Fill(dtContainerNumbers);
                    sqlConnection.Close();

                    if (dtContainerNumbers.Rows.Count > 0)
                    {
                        ToteContainerNumber = dtContainerNumbers.Rows[0]["SerialNumber"].ToString();
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return ToteContainerNumber;
        }

        private void UpdateSerialContainer(string ContainerNo)
        {
            try
            {
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();

                sql = "UPDATE " + Common.AsmTable + " SET Container_Serial = @ContainerNo WHERE Asm_Num = '" + Common.SerialNo + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ContainerNo", ContainerNo);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
            
        }

        private void UpdateToteSerialContainer(string ContainerNo)
        {
            try
            {
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();

                sql = "UPDATE " + Common.AsmTable + " SET Tote_Serial = @ContainerNo WHERE Asm_Num = '" + Common.SerialNo + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ContainerNo", ContainerNo);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

        }


        private void UpdateAsmStatusComplete()
        {
            try
            {
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();

                sql = "UPDATE " + Common.AsmTable + " SET Result = 'Completed' WHERE Asm_Num = '" + Common.SerialNo + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
            
        }

        private void UpdateCustomerCode()
        {
            try
            {
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();

                sql = "UPDATE " + Common.AsmTable + " SET CustomerCode = @Customer WHERE Asm_Num = '" + Common.SerialNo + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@Customer", Common.CustomerID);
                
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

        }

        private void UpdateJobQuantities()
        {
            try
            {
                string sql = string.Empty;
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();

                sql = "UPDATE JobQuantities SET CompletedQty = @CompletedQty WHERE jobnum = '" + Common.JobNum + "'";

                sqlConnection.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@CompletedQty", Common.JobCompleted);
                cmd.Connection = sqlConnection;
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
            
        }

        private void getContainerLayerDetails(string partNo)
        {
            try
            {
                string tsql = string.Empty;
                if (Common.LineNum == "ALP001")
                {
                    tsql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE NewPartNum = '" + partNo + "'";
                }
                else
                {
                    tsql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE Part_Num = '" + partNo + "'";
                }                
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daAsm = new SqlDataAdapter(cmd);
                var dtAsm = new DataTable();
                daAsm.Fill(dtAsm);
                sqlConnection.Close();

                if (dtAsm.Rows.Count > 0)
                {
                    if (Common.LineNum == "227")
                    {
                        if (!string.IsNullOrEmpty(Common.CustomerCode))
                        {
                            var tsql1 = "SELECT * FROM ShippingCustomer WHERE Customer = '" + Common.CustomerCode + "'";
                            var sqlConnection1 = new SqlConnection(Common.ASMConnectionString);
                            var cmd1 = new SqlCommand();
                            if (sqlConnection1.State != ConnectionState.Open)
                            {
                                sqlConnection1.Open();
                            }
                            cmd1.Connection = sqlConnection1;
                            cmd1.CommandType = System.Data.CommandType.Text;
                            cmd1.CommandText = tsql1;

                            var daAsm1 = new SqlDataAdapter(cmd1);
                            var dtAsm1 = new DataTable();
                            daAsm1.Fill(dtAsm1);
                            sqlConnection1.Close();
                            foreach (var row in dtAsm1.Rows.Cast<DataRow>().Where(row => Convert.ToString(row["Customer"]) == Common.CustomerCode))
                            {
                                Common.ContainerLayerParts = Convert.ToInt32(dtAsm1.Rows[0]["Layer_PartsQty"]);
                                Common.ContainerLayers = Convert.ToInt32(dtAsm1.Rows[0]["Layers_Qty"]);
                                Common.CustomerID = dtAsm1.Rows[0]["CustomerCode"].ToString();
                            }
                        }
                    }
                    else
                    {
                        Common.ContainerLayerParts = Convert.ToInt32(dtAsm.Rows[0]["Layer_PartsQty"]);
                        Common.ContainerLayers = Convert.ToInt32(dtAsm.Rows[0]["Layers_Qty"]);
                    }                    
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private int getContainerPartsCount(string ContainerNo)
        {
            int PCount = 0;

            try
            {
                //var tsql = "SELECT ISNULL(count(Container_Serial),0) AS PCount FROM " + AsmTable + " WHERE Container_Serial = '" + ContainerNo + "'";
                var tsql = "SELECT ISNULL(CompletedQty,0) AS CompletedQty FROM ContainerQuantities WHERE ContainerNum = '" + ContainerNo + "' and JobNum = '" + Common.JobNum + "' and LineNum = '" + Common.LineNum + "'";
                Common.LogMessage("getContainerpartsCount: " + tsql);
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daAsm = new SqlDataAdapter(cmd);
                var dtAsm = new DataTable();
                daAsm.Fill(dtAsm);
                sqlConnection.Close();

                if (dtAsm.Rows.Count > 0)
                {
                    PCount = Convert.ToInt32(dtAsm.Rows[0]["CompletedQty"]);
                }

            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            
             return PCount;
        }

        private int getToteContainerPartsCount(string ContainerNo)
        {
            int PCount = 0;

            try
            {
                //var tsql = "SELECT ISNULL(count(Container_Serial),0) AS PCount FROM " + AsmTable + " WHERE Container_Serial = '" + ContainerNo + "'";
                var tsql = "SELECT ISNULL(CompletedQty,0) AS CompletedQty FROM ToteQuantities WHERE ToteNum = '" + ContainerNo + "' and JobNum = '" + Common.JobNum + "' ";
                Common.LogMessage("getContainerpartsCount: " + tsql);
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daAsm = new SqlDataAdapter(cmd);
                var dtAsm = new DataTable();
                daAsm.Fill(dtAsm);
                sqlConnection.Close();

                if (dtAsm.Rows.Count > 0)
                {
                    PCount = Convert.ToInt32(dtAsm.Rows[0]["CompletedQty"]);
                }

            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }


            return PCount;
        }


        private void UpdateContainerCompletedQty(string ContainerNum, string JobNum, string LineNum)
        {
            string sql = string.Empty;
            try
            {
                if (Common.ContainerQtyCompleted == "1")
                    sql = "INSERT INTO ContainerQuantities (JobNum, LineNum, ContainerNum, CompletedQty) VALUES( @JobNum , @LineNum , @ContainerNum , @CompletedQty );";
                else
                    sql = "UPDATE ContainerQuantities set  CompletedQty = @CompletedQty where JobNum =  @JobNum and LineNum = @LineNum and  ContainerNum= @ContainerNum ;";

                Common.LogMessage("Container Qty sql:" + sql);

                var cmd1 = new SqlCommand();
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);

                sqlConnection.Open();
                cmd1.CommandText = sql;
                cmd1.Parameters.AddWithValue("@JobNum", JobNum);
                cmd1.Parameters.AddWithValue("@LineNum", LineNum);
                cmd1.Parameters.AddWithValue("@ContainerNum", ContainerNum);
                cmd1.Parameters.AddWithValue("@CompletedQty", Common.ContainerQtyCompleted);
                cmd1.Connection = sqlConnection;
                cmd1.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
        }

        private void UpdateToteContainerCompletedQty(string ContainerNum, string JobNum, string LineNum)
        {
            string sql = string.Empty;
            try
            {
                if (Common.ToteContainerQtyCompleted == 1)
                    sql = "INSERT INTO ToteQuantities (JobNum, ToteNum, CompletedQty) VALUES( @JobNum , @ContainerNum , @CompletedQty );";
                else
                    sql = "UPDATE ToteQuantities set  CompletedQty = @CompletedQty where JobNum =  @JobNum and  ToteNum= @ContainerNum ;";

                Common.LogMessage("Container Qty sql:" + sql);

                var cmd1 = new SqlCommand();
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);

                sqlConnection.Open();
                cmd1.CommandText = sql;
                cmd1.Parameters.AddWithValue("@JobNum", JobNum);
                cmd1.Parameters.AddWithValue("@ContainerNum", ContainerNum);
                cmd1.Parameters.AddWithValue("@CompletedQty", Common.ToteContainerQtyCompleted);
                cmd1.Connection = sqlConnection;
                cmd1.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
        }


        private void GetJobQuantities(string JobNum, ref string JobCompleted, ref string JobTotalQty)
        {
            var sql = "SELECT * FROM JobQuantities WHERE JobNum = '" + JobNum + "'";

            Common.LogMessage("GetJobQuantities sql: " + sql);
            try
            {
                SqlConnection sqlConnection = new SqlConnection(Common.ASMConnectionString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.CommandText = sql;
                var daJobQuantities = new SqlDataAdapter(cmd);
                var dtJobQuantities = new DataTable();
                daJobQuantities.Fill(dtJobQuantities);
                sqlConnection.Close();

                if (dtJobQuantities.Rows.Count > 0)
                {
                    JobCompleted = dtJobQuantities.Rows[0]["CompletedQty"].ToString();
                    JobTotalQty = dtJobQuantities.Rows[0]["JobQty"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            
        }

        private string GetContainerCompletedQty(string ContainerNo, string JobNum, string LineNum)
        {
            string ContainerCompleted = string.Empty;

            try
            {
                var tsql = "SELECT ISNULL(CompletedQty,0) As CompletedQty FROM ContainerQuantities WHERE LineNum = '" + LineNum + "' and JobNum='" + JobNum + "' and ContainerNum='" + ContainerNo + "'";

                Common.LogMessage("GetContainerCompletedQty sql: " + tsql);

                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerNumbers = new SqlDataAdapter(cmd);
                var dtContainerNumbers = new DataTable();
                daContainerNumbers.Fill(dtContainerNumbers);
                sqlConnection.Close();

                if (dtContainerNumbers.Rows.Count > 0)
                {
                    ContainerCompleted = dtContainerNumbers.Rows[0]["CompletedQty"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return ContainerCompleted;
        }

        private string GetToteContainerCompletedQty(string ContainerNo, string JobNum, string LineNum)
        {
            string ContainerCompleted = string.Empty;

            try
            {
                var tsql = "SELECT ISNULL(CompletedQty,0) As CompletedQty FROM ToteQuantities WHERE JobNum='" + JobNum + "' and ToteNum='" + ContainerNo + "'";

                Common.LogMessage("GetContainerCompletedQty sql: " + tsql);

                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerNumbers = new SqlDataAdapter(cmd);
                var dtContainerNumbers = new DataTable();
                daContainerNumbers.Fill(dtContainerNumbers);
                sqlConnection.Close();

                if (dtContainerNumbers.Rows.Count > 0)
                {
                    ContainerCompleted = dtContainerNumbers.Rows[0]["CompletedQty"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return ContainerCompleted;
        }


        private string GetPartDescription(string PartNum)
        {
            string strPartDesc = string.Empty;
            try
            {
                var tsql = "select fdescript from inmast where fpartno = '" + PartNum + "'";
                var sqlConnString = Common.M2MConnectionString;
                var sqlConnection = new SqlConnection(sqlConnString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;
                var daContainer = new SqlDataAdapter(cmd);
                var dtContainer = new DataTable();
                daContainer.Fill(dtContainer);
                sqlConnection.Close();
                if (dtContainer.Rows.Count > 0)
                {
                    strPartDesc = dtContainer.Rows[0]["fdescript"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
            return strPartDesc;
        }

        private int VerifyContainerNumberExist(string ContainerNum)
        {
            int CCount = 0;

            try
            {
                var tsql = "SELECT ISNULL(CompletedQty,0) As CompletedQty FROM ContainerQuantities WHERE LineNum = '" + Common.LineNum + "' and JobNum <>'" + Common.JobNum + "' and ContainerNum='" + ContainerNum + "'";
                Common.LogMessage("VerifyContainerNumberExist: " + tsql);
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerNumbers = new SqlDataAdapter(cmd);
                var dtContainerNumbers = new DataTable();
                daContainerNumbers.Fill(dtContainerNumbers);
                sqlConnection.Close();

                if (dtContainerNumbers.Rows.Count > 0)
                {
                    CCount = Convert.ToInt32(dtContainerNumbers.Rows[0]["CompletedQty"]);
                }               
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return CCount;
        }

        private int VerifyToteContainerNumberExist(string ContainerNum)
        {
            int CCount = 0;

            try
            {
                var tsql = "SELECT ISNULL(CompletedQty,0) As CompletedQty FROM ToteQuantities WHERE JobNum <>'" + Common.JobNum + "' and ToteNum='" + ContainerNum + "'";
                Common.LogMessage("VerifyContainerNumberExist: " + tsql);
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerNumbers = new SqlDataAdapter(cmd);
                var dtContainerNumbers = new DataTable();
                daContainerNumbers.Fill(dtContainerNumbers);
                sqlConnection.Close();

                if (dtContainerNumbers.Rows.Count > 0)
                {
                    CCount = Convert.ToInt32(dtContainerNumbers.Rows[0]["CompletedQty"]);
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return CCount;
        }

        private string fetchContainerNumber(string LineNum)
        {
            string ContainerNumber = string.Empty;

            try
            {
                ContainerNumber = GetMaxContainerNumber(LineNum);

                if (ContainerNumber == "0")
                {
                    var tsql = "SELECT * FROM ContainerNumbers WHERE LineNum = '" + LineNum + "'";
                    Common.LogMessage("fetchContainerNumber: " + tsql);
                    var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                    var cmd = new SqlCommand();
                    cmd.Connection = sqlConnection;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = tsql;

                    var daContainerNumbers = new SqlDataAdapter(cmd);
                    var dtContainerNumbers = new DataTable();
                    daContainerNumbers.Fill(dtContainerNumbers);
                    sqlConnection.Close();

                    if (dtContainerNumbers.Rows.Count > 0)
                    {
                        ContainerNumber = dtContainerNumbers.Rows[0]["ContainerNumber"].ToString();
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            

            return ContainerNumber;
        }

        private string GetMaxContainerNumber(string LineNum)
        {
            string ContainerNumber = string.Empty;

            try
            {
                var tsql = "select convert(varchar(20), ISNULL(max(convert(int, substring(ContainerNum, Len(LineNum)+ 1,  Len(ContainerNum) - Len(LineNum)))),0)) As NextContainerNum from ContainerQuantities where LineNum = '" + LineNum + "'";
                Common.LogMessage("GetMaxContainerNumber: " + tsql);
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerQuantities = new SqlDataAdapter(cmd);
                var dtContainerQuantities = new DataTable();
                daContainerQuantities.Fill(dtContainerQuantities);
                sqlConnection.Close();

                if (dtContainerQuantities.Rows.Count > 0)
                {

                    ContainerNumber = dtContainerQuantities.Rows[0]["NextContainerNum"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return ContainerNumber;
        }

        private string GetMaxToteContainerNumber(string LineNum)
        {
            string ContainerNumber = string.Empty;

            try
            {
                var tsql = "select ISNULL(max(ToteNum),0) As NextContainerNum from ToteQuantities ;";
                Common.LogMessage("GetMaxToteContainerNumber: " + tsql);
                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerQuantities = new SqlDataAdapter(cmd);
                var dtContainerQuantities = new DataTable();
                daContainerQuantities.Fill(dtContainerQuantities);
                sqlConnection.Close();

                if (dtContainerQuantities.Rows.Count > 0)
                {

                    ContainerNumber = dtContainerQuantities.Rows[0]["NextContainerNum"].ToString();
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

            return ContainerNumber;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                // Get the COPYDATASTRUCT struct from lParam.
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

                // If the size matches
                if (cds.cbData == Marshal.SizeOf(typeof(FalconStruct)))
                {
                    // Marshal the data from the unmanaged memory block to a 
                    // MyStruct managed struct.
                    FalconStruct falconStruct = (FalconStruct)Marshal.PtrToStructure(cds.lpData,
                        typeof(FalconStruct));

                    Common.SerialNo = falconStruct.AsmNumber;
                    Common.ASMConnectionString = falconStruct.ASMConnectionString;
                    Common.M2MConnectionString = falconStruct.M2MConnectionString;
                    Common.LineNum = falconStruct.LineNum;
                    Common.AsmTable = falconStruct.AsmTable;

                    LoadSerialData();
                }
            }

            base.WndProc(ref m);
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct FalconStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string AsmNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string ASMConnectionString;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string M2MConnectionString;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string AsmTable;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string LineNum;
        }


        #region Native API Signatures and Types

        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another 
        /// application.
        /// </summary>
        internal const int WM_COPYDATA = 0x004A;


        /// <summary>
        /// The COPYDATASTRUCT structure contains data to be passed to another 
        /// application by the WM_COPYDATA message. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // Specifies data to be passed
            public int cbData;          // Specifies the data size in bytes
            public IntPtr lpData;       // Pointer to data to be passed
        }

        #endregion

        private void startNewContainerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string ContainerNo = string.Empty;
                int ContainerCount = 0;
                if (string.IsNullOrEmpty(Common.JobNum))
                {
                    MessageBox.Show("Job Num is Not Available");
                    Common.IsNewContainer = false;
                    return;
                }
                if (MessageBox.Show("Do you want to Start New Container?", "New Container", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Common.ContainerQtyCompleted = GetContainerCompletedQty(Common.ContainerNum, Common.JobNum, Common.LineNum);
                    Common.LogMessage("Common.ContainerQtyCompleted : " + Common.ContainerQtyCompleted);
                    PrintContainerLabel(Common.PartNum, Convert.ToInt32(Common.ContainerQtyCompleted));
                    ContainerNo = Common.ContainerNum;
                    Common.LogMessage("Common.ContainerNum : " + Common.ContainerNum);
                    Common.LogMessage("Common.LineNum : " + Common.LineNum);
                    ContainerCount = Convert.ToInt32(ContainerNo.Substring(Common.LineNum.Length)) + 1;
                    Common.LogMessage("ContainerCount : " + ContainerCount);
                    Common.ContainerNum = Common.LineNum + ContainerCount.ToString("00000");
                    lblContainerVal.Text = Common.ContainerNum;
                    lblContainerQtyVal.Text = "0 Of " + (Common.ContainerLayerParts * Common.ContainerLayers);
                    Common.IsNewContainer = true;
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void FlushContainerQty(string LineNum, string JobNum)
        {
            try
            {
                var tsql = "DELETE FROM ContainerQuantities WHERE LineNum = '" + LineNum + "' and JobNum='" + JobNum + "' ";
                var sqlConnString = Common.ASMConnectionString;
                var sqlConnection = new SqlConnection(sqlConnString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daContainerNumbers = new SqlDataAdapter(cmd);
                var dtContainerNumbers = new DataTable();
                daContainerNumbers.Fill(dtContainerNumbers);
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void reprintMasterLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.LineNum = ConfigurationManager.AppSettings["Line Number"].ToString();
            if (Common.LineNum == "227")
            {
                if (string.IsNullOrEmpty(Common.CustomerCode))
                {
                    frmCountrySetup frmCountry = new frmCountrySetup();
                    frmCountry.ShowDialog();
                }
            }
            gbMasterLabelReprint.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtContainerQty.Text = "";
            txtInputContainer.Text = "";
            txtLabelReprintQty.Text = "";
            gbMasterLabelReprint.Visible = false;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            RePrintContainerLabel();
        }

        private void RePrintContainerLabel()
        {
            string ContainerNo = txtInputContainer.Text;

            if (!string.IsNullOrEmpty(ContainerNo))
            {
                UpdateLabelDetailsByContainer(ContainerNo);
            }
        }

        private void UpdateToteLabelDetailsByContainer(string ContainerNo)
        {
            string PartNo = string.Empty;
            string JobNum = string.Empty;
            string ModifiedPrintFormat = string.Empty;
            try
            {
                Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
                Common.M2MConnectionString = ConfigurationManager.AppSettings["M2M_CONNECTION_STRING"].ToString();
                Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var tsql = "SELECT * FROM " + Common.AsmTable + " WHERE Tote_Serial = '" + ContainerNo + "'";
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daAsm = new SqlDataAdapter(cmd);
                var dtAsm = new DataTable();
                daAsm.Fill(dtAsm);
                sqlConnection.Close();

                if (dtAsm.Rows.Count > 0)
                {
                    PartNo = Convert.ToString(dtAsm.Rows[0]["Part_Num"]);
                    JobNum = Convert.ToString(dtAsm.Rows[0]["Job_Num"]);
                }

                getContainerLabelPrintDetails(PartNo);
                Common.PartDesc = GetPartDescription(PartNo);
                //ModifyPrintToteLabel(PartNo, Common.POID, Convert.ToInt32(txtContainerQty.Text), Convert.ToInt32(txtLabelReprintQty.Text), Common.SupplierNo, txtInputContainer.Text, Common.PartDesc, Common.JobPrefix, Common.JobNum, Common.CustomerID);
                ModifiedPrintFormat = ModifyPrintToteLabel(PartNo, Common.POID, Convert.ToInt32(txtToteContainerQty.Text), Convert.ToInt32(txtToteLabelReprintQty.Text), Common.SupplierNo, txtInputToteContainer.Text, Common.PartDesc, Common.JobPrefix, JobNum, Common.CustomerID);
                Common.LogMessage("Print File: " + ModifiedPrintFormat);
                //for (int p = 0; p < Convert.ToInt32(txtLabelReprintQty.Text); p++)
                //{
                Common.LogMessage("Print Qty: " + txtToteLabelReprintQty.Text);
                Print(ModifiedPrintFormat, Common.PrinterIP);
                //}
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }


        private void UpdateLabelDetailsByContainer(string ContainerNo)
        {
            string PartNo = string.Empty;
            string JobNum = string.Empty;
            string ModifiedPrintFormat = string.Empty;

            Common.ASMConnectionString = ConfigurationManager.AppSettings["ASM_CONNECTION_STRING"].ToString();
            Common.M2MConnectionString = ConfigurationManager.AppSettings["M2M_CONNECTION_STRING"].ToString();
            Common.AsmTable = ConfigurationManager.AppSettings["Assembly Table"].ToString();

            var sqlConnection = new SqlConnection(Common.ASMConnectionString);
            var tsql = "SELECT * FROM " + Common.AsmTable + " WHERE Container_Serial = '" + ContainerNo + "'";
            var cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = tsql;

            var daAsm = new SqlDataAdapter(cmd);
            var dtAsm = new DataTable();
            daAsm.Fill(dtAsm);
            sqlConnection.Close();

            if (dtAsm.Rows.Count > 0)
            {
                PartNo = Convert.ToString(dtAsm.Rows[0]["Part_Num"]);
                JobNum = Convert.ToString(dtAsm.Rows[0]["Job_Num"]);
            }

            getContainerLabelPrintDetails(PartNo);
            Common.PartDesc = GetPartDescription(PartNo);
            ModifiedPrintFormat = ModifyPrintLabel(PartNo, Common.POID, Convert.ToInt32(txtContainerQty.Text), Convert.ToInt32(txtLabelReprintQty.Text), Common.SupplierNo, txtInputContainer.Text, Common.PartDesc, Common.JobPrefix, JobNum, Common.FacilityID);
            Common.LogMessage("Print File: " + ModifiedPrintFormat);
            for (int p = 0; p < Convert.ToInt32(txtLabelReprintQty.Text); p++)
            {
                Common.LogMessage("Print Qty: " + txtLabelReprintQty.Text);
                Print(ModifiedPrintFormat, Common.PrinterIP);
            }
        }

        private void PrintContainerLabel(string PartNo, int ContainerQty)
        {
            string ModifiedPrintFormat = "";

            getContainerLabelPrintDetails(PartNo);
            Common.PartDesc = GetPartDescription(PartNo);
            ModifiedPrintFormat = ModifyPrintLabel(PartNo, Common.POID, ContainerQty,Common.PrintQty, Common.SupplierNo, Common.ContainerNum, Common.PartDesc, Common.JobPrefix, Common.JobNum, Common.FacilityID);
            Common.LogMessage("Print File: " + ModifiedPrintFormat);
            for (int p = 0; p < Common.PrintQty; p++)
            {
                Common.LogMessage("Print Qty: " + Common.PrintQty);
                Print(ModifiedPrintFormat, Common.PrinterIP);
            }
        }

        private void PrintToteContainerLabel(string PartNo, int ContainerQty)
        {
            string ModifiedPrintFormat = "";
            Common.LogMessage("PrintToteContainerLabel : Part" + PartNo + ": ToteContainerQty: " + ContainerQty);
            getContainerLabelPrintDetails(PartNo);
            Common.PartDesc = GetPartDescription(PartNo);
            ModifiedPrintFormat = ModifyPrintToteLabel(PartNo, Common.POID, Common.ContainerLayerParts, ContainerQty, Common.SupplierNo, Common.ToteContainerNum, Common.PartDesc, Common.JobPrefix, Common.JobNum, Common.CustomerID);
            Common.LogMessage("Print Tote File: " + ModifiedPrintFormat);
            Print(ModifiedPrintFormat, Common.PrinterIP);
        }


        private string ModifyPrintLabel(string PartNo, string POID, int ContainerLayerParts, int PrintQty, string SupplierNo, string ContainerNum, string PartDesc, string JobPrefix, string JobNum, string FacilityID)
        {
            string printFormat = "";
            string ModifiedPrintFormat = "";
            TextReader printFormatReader = new StreamReader(Application.StartupPath + "\\TempMasterLabel.prn");
            printFormat = printFormatReader.ReadToEnd();
            printFormatReader.Close();
            ModifiedPrintFormat = printFormat;
            if (Common.LineNum.StartsWith("ALP"))
            {
                ModifiedPrintFormat = UpdateBetweenStrings(printFormat, "<STX>H2;f0;o13,17;c26;b0;h68;w68;d3,", "<ETX>", PartNo);
            }
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F18<LF>", "<ETX>", "P" + PartNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F19<LF>", "<ETX>", "Q" + ContainerLayerParts.ToString());
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F21<LF>", "<ETX>", "V" + SupplierNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F22<LF>", "<ETX>", "S" + ContainerNum);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F23<LF>", "<ETX>", PartDesc);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F28<LF>", "<ETX>", PartNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F29<LF>", "<ETX>", ContainerLayerParts.ToString());
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F30<LF>", "<ETX>", POID);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F31<LF>", "<ETX>", SupplierNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F32<LF>", "<ETX>", ContainerNum);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F35<LF>", "<ETX>", JobPrefix + JobNum);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F36<LF>", "<ETX>", JobNum.Substring(0, (JobNum.IndexOf("-"))));
            if (Common.LineNum == "227")
            {
                ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F37<LF>", "<ETX>", Convert.ToInt32(Common.CustomerID).ToString("000"));
            }
            else
            {
                ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F37<LF>", "<ETX>", Convert.ToInt32(FacilityID).ToString("000"));
            }
            return ModifiedPrintFormat;
        }

        private string ModifyPrintToteLabel(string PartNo, string POID, int ContainerLayerParts, int PrintQty, string SupplierNo, string ContainerNum, string PartDesc, string JobPrefix, string JobNum, string FacilityID)
        {
            string printFormat = "";
            string ModifiedPrintFormat = "";
            TextReader printFormatReader = new StreamReader(Application.StartupPath + "\\TempToteLabel.prn");
            printFormat = printFormatReader.ReadToEnd();
            printFormatReader.Close();
            Common.LogMessage(" TempToteLabel: " + printFormat);
            ModifiedPrintFormat = UpdateBetweenStrings(printFormat, "<STX>H2;f0;o186,17;c26;b0;h68;w68;d3", "<ETX>", PartNo);           
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F18<LF>", "<ETX>", "P" + PartNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F19<LF>", "<ETX>", "Q" + PrintQty.ToString());
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F21<LF>", "<ETX>", "V" + SupplierNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F22<LF>", "<ETX>", "S" + ContainerNum);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F23<LF>", "<ETX>", PartDesc);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F28<LF>", "<ETX>", PartNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F29<LF>", "<ETX>", ContainerLayerParts.ToString());
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F30<LF>", "<ETX>", POID);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F31<LF>", "<ETX>", SupplierNo);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F32<LF>", "<ETX>", ContainerNum);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F35<LF>", "<ETX>", JobPrefix + JobNum);
            ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F36<LF>", "<ETX>", JobNum.Substring(0, JobNum.IndexOf("-")));
            if (Common.LineNum == "227")
            {
                ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F37<LF>", "<ETX>", Convert.ToInt32(Common.CustomerID).ToString("000"));
            }
            else
            {
                ModifiedPrintFormat = UpdateBetweenStrings(ModifiedPrintFormat, "<ESC>F37<LF>", "<ETX>", Convert.ToInt32(FacilityID).ToString("000"));
            }
            if (string.IsNullOrEmpty(ModifiedPrintFormat))
            {
                ModifiedPrintFormat = printFormat;
            }
            return ModifiedPrintFormat;
        }


        public static string UpdateBetweenStrings(string strSource, string strStart, string strEnd, string Replace)
        {
            int Start, End;
            string FindString = "";
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                FindString = strSource.Substring(0, Start) + Replace + strSource.Substring(End);
            }            
            return FindString;
        }

        private void getContainerLabelPrintDetails(string PartNo)
        {
            try
            {
                Common.PrinterIP = ConfigurationManager.AppSettings["Master Label Printer IP"].ToString();
                Common.PrintQty = Convert.ToInt32(ConfigurationManager.AppSettings["Master Label Print Qty"].ToString());

                //var tsql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE Part_Num = '" + PartNo + "'";
                string tsql = string.Empty;

                if (Common.LineNum == "ALP001")
                {
                    tsql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE NewPartNum = '" + PartNo + "'";
                }
                else
                {
                    tsql = "SELECT * FROM " + Common.AsmTable + "_ProcessMatrix WHERE Part_Num = '" + PartNo + "'";
                } 

                var sqlConnection = new SqlConnection(Common.ASMConnectionString);
                var cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = tsql;

                var daAsm = new SqlDataAdapter(cmd);
                var dtAsm = new DataTable();
                daAsm.Fill(dtAsm);
                sqlConnection.Close();

                if (dtAsm.Rows.Count > 0)
                {                    
                    Common.JobPrefix = dtAsm.Rows[0]["JobPrefix"].ToString();                   
                    if (Common.LineNum != "227")
                    {
                        Common.SupplierNo = dtAsm.Rows[0]["SupplierNo"].ToString();
                        Common.POID = dtAsm.Rows[0]["POID"].ToString();
                        Common.FacilityID = dtAsm.Rows[0]["FacilityID"].ToString();
                    }
                }

                if (Common.LineNum == "227")
                {
                    if (!string.IsNullOrEmpty(Common.CustomerCode))
                    {
                        var tsql1 = "SELECT * FROM ShippingCustomer WHERE Customer = '" + Common.CustomerCode + "'";
                        var sqlConnection1 = new SqlConnection(Common.ASMConnectionString);
                        var cmd1 = new SqlCommand();
                        if (sqlConnection1.State != ConnectionState.Open)
                        {
                            sqlConnection1.Open();
                        }
                        cmd1.Connection = sqlConnection1;
                        cmd1.CommandType = System.Data.CommandType.Text;
                        cmd1.CommandText = tsql1;

                        var daAsm1 = new SqlDataAdapter(cmd1);
                        var dtAsm1 = new DataTable();
                        daAsm1.Fill(dtAsm1);
                        sqlConnection1.Close();
                        foreach (var row in dtAsm1.Rows.Cast<DataRow>().Where(row => Convert.ToString(row["Customer"]) == Common.CustomerCode))
                        {
                            Common.SupplierNo = dtAsm1.Rows[0]["SupplierNo"].ToString();
                            Common.POID = dtAsm1.Rows[0]["POID"].ToString();
                            Common.FacilityID = dtAsm1.Rows[0]["FacilityID"].ToString();
                            Common.CustomerID = dtAsm1.Rows[0]["CustomerCode"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }

        }

        private static void Print(string data, string IP)
        {
            NetworkStream ns = null;
            Socket socket = null;

            IPEndPoint adresIP = new IPEndPoint(IPAddress.Parse(IP), 9100);


            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(adresIP);

            ns = new NetworkStream(socket);

            byte[] toSend = Encoding.ASCII.GetBytes(data);
            
            ns.Write(toSend, 0, toSend.Length);
            ns.Flush();
            if (ns != null)
                ns.Close();
            if (socket != null && socket.Connected)
                socket.Close();
        }

        private void updateConainerSerialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUpdateSerial frmUpdateSerial = new frmUpdateSerial();
            frmUpdateSerial.ShowDialog();
        }

        private void containerQtyUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmContainerQtyUpdate frm = new frmContainerQtyUpdate();
            frm.ShowDialog();
        }

        private void frmValidation_Load(object sender, EventArgs e)
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) System.Diagnostics.Process.GetCurrentProcess().Kill();
               
            if (string.IsNullOrEmpty(Common.LineNum))
            {
                Common.LineNum = ConfigurationManager.AppSettings["Line Number"].ToString();
            }
            if ((Common.LineNum == "ALP001" || Common.LineNum == "221"))
            {
                _serialPort = new System.IO.Ports.SerialPort("COM1");

                _serialPort.BaudRate = 9600;
                _serialPort.Parity = System.IO.Ports.Parity.None;
                _serialPort.StopBits = System.IO.Ports.StopBits.One;
                _serialPort.DataBits = 8;
                _serialPort.Handshake = System.IO.Ports.Handshake.None;

                _serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceivedHandler);

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
                lblValidationTitleVal.Text = "Scan Finished Part#"; 
            }
            try
            {
                Common.ScreenNo = (ConfigurationManager.AppSettings["ScreenNo"] != null) ? Convert.ToInt32(ConfigurationManager.AppSettings["ScreenNo"]) : 0;
            }
            catch (Exception ex)
            {

            }
            tmrMonitorPos.Enabled = true;
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

        private void DataReceivedHandler(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(250);
                System.IO.Ports.SerialPort sp = (System.IO.Ports.SerialPort)sender;
                string indata = sp.ReadExisting();
                Common.LogMessage("Reading.." + indata);
                frmValidation frmValidation = getForm<frmValidation>();
                Common.LogMessage("LineNum.." + Common.LineNum);
                if (Common.LineNum == "221")
                {
                    Common.NewPartNum = indata.Substring(1, 7);
                    Common.SerialNo = indata.Substring(10).Trim();

                    Common.LogMessage("PartNum.." + Common.NewPartNum);
                    //if (string.IsNullOrEmpty(Common.NewPartNum))
                    //{
                    //    Common.NewPartNum = indata.Substring(1).Trim();
                    //    frmValidation.lblValidationTitleVal.Text = "Scan Serial#";
                    //    return;
                    //}
                    Common.LogMessage("SerialNo.." + Common.SerialNo);
                    var dtAsm = getAssemblyDetails(Common.SerialNo);
                    if (dtAsm.Rows.Count > 0)
                    {
                        Common.OrginalPartNum = dtAsm.Rows[0]["Part_Num"].ToString();
                        Common.LogMessage("OrginalPartNum.." + Common.OrginalPartNum);
                        if (Convert.ToString(dtAsm.Rows[0]["Result"]) == "Completed")
                        {
                            frmValidation.lblValidationTitleVal.Text = "Part# already completed. Re-Scan Serial#";
                            //Common.NewPartNum = string.Empty;
                            Common.OrginalPartNum = string.Empty;
                            Common.SerialNo = string.Empty;
                            return;
                        }
                        else if (Common.NewPartNum == Common.OrginalPartNum)
                        {
                            frmValidation.lblValidationTitleVal.Text = "";
                            LoadSerialData();
                        }
                        else
                        {
                            frmValidation.lblValidationTitleVal.Text = "Scanned Part# did not match with Original Part#";
                            Common.NewPartNum = string.Empty;
                            Common.OrginalPartNum = string.Empty;
                            Common.SerialNo = string.Empty;
                            return;
                        }
                    }
                    else
                    {
                        frmValidation.lblValidationTitleVal.Text = "Serial # not in Database";
                        Common.NewPartNum = string.Empty;
                        Common.OrginalPartNum = string.Empty;
                        Common.SerialNo = string.Empty;
                        return;
                    }
                }
                else if (Common.LineNum == "ALP001")
                {
                    Common.LogMessage("NewPartNum.." + Common.NewPartNum);
                    if (string.IsNullOrEmpty(Common.NewPartNum))
                    {
                        Common.NewPartNum = indata.Substring(1).Trim(); 
                        frmValidation.lblValidationTitleVal.Text = "Scan Serial#";
                        return;
                    }
                    Common.LogMessage("SerialNo.." + Common.SerialNo);
                    if (string.IsNullOrEmpty(Common.SerialNo))
                    {
                        Common.SerialNo = indata.Substring(1).Trim();
                        var dtAsm = getAssemblyDetails(Common.SerialNo);
                        if (dtAsm.Rows.Count > 0)
                        {
                            Common.OrginalPartNum = dtAsm.Rows[0]["Part_Num"].ToString();
                            Common.LogMessage("OrginalPartNum.." + Common.OrginalPartNum);
                            if (Convert.ToString(dtAsm.Rows[0]["Result"]) == "Completed")
                            {
                                frmValidation.lblValidationTitleVal.Text = "Part# already completed. Re-Scan Serial#";
                                //Common.NewPartNum = string.Empty;
                                Common.OrginalPartNum = string.Empty;
                                Common.SerialNo = string.Empty;
                                return;
                            }
                            else if (Common.NewPartNum == Common.OrginalPartNum)
                            {
                                frmValidation.lblValidationTitleVal.Text = "";
                                LoadSerialData();
                            }
                            else
                            {
                                frmValidation.lblValidationTitleVal.Text = "Scanned Part# did not match with Original Part#";
                                Common.NewPartNum = string.Empty;
                                Common.OrginalPartNum = string.Empty;
                                Common.SerialNo = string.Empty;
                                return;
                            }
                        }
                        else
                        {
                            frmValidation.lblValidationTitleVal.Text = "Serial # not in Database";
                            Common.NewPartNum = string.Empty;
                            Common.OrginalPartNum = string.Empty;
                            Common.SerialNo = string.Empty;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }
        }

        private void btnTotePrint_Click(object sender, EventArgs e)
        {
            string ToteContainerNo = txtInputToteContainer.Text;

            if (!string.IsNullOrEmpty(ToteContainerNo))
            {
                UpdateToteLabelDetailsByContainer(ToteContainerNo);
            }
        }

        private void btnToteCancel_Click(object sender, EventArgs e)
        {
            txtToteContainerQty.Text = "";
            txtInputToteContainer.Text = "";
            txtToteLabelReprintQty.Text = "";
            gbToteReprint.Visible = false;
        }

        private void endProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void reprintToteLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Common.LineNum = ConfigurationManager.AppSettings["Line Number"].ToString();
                if (Common.LineNum == "227")
                {
                    if (string.IsNullOrEmpty(Common.CustomerCode))
                    {
                        frmCountrySetup frmCountry = new frmCountrySetup();
                        frmCountry.ShowDialog();
                    }
                    gbToteReprint.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Common.LogMessage("Error Occured.." + ex.Message + ": " + ex.StackTrace);
            }            
        }

        private void tmrMonitorPos_Tick(object sender, EventArgs e)
        {
            if (Common.ScreenNo > 0)
            {
                var frmMain = getForm<frmValidation>();
                Common.SetMonitorForm(frmMain.Text, Common.ScreenNo);
            }
            tmrMonitorPos.Enabled = false;
        }

        private void tmrPassClear_Tick(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.Blue;
        }
    }
}
