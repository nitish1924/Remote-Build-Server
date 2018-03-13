///////////////////////////////////////////////////////////////////////
// Repo.cs - Ver 1-Repository for Project 4                          //
//    -it is Repository which keeps all the checked in files  in its //
//       storage                                                     //
// -it receives request from child builder to send the required files//
// -it also receives test logs and build logs                        //
//                                                                   //
// Nitish Kumar, CSE681 - Software Modeling and Analysis, Fall 2017  //
///////////////////////////////////////////////////////////////////////
/*
 * it is Repository which keeps all the checked in files  in its storage
 * it receives request from child builder to send the required files
 * it also receives test logs and build logs 
 * 
 * Maintainance History
 * Ver 1- 12/06/2017
 */


using MessagePassingComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MockRepo
{
    class Repo
    {
        
        Comm c;
        private List<string> fileList = new List<string>();
        public Repo()
        {
            c = new Comm("http://localhost", 8006);
            InitiateRec();
        }

        private void InitiateRec()
        {
            Thread t = new Thread(new ThreadStart(communication));
            t.Start();
        }
        public void communication()
        {

            
            Console.WriteLine("Receiver started on Repo");
           
            string port = "";
            while (true)
            {
                CommMessage a = c.getMessage();
                //if(a.type == CommMessage.MessageType.connect)
                //{
                //    Console.WriteLine("Connected to child");
                //}
                if (a.type == CommMessage.MessageType.request)
                {
                   fileList = a.arguments;
                    port = a.command;
                    Console.WriteLine("File Transfer Request received from child :" + port+"\n");
                    a.show();
                    string targetpath = @"..//..//..//ChildProc//" + port;
                    string sourcepath = @"..//..//..//MockRepo//RepoStorage";
                    string[] files = System.IO.Directory.GetFiles(sourcepath, "*.cs");
                    foreach (string s in files)
                    {

                        string fileName = System.IO.Path.GetFileName(s);
                        string destFile = System.IO.Path.Combine(targetpath, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                    Console.WriteLine("File successfully transferred to: "+targetpath+"\n");

                    CommMessage comMsg = new CommMessage(CommMessage.MessageType.file);
                    comMsg.from = "http://localhost:8006/IMessagePassingComm";
                    comMsg.to = "http://localhost:"+port+"/IPluggableComm";
                    comMsg.author = "Nitsh Kumar";
                   comMsg.arguments = fileList;
                    c.postMessage(comMsg);

                }

                if (a.type == CommMessage.MessageType.reply)
                {
                   
                    a.show();
                    try
                    {
                        string path = @"..//..//..//MockRepo//RepoStorage//BuildLog" + System.DateTime.Now.Millisecond + ".txt";
                        var myFile=File.Create(path);
                        myFile.Close();
                        StreamWriter SW = new StreamWriter(path);
                        SW.WriteLine(a.command);
                        SW.Close();
                        Console.WriteLine("\n\n===============Build log received @ : "+path+"\n\n");
                        Console.WriteLine("Build Log : " + a.command + "\n\n");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                if (a.type == CommMessage.MessageType.file)
                {
                    a.show();
                    string path = @"..//..//..//MockRepo//RepoStorage//TestLog" + System.DateTime.Now.Millisecond + ".txt";
                    var myFile = File.Create(path);
                    myFile.Close();
                    StreamWriter SW = new StreamWriter(path);
                    SW.WriteLine(a.command);
                    SW.Close();
                    Console.WriteLine("\n\n===============Test log received @ : " + path + "\n\n");
                    Console.WriteLine("Test Log : " + a.command + "\n\n");
                }
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Repository";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            Repo r = new Repo();
        }
    }
}
