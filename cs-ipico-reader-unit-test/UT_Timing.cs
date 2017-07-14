using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IpicoReader.UT
{
    public class UT_Timing
    {
        [TestMethod]
        public void TestTiming()
        {
            Run("localhost");
        }

        public void Run(string host)
        {
            Console.WriteLine("Reader Time: {0}", ReaderUtil.GetReaderTime(host, ReaderType.Elite));
            Console.WriteLine("Time Difference: {0}", ReaderUtil.CompareClocks(host, ReaderType.Elite));

            //ReaderUtil.UpdateReaderTime(host, ReaderType.Elite);
        }
    }
}
