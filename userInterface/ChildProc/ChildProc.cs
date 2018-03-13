/////////////////////////////////////////////////////////////////////////////////////
// ChildProc.cs - demonstrate creation of multiple .net processes                     //
//  Ver 2.0- for project #4                                                        //
// Jim Fawcett & Nitish Kumar, CSE681 - Software Modeling and Analysis, Fall 2017 //
///////////////////////////////////////////////////////////////////////////////////
/*
 * Child process is child builder which performs the main building of test libraries
 * it receives request from mother build to start building test libraries
 * 
 * Maintainance History
 * Ver 2.0 - 06 December 2017
 * it receives request from mother build to start building test libraries
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MessagePassingComm;
using System.Threading;
using System.IO;
using Utilities;


namespace ChildProc
{
    public class ChildProc
    {
        int count = 0;
        Comm comm;
        int port;
        private List<string> fileList = new List<string>();
        string dllName = "";
       
        

        public ChildProc(int port)
        {
            this.port = port;
            comm = new Comm("http://localhost", port);
            //  InitiateReceiver();
        }

        public void InitiateReceiver()
        {
            Thread t = new Thread(new ThreadStart(communication));
            t.Start();
        }

        public void communication()
        {
            Console.WriteLine("BuilderStarted");
            while (true)
            {
                CommMessage a = comm.getMessage();
                if (a.type == CommMessage.MessageType.request)
                {
                    a.show();
                    CommMessage comMsg = new CommMessage(CommMessage.MessageType.reply);
                    comMsg.from = "http://localhost:" + port + "/IMessagePassingComm";
                    comMsg.to = "http://localhost:8081/IPluggableComm";
                    comMsg.author = "Nitsh Kumar";
                    comm.postMessage(comMsg);




                }
                if (a.type == CommMessage.MessageType.test)
                {
                    string path = a.command;
                    string readText = File.ReadAllText(RepoEnvironment.fileStorage + "\\" + path);
                    TestRequest test = readText.FromXml<TestRequest>();
                    Console.WriteLine("test xml" + test.ToString());

                    

                }
                if (a.type == CommMessage.MessageType.reply)
                {
                    string path = "";
                    a.show();
                    try
                    {
                         path = File.ReadAllText(Path.Combine(RepoEnvironment.fileStorage, a.command));
                    }
                    catch(Exception E)
                    {
                        Console.WriteLine("Exception " + E.Message);

                    }
                    TestRequest tr = path.FromXml<TestRequest>();
                    fileList.Clear();

                    foreach (TestElement test in tr.tests)
                    {
                        fileList.Add(test.testDriver);
                        foreach (string testfile in test.testCodes)
                        {
                            Console.WriteLine(test);
                            fileList.Add(testfile);
                        }
                    }
                    
                    CommMessage comMsg = new CommMessage(CommMessage.MessageType.file);
                    comMsg.from = "http://localhost:" + port + "/IMessagePassingComm";
                    comMsg.to = "http://localhost:8081/IPluggableComm";
                    comMsg.author = "Nitsh Kumar";
                    comMsg.arguments.AddRange(fileList);
                    comm.postMessage(comMsg);
                    Console.WriteLine("Request to Repo");
                    connectRepo();
                }
                if(a.type == CommMessage.MessageType.file)
                {
                    Console.WriteLine("Files received from Repo\n");
                    a.show();
                   
                    try
                    {
                        

                        createdll(fileList);
                       
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception " + e.Message);
                    }

                }


            }



        }




        public void connectRepo()
        {
            CommMessage co = new CommMessage(CommMessage.MessageType.request);
            co.from = "http://localhost:" + port + "/IMessagePassingComm";
            co.to = "http://localhost:8006/IPluggableComm";
            co.author = "Nitsh Kumar";
            co.arguments = fileList;
            co.command = port.ToString();
            comm.postMessage(co);
            co.show();
        }



        public void createdll(List<String> files)
        {
            Process pr = new Process();
            pr.StartInfo.FileName = "cmd.exe";
            pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            count += 1;
                pr.StartInfo.Arguments = "/Ccsc /target:library /out:td"+count+".dll *.cs ";
                pr.StartInfo.WorkingDirectory = @"..//..//..//ChildProc//"+port;
                pr.StartInfo.RedirectStandardError = true;
                pr.StartInfo.RedirectStandardOutput = true;
                pr.StartInfo.UseShellExecute = false;
                pr.Start();
                pr.WaitForExit();
                string errors = pr.StandardError.ReadToEnd();
                string output = pr.StandardOutput.ReadToEnd();
            string log= "Build Successfull & td"+count+".dll created for child " +port+"/n"+output;
                Console.WriteLine("\nBuild log: "+log+"\n\n");
            dllName = "td" + count;
               Console.WriteLine("\n\nBuild log Created and sent to Repository\n\n");
           
            CommMessage comMsg = new CommMessage(CommMessage.MessageType.reply);
            comMsg.command = log;
            comMsg.from = "http://localhost:" + port + "/IMessagePassingComm";
            comMsg.to = "http://localhost:8006/IPluggableComm";
            comMsg.author = "Nitsh Kumar";
            comm.postMessage(comMsg);
            startTestHarness();
        }


        public void startTestHarness()
        {


            CommMessage comMsg = new CommMessage(CommMessage.MessageType.request);
            comMsg.command = port.ToString();
            comMsg.from = "http://localhost:" + port + "/IMessagePassingComm";
            comMsg.to = "http://localhost:8077/IPluggableComm";
            comMsg.author = "Nitsh Kumar";

            comm.postMessage(comMsg);
            
        }

        static void Main(string[] args)
        {
            ChildProc proc = new ChildProc(Int32.Parse(args[0]));
            proc.InitiateReceiver();
            Console.Title = "ChildProc";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            Console.Write("\n  Demo Child Process");
            Console.Write("\n ====================");

            if (args.Count() == 0)
            {
                Console.Write("\n  please enter integer value on command line");
                return;
            }
            else
            {
                Console.Write("\n  Hello from child #{0}\n\n", args[0]);
            }
            Console.Write("\n  Press key to exit");
            Console.ReadKey();
            Console.Write("\n  ");


        }
    }
    
}
