using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GoogleLunarCalendar
{
    public sealed class ChineseCalendarInfo
    {
        public const string ChineseNumber = "〇一二三四五六七八九";

        public const string CelestialStem = "甲乙丙丁戊己庚辛壬癸";

        public const string TerrestrialBranch = "子丑寅卯辰巳午未申酉戌亥";

        public const string Animals = "鼠牛虎兔龙蛇马羊猴鸡狗猪";

        private DateTime m_SolarDate;

        private int m_LunarYear;

        private int m_LunarMonth;

        private int m_LunarDay;

        private bool m_IsLeapMonth = false;

        private string m_LunarYearSexagenary = null;

        private string m_LunarYearAnimal = null;

        private string m_LunarYearText = null;

        private string m_LunarMonthText = null;

        private string m_LunarDayText = null;

        private string m_SolarWeekText = null;

        private string m_SolarConstellation = null;

        private string m_SolarBirthStone = null;

        private static ChineseLunisolarCalendar calendar = new ChineseLunisolarCalendar();

        public static readonly string[] ChineseWeekName = new string[7]
		{
			"星期天",
			"星期一",
			"星期二",
			"星期三",
			"星期四",
			"星期五",
			"星期六"
		};

        public static readonly string[] ChineseDayName = new string[30]
		{
			"初一",
			"初二",
			"初三",
			"初四",
			"初五",
			"初六",
			"初七",
			"初八",
			"初九",
			"初十",
			"十一",
			"十二",
			"十三",
			"十四",
			"十五",
			"十六",
			"十七",
			"十八",
			"十九",
			"二十",
			"廿一",
			"廿二",
			"廿三",
			"廿四",
			"廿五",
			"廿六",
			"廿七",
			"廿八",
			"廿九",
			"三十"
		};

        public static readonly string[] ChineseMonthName = new string[12]
		{
			"正",
			"二",
			"三",
			"四",
			"五",
			"六",
			"七",
			"八",
			"九",
			"十",
			"十一",
			"十二"
		};

        public static readonly string[] Constellations = new string[12]
		{
			"白羊座",
			"金牛座",
			"双子座",
			"巨蟹座",
			"狮子座",
			"处女座",
			"天秤座",
			"天蝎座",
			"射手座",
			"摩羯座",
			"水瓶座",
			"双鱼座"
		};

        public static readonly string[] BirthStones = new string[12]
		{
			"钻石",
			"蓝宝石",
			"玛瑙",
			"珍珠",
			"红宝石",
			"红条纹玛瑙",
			"蓝宝石",
			"猫眼石",
			"黄宝石",
			"土耳其玉",
			"紫水晶",
			"月长石，血石"
		};

        public DateTime SolarDate
        {
            get
            {
                return m_SolarDate;
            }
            set
            {
                if (!m_SolarDate.Equals(value))
                {
                    m_SolarDate = value;
                    LoadFromSolarDate();
                }
            }
        }

        public string SolarWeekText
        {
            get
            {
                if (string.IsNullOrEmpty(m_SolarWeekText))
                {
                    int dayOfWeek = (int)m_SolarDate.DayOfWeek;
                    m_SolarWeekText = ChineseWeekName[dayOfWeek];
                }
                return m_SolarWeekText;
            }
        }

        public string SolarConstellation
        {
            get
            {
                return m_SolarConstellation;
            }
        }

        public string SolarBirthStone
        {
            get
            {
                return m_SolarBirthStone;
            }
        }

        public int LunarYear
        {
            get
            {
                return m_LunarYear;
            }
        }

        public int LunarMonth
        {
            get
            {
                return m_LunarMonth;
            }
        }

        public bool IsLeapMonth
        {
            get
            {
                return m_IsLeapMonth;
            }
        }

        public int LunarDay
        {
            get
            {
                return m_LunarDay;
            }
        }

        public string LunarYearSexagenary
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarYearSexagenary))
                {
                    int sexagenaryYear = calendar.GetSexagenaryYear(SolarDate);
                    m_LunarYearSexagenary = "甲乙丙丁戊己庚辛壬癸".Substring((sexagenaryYear - 1) % 10, 1) + "子丑寅卯辰巳午未申酉戌亥".Substring((sexagenaryYear - 1) % 12, 1);
                }
                return m_LunarYearSexagenary;
            }
        }

        public string LunarYearAnimal
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarYearAnimal))
                {
                    int sexagenaryYear = calendar.GetSexagenaryYear(SolarDate);
                    m_LunarYearAnimal = "鼠牛虎兔龙蛇马羊猴鸡狗猪".Substring((sexagenaryYear - 1) % 12, 1);
                }
                return m_LunarYearAnimal;
            }
        }

        public string LunarYearText
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarYearText))
                {
                    m_LunarYearText = "鼠牛虎兔龙蛇马羊猴鸡狗猪".Substring(calendar.GetSexagenaryYear(new DateTime(m_LunarYear, 1, 1)) % 12 - 1, 1);
                    StringBuilder stringBuilder = new StringBuilder();
                    int num = LunarYear;
                    do
                    {
                        int index = num % 10;
                        stringBuilder.Insert(0, "〇一二三四五六七八九"[index]);
                        num /= 10;
                    }
                    while (num > 0);
                    m_LunarYearText = stringBuilder.ToString();
                }
                return m_LunarYearText;
            }
        }

        public string LunarMonthText
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarMonthText))
                {
                    m_LunarMonthText = (IsLeapMonth ? "闰" : "") + ChineseMonthName[LunarMonth - 1];
                }
                return m_LunarMonthText;
            }
        }

        public string LunarDayText
        {
            get
            {
                if (string.IsNullOrEmpty(m_LunarDayText))
                {
                    m_LunarDayText = ChineseDayName[LunarDay - 1];
                }
                return m_LunarDayText;
            }
        }

        public ChineseCalendarInfo()
            : this(DateTime.Now.Date)
        {
        }

        public ChineseCalendarInfo(DateTime date)
        {
            m_SolarDate = date;
            LoadFromSolarDate();
        }

        private void LoadFromSolarDate()
        {
            m_IsLeapMonth = false;
            m_LunarYearSexagenary = null;
            m_LunarYearAnimal = null;
            m_LunarYearText = null;
            m_LunarMonthText = null;
            m_LunarDayText = null;
            m_SolarWeekText = null;
            m_SolarConstellation = null;
            m_SolarBirthStone = null;
            m_LunarYear = calendar.GetYear(m_SolarDate);
            m_LunarMonth = calendar.GetMonth(m_SolarDate);
            int leapMonth = calendar.GetLeapMonth(m_LunarYear);
            if (leapMonth == m_LunarMonth)
            {
                m_IsLeapMonth = true;
                m_LunarMonth--;
            }
            else if (leapMonth > 0 && leapMonth < m_LunarMonth)
            {
                m_LunarMonth--;
            }
            m_LunarDay = calendar.GetDayOfMonth(m_SolarDate);
            CalcConstellation(m_SolarDate, out m_SolarConstellation, out m_SolarBirthStone);
        }

        public static void CalcConstellation(DateTime date, out string constellation, out string birthstone)
        {
            int num = Convert.ToInt32(date.ToString("MMdd"));
            int num2;
            if (num >= 321 && num <= 419)
            {
                num2 = 0;
                goto IL_017a;
            }
            if (num >= 420 && num <= 520)
            {
                num2 = 1;
                goto IL_017a;
            }
            if (num >= 521 && num <= 621)
            {
                num2 = 2;
                goto IL_017a;
            }
            if (num >= 622 && num <= 722)
            {
                num2 = 3;
                goto IL_017a;
            }
            if (num >= 723 && num <= 822)
            {
                num2 = 4;
                goto IL_017a;
            }
            if (num >= 823 && num <= 922)
            {
                num2 = 5;
                goto IL_017a;
            }
            if (num >= 923 && num <= 1023)
            {
                num2 = 6;
                goto IL_017a;
            }
            if (num >= 1024 && num <= 1121)
            {
                num2 = 7;
                goto IL_017a;
            }
            if (num >= 1122 && num <= 1221)
            {
                num2 = 8;
                goto IL_017a;
            }
            if (num >= 1222 || num <= 119)
            {
                num2 = 9;
                goto IL_017a;
            }
            if (num >= 120 && num <= 218)
            {
                num2 = 10;
                goto IL_017a;
            }
            if (num >= 219 && num <= 320)
            {
                num2 = 11;
                goto IL_017a;
            }
            constellation = "未知星座";
            birthstone = "未知诞生石";
            return;
        IL_017a:
            constellation = Constellations[num2];
            birthstone = BirthStones[num2];
        }

        private static DateTime GetLunarNewYearDate(int year)
        {
            DateTime time = new DateTime(year, 1, 1);
            int year2 = calendar.GetYear(time);
            int month = calendar.GetMonth(time);
            int num = 0;
            int num2 = calendar.IsLeapYear(year2) ? 13 : 12;
            while (num2 >= month)
            {
                num += calendar.GetDaysInMonth(year2, num2--);
            }
            num = num - calendar.GetDayOfMonth(time) + 1;
            return time.AddDays((double)num);
        }

        public static DateTime GetDateFromLunarDate(int year, int month, int day, bool IsLeapMonth)
        {
            if (year < 1902 || year > 2100)
            {
                throw new Exception("只支持1902～2100期间的农历年");
            }
            if (month < 1 || month > 12)
            {
                throw new Exception("表示月份的数字必须在1～12之间");
            }
            if (day < 1 || day > calendar.GetDaysInMonth(year, month))
            {
                throw new Exception("农历日期输入有误");
            }
            int num = 0;
            int num2 = 0;
            int leapMonth = calendar.GetLeapMonth(year);
            num2 = (((leapMonth != month + 1 || !IsLeapMonth) && (leapMonth <= 0 || leapMonth > month)) ? (month - 1) : month);
            while (num2 > 0)
            {
                num += calendar.GetDaysInMonth(year, num2--);
            }
            return GetLunarNewYearDate(year).AddDays((double)(num + day - 1));
        }

        public static DateTime GetDateFromLunarDate(DateTime date, bool IsLeapMonth)
        {
            return GetDateFromLunarDate(date.Year, date.Month, date.Day, IsLeapMonth);
        }

        public static ChineseCalendarInfo FromLunarDate(int year, int month, int day, bool IsLeapMonth)
        {
            DateTime dateFromLunarDate = GetDateFromLunarDate(year, month, day, IsLeapMonth);
            return new ChineseCalendarInfo(dateFromLunarDate);
        }

        public static ChineseCalendarInfo FromLunarDate(DateTime date, bool IsLeapMonth)
        {
            return FromLunarDate(date.Year, date.Month, date.Day, IsLeapMonth);
        }

        public static ChineseCalendarInfo FromLunarDate(string date, bool IsLeapMonth)
        {
            Regex regex = new Regex("^\\d{7}(\\d)$");
            Match match = regex.Match(date);
            if (!match.Success)
            {
                throw new Exception("日期字符串输入有误！");
            }
            DateTime date2 = DateTime.Parse(string.Format("{0}-{1}-{2}", date.Substring(0, 4), date.Substring(4, 2), date.Substring(6, 2)));
            return FromLunarDate(date2, IsLeapMonth);
        }
    }
}
