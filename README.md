# MSC.Script
Core scripting for msc
# ???
you can run your script with msc.script
# How To?
Just write your script and Install methods with CommendLine and start script
# Based on MSC
This project just support msc methods for requests
# Base Method
Supported Instructions:
- MemoryString
- Ret
# Config Method
Supported Instructions:
- MemoryString
- UserAgent
- Cookies
- KeepAlive
- UserAgent
- TimeOut
- Gzip
- AddHeader
- ContectType
- Referer
- DataSet
- PostData
- AddAuthorization
# Request Method
Supported Instructions:
- MemoryString (sourcepage, cookies, regex)
- Ret (sourcepage, cookies, regex)
- RequestManage (getdata, postdata)
- SetConfig (for set a setting config)
# Print Method
Supported Instructions:
- Ret
- MemoryString
# MemoryString
you can use memorystring by index in value Instructions (replace value)
- Like: PostData=>link=|memorystring-1|&value=0
# Intruction
Type=>Value
# Method
- Start method: ==>TypeMethod<==
- End method: <==>
- Type Methods: Config - Request - Base - Print
- Set ID For Yor Method: ==>Config-1<==
# And More
- You can regex a string and return string
