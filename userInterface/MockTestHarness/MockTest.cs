///////////////////////////////////////////////////////////////////////////
// DllLoader.cs - Demonstrate Robust loading and dynamic invocation of   //
//                Dynamic Link Libraries found in specified location     //
// ver 2 - tests now return bool for pass or fail                        //
//  Project # 4                                                                     //
// Jim Fawcett,Nitish Kumar CSE681 - Software Modeling and Analysis, Fall 2017       //
///////////////////////////////////////////////////////////////////////////
/*
 * If user has entered args on command line then DllLoader assumes that the
 * first parameter is the path to a directory with testers to run.
 * 
 * Otherwise DllLoader checks if it is running from a debug directory.
 * 1.  If so, it assumes the testers directory is "../../Testers"
 * 2.  If not, it assumes the testers directory is "./testers"
 * 
 * If none of these are the case, then DllLoader emits an error message and
 * quits.
 */

using System;
using System.Reflection;
using System.IO;
using MessagePassingComm;
using System.Threading;

namespace DllLoaderDemo
{
  public  class DllLoaderExec
    {
        Comm c;
        string port = "";
        string st = "";
        public DllLoaderExec()
        {
            c = new Comm("http://localhost",8077);
            InitiateRec();
        }
       
        private void InitiateRec()
        {
            Thread t = new Thread(new ThreadStart(communication));
            t.Start();
        }
        public void communication()
        {
            Console.WriteLine("Receiver started on Test Harness");
            while (true)
            {
                CommMessage a = c.getMessage();
                if (a.type == CommMessage.MessageType.request)
                {
                    a.show();
                    port = a.command;
                    Console.WriteLine("Testing request received from :"+port+"\n");
                     testH();
                   Console.WriteLine("Testing completed");



                }
            }
        }
        public static string testersLocation { get; set; } = ".";

        /*----< library binding error event handler >------------------*/
        /*
         *  This function is an event handler for binding errors when
         *  loading libraries.  These occur when a loaded library has
         *  dependent libraries that are not located in the directory
         *  where the Executable is running.
         */
        static Assembly LoadFromComponentLibFolder(object sender, ResolveEventArgs args)
        {
            Console.Write("\n  called binding error event handler");
            string folderPath = testersLocation;
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
        //----< load assemblies from testersLocation and run their tests >-----

        string loadAndExerciseTesters()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromComponentLibFolder);

            try
            {
                

                // load each assembly found in testersLocation

                string[] files = Directory.GetFiles(testersLocation, "*.dll");
                foreach (string file in files)
                {
                    //Assembly asm = Assembly.LoadFrom(file);
                    Assembly asm = Assembly.LoadFile(file);
                    string fileName = Path.GetFileName(file);
                    Console.Write("\n  loaded {0}", fileName);

                    // exercise each tester found in assembly

                    Type[] types = asm.GetTypes();
                    foreach (Type t in types)
                    {
                        // if type supports ITest interface then run test

                        if (t.GetInterface("DllLoaderDemo.ITest", true) != null)
                            if (!runSimulatedTest(t, asm))
                                Console.Write("\n  test {0} failed to run", t.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Simulated Testing completed";
        }
        //
        //----< run tester t from assembly asm >-------------------------------

        bool runSimulatedTest(Type t, Assembly asm)
        {
            try
            {
                Console.Write(
                  "\n  attempting to create instance of {0}", t.ToString()
                  );
                object obj = asm.CreateInstance(t.ToString());

                // announce test

                MethodInfo method = t.GetMethod("say");
                if (method != null)
                    method.Invoke(obj, new object[0]);

                // run test

                bool status = false;
                method = t.GetMethod("test");
                if (method != null)
                    status = (bool)method.Invoke(obj, new object[0]);

                Func<bool, string> act = (bool pass) =>
                {
                    if (pass)
                        return "passed";
                    return "failed";
                };
                st = act(status);
                
                Console.Write("\n  test {0}", st);
            }
            catch (Exception ex)
            {
                Console.Write("\n  test failed with message \"{0}\"", ex.Message);
                return false;
            }

            ///////////////////////////////////////////////////////////////////
            //  You would think that the code below should work, but it fails
            //  with invalidcast exception, even though the types are correct.
            //
            //    DllLoaderDemo.ITest tester = (DllLoaderDemo.ITest)obj;
            //    tester.say();
            //    tester.test();
            //
            //  This is a design feature of the .Net loader.  If code is loaded 
            //  from two different sources, then it is considered incompatible
            //  and typecasts fail, even thought types are Liskov substitutable.
            //
            return true;
        }
        //
        //----< extract name of current directory without its parents ---------

        string GuessTestersParentDir()
        {
            string dir = Directory.GetCurrentDirectory();
            int pos = dir.LastIndexOf(Path.DirectorySeparatorChar);
            string name = dir.Remove(0, pos + 1).ToLower();
            if (name == "debug")
                return "../..";
            else
                return ".";
        }
        //----< run demonstration >--------------------------------------------

        [STAThread]
       static void Main(string[] args)
        {
            Console.Title = "Test Harness";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("\n  Demonstrating Robust Test Loader");
            Console.Write("\n ==================================\n");

            DllLoaderExec loader = new DllLoaderExec();

            
        }

       public void testH()
        {
            Console.Write("\n  Demonstrating Robust Test Loader");
            Console.Write("\n ==================================\n");

            

            
                DllLoaderExec.testersLocation = "..//..//..//ChildProc//"+port;

            // convert testers relative path to absolute path

            DllLoaderExec.testersLocation = Path.GetFullPath(DllLoaderExec.testersLocation);
            Console.Write("\n  Loading Test Modules from:\n    {0}\n", DllLoaderExec.testersLocation);

            // run load and tests

            string result = loadAndExerciseTesters();

            Console.Write("\n\n  {0}", result);
            Console.Write("\n\n");
            Console.WriteLine("\n\n=============Test log sent to repository===========\n\n");
            

            CommMessage comMs = new CommMessage(CommMessage.MessageType.file);
            comMs.from = "http://localhost:8077/IMessagePassingComm";
            comMs.to = "http://localhost:8006/IPluggableComm";
            comMs.command = "Test Modules from: "+ DllLoaderExec.testersLocation+" tesed & Test Result: "+result+"& test :"+st+"\n\n";
            comMs.author = "Nitsh Kumar";
            c.postMessage(comMs);
        }
    }
}
