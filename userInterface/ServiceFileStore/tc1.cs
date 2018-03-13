///////////////////////////////////////////////////////////////////////////
// TestedLIb.cs - Simulates operation of a tested package                //
//                                                                       //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2017       //
///////////////////////////////////////////////////////////////////////////

using System;

namespace DllLoaderDemo
{
  public class Tested : ITested
  {
    public Tested()
    {
      Console.Write("\n    constructing instance of Tested");
    }
    public bool say()
    {
            int a=5, b=6, c=11;
            int sum=0;
            sum = a + b;
            if (sum == c)
                return true;
            else
                return false;


      Console.Write("\n    Production code - TestedLib");
      TestedLibDependency tld = new TestedLibDependency();
      tld.sayHi();
    }
  }
}
