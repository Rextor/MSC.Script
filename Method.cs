using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSC.Script
{
    public class Method
    {
        public List<Instruction> Instructions = new List<Instruction>();
        public MethodType Type { set; get; }
        public int IndexPous { set; get; }
        public int IndexPlay { set; get; }
    }
    public class Body
    {
        public List<Method> Methods = new List<Method>();
        public List<Method> Initialize(string[] Lines)
        {
            bool InAdding = false;
            int indexplay = 0;
            Method method = new Method();
            for (int i = 0; i < Lines.Length; i++)
            {  
                if (!InAdding)
                {
                    if (IsStartMethodLine(Lines[i]))
                    {
                        indexplay++;
                        method = new Method();
                        method.IndexPlay = indexplay;
                        Match mh = Regex.Match(Lines[i], @"==>(.*?)<==");
                        string valuenow = mh.Groups[1].Value.ToString();
                        string[] st = valuenow.Split('-');
                        method.Type = GetPous(st[0]);
                        if (st.Length == 2)
                        {
                            try
                            {
                                method.IndexPous = int.Parse(st[1]);
                            }
                            catch { throw new Exception("Couldn't get index pous value! ==>" + method.Type.ToString()); }
                        }
                        InAdding = true;
                    }
                }
                else
                {
                    if (!IsEndMethodLine(Lines[i]))
                    {
                        if (!IsCommendLine(Lines[i]))
                        {
                            if (!string.IsNullOrWhiteSpace(Lines[i]))
                            {
                                Instruction line = new Instruction();
                                method.Instructions.Add(Instruction.ReadLine(Lines[i]));
                            }
                        }
                    }
                    else { InAdding = false; Methods.Add(method); }
                }
            }
            if(InAdding == true)
                throw new Exception("A method haven't end line!");
            return Methods;
        }
        bool IsCommendLine(string Line)
        {
            if (Line.StartsWith(@"\*/"))
                return true;
            else return false;
        }
        bool IsEndMethodLine(string Line)
        {
            if (Line == "<==>")
                return true;
            else return false;
        }
        bool IsStartMethodLine(string Line)
        {
            if (string.IsNullOrWhiteSpace(Line))
                return false;
            if (Line.Substring(0, 3) == "==>" && Line.Substring(Line.Length - 3, 3) == "<==")
                return true;
            else return false;
        }
        MethodType GetPous(string Type)
        {
            var List = Enum.GetValues(typeof(MethodType)).Cast<MethodType>().ToList();
            MethodType ReturnDef = MethodType.Unkown;
            foreach (MethodType item in List)
                if (Type.ToLower() == item.ToString().ToLower())
                {
                    ReturnDef = item;
                    break;
                }
            if (ReturnDef != MethodType.Unkown)
                return ReturnDef;
            else
                throw new Exception("Unkown Pous! ==> " + Type);
        }
    }
    public class Instruction
    {
        public override string ToString()
        {
            return Type.ToString() + "=>" + Value;
        }
        public OpCode Type { set; get; }
        public string Value { set; get; }
        public static Instruction ReadLine(string Line)
        {
            Instruction line = new Instruction();
            int start = Line.IndexOf("=>");
            line.Type = GetModule(Line.Substring(0, start));
            line.Value = Line.Substring(start + 2, Line.Length - start - 2);
            return line;
        }
        static OpCode GetModule(string Type)
        {
            var List = Enum.GetValues(typeof(OpCode)).Cast<OpCode>().ToList();
            OpCode ReturnDef = OpCode.Unkown;
            foreach (OpCode item in List)
                if (Type.ToLower() == item.ToString().ToLower())
                {
                    ReturnDef = item;
                    break;
                }
            if (ReturnDef != OpCode.Unkown)
                return ReturnDef;
            else
                throw new Exception("Unkown Module! ==> " + Type);
        }
    }
}
