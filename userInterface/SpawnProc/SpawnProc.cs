/////////////////////////////////////////////////////////////////////////////////////
// SpawnProc - demonstrate creation of multiple .net processes                     //
//Version 1.0                                                                      //
// Project # 4                                                                                //
// Jim Fawcett & Nitish Kumar, CSE681 - Software Modeling and Analysis, Fall 2017  //
////////////////////////////////////////////////////////////////////////////////////
/*
 * Spawn Proc is used as a mother builder in project 4.
 * It creates the requested number of child by the client
 * It implements the process pool so that the child can conduct multiple builds in parallel.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MessagePassingComm;
using System.Threading;

namespace SpawnProc
{
  public class SpawnProc
  {
        Comm c;
        List<int> list = new List<int>();
        List<string> fileList = new List<string>();

        public SpawnProc()
        {
            c = new Comm("http://localhost", 8081);
            InitiateRec();
        }

        private void InitiateRec()
        {
            Thread t = new Thread(new ThreadStart(communication));
            t.Start();
        }

        public void communication()
        {
            


            Console.WriteLine("Receiver started on mother builder");
            while (true)
            {
                CommMessage a = c.getMessage();
                
                if (a.type == CommMessage.MessageType.kill)
                {
                    a.show();
                    killProcesses();
                    DeleteFolder();
                    Console.WriteLine("================All the child processes killed================");
                    


                }
                if (a.type == CommMessage.MessageType.request)
                {
                    
                    a.show();
                    string child = a.command;

                    Console.WriteLine("===============Communication from GUI to mother Process started &  " + child+ "  child processes created================");

                    createChild(child);


                    
                    

                }
                if (a.type == CommMessage.MessageType.reply)
                {
                    a.show();
                   

                }
                if (a.type == CommMessage.MessageType.test)
                {
                    a.show();
                    int port = list.First();
                    Console.WriteLine("" + port);
                    string testPath = a.command;
                    CommMessage comMsg = new CommMessage(CommMessage.MessageType.reply);
                    comMsg.from = "http://localhost:8081/IMessagePassingComm";
                    comMsg.to = "http://localhost:" + port + "/IPluggableComm";
                    comMsg.author = "Nitsh Kumar";
                    comMsg.command = testPath;
                    c.postMessage(comMsg);
                    
                    list.Remove(port);
                    list.Add(port);

                    
                }
                if (a.type == CommMessage.MessageType.file)
                {
                    a.show();
                    fileList.Clear();
                    fileList.AddRange(a.arguments);

                    Thread t = new Thread(new ThreadStart(sendFiles));
                    t.Start();
                }

            }
            
          


        }

        private void sendFiles()
        {

            foreach (string test in fileList)
            {
                c.postFile(test);
            }
        }

        private void createChild(string child)
        {
            for (int i = 0; i < Int32.Parse(child); i++)
            {
                createProcess(8082 + i + 1);
                list.Add(8082 + i + 1);
                int num = 8082 + i + 1;
                string childpath  = "../../../ChildProc/"+num;
                if (!Directory.Exists(childpath))
                    Directory.CreateDirectory(childpath);   

            }
        }

       
        static bool createProcess(int i)
    {
      Process proc = new Process();
      string fileName = "..\\..\\..\\ChildProc\\bin\\debug\\ChildProc.exe";
      string absFileSpec = Path.GetFullPath(fileName);

      Console.Write("\n  attempting to start {0}", absFileSpec);
      string commandline = i.ToString();
      try
      {
        Process.Start(fileName, commandline);
      }
      catch(Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
      return true;
    }

        private void killProcesses()
        {
            try
            {
                foreach (var process in Process.GetProcessesByName("ChildProc"))
                {
                    process.Kill();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }
        private void DeleteFolder()
        {
            
        }
  static void Main(string[] args)
    {
      Console.Title = "Mother Builder";
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.DarkBlue;

      Console.Write("\n  Demo Parent Process");
            

            Console.Write("\n =====================");

            if (args.Count() == 0)
            {
                Console.Write("\n  please enter number of processes to create on command line");
                return;
            }
            else
            {
                int count = Int32.Parse(args[0]);
                for (int i = 1; i <= count; ++i)
                {
                    if (createProcess(i))
                    {
                        Console.Write(" - succeeded");
                    }
                    else
                    {
                        Console.Write(" - failed");
                    }
                }
            }
            Console.Write("\n  Press key to exit");
            Console.ReadKey();
            Console.Write("\n  ");
        }
    }
}
