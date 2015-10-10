using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace BackTrack
{
    
    public partial class BackTrack : Form
    {
        //function import
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
                string fileName,
                [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
                [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                int flags,
                IntPtr template);

        public BackTrack()
        {
            InitializeComponent();
        }

        private void ReadData_Click(object sender, EventArgs e)
        {
            /*
            hFile = CreateFile("\\\\.\\physicaldrive1",
                    GENERIC_READ | FILE_SHARE_READ,
                    0,
                    OPEN_EXISTING,
                    0,
                    0);

            //FileStream fileStream = new FileStream("\\\\.\\physicaldrive1", FileMode.Open);


            int[] buffer = new int[5000];
            int i = 5000;
            while(i>0)
            {
                i--;
                buffer[i-1] = (int)fileStream.ReadByte();
            }
            i = 50;
            */
            int sizeToRead = 512;
            string FileName = "\\\\.\\physicaldrive1";

            if ((sizeToRead < 1) || (sizeToRead == null))
                throw new System.ArgumentException("Size parameter cannot be null or 0 or less than 0!");

            SafeFileHandle drive = CreateFile(fileName: FileName,
                         fileAccess: FileAccess.Read,
                         fileShare: FileShare.Write | FileShare.Read | FileShare.Delete,
                         securityAttributes: IntPtr.Zero,
                         creationDisposition: FileMode.Open,
                         flags: 4, //with this also an enum can be used. (as described above as EFileAttributes)
                         template: IntPtr.Zero);

            if (drive.IsInvalid)
            {
                throw new IOException("Unable to access drive. Win32 Error Code " + Marshal.GetLastWin32Error());
                //if get windows error code 5 this means access denied. You must try to run the program as admin privileges.
            }

            FileStream diskStreamToRead = new FileStream(drive, FileAccess.Read);
            byte[] buf = new byte[512];
            diskStreamToRead.Read(buf, 0, 512);
            try { diskStreamToRead.Close(); } catch { }
            try { drive.Close(); } catch { }

        }
    }
}
