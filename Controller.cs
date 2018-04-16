using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSC;

namespace MSC.Script
{
    public class Controller
    {
        public List<string> MemoryString = new List<string>();
        public string GetMemoryString(int Index)
        {
            try {
                if (!string.IsNullOrWhiteSpace(MemoryString[Index - 1]))
                    return MemoryString[Index - 1];
            }
            catch { }
            return "NoString";
        }
        public string GetLastMemoryString()
        {
            try {
                if (!string.IsNullOrWhiteSpace(MemoryString[MemoryString.Count - 1]))
                    return MemoryString[MemoryString.Count - 1];
            }
            catch { }
            return "NoString";
        }
        public void AddMemodyString(string Input)
        {
            MemoryString.Add(Input);
        }
        public string[] GetAllMemoryString()
        {
            string[] strs = new string[MemoryString.Count];
            for (int i = 0; i <= MemoryString.Count - 1; i++)
            {
                if (!string.IsNullOrWhiteSpace(MemoryString[i]))
                    strs[i] = MemoryString[i];
            }
            return strs;
        }
        public string[] GetHovertMemoryString(int start, int end)
        {
            string[] strs = new string[end - start];
            int b = 0;
            for (int i = start; i < end; i++)
            {
                if (!string.IsNullOrWhiteSpace(MemoryString[i]))
                    strs[b] = MemoryString[i];
                b++;
            }
            return strs;
        }

        public List<ConfigDef> ConfigDefes = new List<ConfigDef>();
        public void NewConfigDef()
        {
            ConfigDef configdef = new ConfigDef();
            configdef.Create();
            ConfigDefes.Add(configdef);
        }
        public ConfigDef GetConfigdef(int index)
        {
            return ConfigDefes[index - 1];
        }
        public ConfigDef GetLastConfigdef()
        {
            return ConfigDefes[ConfigDefes.Count - 1];
        }

        public List<RequestDef> RequestDefes = new List<RequestDef>();
        public void NewRequestDef()
        {
            RequestDef requestdef = new RequestDef();
            requestdef.Create();
            RequestDefes.Add(requestdef);
        }
        public RequestDef GetRequestDef(int index)
        {
            return RequestDefes[index - 1];
        }
        public RequestDef GetLastRequestDef()
        {
            return RequestDefes[RequestDefes.Count - 1];
        }
        public void SetLastRequestDef(RequestDef Requestdef)
        {
            RequestDefes[RequestDefes.Count - 1] = Requestdef;
        }
    }
}
