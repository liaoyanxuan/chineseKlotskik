using aliyun_api_gateway_sdk.Constant;
using aliyun_api_gateway_sdk.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*当error_code为0 通过不通过isok值判断*/
/*用户传递上来真实姓名脱敏返回*/
/*用户传递上来IdcardNo的脱敏返回*/
/*true：匹配 false：不匹配*/
/*
 * {
  "error_code": 0,

  "reason": "Success",
  "result": {
      "realname": "张*",
      "idcard": "3303***********",
      "isok": false,
      "IdCardInfor": {
           "province":"浙江省",
           "city":"杭州市",
          "district":"xx县",
          "area": "浙江省杭州市区xx县",
          "sex": "男",
          "birthday": "1965-3-10"
      } }
}
 * 注意：
  1.解析：先判断error_code为0仅代表端口通讯成功再看业务逻辑码isok值当isok为true为匹配 false为不匹配。
  2.出现'库无'时 "error_code":206501，有以下几种原因
   (1)现役军人、武警官兵、特殊部门人员及特殊级别官员；
 (2)退役不到2年的军人和士兵（根据军衔、兵种不同，时间会有所不同，一般为2年）；
 (3)户口迁出，且没有在新的迁入地迁入 eg：刚上大学或刚毕业的大学生；
 (4)户口迁入新迁入地，当地公安系统未将迁移信息上报到公安部（上报时间地域不同而有所差异）；
 (5)更改姓名，当地公安系统未将更改信息上报到公安部（上报时间因地域不同而有所差异）；
 (6)移民；
 (7)未更换二代身份证；
 (8)死亡。
 (9)身份证号确实不存在
 */
namespace aliyun_api_gateway_sdk
{
    class IDCodeAndNameVerifeidApi
	{
      ////产品地址：  https://market.aliyun.com/products/57124001/cmapi00038162.html?spm=5176.2020520132.101.14.22527218Gu57bi#sku=yuncode32162000017
        private const String appKey = "203994959";
        private const String appSecret = "Q48EQOzkLKlt4mKnA82JLqM1yP9Kq8Kj";
        private const String appcode = "8abef050dfde47099347c115f26cfedf";
        private const String host = "https://lfeid.market.alicloudapi.com";

      
        public static NameCodeResult doGet(string cardId,string name)
        {
           

            String path = "/idcheck/life";
            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();            
            List<String> signHeader = new List<String>();

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_TEXT);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_TEXT);
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("Authorization", "APPCODE " + appcode);
          

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("cardNo", cardId);
            querys.Add("realName", name);

        
            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("Authorization");


            NameCodeResult jsonResult = null;
            using (HttpWebResponse response = HttpUtil.HttpGet(host, path, appKey, appSecret, 30000, headers, querys, signHeader))
            {
                if (response != null)
                {
                    Debug.Log(response.StatusCode);
                    Debug.Log(response.Method);
                    Debug.Log(response.Headers);
                    Stream st = response.GetResponseStream();
                    StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                    string content = reader.ReadToEnd();
                    Debug.Log(content);
                    Debug.Log(Constants.LF);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        jsonResult = JsonUtility.FromJson<NameCodeResult>(content);
                    }
                }
            }

            return jsonResult;
        }

/*
        public static void doGetTest()
        {
            String querys = "cardNo=330329199001021122&realName=%E6%9D%8E%E5%9B%9B";
            String bodys = "";
            String url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            Console.WriteLine(httpResponse.StatusCode);
            Console.WriteLine(httpResponse.Method);
            Console.WriteLine(httpResponse.Headers);
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            Console.WriteLine(reader.ReadToEnd());
            Console.WriteLine("\n");
        }
        */

        private static void doPostForm() {
            String path = "/postform";          

            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            Dictionary<String, String> bodys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_FORM);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");
                        
            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //注意：业务body部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            bodys.Add("b-body2", "bodyvalue1");
            bodys.Add("a-body1", "bodyvalue2");

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpPost(host, path, appKey, appSecret, 30000, headers, querys, bodys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }

        private static void doPostStream()
        {
            String path = "/poststream";
           
            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            Dictionary<String, String> bodys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();
            byte[] bobyContent = new byte[10];

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_STREAM);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);

            //注意：如果有非Form形式数据(body中只有value，没有key)；如果body中是key/value形式数据，不要指定此行
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_MD5, MessageDigestUtil.Base64AndMD5(bobyContent));
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //注意：业务body部分
            bodys.Add("", BitConverter.ToString(bobyContent));

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpPost(host, path, appKey, appSecret, 30000, headers, querys, bodys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }

        private static void doPostString()
        {
            String bobyContent = "{\"inputs\": [{\"image\": {\"dataType\": 50,\"dataValue\": \"base64_image_string(此行)\"},\"configure\": {\"dataType\": 50,\"dataValue\": \"{\"side\":\"face(#此行此行)\"}\"}}]}";

            String path = "/poststring";
            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            Dictionary<String, String> bodys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_JSON);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);

            //注意：如果有非Form形式数据(body中只有value，没有key)；如果body中是key/value形式数据，不要指定此行
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_MD5, MessageDigestUtil.Base64AndMD5(Encoding.UTF8.GetBytes(bobyContent)));
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //注意：业务body部分
            bodys.Add("", bobyContent);

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpPost(host, path, appKey, appSecret, 30000, headers, querys, bodys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }
        
        private static void doPutStream()
        {
            String path = "/putstream";
           
            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            Dictionary<String, String> bodys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();
            byte[] bobyContent = new byte[10];

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_STREAM);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);

            //注意：如果有非Form形式数据(body中只有value，没有key)；如果body中是key/value形式数据，不要指定此行
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_MD5, MessageDigestUtil.Base64AndMD5(bobyContent));
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //注意：业务body部分
            bodys.Add("", BitConverter.ToString(bobyContent));

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpPut(host, path, appKey, appSecret, 30000, headers, querys, bodys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }

        private static void doPutString()
        {
            String bobyContent = "{\"inputs\": [{\"image\": {\"dataType\": 50,\"dataValue\": \"base64_image_string(此行)\"},\"configure\": {\"dataType\": 50,\"dataValue\": \"{\"side\":\"face(#此行此行)\"}\"}}]}";

            String path = "/putstring";
            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            Dictionary<String, String> bodys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_JSON);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);

            //注意：如果有非Form形式数据(body中只有value，没有key)；如果body中是key/value形式数据，不要指定此行
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_MD5, MessageDigestUtil.Base64AndMD5(Encoding.UTF8.GetBytes(bobyContent)));
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //注意：业务body部分
            bodys.Add("", bobyContent);

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpPut(host, path, appKey, appSecret, 30000, headers, querys, bodys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }

        private static void doDelete()
        {
            String path = "/delete";
            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_JSON);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpDelete(host, path, appKey, appSecret, 30000, headers, querys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }

        private static void doHead()
        {
            String path = "/head";

            Dictionary<String, String> headers = new Dictionary<string, string>();
            Dictionary<String, String> querys = new Dictionary<string, string>();
            List<String> signHeader = new List<String>();

            //设定Content-Type，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_JSON);
            //设定Accept，根据服务器端接受的值来设置
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);
            //如果是调用测试环境请设置
            //headers.Add(SystemHeader.X_CA_STAGE, "TEST");

            //注意：业务header部分，如果没有则无此行(如果有中文，请做Utf8ToIso88591处理)
            headers.Add("b-header2", MessageDigestUtil.Utf8ToIso88591("headervalue1"));
            headers.Add("a-header1", MessageDigestUtil.Utf8ToIso88591("headervalue2处理"));

            //注意：业务query部分，如果没有则无此行；请不要、不要、不要做UrlEncode处理
            querys.Add("b-query2", "queryvalue2");
            querys.Add("a-query1", "queryvalue1");

            //指定参与签名的header            
            signHeader.Add(SystemHeader.X_CA_TIMESTAMP);
            signHeader.Add("a-header1");
            signHeader.Add("b-header2");

            using (HttpWebResponse response = HttpUtil.HttpHead(host, path, appKey, appSecret, 30000, headers, querys, signHeader))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(Constants.LF);

            }
        }
   
    }
}
