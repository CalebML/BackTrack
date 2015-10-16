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
        //function import (setting up "CreateFile")
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
                string fileName,
                [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
                [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                int flags,
                IntPtr template);

        //struct so it's built on the stack
        public struct time
        {
            public int hours;
            public int minutes;
            public int seconds;

            //constructor
            public time(int hour, int minute, int second)
                {
                    hours = hour;
                    minutes = minute;
                    seconds = second;
                }
        }

        public struct LocPoint
        {
            public int degreeNorthSouth;
            public float minuteNorthSouth;
            public int degreeEastWest;
            public float minuteEastWest;
            public char northSouth;
            public char eastWest;
            public int elevation;
            public time capTime;

            //constructor
            public LocPoint(int degreeNS, 
                            float minuteNS,
                            int degreeEW,
                            float minuteEW,
                            char NSIndicator,
                            char EWIndicator,
                            int initElevation,
                            time locTime)
            {
                degreeNorthSouth = degreeNS;
                minuteNorthSouth = minuteNS;
                degreeEastWest = degreeEW;
                minuteEastWest = minuteEW;
                northSouth = NSIndicator;
                eastWest = EWIndicator;
                elevation = initElevation;
                capTime = locTime;
            }
        }

        public BackTrack()
        {
            InitializeComponent();
        }

        private void ReadData_Click(object sender, EventArgs e)
        {
            //DriveInfo[] colDrives = DriveInfo.GetDrives();

            byte[] sdAddr = new byte[32];
            int intSdAddr = 0;

            //Read the current address, This tells us how many data "sets" we need to read
            //we need the first 4 bytes (32-bits)
            //ReadDrive("E:\\", 32);                    //This must be ran as admin
            sdAddr = ReadDrive("\\\\.\\physicaldrive1", 32);     //this does not
            for (int i = 0; i < 4; i++)
            {
                intSdAddr = intSdAddr << 8;
                intSdAddr += sdAddr[i];
            }

            byte[] sdData = new byte[intSdAddr * 32];
            char[] CharSdData = new char[intSdAddr * 32];

            //read data from SD card
            int bytesToRead = ((intSdAddr * 32) + 32);
            sdData = ReadDrive("\\\\.\\physicaldrive1", bytesToRead);

            for (int i = bytesToRead; i>32; i--)
            {
                CharSdData[bytesToRead - i] = (char)sdData[i-1];
            }
            //CharSdData.Reverse();

            //create an array of LocPoints (Location Points)
            LocPoint[] hike = new LocPoint[intSdAddr];

            //handle each location point
            for(int i = 0; i < intSdAddr; i++)
            {
                hike[i].degreeNorthSouth = CharSdData[i*32];
            }
        }

        private void clearSDCard_Click(object sender, EventArgs e)
        {
            //set up array of 0 bytes to clear address section of SD card
            byte[] clearIndex = Enumerable.Repeat((byte)0x00, 32).ToArray();
                //new byte[32];
            //clearIndex[1] = 0x00;
            writeToDisk("\\\\.\\physicaldrive1", clearIndex);
        }

        //function to read 
        private static byte[] ReadDrive(string FileName, int sizeToRead)
        {

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
            //diskStreamToRead.Read(buf, 0, 512);

            for(int i = 0; i < sizeToRead; i++)
            {
                buf[i] = (byte)diskStreamToRead.ReadByte();
                //long temp = diskStreamToRead.Position;
                diskStreamToRead.Seek((long)511, SeekOrigin.Current);
            }

            try { diskStreamToRead.Close(); } catch { }
            try { drive.Close(); } catch { }
            return buf;
        }

        //function to write
        private void writeToDisk(string FileName, byte[] dataToWrite)
        {
            if (dataToWrite == null)
                throw new System.ArgumentException("dataToWrite parameter cannot be null!");

            SafeFileHandle drive = CreateFile(fileName: FileName,
                             fileAccess: FileAccess.Write,
                             fileShare: FileShare.Write | FileShare.Read | FileShare.Delete,
                             securityAttributes: IntPtr.Zero,
                             creationDisposition: FileMode.Open,
                             flags: 4, //with this also an enum can be used. (as described above as EFileAttributes)
                             template: IntPtr.Zero);

            FileStream diskStreamToWrite = new FileStream(drive, FileAccess.Write);

            //diskStreamToWrite.Write(dataToWrite, 0, dataToWrite.Length);
            for (int i = 0; i < (dataToWrite.Length * 512); i++)
            {
                diskStreamToWrite.WriteByte(dataToWrite[i% dataToWrite.Length]);
                //diskStreamToWrite.Seek((long)511, SeekOrigin.Current);    //cannot seek when writing
                                                                            //to correct for this we write the same data 512 times
            }

            try { diskStreamToWrite.Close(); } catch { }
            try { drive.Close(); } catch { }
        }
    }
}
