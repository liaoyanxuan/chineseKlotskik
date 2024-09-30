using System;
using System.Text.RegularExpressions;

namespace ConnUtils
{
    /*
    身份证前两位各省对应的编号是：

    1、华北地区：北京市|11，天津市|12，河北省|13，山西省|14，内蒙古自治区|15；

    2、东北地区： 辽宁省|21，吉林省|22，黑龙江省|23；

    3、华东地区： 上海市|31，江苏省|32，浙江省|33，安徽省|34，福建省|35，江西省|36，山东省|37；

    4、华中地区： 河南省|41，湖北省|42，湖南省|43；

    5、华南地区：广东省|44，广西壮族自治区|45，海南省|46；

    6、西南地区： 四川省|51，贵州省|52，云南省|53，西藏自治区|54，重庆市|50；

    7、西北地区： 陕西省|61，甘肃省|62，青海省|63，宁夏回族自治区|64，新疆维吾尔自治区|65；

    8、特别地区：台湾地区(886)|83，香港特别行政区（852)|81，澳门特别行政区（853)|82。
*/
    /**************************************************************************************/
    /************************************身份证号码的验证*************************************/
    /**************************************************************************************/

    /**  
    * 身份证15位编码规则：dddddd yymmdd xx p
    * dddddd：地区码
    * yymmdd: 出生年月日
    * xx: 顺序类编码，无法确定
    * p: 性别，奇数为男，偶数为女
    * <p />
    * 身份证18位编码规则：dddddd yyyymmdd xxx y
    * dddddd：地区码
    * yyyymmdd: 出生年月日
    * xxx:顺序类编码，无法确定，奇数为男，偶数为女
    * y: 校验码，该位数值可通过前17位计算获得
    * <p />
    * 18位号码加权因子为(从右到左) Wi = [ 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2,1 ]
    * 验证位 Y = [ 1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2 ]
    * 校验位计算公式：Y_P = mod( ∑(Ai×Wi),11 )
    * i为身份证号码从右往左数的 2...18 位; Y_P为脚丫校验码所在校验码数组位置
    **/
    public class IdentityCardValidation
    {
        /// <summary> 
        /// 验证身份证合理性 
        /// </summary> 
        /// <param name="idNumber">身份证号</param> 
        /// <returns></returns> 
        public static bool CheckIdCard(string idNumber)
        {
            if (idNumber.Length == 18)
            {
                var check = CheckIdCard18(idNumber);
                return check;
            }
            if (idNumber.Length != 15) return false;
            {
                var check = CheckIdCard15(idNumber);
                return check;
            }
        }

        /**
        * 验证输入的名字是否为“中文”或者是否包含“·”
        */
        /*
       public static bool isLegalName(String name)
       {
           if (name.Contains("·") || name.Contains("•"))
           {
               if (name.matches("^[\\u4e00-\\u9fa5]+[·•][\\u4e00-\\u9fa5]+$"))
               {
                   return true;
               }
               else
               {
                   return false;
               }
           }
           else
           {
               if (name.matches("^[\\u4e00-\\u9fa5]+$"))
               {
                   return true;
               }
               else
               {
                   return false;
               }
           }
       }
       */


        public static bool CheckName(String name)
        {

            //string reg = "[\u4e00-\u9fa5\u9fa6-\u9fcb\u3400-\u4db5\u20000-\u2a6d6\u2A700-\u2B734\u2B740-\u2B81D\u2F00-\u2FD5\u2E80-\u2EF3\uF900-\uFAD9\u2F800-\u2FA1D\uE815-\uE86F\uE400-\uE5E8\uE600-\uE6CF\u31C0-\u31E3\u2FF0-\u2FFB\u3105-\u3120\u31A0-\u31BA@]";
            //string reg = "[\u4e00-\u9fa5]";
            string reg = "[\u2E80-\uFE4F]";

            string regContainDocStr = "^" + reg + "+[·•]" + reg + "+$";   //包含1个点
			Regex regexContainDoc = new Regex(regContainDocStr);

			string regContainDocStr2 = "^" + reg + "+[·•]" + reg+ "+[·•]" + reg + "+$";   //包含2个点
			Regex regexContainDoc2 = new Regex(regContainDocStr2);

			string normalStr= "^" +reg +"+$";
            Regex normalRegex = new Regex(normalStr);

            if (name.Contains("·") || name.Contains("•"))
            {
                if (regexContainDoc.IsMatch(name))
                {
                    return true;
                }
                else if (regexContainDoc2.IsMatch(name))
				{
					return true;
				}else
				{
                    return false;
                }
            }
            else
            {
                if (normalRegex.IsMatch(name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

       

        /// <summary> 
        /// 18位身份证号码验证 
        /// </summary> 
        private static bool CheckIdCard18(string idNumber)
        {
            if (long.TryParse(idNumber.Remove(17), out var n) == false
                || n < Math.Pow(10, 16)
                || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证 
            }
            //省份编号
            const string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证 
            }
            var birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            if (DateTime.TryParse(birth, out _) == false)
            {
                return false;//生日验证 
            }
            string[] arrArrifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                // 加权求和 
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            //得到验证码所在位置
            Math.DivRem(sum, 11, out var y);
            var x = idNumber.Substring(17, 1).ToLower();
            var yy = arrArrifyCode[y];
            if (arrArrifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证 
            }
            return true;//符合GB11643-1999标准 
        }

        /// <summary> 
        /// 15位身份证号码验证 
        /// </summary> 
        private static bool CheckIdCard15(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证 
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证 
            }
            string birth = idNumber.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证 
            }
            return true;
        }
    }
}
 