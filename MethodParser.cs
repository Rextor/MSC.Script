using MSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSC.Script
{
    public class MethodParser
    {
        public Controller Controller = new Controller();
        public CommendLine CMD;
        public void ParseMethods(List<Method> list, CommendLine cmd)
        {
            CMD = cmd;
            foreach (Method method in list)
                ExecuteMethod(method);
        }
        public void ExecuteMethod(Method method)
        {
            switch (method.Type)
            {
                case MethodType.Config:
                    Controller.NewConfigDef();
                    ExecuteInstructions(method);
                    break;
                case MethodType.Request:
                    Controller.NewRequestDef();
                    ExecuteInstructions(method);
                    break;
                case MethodType.Print:
                case MethodType.Base:
                    ExecuteInstructions(method);
                    break;
            }
        }
        public void ExecuteInstructions(Method method)
        {
            foreach (Instruction inst in method.Instructions)
                ExecuteInstruction(inst, method.Type);
        }
        public void ExecuteInstruction(Instruction inst, MethodType type)
        {
            inst = ReplaceMemoryStringsOnValue(inst);
            switch (type)
            {
                case MethodType.Base:
                    ExecuteBaseInstruction(inst);
                    break;
                case MethodType.Request:
                    ExecuteRequestInstruction(inst);
                    break;
                case MethodType.Print:
                    ExecutePrintInstruction(inst);
                    break;
                case MethodType.Config:
                    ExecuteConfigInstruction(inst);
                    break;
            }
        }
        public void ExecuteRequestInstruction(Instruction line)
        {
            RequestDef RD = Controller.GetLastRequestDef();
            switch (line.Type)
            {
                case OpCode.RequestManage:

                    string[] Rs = line.Value.Split('.');

                    ConfigDef cd = Controller.GetConfigdef(int.Parse(Rs[0]));
                    RequestDef helper = null;
                    if (Rs.Length >= 3)
                    {
                        try
                        {
                            if (Rs[2] == "this")
                                helper = RD;
                            else
                                helper = Controller.GetRequestDef(int.Parse(Rs[2]));
                        }
                        catch { }
                    }

                    if (Rs[1].ToLower() == "getdata")
                        RD = GetData(cd, helper);
                    else if (Rs[1].ToLower() == "postdata")
                        RD = PostData(cd, helper);

                    break;
                case OpCode.MemoryString:
                    if (line.Value.ToLower().StartsWith("regex"))
                        Controller.AddMemodyString(ParseRegex(line.Value));
                    else if (line.Value.ToLower() == "sourcepage")
                        Controller.AddMemodyString(RD.GetSourcePage());
                    else if (line.Value.ToLower() == "cookies")
                        Controller.AddMemodyString(RD.GetCookies());
                    else
                        Controller.AddMemodyString(line.Value);
                    break;
                case OpCode.Ret:
                    string val = line.Value;
                    if (line.Value.ToLower().StartsWith("regex"))
                        val = ParseRegex(line.Value);
                    else if (line.Value.ToLower() == "sourcepage")
                        val = RD.GetSourcePage();
                    else if (line.Value.ToLower() == "cookies")
                        val = RD.GetCookies();
                    line.Value = val;
                    ParseRetModule(line);
                    break;
                case OpCode.SetConfig:
                    string[] Res = line.Value.Split(':');
                    ConfigDef cr = Controller.GetConfigdef(int.Parse(Res[0]));
                    int indexd = line.Value.IndexOf(':');
                    Instruction doc = Instruction.ReadLine(line.Value.Substring(indexd + 1, line.Value.Length - indexd - 1));
                    SetConfig(doc, cr);
                    break;
                default:
                    CMD.OutPuter.AddMessage("The" + line.Type.ToString() + " Module not support on Request method");
                    break;
            }
            Controller.SetLastRequestDef(RD);
        }
        private string ParseRegex(string value)
        {
            int startindex = value.IndexOf("{");
            string[] arr = value.Substring(0, startindex).Split('-');
            string indexmemroy = arr[1];
            string input = ParseMemoryString("MemoryString-" + indexmemroy);
            string pattern = value.Substring(value.IndexOf('{') + 1, value.LastIndexOf("}") - value.IndexOf('{') - indexmemroy.Length);
            int indexmatch = 0;
            if (arr.Length >= 3) indexmatch = int.Parse(arr[2]);
            int grpmatch = 1;
            if (arr.Length >= 4) int.Parse(arr[3]);
            MatchCollection mc = Regex.Matches(input, pattern);
            return mc[indexmatch].Groups[grpmatch].Value;
        }
        private RequestDef GetData(ConfigDef Cd, RequestDef helper)
        {
            RequestDef R = new RequestDef();
            R.configdef = Cd;
            R.Helper = helper;
            R.GetData();
            return R;
        }
        private RequestDef PostData(ConfigDef Cd, RequestDef helper)
        {
            RequestDef R = new RequestDef();
            R.configdef = Cd;
            R.Helper = helper;
            R.PostData();
            return R;
        }


        public string ParseMemoryString(string Script)
        {
            string[] strRet1 = Script.Split('-');
            if (strRet1.Length == 2)
            {
                if (strRet1[1].ToLower() == "all")
                {
                    string[] strings = Controller.GetAllMemoryString();
                    foreach (string item in strings)
                    {
                        CMD.OutPuter.AddMessage(item, Log.Type.OutPut);
                    }
                    return strings.ToString();
                }
                else if (strRet1[1].ToLower().Contains(":"))
                {
                    string[] startend = strRet1[1].Split(':');
                    string[] strings = Controller.GetHovertMemoryString(int.Parse(startend[0]), int.Parse(startend[1]));
                    return strings.ToString();
                }
                else
                {
                    string outs = Controller.GetMemoryString(int.Parse(strRet1[1]));
                    return outs;
                }
            }
            else
            {
                return Script;
            }
        }

        public void ExecutePrintInstruction(Instruction line)
        {
            switch (line.Type)
            {
                case OpCode.MemoryString:
                    Controller.AddMemodyString(line.Value);
                    break;
                case OpCode.Ret:
                    ParseRetModule(line);
                    break;
                default:
                    CMD.OutPuter.AddMessage("The " + line.Type.ToString() + " Module not support on Print method");
                    break;
            }

        }
        private void ParseRetModule(Instruction line)
        {
            if (line.Value.ToLower().StartsWith(OpCode.MemoryString.ToString().ToLower()))
                ParseMemoryString(line.Value);
            else CMD.OutPuter.AddMessage(line.Value, Log.Type.OutPut);
        }
        public void ExecuteBaseInstruction(Instruction line)
        {
                switch (line.Type)
                {
                    case OpCode.MemoryString:
                        Controller.AddMemodyString(line.Value);
                        break;
                    case OpCode.Ret:
                        ParseRetModule(line);
                        break;
                    default:
                        CMD.OutPuter.AddMessage("The " + line.Type.ToString() + " Module not support on Base method");
                        break;
                }
        }
        private void SetConfig(Instruction line, ConfigDef config)
        {
            switch (line.Type)
            {
                case OpCode.URL:
                    string[] URLs = line.Value.Split('|');
                    if (URLs.Length == 2 && URLs[1] == "1")
                        config.SetURL(line.Value, true);
                    else config.SetURL(line.Value);
                    break;
                case OpCode.UserAgent:
                    config.SetUserAgent(line.Value);
                    break;
                case OpCode.Referer:
                    config.SetReferer(line.Value);
                    break;
                case OpCode.KeepAlive:
                    config.SetKeepAlive(line.Value);
                    break;
                case OpCode.Method:
                    config.SetMethod(line.Value);
                    break;
                case OpCode.DataSet:
                    config.SetDataSet(line.Value);
                    break;
                case OpCode.PostData:
                    config.SetPostData(line.Value);
                    break;
                case OpCode.Cookies:
                    config.SetCookes(line.Value);
                    break;
                case OpCode.ContectType:
                    config.SetContectType(line.Value);
                    break;
                case OpCode.AllowAutoRedirect:
                    config.SetAllowAutoRedirect(line.Value);
                    break;
                case OpCode.AddAuthorization:
                    config.AddAuthorization(line.Value);
                    break;
                case OpCode.Headers:
                    config.SetHeaders(line.Value);
                    break;
                case OpCode.AddHeader:
                    config.AddHeaders(line.Value);
                    break;
                case OpCode.TimeOut:
                    config.SetTimeOut(line.Value);
                    break;
                case OpCode.Gzip:
                    config.SetGzipDecomprossor(line.Value);
                    break;
                default:
                    CMD.OutPuter.AddMessage("The " + line.Type.ToString() + " Module not support on Config method");
                    break;
            }
        }
        public void ExecuteConfigInstruction(Instruction line)
        {

            if (line.Type == OpCode.MemoryString)
                Controller.AddMemodyString(line.Value);
            else
                SetConfig(line, Controller.GetLastConfigdef());

        }
        public Instruction ReplaceMemoryStringsOnValue(Instruction line)
        {
            string pattern = @"\|memorystring-(.*?)\|";

            foreach (Match m in Regex.Matches(line.Value, pattern, RegexOptions.IgnoreCase))
            {
                if (m.Groups.Count == 0)
                    continue;
                if (m.Groups[1].Value == "")
                    continue;

                try {
                    line.Value = line.Value.Replace(m.Value, ParseMemoryString("memorystring-" + m.Groups[1].Value));
                }
                catch { }
            }
            return line;
        }
    }
}
