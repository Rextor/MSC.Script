using MSC.Brute;
using System.Collections.Generic;

namespace MSC.Script
{
    public enum OpCode
    {
        Cookies,
        URL,
        KeepAlive,
        Method,
        SourcePage,
        MemoryString,
        Ret,
        UserAgent,
        TimeOut,
        Gzip,
        Headers,
        AddHeader,
        Redirect,
        ContectType,
        Referer,
        DataSet,
        PostData,
        RequestManage,
        AllowAutoRedirect,
        AddAuthorization,
        SetConfig,
        Unkown
    }
    public enum MethodType
    {
        Config,
        Request,
        Print,
        Base,
        Unkown
    }
    public class MemoryString
    {
        List<string> ListM { set; get; }
        public void Creat()
        {
            ListM = new List<string>();
        }
        public void AddString(string Input)
        {
            ListM.Add(Input);
        }
        public string GetString(int Index)
        {
            return ListM[Index];
        }
    }
    public class RequestDef
    {
        RequestManage Request { set; get; }
        public RequestDef Helper { set; get; }
        public ConfigDef configdef { set; get; }
        Requester Rr = new Requester();
        public RequestManage GetManage()
        {
            return Request;
        }
        public void Create()
        {
            Request = new RequestManage();
        }
        public string GetLocation()
        {
            return Request.Location;
        }
        public string GetSourcePage()
        {
            return Request.SourcePage;
        }
        public string GetCookies()
        {
            return Request.CookiesString;
        }
        public void GetData()
        {
            try {
                Request = Rr.GETData(configdef.GetConfig(), Helper.Request);
            }
            catch {
                try {
                    Request = Rr.GETData(configdef.GetConfig());
                }
                catch { }
            }
        }
        public void PostData()
        {
            try {
                Request = Rr.POSTData(configdef.GetConfig(), Helper.Request); }
            catch {
                try {
                    Request = Rr.POSTData(configdef.GetConfig());
                }
                catch { }
                }
        }
    }
    public class ConfigDef
    {
        Config config { set; get; }
        public void Create()
        {
            config = new Config();
        }
        public void SetConfig(Config _config)
        {
            config = _config;
        }
        public Config GetConfig()
        {
            return config;
        }
        public void SetDataSet(string Value)
        {
            config.DataSet = Value;
        }
        public void SetPostData(string Value)
        {
            config.PostData = Value;
        }
        public void SetCookes(string Value)
        {
            config.Cookies = Value;
        }
        public void SetUserAgent(string Value)
        {
            config.UserAgent = Value;
        }
        public void AddAuthorization(string Value)
        {
            config.AddAuthorization(Value);
        }
        public void SetURL(string Value)
        {
            config.LoginURL = Value;
        }
        public void SetURL(string Value,bool SetReferer)
        {
            config.LoginURL = Value;
            if (SetReferer)
                config.Referer = Value;
        }
        public void SetReferer(string Value)
        {
            config.Referer = Value;
        }
        public void SetContectType(string Value)
        {
            config.ContectType = Value;
        }
        public void SetHeaders(string Value)
        {
            config.Headers = Value;
        }
        public void AddHeaders(string Value)
        {
            config.AddHeader(Value);
        }
        public void SetGzipDecomprossor(string Value)
        {
            config.DecompressionGZip = ParseBool(Value);
        }
        public void SetAllowAutoRedirect(string Value)
        {
            config.AllowAutoRedirect = ParseBool(Value);
        }
        public void SetTimeOut(string Value)
        {
            config.TimeOut = int.Parse(Value);
        }
        public void SetMethod(string Value)
        {
            MSC.Method method;
            switch (Value.ToString().ToLower())
            {
                case "get":
                    method = MSC.Method.GET;
                    break;
                case "post":
                    method = MSC.Method.POST;
                    break;
                case "put":
                    method = MSC.Method.PUT;
                    break;
                default:
                    throw new System.Exception("Unkown Method!");
            }
            config.Method = method;
        }
        public void SetKeepAlive(string Value)
        {
            config.KeepAlive = ParseBool(Value);
        }
        private static bool ParseBool(string Value)
        {
            bool flag;
            switch (Value)
            {
                case "0":
                    flag = false;
                    break;
                case "1":
                    flag = true;
                    break;
                case "true":
                    flag = true;
                    break;
                case "false":
                    flag = false;
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
        }
    }
}
