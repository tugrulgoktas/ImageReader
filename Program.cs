using ImageReader.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace ImageReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = Constants.Location.DesktopPath + @"\89364279_1591862430992841_5161106803391463424_o.jpg";
            ReaderFile(filePath);
        }

        public static void ReaderFile(string filename)
        {
            FileInfo info = new FileInfo(filename);
            long fileSize = info.Length;

            FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Image image = Image.FromStream(stream, false, false);

            try
            {
                PropertyItem item;
                string data;
                string version;
                string cameraBrand;
                string cameraModel;
                DateTime dateTaken;
                DateTime dateModified;

                PropertyItem[] props = image.PropertyItems;

                //display properties
                foreach (PropertyItem prop in props)
                {
                    string output = String.Format("{0} ({1}) => {2}",
                                        prop.Id, prop.Id.ToString("X"),
                                            Encoding.UTF8.GetString(prop.Value));
                    Console.WriteLine(output);
                }

                item = image.GetPropertyItem(0x0001);
                string latitudeRef = Encoding.UTF8.GetString(item.Value);

                item = image.GetPropertyItem(0x0002);
                string latitude = Encoding.UTF8.GetString(item.Value);

                float lat = ExifGpsToFloat(image.GetPropertyItem(0x0001), image.GetPropertyItem(0x0002));
                float longit = ExifGpsToFloat(image.GetPropertyItem(0x0003), image.GetPropertyItem(0x0004));
                string latlong = String.Format("{0},{1}", lat, longit);
                Console.WriteLine("GPS Information");
                Console.WriteLine(latlong);

                // Get version of Exif metadata.
                item = image.GetPropertyItem(0x9000);
                version = Encoding.UTF8.GetString(item.Value);

                // Camera brand.
                item = image.GetPropertyItem(0x010F);
                cameraBrand = Encoding.UTF8.GetString(item.Value, 0, item.Value.Length - 1);

                // Camera model.
                item = image.GetPropertyItem(0x0110);
                cameraModel = Encoding.UTF8.GetString(item.Value, 0, item.Value.Length - 1);

                // Date photo taken.
                item = image.GetPropertyItem(0x9003);  // Item 36867
                data = Encoding.UTF8.GetString(item.Value, 0, item.Value.Length - 1);
                dateTaken = DateTime.ParseExact(data, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);

                // Date photo modified.
                item = image.GetPropertyItem(0x9004);
                data = Encoding.UTF8.GetString(item.Value, 0, item.Value.Length - 1);
                dateModified = DateTime.ParseExact(data, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                // Error
            }

        }

        private static float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem)
        {
            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            float degrees = degreesNumerator / (float)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            float minutes = minutesNumerator / (float)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            float seconds = secondsNumerator / (float)secondsDenominator;

            float coorditate = degrees + (minutes / 60f) + (seconds / 3600f);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, or W
            if (gpsRef == "S" || gpsRef == "W")
                coorditate = 0 - coorditate;
            return coorditate;
        }

        //public static void ReaderFile(string filename)
        //{
        //    //load the image file bytes into memory
        //    Bitmap image = new Bitmap(filename);

        //    //request file properties from image object
        //    PropertyItem[] props = image.PropertyItems;

        //    //display properties
        //    foreach (PropertyItem prop in props)
        //    {
        //        Console.WriteLine(
        //            prop.Id.ToString());
        //    }
        //}

    }
}
