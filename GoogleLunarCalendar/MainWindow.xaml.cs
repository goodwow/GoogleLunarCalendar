using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.IO;

namespace GoogleLunarCalendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ChineseLunisolarCalendar chineseDate = new ChineseLunisolarCalendar();

        public MainWindow()
        {
            InitializeComponent();

            dtpKSSJ.SelectedDate = DateTime.Now;
            dtpJSSJ.SelectedDate = DateTime.Now;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtHDNR.Text))
                {
                    MessageBox.Show("请填写内容");
                }
                else
                {
                    string trigger = string.Format("P0DT{0}H{1}M0S", txtTXXS.Text.Trim(), txtTXFZ.Text.Trim());
                    DateTime dtKSSJ = dtpKSSJ.SelectedDate.Value;
                    DateTime dtJSSJ = dtpJSSJ.SelectedDate.Value;
                    if (dtJSSJ < dtKSSJ)
                    {
                        MessageBox.Show("结束日期应该晚于开始日期");
                        return;
                    }

                    StreamWriter streamWriter = new StreamWriter(txtHDNR.Text + ".ics", false, Encoding.UTF8);
                    streamWriter.WriteLine("BEGIN:VCALENDAR");
                    streamWriter.WriteLine("PRODID:-//Google Inc//Google Calendar 70.9054//EN");
                    streamWriter.WriteLine("VERSION:2.0");
                    while (true)
                    {
                        DateTime dateTime = ChineseCalendarInfo.FromLunarDate(dtKSSJ, chineseDate.IsLeapMonth(dtKSSJ.Year, dtKSSJ.Month)).SolarDate;
                        string strKSSJ = dateTime.ToString("yyyyMMdd");
                        dateTime = dateTime.AddDays(1.0);
                        string strJSSJ = dateTime.ToString("yyyyMMdd");

                        streamWriter.WriteLine("BEGIN:VEVENT");
                        streamWriter.WriteLine("DTSTART;VALUE=DATE:" + strKSSJ + "");
                        streamWriter.WriteLine("DTEND;VALUE=DATE:" + strJSSJ + "");
                        //streamWriter.WriteLine("CLASS:PRIVATE");
                        streamWriter.WriteLine("DESCRIPTION:" + txtHDSM.Text + "");
                        streamWriter.WriteLine("LOCATION:" + txtHDDD.Text + "");
                        streamWriter.WriteLine("SEQUENCE:0");
                        streamWriter.WriteLine("STATUS:CONFIRMED");
                        streamWriter.WriteLine("SUMMARY:" + txtHDNR.Text + "");
                        streamWriter.WriteLine("TRANSP:TRANSPARENT");
                        if (ckSFTX.IsChecked.Value)
                        {
                            streamWriter.WriteLine("BEGIN:VALARM");
                            streamWriter.WriteLine("ACTION:DISPLAY");
                            streamWriter.WriteLine("DESCRIPTION:");
                            streamWriter.WriteLine("TRIGGER:" + trigger);
                            streamWriter.WriteLine("END:VALARM");
                        }
                        streamWriter.WriteLine("END:VEVENT");
                        if (cboCFPL.SelectedValue != null && (cboCFPL.SelectedValue as ComboBoxItem).Content.ToString() == "每月")
                        {
                            dtKSSJ = dtKSSJ.AddMonths(1);
                        }
                        else
                        {
                            dtKSSJ = dtKSSJ.AddYears(1);
                        }
                        if (!(dtKSSJ <= dtJSSJ))
                        {
                            break;
                        }
                    }
                    streamWriter.WriteLine("END:VCALENDAR");
                    streamWriter.Flush();
                    streamWriter.Close();
                    MessageBox.Show("生成成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            spHDTX.Visibility = ckSFTX.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
