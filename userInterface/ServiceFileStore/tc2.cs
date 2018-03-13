///////////////////////////////////////////////////////////////////////////
// Interfaces.cs - Interfaces for DLL Loader Demonstration               //
// ver 2 - changed test return to bool                                   //
//                                                                       //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2004       //
///////////////////////////////////////////////////////////////////////////

using System;

namespace DllLoaderDemo
{
  public interface ITest      // interface for test driver
  {
    void say();
    bool test();
  }

  public interface ITested    // interface for tested code
  {
    bool say();
  }
}
