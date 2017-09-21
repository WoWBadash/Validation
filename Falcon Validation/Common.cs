using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace Falcon_Validation
{
    public class Common
    {
        internal static string SerialNo = "";
        internal static bool IsNewContainer = false;
        internal static string ASMConnectionString = "";
        internal static string M2MConnectionString = "";
        internal static string AsmTable = "";
        internal static string PartNum = "";
        internal static string PartDesc = "";
        internal static string JobNum = "";
        internal static string LineNum = string.Empty;
        internal static string ContainerNum = string.Empty;
        internal static string ToteContainerNum = string.Empty;
        internal static string ContainerQtyCompleted = string.Empty;
        internal static int ToteContainerQtyCompleted = 0;
        internal static string ContainerTotalQty = string.Empty;
        internal static string JobTotalQty = string.Empty;
        internal static string JobCompleted = string.Empty;
        internal static int ContainerLayerParts = 0;
        internal static int ContainerLayers = 0;
        internal static string PrinterIP = string.Empty;
        internal static int PrintQty = 0;
        internal static string POID = string.Empty;
        internal static string SupplierNo = string.Empty;
        internal static string FacilityID = string.Empty;
        internal static string CustomerID = string.Empty;
        internal static string JobPrefix = string.Empty;
        internal static string JobCustomerCheck = string.Empty;
        internal static string CustomerCode = string.Empty;
        internal static bool IsFirstPart = false;
        internal static bool IsToteFirstPart = false;
        internal static string OrginalPartNum = string.Empty;
        internal static string NewPartNum = string.Empty;
        internal static int ScreenNo = 0;
        internal static void LogMessage(string Message)
        {
            using (StreamWriter w = File.AppendText("log_Validation.txt"))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :{0}", Message);
                w.WriteLine("-------------------------------");
            }
        }

        internal static void SetMonitorForm(string strWindowTitle, int NumMonitor)
        {
            Rectangle monitor;
            // Find the target window handle.
            IntPtr hTargetWnd = NativeMethod.FindWindow((string)null, strWindowTitle);
            if (hTargetWnd == IntPtr.Zero)
            {
                return;
            }

            if (NumMonitor >= 1)
            {
                if (Screen.AllScreens.Length >= NumMonitor)
                {
                    NumMonitor--;
                    //Get the data of the monitor
                    monitor = Screen.AllScreens[NumMonitor].WorkingArea;
                    //change the window to the second monitor
                    NativeMethod.SetWindowPos(hTargetWnd, 0,
                    monitor.Left, monitor.Top, monitor.Width,
                    monitor.Height, 0);
                }
            }
        }

        

    }

    #region Native API Signatures and Types
    /// <summary>
    /// The class exposes Windows APIs to be used in this code sample.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal class NativeMethod
    {
        /// <summary>
        /// The FindWindow function retrieves a handle to the top-level window 
        /// whose class name and window name match the specified strings. This 
        /// function does not search child windows. This function does not 
        /// perform a case-sensitive search.
        /// </summary>
        /// <param name="lpClassName">Class name</param>
        /// <param name="lpWindowName">Window caption</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Functions por set the position of a window
        /// </summary>
        [DllImport("user32")]
        public static extern long SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int y, int cx, int cy, int wFlagslong);

    }

    #endregion
}
