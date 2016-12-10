using System;
using System.Collections.Generic;
using System.Threading;

namespace Signature_project_Karpova
{
    class MyThread
    {
        private Thread Thrd;
        private File File;    
        private int firstBlock;
        private int lastBlock;
        private List<byte[]> myThreadResult = new List<byte[]>();

        public MyThread(File File, int firstBlock, int lastBlock)
        {
            this.File = File; 
            this.firstBlock = firstBlock;
            this.lastBlock = lastBlock;
        }

        public void StartMyThread()
        {
            Thrd = new Thread(this.Run);           
            Thrd.Start();
        }

        private void Run()
        {         
           this.myThreadResult = this.File.GetHashCode256(this.firstBlock, this.lastBlock);
        }

        public void JoinMyThread()
        {
            this.Thrd.Join();
        }


        public void PrintMyThreadResult()
        {           
            for (int i = 0; i < this.lastBlock - this.firstBlock; i++)
            {
                Console.Write("{0} -- ", i + this.firstBlock + 1);

                byte[] x = this.myThreadResult[i];

                for (int j = 0; j < x.Length; j++)
                {
                    Console.Write("{0:X2}", x[j]);
                }

                Console.WriteLine();
            }

           

        }
    }
}
