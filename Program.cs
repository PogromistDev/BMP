using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Console = Colorful.Console;


namespace BMP
{
    class Program
    {
        static FileStream fs;

        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "create":  CreateBMP(uint.Parse(args[1]), uint.Parse(args[2]));break;
                case "create_mono": CreateBMPmono(uint.Parse(args[1]), uint.Parse(args[2]));break;
                case "read": ReadBMP();break;
            }
        }

        static void CreateBMP(uint imageWidth, uint imageHeight)
        {
            fs = new FileStream("1.bmp", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            //File header

            //BMP file signature
            bw.Write('B');
            bw.Write('M');
            //Size of file in bytes
            bw.Write((uint)0);
            //Reserved
            bw.Write((ushort)0);
            bw.Write((ushort)0);
            //Offset to the data
            bw.Write((uint)54);

            //Image header

            //Header size in bytes
            bw.Write((uint)40);
            //Image width and height
            bw.Write(imageWidth);
            bw.Write(imageHeight);
            //Planes
            bw.Write((ushort)1);
            //Image bit format
            bw.Write((ushort)24);

            bw.Write((uint)0);
            bw.Write((uint)0);

            bw.Write((uint)0);
            bw.Write((uint)0);

            bw.Write((uint)0);
            bw.Write((uint)0);

            //Data

            uint remainder = (imageWidth * 3) % 4;


            for (int height = 0; height < imageHeight; height++)
            {
                for (int width = 0; width < imageWidth; width++)
                {
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                    bw.Write((byte)255);
                }

                if (remainder > 0)
                {
                    uint padding = 4 - remainder;

                    bw.Write(new byte[padding]);
                }

            }

            //Close file

            bw.Close();
            fs.Close();
        }

        static void CreateBMPmono(uint imageWidth, uint imageHeight)
        {
            fs = new FileStream("1-monochrome.bmp", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            //File header

            //BMP file signature
            bw.Write('B');
            bw.Write('M');
            //Size of file in bytes
            bw.Write((uint)0);
            //Reserved
            bw.Write((ushort)0);
            bw.Write((ushort)0);
            //Offset to the data
            bw.Write((uint)62);

            //Image header

            //Header size in bytes
            bw.Write((uint)40);
            //Image width and height
            bw.Write(imageWidth);
            bw.Write(imageHeight);
            //Planes
            bw.Write((ushort)1);
            //Image bit format
            bw.Write((ushort)1);

            //Compression type
            bw.Write((uint)0);
            //SizeImage
            bw.Write((uint)0);
            //XPerMeter
            bw.Write((uint)0);
            //YPerMeter
            bw.Write((uint)0);
            //Color used
            bw.Write((uint)0);
            //Important colors number
            bw.Write((uint)0);

            //Palete

            bw.Write((uint)0x00000000);
            bw.Write((uint)0x00ffffff);

            //Data

            uint imgDivEight = (imageWidth / 8) + (uint)(imageWidth % 8 > 0 ? 1 : 0);

            uint remainder = (imgDivEight >= 4? imgDivEight % 4 : imgDivEight);


            for (int height = 0; height < imageHeight; height++)
            {
                for (int width = 0; width < imgDivEight; width++)
                {
                    bw.Write((byte)0xff);
                }

                if (remainder > 0)
                {
                    uint padding = 4 - remainder;

                    bw.Write(new byte[padding]);
                }

            }

            long fileSize = bw.BaseStream.Position;
            bw.BaseStream.Seek(2, SeekOrigin.Begin);
            bw.Write((uint)fileSize);

            //Close file

            bw.Close();
            fs.Close();
        }

        static void ReadBMP()
        {
            fs = new FileStream("1.bmp", FileMode.Open);

            uint width, height;

            BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

            //Read
            Console.WriteLine("File signature: {0}", new string(br.ReadChars(2)));
            Console.WriteLine("File size: {0}", br.ReadUInt32());
            Console.WriteLine("Reserved 0: {0}", br.ReadUInt16());
            Console.WriteLine("Reserved 1: {0}", br.ReadUInt16());
            Console.WriteLine("Data offset: {0}", br.ReadUInt32());

            Console.WriteLine("Header size: {0}", br.ReadUInt32());

            width = br.ReadUInt32();
            height = br.ReadUInt32();

            Console.WriteLine("Image width: {0}",  width);
            Console.WriteLine("Image height: {0}", height);
            Console.WriteLine("Planes: {0}", br.ReadUInt16());
            Console.WriteLine("Bits format: {0}", br.ReadUInt16());

            Console.WriteLine("Compression: {0}", br.ReadUInt32());
            Console.WriteLine("Image size: {0}", br.ReadUInt32());
            Console.WriteLine("Res per meter: {0}", br.ReadUInt32());
            Console.WriteLine("Res per meter: {0}", br.ReadUInt32());
            Console.WriteLine("Color used: {0}", br.ReadUInt32());
            Console.WriteLine("Important color: {0}", br.ReadUInt32());


            //Data reading

            uint remainder = (width * 3) % 4;
            int cursorTop = Console.CursorTop;

            for (int hgh = 0; hgh < height; hgh++)
            {
                for (int wdth = 0; wdth < width; wdth++)
                {
                    byte R, G, B;
                    B = br.ReadByte();
                    G = br.ReadByte();
                    R = br.ReadByte();

                    Color color = Color.FromArgb(R, G, B);
                    Console.SetCursorPosition(Console.CursorLeft, cursorTop + ((int)height - hgh));
                    Console.BackgroundColor = color;
                    Console.Write("  ", color);
                }

                Console.WriteLine();

                if (remainder > 0)
                {
                    uint padding = 4 - remainder;

                    br.ReadBytes((int)padding);
                }

            }

            //Close file


            Console.SetCursorPosition(0, cursorTop + (int)height);
            Console.ResetColor();

            br.Close();
            fs.Close();
        }
    }
}
