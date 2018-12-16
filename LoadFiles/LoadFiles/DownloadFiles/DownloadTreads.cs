using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using LoadFiles.DownloadFiles;
using System.Threading;
using System.IO;

namespace LoadFiles
{
    internal class DownloadTreads : IDownloadFile
    {
        //HttpWebRequest wr;
        //HttpWebResponse ws;

        int fileSize;
        int treads;
        double partSize;
        int byteStart, byteEnd;
        string url, file;


        public DownloadTreads(int treads, string url, string resultFileName)
        {
            this.treads = treads;
            this.url = url;
            this.file = resultFileName;
        }

        void IDownloadFile.GetSize()
        {
            HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse ws = (HttpWebResponse)wr.GetResponse();

            fileSize = Convert.ToInt32(ws.ContentLength);
            partSize = Math.Floor((double)fileSize / treads);
        }

        void IDownloadFile.LoadFile()
        {
            List<FileStream> list = new List<FileStream>();

            for (int i = 0; i < partSize; i++)
            {

                Thread mainThread = new Thread(new ParameterizedThreadStart(TreadDownFile));
                mainThread.Start(i);

            }
        }


        private FileStream TreadDownFile(object i)
        {

            byteStart = (int)i * (int)Math.Ceiling(partSize);
            byteEnd = ((int)i + 1) * (int)Math.Ceiling(partSize);
            HttpWebRequest localRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            localRequest.AddRange(byteStart, byteEnd);
            HttpWebResponse response = (HttpWebResponse)localRequest.GetResponse();
            Stream localStream = response.GetResponseStream();


            int newByte = 262144;
            byte[] inBuf = new byte[newByte];
            int bytesRealTotal = 0;


            string name = Directory.GetCurrentDirectory() + "\\" + i + ".txt";
            FileStream fileStream = new FileStream(name, FileMode.Create, FileAccess.Write);

            while (true)
            {
                int n = localStream.Read(inBuf, 0, newByte);
                if (n <= 0) break;
                fileStream.Write(inBuf, 0, n);
            }

        }
    }
}
