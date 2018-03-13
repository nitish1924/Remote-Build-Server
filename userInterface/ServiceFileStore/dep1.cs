///////////////////////////////////////////////////////////////////////////
// TestedLibDependency.cs - library that tested depends on               //
//                                                                       //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2017       //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllLoaderDemo
{
  public class TestedLibDependency
  {
    public void sayHi()
    {
      Console.Write("\n    Hi from TestedLibDependency");
    }
  }
}
