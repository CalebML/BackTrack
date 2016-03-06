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

        //custom cursor
        Cursor RedPoint;

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

        //custom cursor stuff
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);


        const int IMAGE_CURSOR = 2;
        const uint LR_LOADFROMFILE = 0x00000010;
        //IntPtr ipImage = 

        //Cursor.Current = testCursor;

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
            
            //override the ToString method to output data
            public override string ToString()
            {

                return hours + ","
                    + minutes + ","
                    + seconds; ;
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
            public double distancedHiked;

            //constructor
            public LocPoint(int degreeNS, 
                            float minuteNS,
                            int degreeEW,
                            float minuteEW,
                            char NSIndicator,
                            char EWIndicator,
                            int initElevation,
                            time locTime,
                            double InitDistancedHiked)
            {
                degreeNorthSouth = degreeNS;
                minuteNorthSouth = minuteNS;
                degreeEastWest = degreeEW;
                minuteEastWest = minuteEW;
                northSouth = NSIndicator;
                eastWest = EWIndicator;
                elevation = initElevation;
                capTime = locTime;
                distancedHiked = InitDistancedHiked;
            }

            //override the ToString method to output data
            public override string ToString()
            {

                return degreeNorthSouth + ","
                    + minuteNorthSouth + ","
                    + northSouth + ","
                    + degreeEastWest + ","
                    + minuteEastWest + ","
                    + eastWest + ","
                    + elevation + ","
                    + capTime + ","
                    + distancedHiked;
                    //base.ToString();
            }
        }

        public BackTrack()
        {
            InitializeComponent();

            try
            {
                //try to access internet
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


            //set up cursor
            RedPoint = new Cursor(LoadImage(IntPtr.Zero,
                                                @"Red_Point.cur",
                                                IMAGE_CURSOR,
                                                0,
                                                0,
                                                LR_LOADFROMFILE));
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
            //allow the user to save hikes
            SaveHike.Enabled = true;

            //DriveInfo[] colDrives = DriveInfo.GetDrives();

            byte[] sdAddr = new byte[32];
            int intSdAddr = 0;

            string drive = GetSDDriveNum();

            //Read the current address, This tells us how many data "sets" we need to read
            //we need the first 4 bytes (32-bits)
            //ReadDrive("E:\\", 32);                    //This must be ran as admin
            //sdAddr = ReadDrive("\\\\.\\physicaldrive1", 32);     //this does not
                //this workaround no longer works
            
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
            //remove old points
            //MainMap.Overlays.Remove(Points);
            ClearMap();

            if (Points == null)
            {
                Points = new GMapOverlay();
            }
            if(Route == null)
            {
                Route = new GMapOverlay();
            }            

            //add route
            Route.Routes.Add(new GMapRoute("Route1"));
            GMapRoute r = new GMapRoute("hikeRoute");

            //keep track of where the map needs to show once the points are added
            double highLat = -100, lowLat = 100, highLon = -200, lowLon = 200;
            //double lastLat = 0, lastLon = 0;

            calcDistanceforPoints();

            for (int i = hike.Length-1; i >= 0; i--)
            {
                double lat = convertGpsToDouble(hike[i].degreeNorthSouth, hike[i].minuteNorthSouth, hike[i].northSouth);
                double lon = convertGpsToDouble(hike[i].degreeEastWest, hike[i].minuteEastWest, hike[i].eastWest);

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

                //figure out time hiked
                time hikeTime = calcHikeTime(hike[hike.Length - 1], hike[i]);

                //add tool tip
                Marker.ToolTipText = "Point Number: " + (hike.Length - i)
                                        + "\nTime: " + hike[i].capTime.hours + ":" + hike[i].capTime.minutes + ":" + hike[i].capTime.seconds + " UTC"
                                        + "\nTime Hiked: " + hikeTime.hours + ":" + hikeTime.minutes + ":" + hikeTime.seconds
                                        //+ (hike[i].capTime.hours - hike[0].capTime.hours) + ":" + (hike[i].capTime.minutes - hike[0].capTime.minutes) + ":" + (hike[i].capTime.seconds - hike[0].capTime.seconds)
                                        + "\nDistance Hiked: " + hike[i].distancedHiked.ToString("F") + " Km"
                                        + "\nLatitude: " + hike[i].degreeNorthSouth + "º " + hike[i].minuteNorthSouth + "' " + hike[i].northSouth
                                        + "\nLongitude: " + hike[i].degreeEastWest + "º " + hike[i].minuteEastWest + "' " + hike[i].eastWest;

                
                //Marker.IsHitTestVisible = false;      //makes marker unclickable and tool tips wont show up
                Marker.Tag = (hike.Length - 1) - i;         //add a tag to identify the marker for later

                Points.Markers.Add(Marker);
            }
            Route.Routes.Add(r);
            MainMap.Overlays.Add(Route);

            MainMap.Overlays.Add(Points);

            //move the map
            MainMap.Position = new PointLatLng((highLat + lowLat)/2, (highLon + lowLon)/2);

            populateComboBoxes();
        }

        private time calcHikeTime(LocPoint start, LocPoint fin)
        {
            time hikeTime;
            hikeTime.hours = 0;
            hikeTime.minutes = 0;
            hikeTime.seconds = 0;

            //figure out time hiked
            hikeTime.seconds = (fin.capTime.seconds - start.capTime.seconds);
            hikeTime.minutes = (fin.capTime.minutes - start.capTime.minutes);
            hikeTime.hours = (fin.capTime.hours - start.capTime.hours);
            if (hikeTime.seconds < 0)
            {
                hikeTime.seconds = hikeTime.seconds + 60;
                hikeTime.minutes--;
            }
            if (hikeTime.minutes < 0)
            {
                hikeTime.minutes = hikeTime.minutes + 60;
                hikeTime.hours--;
            }

            return hikeTime;
        }

        private double convertGpsToDouble(int degrees, float minutes, char indicator)
        {
            double latLon = degrees;
            latLon += (minutes / 60);
            if ( (indicator == 'S') | (indicator == 'W') )
            {
                latLon = latLon * -1;
            }

            return latLon;
        }

        private void populateComboBoxes()
        {
            //clear items in startand end point
            StartPoint.Items.Clear();
            EndPoint.Items.Clear();

            if (hike != null)
            {
                int i = 0;
                while (i < hike.Length)
                {
                    string value = "Point " + Convert.ToString(i + 1);
                    StartPoint.Items.Add(value);
                    EndPoint.Items.Add(value);
                    i++;
                }

                if (hike.Length > 0)
                {
                    StartPoint.SelectedIndex = 0;
                    EndPoint.SelectedIndex = hike.Length - 1;
                }
            }
        }

        /******************************************
        * Looks at all points in hike and calculates 
        *   the distance that has been hiked so far
        *   at each point
        *
        *******************************************/
        private void calcDistanceforPoints()
        {
            double lastLat = 0, lastLon = 0;
            double lat = 0, lon = 0;

            for (int i = hike.Length - 1; i >= 0; i--)
            {
                lat = convertGpsToDouble(hike[i].degreeNorthSouth, hike[i].minuteNorthSouth, hike[i].northSouth);
                lon = convertGpsToDouble(hike[i].degreeEastWest, hike[i].minuteEastWest, hike[i].eastWest);

                if (i != hike.Length - 1)
                {
                    lastLat = lat - lastLat;
                    if (lastLat < 0)
                    {
                        lastLat *= -1;
                    }
                    lastLon = lon - lastLon;
                    if (lastLon < 0)
                    {
                        lastLon *= -1;
                    }
                    //lastLat and lastLon hold the x and y distance traveled

                    lastLat *= lastLat;
                    lastLon *= lastLon;

                    hike[i].distancedHiked = Math.Sqrt(lastLat + lastLon);

                    //convert to kelometers
                    hike[i].distancedHiked *= (10000 / 90);

                    //add distance hoked before this point
                    hike[i].distancedHiked += hike[i + 1].distancedHiked;
                }
                else
                {
                    hike[i].distancedHiked = 0;
                }
                lastLat = lat;
                lastLon = lon;
            }
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

        //lastSelected is a global, it keeps track of the last markr clicked on
        //-1 repersents none selected
        static int lastSelected = -1;


        private void MarkerClick(object sender, EventArgs e)
        {

            GMapMarker Marker = (GMapMarker)sender;

            if(lastSelected != -1)
            {
                //create new marker and find old selected marker
                GMapMarker newLastMarker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(Points.Markers.ElementAt(lastSelected).Position, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.green);
                GMapMarker lastMarker = Points.Markers.ElementAt(lastSelected);

                //copy data
                newLastMarker.ToolTipText = lastMarker.ToolTipText;
                newLastMarker.Tag = lastMarker.Tag;

                //UN-highlight tooltip text
                newLastMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                //insert new marker and remove the old one
                Points.Markers.Insert((int)lastMarker.Tag, newLastMarker);
                Points.Markers.Remove(lastMarker);
            }

            if ((int)Marker.Tag != lastSelected)
            {
                //use Marker.Tag
                GMapMarker newMarker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(Marker.Position, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_dot);
                //Points.Markers.ElementAt((int)Marker.Tag);
                newMarker.ToolTipText = Marker.ToolTipText;
                newMarker.Tag = Marker.Tag;

                //highlight tooltip text
                newMarker.ToolTipMode = MarkerTooltipMode.Always;

                //replace marker
                Points.Markers.Insert((int)Marker.Tag, newMarker);
                Points.Markers.Remove(Marker);

                lastSelected = (int)newMarker.Tag;

                //enable the remove point button
                RemovePoint.Enabled = true;
            }
            else
            {
                RemovePoint.Enabled = false;
                lastSelected = -1;
            }

        }

        private void SaveHike_Click(object sender, EventArgs e)
        {
            if (saveHikeDialog.ShowDialog() == DialogResult.OK)
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveHikeDialog.OpenFile();

                for(int i = 0; i < hike.Length; i++)
                {
                    string saveVal = hike[i].ToString();
                    fs.Write(Encoding.ASCII.GetBytes(saveVal), 0, saveVal.Length);
                    fs.Write(Encoding.ASCII.GetBytes("\n"), 0, 1);
                }

                //var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                //binaryFormatter.Serialize(fs, hike);
            }
            else
            {
                //not a valid file
            }
        }

        private void LoadHike_Click(object sender, EventArgs e)
        {
            if(openHikeDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(openHikeDialog.FileName);
                    //(System.IO.FileStream)saveHikeDialog.OpenFile());

                    //clear the current hike
                    ClearMap();
                    int count = 0;
                    while (sr.ReadLine() != null)
                        count++;
                    hike = new LocPoint[count];
                    MainMap.Refresh();

                    //load points into hike variable
                    string line;
                    string[] lineParts = new string[11];
                    sr.DiscardBufferedData();
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    count = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineParts = line.Split(',');
                        hike[count].degreeNorthSouth = Convert.ToInt32(lineParts[0]);
                        hike[count].minuteNorthSouth = Convert.ToSingle(lineParts[1]);
                        hike[count].northSouth = Convert.ToChar(lineParts[2]);
                        hike[count].degreeEastWest = Convert.ToInt32(lineParts[3]);
                        hike[count].minuteEastWest = Convert.ToSingle(lineParts[4]);
                        hike[count].eastWest = Convert.ToChar(lineParts[5]);
                        hike[count].elevation = Convert.ToInt32(lineParts[6]);
                        hike[count].capTime.hours = Convert.ToInt32(lineParts[7]);
                        hike[count].capTime.minutes = Convert.ToInt32(lineParts[8]);
                        hike[count].capTime.seconds = Convert.ToInt32(lineParts[9]);
                        hike[count].distancedHiked = Convert.ToDouble(lineParts[10]);

                        count++;
                    }

                    //put points on the map
                    AddPointsToMap();
                    MainMap.Refresh();

                    //allow the user to save hikes
                    SaveHike.Enabled = true;
                }
                catch
                {
                    //error opening file
                    MessageBox.Show("Error opening file", "Error");
                }
            }
            else
            {
                //not a valid file
            }
        }

        private void ClearMap()
        {
            if (Points != null)
            {
                MainMap.Overlays.Remove(Points);
                Points.Markers.Clear();
            }
            if (Route != null)
            {
                MainMap.Overlays.Remove(Route);
                Route.Routes.Clear();
            }

            //clear last selected
            lastSelected = -1;
        }

        private void StartPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            calcAvgSpeed();
        }

        private void EndPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            calcAvgSpeed();
        }

        private void calcAvgSpeed()
        {
            int endIndex = (hike.Length - 1) - EndPoint.SelectedIndex;
            int startIndex = (hike.Length - 1) - StartPoint.SelectedIndex;

            if ((EndPoint.SelectedIndex >= 0) & (StartPoint.SelectedIndex >= 0))
            {
                time hikeTime = calcHikeTime(hike[startIndex], hike[endIndex]);

                float hours = hikeTime.seconds;
                hours /= 60;
                hours += hikeTime.minutes;
                hours /= 60;
                hours += hikeTime.hours;

                double minutes = hikeTime.seconds;
                minutes /= 60;
                minutes += (hikeTime.hours * 60) + hikeTime.minutes;

                double dist = hike[endIndex].distancedHiked - hike[startIndex].distancedHiked;

                if (KmHr.Checked)
                {
                    double speed = dist / hours;
                    AvgSpeed.Text = speed.ToString("F") + " Km/hr";
                }
                else
                {
                    double speed = dist * 3280.8;
                    speed /= minutes;
                    AvgSpeed.Text = speed.ToString("F") + " Ft/min";
                }
            }
        }

        private void KmHr_CheckedChanged(object sender, EventArgs e)
        {
            calcAvgSpeed();
        }

        private void RemovePoint_Click(object sender, EventArgs e)
        {
            if(lastSelected != -1)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove Point " + Convert.ToString(lastSelected + 1),
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if(dialogResult == DialogResult.Yes)
                {
                    //remove selected point
                    bool pastPointBeingRemoved = false;
                    LocPoint[] newHike = new LocPoint[hike.Length - 1];
                    for (int i = 0; i < hike.Length; i++)
                    {
                        if ( (i != (hike.Length - 1) - lastSelected) & (pastPointBeingRemoved == false) )
                        {
                            newHike[i] = hike[i];
                        }
                        else if (i == (hike.Length - 1) - lastSelected)
                        {
                            pastPointBeingRemoved = true;
                        }
                        else
                        {
                            newHike[i - 1] = hike[i];
                        }
                    }
                    
                    //switch hike to newHike
                    hike = newHike;
                    ClearMap();
                    AddPointsToMap();
                    MainMap.Refresh();

                }
            }
        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            MainMap.Cursor = RedPoint;
            //MainMap.Cursor = Cursors.Hand;
            //MainMap.Cursor = new Cursor(path);

            MainMap.MouseClick += new MouseEventHandler(MainMap_MouseClick);

            //gray out all buttons here and change add to cancel?
            ToggleButtons();

        }

        //class level to save across multiple calls
        bool toggled = false;
        bool readSD = true,
            clearSD = false,
            readPC = true,
            writePC = false,
            removePoint = false,
            addPoint = true;

        private void ToggleButtons()
        {
            if (!toggled)
            {
                //save button states
                readSD = ReadData.Enabled;
                clearSD = clearSDCard.Enabled;
                readPC = LoadHike.Enabled;
                writePC = SaveHike.Enabled;
                removePoint = RemovePoint.Enabled;
                addPoint = AddPoint.Enabled;

                //disable all
                ReadData.Enabled = false;
                clearSDCard.Enabled = false;
                LoadHike.Enabled = false;
                SaveHike.Enabled = false;
                RemovePoint.Enabled = false;
                AddPoint.Enabled = false;

                toggled = true;
            }
            else
            {
                ReadData.Enabled = readSD;
                clearSDCard.Enabled = clearSD;
                LoadHike.Enabled = readPC;
                SaveHike.Enabled = writePC;
                RemovePoint.Enabled = removePoint;
                AddPoint.Enabled = addPoint;

                toggled = false;
            }
        }

        private void MainMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //change curser back
                MainMap.Cursor = Cursors.Default;

                //get click location
                double lat = MainMap.FromLocalToLatLng(e.X, e.Y).Lat;
                double lng = MainMap.FromLocalToLatLng(e.X, e.Y).Lng;

                //delete MouseEventHandler
                //MouseEventHandler.Remove(MainMap_MouseClick, MainMap_MouseClick);
                MainMap.MouseClick -= new MouseEventHandler(MainMap_MouseClick);

                //add point to map
                LocPoint[] newHike;
                if(hike != null)
                    newHike = new LocPoint[hike.Length + 1];
                else
                    newHike = new LocPoint[1];

                int posToAdd = 0;
                if (lastSelected == -1)     //if nothing is selected
                {
                   /* if (hike != null)
                        posToAdd = hike.Length;
                    else*/
                        posToAdd = 0;
                }
                else
                {
                    posToAdd = (hike.Length - 1) - lastSelected;
                }

                bool pastAddedPoint = false;
                for (int i = 0; i < newHike.Length; i++)
                {
                    if ( ((newHike.Length - 1) - i != posToAdd) && (!pastAddedPoint) )
                        newHike[(newHike.Length - 1) - i] = hike[(hike.Length - 1) - i];
                    else
                    {
                        if (!pastAddedPoint)
                        {
                            //setup position in newHike
                            int pos = (newHike.Length - 1) - i;

                            //put location in newHike
                            newHike[pos].degreeNorthSouth = (int)lat;
                            newHike[pos].minuteNorthSouth = (float)((lat - newHike[pos].degreeNorthSouth) * 60);
                            newHike[pos].degreeEastWest = (int)lng;
                            newHike[pos].minuteEastWest = (float)((lng - newHike[pos].degreeEastWest) * 60);
                            newHike[pos].elevation = -1;

                            //fix negitives if they exist
                            if (newHike[pos].degreeNorthSouth > 0)
                                newHike[pos].northSouth = 'N';
                            else
                            {
                                newHike[pos].northSouth = 'S';
                                newHike[pos].degreeNorthSouth *= -1;
                                newHike[pos].minuteNorthSouth *= -1;
                            }

                            if (newHike[pos].degreeEastWest > 0)
                                newHike[pos].eastWest = 'E';
                            else
                            {
                                newHike[pos].eastWest = 'W';
                                newHike[pos].degreeEastWest *= -1;
                                newHike[pos].minuteEastWest *= -1;
                            }

                            //set time equal to point before if possable
                            if(hike != null)
                            {
                                //if there is a point before this one
                                if(pos != newHike.Length)
                                {
                                    newHike[pos].capTime.hours = newHike[pos + 1].capTime.hours;
                                    newHike[pos].capTime.minutes = newHike[pos + 1].capTime.minutes;
                                    newHike[pos].capTime.seconds = newHike[pos + 1].capTime.seconds;
                                }
                            }

                            //mark pastAddedPoint
                            pastAddedPoint = true;
                        }
                        else
                        {
                            newHike[(newHike.Length - 1) - i] = hike[hike.Length - i];
                        }
                    }
                }

                //switch hike to newHike
                hike = newHike;
                ClearMap();
                AddPointsToMap();
                MainMap.Refresh();

                //toggle buttons back
                ToggleButtons();
            }
        }
    }
}
