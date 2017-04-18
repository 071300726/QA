using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ted.SH;

namespace SH
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = System.IO.File.ReadAllLines("tokenDict.txt");

            var analyzer = new LexicalAnalyzer();
            analyzer.Init(lines);

            analyzer.Parse("var abc = 123; var c=3");
        }
    }
}
