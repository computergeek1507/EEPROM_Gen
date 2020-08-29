using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEPROM_Gen
{
    class EEPROMData
    {
        public string SerialNumber { get; set; }

        public string Name { get; set; }

        public double Version { get; set; }

        public string TARFilePath { get; set; }

        public string TARFileName
        {
            get { return "tmp/cape-info.tgz"; }
        }

        public string SigFilePath { get; set; }

        public void WriteFile(string binFile)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(binFile, FileMode.Create)))
            {
                Write(bw, "FPP02", 6);//start
                Write(bw, Name, 26);
                Write(bw, Version.ToString("N1"), 10);
                Write(bw, SerialNumber, 16);
                WriteGZFile(bw);
                // Write string   
                //binWriter.Write(authorName);
                // Write string   
                // Write integer  
                //binWriter.Write(age);
                //binWriter.Write(bookTitle);
                // Write boolean  
                //binWriter.Write(mvp);
                // Write double   
                //binWriter.Write(price);
                Write(bw, "0", 6);//footer
            }
        }

        void WriteGZFile(BinaryWriter w)
        {
            Write(w, "980", 4);
            long length = new System.IO.FileInfo(TARFilePath).Length;
            Write(w, length.ToString(), 6);
            Write(w, "2", 2);
            Write(w, TARFileName, 64);
            ReadWriteGZFile(w);
        }

        void ReadWriteGZFile(BinaryWriter w)
        {
            using (BinaryReader br = new BinaryReader(File.Open(TARFilePath, FileMode.Open)))
            {
                int length = (int)br.BaseStream.Length;
                while (br.BaseStream.Position != length)
                {
                    int bytesToRead = br.ReadInt32();
                    byte[] v = br.ReadBytes(bytesToRead);
                    w.Write(v);
                }
            }
        }

        void Write(BinaryWriter w, string value, int len)
        {
            byte[] LogoDataBy = ASCIIEncoding.ASCII.GetBytes(value);
            var newArray = new byte[len];

            var startAt = newArray.Length - LogoDataBy.Length;
            Array.Copy(LogoDataBy, 0, newArray, startAt, LogoDataBy.Length);
            w.Write(newArray, 0, len);
        }
    }
}
