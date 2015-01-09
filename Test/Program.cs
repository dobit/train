using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using LFNet.Net.Http;
//using LFNet.TrainTicket;
//using LFNet.TrainTicket.Config;
using System.Net.Http;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
          Console.WriteLine(new ValidateCode(false).CheckCode);

            //Console.WriteLine("bin216:"+Utils.Bin216("123456"));
            //Console.WriteLine("Encode32:" + Utils.Encode32("123456"));
            //var data = new Base32().stringToLongArray("1234567",true);
            //Console.WriteLine("stringToLongArray:" + string.Join(",", data));
            //Console.WriteLine("longArrayToString:" + new Base32().longArrayToString(data, true));
            //Console.WriteLine("Base32:" + new Base32().encrypt("0000", "ABC123"));


            //Console.WriteLine("encode:" + Utils.Encode32(Utils.Bin216(new Base32().encrypt("0000", "NjY1NjIy"))));
            //Console.ReadLine();



            //System.Net.ServicePointManager.ServerCertificateValidationCallback =(sender, certificate, chain, errors) => true;

            //IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses("dynamic.12306.cn");
          

           
            //string ret = account.OrderTicket(list[0], Passengers);
            //if(string.IsNullOrEmpty(ret))
            //{
            //  Console.WriteLine("OrderId="+account.GetOderId());
            //}
            //else
            //{
            //    Console.WriteLine(ret);
            //}
            Console.ReadLine();


        }


        public class  TestClass
        {
            public string Name { get; set; }
            public string Type { get; set; }

            public int Method { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> Data { get; set; } 
        }
    }





    /// <summary>
    /// 验证码类
    /// </summary>
    public class ValidateCode
    {
        #region Properties

        private int _codeLength=5;
        public int CodeLength
        {
            get { return _codeLength; }
            set { _codeLength = value; }
        }

        private string _checkCode;

        public string CheckCode
        {
            get
            {
                if (_checkCode == null)
                {
                    _checkCode = GenerateCheckCode(useAlphaChars,_codeLength);
                    
                }
                return _checkCode;
            }
            set
            {

                _checkCode = value;
                Set(value);
            }
        }

        private Image _image;
        public Image Image
        {
            get
            {
                if (_image == null)
                {

                    _image = GenerateImage();
                }
                return _image;
            }
            //set { _image = value; }
        }

        private bool useAlphaChars = true;
        #endregion

        public ValidateCode()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usealphaChars">使用字母</param>
        public ValidateCode(bool useAlphaChars)
        {
            this.useAlphaChars = useAlphaChars;
        }

        public ValidateCode(bool useAlphaChars,int codeLength)
        {
            this.useAlphaChars = useAlphaChars;
            _codeLength = codeLength;
        }
        public ValidateCode(string code)
        {
            CheckCode = code;
            
        }

        private Image GenerateImage()
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((CheckCode.Length * 15f)), 24, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.HighSpeed;
            try
            {

                //清空图片背景色
                g.Clear(Color.White);

                //int textcolor =0;
                //int fixedNumber = textcolor == 2 ? 60 : 0;
                Pen linePen = new Pen(Color.FromArgb(Next(0x00), Next(0xB0), Next(0xB0)), 1);

                SolidBrush drawBrush = new SolidBrush(Color.FromArgb(Next(0xff), Next(0xff), Next(0xff)));
                for (int i = 0; i < 7; i++)
                {
                    g.DrawArc(linePen, Next(20) - 10, Next(20) - 10, Next(image.Width) + 10, Next(image.Height) + 10,
                              Next(-100, 100), Next(-200, 200));
                }
                Graphics charg = Graphics.FromImage(charbmp);
                Color colorA = Color.Black;
                Color colorB = Color.Red;
                float charx = -13;
                int len = CheckCode.Length;
                for (int i = 0; i < len; i++)
                {
                    m.Reset();
                    m.RotateAt(Next(50) - 25, new PointF(Next(2) + 6, Next(2) + 6));

                    charg.Clear(Color.Transparent);
                    charg.Transform = m;
                    //定义前景色为黑色
                    Color nc = Color.FromArgb(0xff, colorA.R + (i * (colorB.R - colorA.R) / len),
                                              colorA.G + (i * (colorB.G - colorA.G) / len),
                                              colorA.B + (i * (colorB.B - colorA.B) / len));
                    drawBrush.Color = nc;

                    charx = charx + 13 + Next(2);
                    PointF drawPoint = new PointF(charx, 2.0F);
                    charg.DrawString(CheckCode[i].ToString(), fonts[Next(fonts.Length - 1)], drawBrush, new PointF(0, 0));

                    charg.ResetTransform();

                    g.DrawImage(charbmp, drawPoint);
                }

                //画图片的前景噪音点
                for (int i = 0; i < 50; i++)
                {
                    int x = Next(image.Width - 1);
                    int y = Next(image.Height - 1);

                    image.SetPixel(x, y, Color.FromArgb(Next(0xB0), Next(0xB0), Next(0xB0)));
                }



                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);


            }
            catch (Exception exception)
            {

            }
            finally
            {
                g.Dispose();
            }
            return image;
        }

        /// <summary>
        /// 输出Image
        /// </summary>
        /// <param name="response"></param>
        public void Response()
        {
            HttpResponse response = HttpContext.Current.Response;
            response.ContentType = "image/png";
            //image.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            if (Image != null)
            {
                Image.Save(response.OutputStream, ImageFormat.Png);
            }
            response.End();
        }


        #region Static Methods

        private static int _seed = 0;
        private static Bitmap charbmp = new Bitmap(40, 40, PixelFormat.Format32bppArgb);
        private static Matrix m = new Matrix();
        private static byte[] randb = new byte[4];
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

        private static Font[] fonts = {
                                          new Font(new FontFamily("Helvetica"), 12 + Next(1), FontStyle.Bold),
                                          new Font(new FontFamily("Georgia"), 12 + Next(1), FontStyle.Bold),
                                          new Font(new FontFamily("Arial"), 12 + Next(1), FontStyle.Bold),
                                          new Font(new FontFamily("Verdana"), 12 + Next(1), FontStyle.Bold)

                                      };
        FontStyle[] _fontStyles = new FontStyle[] { FontStyle.Bold | FontStyle.Strikeout, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout, FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout, FontStyle.Bold | FontStyle.Strikeout | FontStyle.Italic };


       
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private static int Next(int max)
        {
            rand.GetBytes(randb);
            int value = BitConverter.ToInt32(randb, 0);
            value = value % (max + 1);
            if (value < 0)
                value = -value;
            return value;
        }

        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private static int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public static  string GenerateCheckCode(bool useAlphaChars,int length)
        {
            char code;
            int number;
            string checkCode = "";
            if (checkCode.Length == 0)
            {
                System.Random random = new Random();

                for (int i = 0; i < length; i++)
                {
                    number = random.Next();
                    if (useAlphaChars)
                    {

                        if (number%2 == 0)
                            code = (char) ('0' + (char) (number%10));
                        else
                            code = (char) ('A' + (char) (number%26));
                    }
                    else
                    {
                        code = (char)('0' + (char)(number % 10));
                    }
                    checkCode += code.ToString();
                }
            }
            checkCode = checkCode.Replace('D', 'S').Replace('O', 'K').Replace('0', '8');
            Set(checkCode);
            return checkCode;
        }

        private static void Set(string vCode)
        {

            //string vid = Common.JRequest.GetString("j_cid");
            //if (string.IsNullOrEmpty(vid))
            //{
            //    vid =CookieUtil.GetCookie("j_cid"); 
            //}
            //if (string.IsNullOrEmpty(vid))
            //{
            //    int appId = AppDomain.CurrentDomain.Id;//Process.GetCurrentProcess().Id
            //    vid =appId+"_"+System.Threading.Interlocked.Increment(ref _seed).ToString(); 
            //    CookieUtil.WriteCookie("j_cid", vid,5,Global.CookieDomain);
            //    MemcachedCache.GetVerifyCodeCache().Set("cid_" + vid, vCode.ToLower(), DateTime.Now.AddMinutes(6));
            //}
            //else
            //{
            //    MemcachedCache.GetVerifyCodeCache().Set("cid_" + vid, vCode.ToLower(), DateTime.Now.AddMinutes(6));
            //}

            

           // JCache.Set("cid_"+vid,vCode.ToLower(),DateTime.Now.AddMinutes(6)); //设置缓存 //多进程模式下不适用memeryCached会出现问题

        }
        
        /// <summary>
        /// 获取验证码在缓存中的键值
        /// </summary>
        /// <returns></returns>
        public static string GetCacheCodeName()
        {
            //string vid = Common.JRequest.GetString("j_cid");
            //if (string.IsNullOrEmpty(vid))
            //{
            //    vid = CookieUtil.GetCookie("j_cid");
            //}
            //if (string.IsNullOrEmpty(vid))
            //{
            //    return "";
            //}
            return "cid_"; // + vid;
        }
        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="vCode"></param>
        /// <returns></returns>
        public static bool IsValid(string vCode)
        {
            if(string.IsNullOrEmpty(vCode)) return false;
           
            if (vCode.ToLower() == GetCacheCode(GetCacheCodeName()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证码是否正确，并从缓存中删除验证码
        /// </summary>
        /// <param name="vCode"></param>
        /// <returns></returns>
        public static bool IsValidAndRemove(string vCode)
        {
            if (string.IsNullOrEmpty(vCode)) return false;

            if (vCode.ToLower() == GetCacheCode(GetCacheCodeName()))
            {
                RemoveCacheCode();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取缓存中的验证码
        /// </summary>
        /// <returns></returns>
        public static string GetCacheCode()
        {

            return GetCacheCode(GetCacheCodeName());
        }
        
        public static string GetCacheCode(string cacheCodeName)
        {

            return "";//MemcachedCache.GetVerifyCodeCache().Get(cacheCodeName) as string;
        }

        /// <summary>
        /// 删除缓存中的验证码
        /// </summary>
        public static void RemoveCacheCode()
        {
           // MemcachedCache.GetVerifyCodeCache().Delete(GetCacheCodeName());
        }

        ///// <summary>
        ///// 检测用户验证码cookie，有效信息
        ///// </summary>
        ////public static void CheckUserCookie()
        ////{
        ////    if (string.IsNullOrEmpty(CookieUtil.GetCookie("j_cid")))
        ////    {
        ////       // CookieUtil.WriteCookie("j_cid", Guid.NewGuid().ToString(), 365 * 24 * 60, Global.CookieDomain);
        ////    }
        ////}

        #endregion


    }
}

