///////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - Ver 1-GUI for Project 4                       //
//     -its a user interface for project 4 where files are checked in,//
//       test request is creted and sent to repository                //
//      -it also has a text box which lets user request to mother     //
//          builder to create requested no of child                  //
// Nitish Kumar, CSE681 - Software Modeling and Analysis, Fall 2017  //
///////////////////////////////////////////////////////////////////////
/*
 * Graphical user interface for project 4
 *-its a user interface for project 4 where files are checked in,
 *    test request is creted and sent to repository                
 *  -it also has a text box which lets user request to mother
 *        builder to create requested no of child 
 *        
 *  Maintainance history
 *  Ver 1- 12/06/2017
 */

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using MessagePassingComm;
using SpawnProc;
using System.Threading;
using DllLoaderDemo;


namespace userInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
       
        int count = 0;
        static Comm c = new Comm("http://localhost", 8079);
        static SpawnProc.SpawnProc spawn = new SpawnProc.SpawnProc();
        
        List<string> stringList = new List<string>();

        public string testRequestName { get; private set; }
        public string num { get; private set; }
        public string selectedTest { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

           



            
            List<string> r= new List<string>();
            PopulateListBox(listbox1, r,"listbox1");
            PopulateListBox(listbox3, r, "listbox3auto");
        }

       
        

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            
        }

        private void PopulateListBox(ListBox lsb, List<string> s,string a)
        {
            if (a=="listbox1")
            {
                DirectoryInfo dinfo = new DirectoryInfo(RepoEnvironment.fileStorage);
                FileInfo[] Files = dinfo.GetFiles("*.cs*");
                foreach (FileInfo file in Files)
                {
                    if (!listbox1.Items.Contains(file.Name))
                    {
                        lsb.Items.Add(file.Name);
                    }
                }
            }
            else if(a=="listbox2")
            {
                foreach (string str in s)
                {
                    lsb.Items.Add(str);
                }
                
            }
            else if (a == "listbox3")
            {
                DirectoryInfo dinfo = new DirectoryInfo(RepoEnvironment.fileStorage);
                FileInfo[] Files = dinfo.GetFiles(testRequestName);
                foreach (FileInfo file in Files)
                {
                    if (!listbox3.Items.Contains(file.Name))
                    {
                        lsb.Items.Add(file.Name);
                    }

                    
                }
                

            }
            else if (a=="listbox3auto")
            {
                DirectoryInfo dinfo = new DirectoryInfo(RepoEnvironment.fileStorage);
                FileInfo[] Files = dinfo.GetFiles("*.xml*");
                foreach (FileInfo file in Files)
                {
                    if (!listbox3.Items.Contains(file.Name))
                    {
                        lsb.Items.Add(file.Name);
                    }


                }
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (string str in listbox1.SelectedItems)
            {
                stringList.Add(str);
            }
            PopulateListBox(listbox2, stringList,"listbox2");
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (listbox2.HasItems)
            {
                count++;
            }
            TestElement te1 = new TestElement();
            te1.testName = "test1";
            for (int i = 0; i < stringList.Count; i++)
            {
                if (stringList[i].ToString().StartsWith(value:"td"))
                {
                    te1.addDriver(stringList[i].ToString());
                }
                else
                {
                    te1.addCode(stringList[i].ToString());
                }
            }
            TestRequest tr = new TestRequest();
            tr.author = "Nitish Kumar";
            tr.tests.Add(te1);

            string trXml = tr.ToXml();

            TestRequest newRequest = trXml.FromXml<TestRequest>();
            string typeName = newRequest.GetType().Name;
            if (listbox2.HasItems)
            {
                testRequestName = "testrequest" + System.DateTime.Now.Millisecond + ".xml";
                File.WriteAllText(RepoEnvironment.fileStorage+"\\"+testRequestName, trXml);
            }
            
            listbox2.Items.Clear();
            PopulateListBox(listbox3, stringList, "listbox3");
            stringList.Clear();

        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = true;
            o.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (o.ShowDialog()== true)
            {
                foreach(string item in o.FileNames)
                {
                    listbox4.Items.Add(item);
                }
            }
        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {


            List<string> list = new List<string>();
            foreach (string st in listbox4.Items)
            {
                list.Add(st);
            }
            for (int i = 0; i < list.Count; i++)
            {
                string source = "";
                string targetPath = "";
                string fileName = "";
                source = list[i].ToString();
                source = @"" + source;
                fileName = Path.GetFileName(source);
                targetPath = RepoEnvironment.fileStorage+"/" + fileName; /*@"C:\Nitish\repo\" + fileName;*/
                Console.WriteLine("File uploaded to Repo: "+ fileName );
                File.Copy(source, targetPath, true);
            }

            List<string> p = new List<string>();
            PopulateListBox(listbox1, p, "listbox1");

        }

       
        private void text1_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {


            

            num = Convert.ToString(text1.Text);

            //Thread t = new Thread(new ThreadStart(sendMessageForStart));
            // t.Start();
            sendMessageForStart();

        }

        private void sendMessageForStart()
        {
            CommMessage comMsg = new CommMessage(CommMessage.MessageType.request);
            comMsg.command = num;
            comMsg.from = "http://localhost:8079/IMessagePassingComm";
            comMsg.to = "http://localhost:8081/IPluggableComm";
            comMsg.author = "Nitsh Kumar";

            c.postMessage(comMsg);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             
            sendTestFIle();
        }

        private void sendTestFIle()
        {
            Console.WriteLine("Test Request  "+ selectedTest + " sent to child");
            CommMessage comMsg = new CommMessage(CommMessage.MessageType.test);

            comMsg.from = "http://localhost:8079/IMessagePassingComm";
            comMsg.to = "http://localhost:8081/IPluggableComm";
            comMsg.author = "Nitsh Kumar";
            comMsg.command = selectedTest;

            c.postMessage(comMsg);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Inside method");
            selectedTest = (string)listbox3.SelectedItem;
            sendTestFIle();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CommMessage comMsg = new CommMessage(CommMessage.MessageType.kill);

            comMsg.from = "http://localhost:8079/IMessagePassingComm";
            comMsg.to = "http://localhost:8081/IPluggableComm";
            comMsg.author = "Nitsh Kumar";
            comMsg.command = "Killing processes";

            c.postMessage(comMsg);
        }
    }
}
