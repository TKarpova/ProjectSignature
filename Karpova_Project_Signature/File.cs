using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Signature_project_Karpova
{
    class File
    {
        private string path;
        private int blockSize;
        private long fileLength;
        private int totalBlocks;
        private int threadNumber;
        private int packSize;

        public File(string path)
        {
            this.path = path;
        }

        public int BlockSize
        {
            set { blockSize = value; }
        }

        public int ThreadNumber
        {
            get { return (this.threadNumber); }
        }

        public long FileLength
        {
            get { return (this.fileLength); }
        }

        public void CountFileLength()
        {
            FileStream file = new FileStream(this.path, FileMode.Open, FileAccess.Read);
            this.fileLength = file.Length;
            file.Close();           
        }

        public void CountTotalBlocks()
        {
            long totalBlocks;
            totalBlocks = this.fileLength / this.blockSize;
            if (this.fileLength % this.blockSize != 0)
            {
                totalBlocks++;
            }
            this.totalBlocks = (int)(totalBlocks);
        }

        private int CountAllocationSize(long blockNumber)
        {
            int size;
            if (this.totalBlocks - 1 == blockNumber)
                size = (int)(this.fileLength - (this.totalBlocks - 1) * this.blockSize);
            else
                size = this.blockSize;

            return size;
        }

        public void CountThreadNumber()
        {
            this.packSize = this.totalBlocks / Environment.ProcessorCount;
            if (this.packSize == 0) { this.threadNumber = this.totalBlocks; }
            else { this.threadNumber = Environment.ProcessorCount; }
        }

        public bool isNotEnoughVirtualMemory()
        {
            Process p = Process.GetCurrentProcess();
            int requiredMem;            

            if (this.threadNumber == 1) { requiredMem = (int)(this.fileLength); }
            else { requiredMem = this.blockSize; }          

            if (requiredMem > (int)(p.VirtualMemorySize64 / threadNumber)) return true;
            else return false;
        }

        public int[] CountPackRanges()
        {
            int[] packRange = new int[this.threadNumber * 2];

            for (int i = 0; i < this.threadNumber * 2; i = i + 2)
            {
                packRange[i] = i / 2 * this.packSize;
                packRange[i + 1] = packRange[i] + this.packSize;
            }

            int remainder = this.totalBlocks - this.threadNumber * this.packSize;

            if (remainder > 0)
            {
                int x = 0;
                int y = 1;

                for (int i = (this.threadNumber - remainder) * 2; i < this.threadNumber * 2; i = i + 2)
                {
                    packRange[i] = packRange[i] + x;
                    packRange[i + 1] = packRange[i + 1] + y;
                    x++;
                    y++;
                }
            }

            return packRange;
        }

        public List<byte[]> GetHashCode256(int firstBlock, int lastBlock)
        {           
            FileStream file = new FileStream(this.path, FileMode.Open, FileAccess.Read);
            byte[] byteData = new byte[this.blockSize];
            SHA256 my_sha256 = SHA256Managed.Create();
            List<byte[]> result = new List<byte[]>();
            int size;

            for (int i = firstBlock; i < lastBlock; i++)
            {
              size = CountAllocationSize(i);
              file.Position = i * (long)(this.blockSize);
              file.Read(byteData, 0, size);
              result.Add(my_sha256.ComputeHash(byteData, 0, size));
            }

           file.Close();
           return result;          
           
        }
    }
}

