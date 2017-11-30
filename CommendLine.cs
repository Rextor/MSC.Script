using MSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSC.Script
{
    public class CommendLine
    {
        public Body Body = new Body();
        public Logger OutPuter = new Logger();
        public bool OnWorking = false;

        public void InstallMethods(string[] Lines)
        {
            try {
                Body.Initialize(Lines);
            }
            catch(Exception ex) { OutPuter.AddMessage("ERROR: " + ex.Message, Log.Type.Error); }
        }
        public void StartScript()
        {
            try {
                MethodParser mp = new MethodParser();
                mp.ParseMethods(Body.Methods, this);
            }
            catch(Exception ex) { OutPuter.AddMessage("ERROR: " + ex.Message, Log.Type.Error); }
        }
    }
}
