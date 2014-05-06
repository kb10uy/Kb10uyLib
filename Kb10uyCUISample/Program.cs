using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kb10uy.IO;
using Kb10uy.Scripting;
using Kb10uy.Extension;
using Kb10uy.MultiMedia;
using Kb10uy.Scripting.Text;
using Kb10uy.Audio.FM;

namespace Kb10uyCUISample
{
    class Program
    {
        static void Main(string[] args)
        {
            Config cfg = new Config();
            cfg["A.B"] = 12;
            cfg["A.C"] = "22";
            cfg["B.C.F"] = new ConfigValue[] 
            {
                1,2,3,4,5,
                "This","it",
                new ConfigValue[]{10,20,30}
                
            };

            cfg.SaveFile("test.cfg");

            Console.WriteLine(cfg["B.C.F"][7][2].NumberValue);

            var kr = new KastepsRuntime();
            kr.LoadScriptFromString(@"
begin {
    
}

every {

}

10 {

}
            ");

            Console.ReadLine();
            var ope = new FMOperator();
        }
    }
}
