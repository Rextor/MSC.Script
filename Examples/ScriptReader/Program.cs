using System;
using System.IO;
using MSC.Script;

namespace ScriptReader
{
    class Program
    {
        public static CommendLine cl = new CommendLine();
        static void Main(string[] args)
        {
            Console.WriteLine("MSC Scripting powered by msc and open sourced on github.\ngo to https://github.com/Rextor/MSC/ for clone the msc\n\n");

            cl.OnWorking = true;
            string[] Lines = File.ReadAllLines(args[0]);

            Console.WriteLine("Installing methods...");
            cl.InstallMethods(Lines);
            foreach (Method method in cl.Body.Methods)
            {
                Console.WriteLine("Method {0}", method.Type.ToString() + method.IndexPlay);
                foreach (Instruction line in method.Instructions)
                    Console.WriteLine("   Type:{0}  Value:{1}", line.Type.ToString(), line.Value);
            }

            Console.WriteLine("\nRunning...");
            Console.WriteLine("\n==================>MSC Scripting<==================\n\n");

            cl.OutPuter.OnMessageReceived += Logger_OnMessageReceived;
            cl.StartScript();
            
            Console.ReadKey();
        }

        private static void Logger_OnMessageReceived(object sender, MSC.MessageReceivedArge e)
        {
            Console.WriteLine(e.log.GetMessage());
        }
    }
}
