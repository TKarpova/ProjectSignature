using System;
using System.Collections.Generic;
using System.IO;

namespace Signature_project_Karpova
{

    class Program
   {
       static File GetFile()
        {            
            File myFile;
            while (true)
            {
                try
                {
                    Console.Write("Enter the path: ");
                    string path = Console.ReadLine();
                    myFile = new File(path);
                    myFile.CountFileLength();
                    Console.WriteLine("File length is {0} bytes", myFile.FileLength);
                    if (myFile.FileLength == 0) throw new ArgumentException();
                    break;                  
                }

                catch (ArgumentException) { }

                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found");                   
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("Directory not found");                   
                }
                catch (PathTooLongException)
                {
                    Console.WriteLine("Invalid input - path is too long");                   
                }
                catch(UnauthorizedAccessException)
                {
                    Console.WriteLine("Access is not permitted");
                }
            }

            return myFile;
        }

        static void GetFileMetrics(File myFile)
        {
            int blockSize;

            while (true)
            {
                try
                {
                    Console.Write("Enter one block size: ");
                    blockSize = Convert.ToInt32(Console.ReadLine());
                    if (blockSize <= 0) throw new FormatException();
                    myFile.BlockSize = blockSize;
                    myFile.CountTotalBlocks();
                    myFile.CountThreadNumber();                  
                    if (myFile.isNotEnoughVirtualMemory()) throw new OutOfMemoryException();
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input - positive integer number expected");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Invalid input - your block size is too big");
                }
                catch (OutOfMemoryException)
                {
                    Console.WriteLine("The block is too big, there is not enough virtual memory");                   
                }
            }           
        }


        static void Main()
        { 
            try
           { 
              File myFile = GetFile();
              GetFileMetrics(myFile);               

              int threadNumber = myFile.ThreadNumber;
              int[] packRange = myFile.CountPackRanges();

              List<MyThread> myThreads = new List<MyThread>();

              for (int i = 0; i < threadNumber * 2; i = i + 2)
              {
                MyThread thread = new MyThread(myFile, packRange[i], packRange[i + 1]);
                myThreads.Add(thread);
              }

              for (int i = 0; i < threadNumber; i++)
              {
                myThreads[i].StartMyThread();
              }

              for (int i = 0; i < threadNumber; i++)
              {
                myThreads[i].JoinMyThread();
              }

              for (int i = 0; i < threadNumber; i++)
              {
                myThreads[i].PrintMyThreadResult();
              }

                Console.ReadLine();
            }           
            catch (Exception e)
            {
              Console.WriteLine(e.Message + e.StackTrace);
              Console.ReadLine();
            } 
        }
    }
}

