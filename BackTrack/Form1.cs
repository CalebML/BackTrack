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
using GMap;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.CacheProviders;
using GMap.NET.Internals;
using GMap.NET.ObjectModel;
using GMap.NET.Projections;
using System.Management;
using System.Management.Instrumentation;


namespace BackTrack
{
    
    public partial class BackTrack : Form
    {
        //data for the hike
        LocPoint[] hike;

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

            try
            {
                System.Net.IPHostEntry e =
                     System.Net.Dns.GetHostEntry("www.google.com");
            }
            catch
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection avaible, going to CacheOnly mode.",
                      "Warning", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
            }

            // config map
            //MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.MapProvider = GMapProviders.GoogleTerrainMap;
            MainMap.Position = new PointLatLng(45.604590, -122.807256);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 11;

            MainMap.OnMarkerClick += new MarkerClick(MarkerClick);
            //MainMap.OnMarkerEnter += new MarkerEnter(MainMap_OnMarkerEnter);
            //MainMap.OnMarkerLeave += new MarkerLeave(MainMap_OnMarkerLeave);

            /**************************************
            GMapOverlay test = new GMapOverlay();
            PointLatLng[] points = new PointLatLng[2];
            points[0].Lat = 45.604590;
            points[0].Lng = -122.807256;

            points[1].Lat = 45.604590;
            points[1].Lng = -122.807;

            test.Markers.Add(new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points[0], GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green));
            test.Markers.Add(new GMap.NET.WindowsForms.Markers.GMarkerGoogle(points[1], GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green));
            MainMap.Overlays.Add(test);


            //MapRoute route = new MapRoute(test.Markers.GetEnumerator(), "test");
            GMapRoute route = new GMapRoute(points, "test");
            test.Routes.Add(route);
            ****************************************/
        }

        private string GetSDDriveNum()
        {
            //find the SD card drive number
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            string path = "\\\\\\\\.\\\\physicaldrive1";
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                string DriveModel = wmi_HD["Model"].ToString();
                if ((DriveModel == "SDHC Card") | DriveModel == "SD Card")
                    path = wmi_HD.Path.ToString();
            }

            //Remove escape characters from device ID
            string[] splitPath;
            splitPath = path.Split('\"');
            string drive = splitPath[1].Remove(0, 2);
            drive = drive.Remove(3, 1);

            return drive;
        }

        private void ReadData_Click(object sender, EventArgs e)
        {
            //DriveInfo[] colDrives = DriveInfo.GetDrives();

            byte[] sdAddr = new byte[32];
            int intSdAddr = 0;

            string drive = GetSDDriveNum();

            //Read the current address, This tells us how many data "sets" we need to read
            //we need the first 4 bytes (32-bits)
            //ReadDrive("E:\\", 32);                    //This must be ran as admin
            //sdAddr = ReadDrive("\\\\.\\physicaldrive1", 32);     //this does not
            sdAddr = ReadDrive(drive, 32);
            int shift = 5;

            for (int i = 0; i < 4; i++)
            {
                sdAddr[i] = (byte)(sdAddr[i] >> shift);
                sdAddr[i] = (byte)((sdAddr[i + 1] << (8 - shift)) + sdAddr[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                intSdAddr = intSdAddr << 8;
                intSdAddr += sdAddr[3-i];
            }

            byte[] sdData = new byte[intSdAddr * 32];
            char[] CharSdData = new char[intSdAddr * 32];

            //read data from SD card
            int bytesToRead = ((intSdAddr * 32) + 32);
            //sdData = ReadDrive("E:\\", bytesToRead);
            sdData = ReadDrive(drive, bytesToRead);

            for (int i = bytesToRead; i>32; i--)
            {
                CharSdData[bytesToRead - i] = (char)sdData[i-1];
            }
            //CharSdData.Reverse();

            //create an array of LocPoints (Location Points)
            hike = new LocPoint[intSdAddr];


            //handle each location point
            for (int i = 0; i < intSdAddr; i++)
            {
                char[] buffer = new char[7];
                byte[] byteBuffer = new byte[7];
                //put latatude degrees into struct in array
                hike[i].degreeNorthSouth = (int)CharSdData[i * 32] - '0';
                hike[i].degreeNorthSouth = hike[i].degreeNorthSouth * 10;
                hike[i].degreeNorthSouth += (int)CharSdData[(i * 32) + 1] - '0';


                //pull latatude minute data
                Array.Copy(CharSdData, (i * 32) + 2, buffer, 0, 6);
                string latString = new string(buffer);
                hike[i].minuteNorthSouth = float.Parse(latString);

                //pull N/S indicator
                hike[i].northSouth = CharSdData[(i * 32) + 8];

                //pull longitude degrees
                hike[i].degreeEastWest = (int)CharSdData[(i * 32) + 9] - '0';
                hike[i].degreeEastWest = hike[i].degreeEastWest * 10;
                hike[i].degreeEastWest += (int)CharSdData[(i * 32) + 10] - '0';
                hike[i].degreeEastWest = hike[i].degreeEastWest * 10;
                hike[i].degreeEastWest += (int)CharSdData[(i * 32) + 11] - '0';

                //pull Longitude minute data
                Array.Copy(CharSdData, (i * 32) + 12, buffer, 0, 6);
                string lonString = new string(buffer);
                hike[i].minuteEastWest = float.Parse(lonString);

                //pull E/W indicator
                hike[i].eastWest = CharSdData[(i * 32) + 18];

                //pull elevation data if any
                Array.Copy(CharSdData, (i * 32) + 19, buffer, 0, 5);
                buffer[5] = '\0';
                if(buffer[0] == '?')        //if there is no elevation data
                {
                    hike[i].elevation = -1;     //put -1 for elevation
                }
                else                            //otherwise put the elevation
                {
                    hike[i].elevation = Convert.ToInt32(buffer);
                }

                //pull time
                hike[i].capTime.hours = (int)CharSdData[(i * 32) + 24] - '0';       //hours
                hike[i].capTime.hours = hike[i].capTime.hours * 10;
                hike[i].capTime.hours += (int)CharSdData[(i * 32) + 25] - '0';

                hike[i].capTime.minutes = (int)CharSdData[(i * 32) + 26] - '0';       //minutes
                hike[i].capTime.minutes = hike[i].capTime.minutes * 10;
                hike[i].capTime.minutes += (int)CharSdData[(i * 32) + 27] - '0';

                hike[i].capTime.seconds = (int)CharSdData[(i * 32) + 28] - '0';       //seconds
                hike[i].capTime.seconds = hike[i].capTime.seconds * 10;
                hike[i].capTime.seconds += (int)CharSdData[(i * 32) + 29] - '0';
            }

            AddPointsToMap();
            MainMap.Refresh();
        }

        private void AddPointsToMap()
        {
            if(Points == null)
            {
                Points = new GMapOverlay();
            }
            if(Route == null)
            {
                Route = new GMapOverlay();
            }

            //remove old points
            MainMap.Overlays.Remove(Points);

            //add route
            Route.Routes.Add(new GMapRoute("Route1"));
            GMapRoute r = new GMapRoute("hikeRoute");

            //keep track of where the map needs to show once the points are added
            double highLat = -100, lowLat = 100, highLon = -200, lowLon = 200;

            for (int i = 0; i < hike.Length; i++)
            {
                double lat = hike[i].degreeNorthSouth;
                lat += (hike[i].minuteNorthSouth / 60);
                if (hike[i].northSouth == 'S')
                {
                    lat = lat * -1;
                }

                double lon = hike[i].degreeEastWest;
                lon += (hike[i].minuteEastWest / 60);
                if (hike[i].eastWest == 'W')
                {
                    lon = lon * -1;
                }

                //modify high and low values
                if (lat > highLat)
                {
                    highLat = lat;
                }
                if(lat < lowLat)
                {
                    lowLat = lat;
                }

                if (lon > highLon)
                {
                    highLon = lon;
                }
                if (lon < lowLon)
                {
                    lowLon = lon;
                }

                PointLatLng pos = new PointLatLng(lat, lon);
                //Route.Routes.Add()
                r.Points.Add(pos);

                
                //add "tool tips" and create marker
                GMapMarker Marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(pos, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);
                //Marker.ToolTip = new GMapToolTip(Marker);
                Marker.ToolTipText = "Point Number: " + (hike.Length - i)
                                        + "\nTime: " + hike[i].capTime.hours + ":" + hike[i].capTime.minutes + ":" + hike[i].capTime.seconds + " UTC"
                                        + "\nTime Hiked: " //+ (hike[i].capTime.hours - hike[0].capTime.hours) + ":" + (hike[i].capTime.minutes - hike[0].capTime.minutes) + ":" + (hike[i].capTime.seconds - hike[0].capTime.seconds)
                                        + "\nLatitude: " + hike[i].degreeNorthSouth + "º" + hike[i].minuteNorthSouth + "'" + hike[i].northSouth
                                        + "\nLongitude: " + hike[i].degreeEastWest + "º" + hike[i].minuteEastWest + "'" + hike[i].eastWest;

                //Marker.IsHitTestVisible = false;      //makes marker unclickable and tool tips wont show up
                

                Points.Markers.Add(Marker);
            }
            Route.Routes.Add(r);
            MainMap.Overlays.Add(Route);

            MainMap.Overlays.Add(Points);

            //move the map
            MainMap.Position = new PointLatLng((highLat + lowLat)/2, (highLon + lowLon)/2);

        }

        private void clearSDCard_Click(object sender, EventArgs e)
        {
            //set up array of 0 bytes to clear address section of SD card
            byte[] clearIndex = new byte[1537];
            for(int i = 0; i<1537; i++)
            {
                clearIndex[i] = (byte)0x00;
            }
            clearIndex[0] = (byte)0x01;

            writeToDisk(GetSDDriveNum(), clearIndex);
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

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            if (MainMap.Zoom + 1 < MainMap.MaxZoom)
                MainMap.Zoom += 1;
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            if (MainMap.Zoom - 1 > MainMap.MinZoom)
                MainMap.Zoom -= 1;
        }

        private void MarkerClick(object sender, EventArgs e)
        {
            MainMap.BringToFront(); //for breakpoint
            //Points.Markers.
        }
    }
}
