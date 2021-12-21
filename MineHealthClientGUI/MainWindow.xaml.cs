using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MineHealthDLL_NetFrame;

namespace MineHealthClientGUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            UIUpdate();
        }

        private void UIUpdate()
        {
            if (MHAPI.IsConnected == false)
            {
                button_connect.Content = "연결";
                button_login.IsEnabled = false;
                textbox_loginphone.IsEnabled = false;
                textbox_loginpassword.IsEnabled = false;
            }
            else
            {
                button_connect.Content = "연결해제";
                button_login.IsEnabled = true;
                textbox_loginphone.IsEnabled = true;
                textbox_loginpassword.IsEnabled = true;
            }
        }

        private void button_connect_Click(object sender, RoutedEventArgs e)
        {
            if (MHAPI.IsConnected == false)
            {
                //MHAPI.Connect("211.104.146.87", 9090);
                WriteMessage(MHAPI.Connect("127.0.0.1", 9090));
            }
            else
            {
                MHAPI.DisConnect();
            }

            UIUpdate();
        }

        private void button_login_Click(object sender, RoutedEventArgs e)
        {
            string tempPhone = textbox_loginphone.Text;
            string tempPswd = textbox_loginpassword.Text;

            if (string.IsNullOrEmpty(tempPhone) || string.IsNullOrEmpty(tempPswd))
            {
                WriteMessage("핸드폰번호와 비밀번호를 올바르게 기입해 주세요.");
                return;
            }

            var result = MHAPI.GetLogIn(tempPhone, tempPswd);

            string msg = "";
            switch(result)
            {
                case 0:
                    msg = "로그인에 성공하였습니다.";
                    break;
                case 1:
                    msg = "비밀번호가 올바르지 않습니다.";
                    break;
                case 2:
                    msg = "등록되지 않은 핸드폰번호 입니다.";
                    break;
                default:
                    msg = "시스템 오류";
                    break;
            }

            WriteMessage(msg);
        }

        private void button_send_Click(object sender, RoutedEventArgs e)
        {
            string strtemp = textbox_send.Text;
            if (string.IsNullOrEmpty(strtemp))
                return;
            textbox_message.Text += "<<" + strtemp + Environment.NewLine;
            textbox_send.Text = string.Empty;
            MHAPI.Command(strtemp);
        }

        private void textbox_send_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_send_Click(null, null);
        }
        private void WriteMessage(string msg)
        {
            textbox_message.Text += ">>" + msg + Environment.NewLine;
        }
        private void button_signin_Click(object sender, RoutedEventArgs e)
        {
            var genderTemp = MHAPI.Gender.M;
            if (textbox_signingender.Text == "M")
                genderTemp = MHAPI.Gender.M;
            else if (textbox_signingender.Text == "F")
                genderTemp = MHAPI.Gender.F;
            else
            {
                WriteMessage("성별을 기입해주세요. M or F");
                return;
            }

            var tempphone = textbox_signinphone.Text;
            var temppassword = textbox_signinpassword.Text;
            var tempbirth = textbox_signinbirthday.Text;
            var tempnickname = textbox_signinnickname.Text;

            if (string.IsNullOrEmpty(tempphone) || string.IsNullOrEmpty(temppassword) || string.IsNullOrEmpty(tempbirth) || string.IsNullOrEmpty(tempnickname))
            {
                WriteMessage("정보를 올바르게 기입해 주세요.");
                return;
            }

            var result = MHAPI.GetSignIn(tempphone, temppassword, tempbirth, tempnickname, genderTemp);


            string msg = "";
            switch (result)
            {
                case 0:
                    msg = "회원가입에 성공하였습니다.";
                    break;
                case 1:
                    msg = "비밀번호가 올바르지 않습니다.";
                    break;
                case 2:
                    msg = "이미 등록된 핸드폰번호 입니다.";
                    break;
                default:
                    msg = "시스템 오류";
                    break;
            }

            WriteMessage(msg);
        }
        private void button_userinfo_Click(object sender, RoutedEventArgs e)
        {
            var tempphone = textbox_userinfo.Text;

            if (string.IsNullOrEmpty(tempphone))
            {
                WriteMessage("정보를 올바르게 기입해 주세요.");
                return;
            }

            var result = MHAPI.GetUserInfo(tempphone);
            string msg = "";
            switch (result)
            {
                case "-1":
                    msg = "시스템 오류";
                    break;
                case "1":
                    msg = "등록되지 않은 핸드폰번호입니다.";
                    break;
                default:
                    msg = result;
                    break;
            }
            WriteMessage(msg);
        }

        private void button_refresh_Click(object sender, RoutedEventArgs e)
        {
            if (MHAPI.IsConnected)
            {
                string strtemp = MHAPI.Refresh();
                if (string.IsNullOrEmpty(strtemp))
                {
                    WriteMessage(strtemp);
                }
            }
        }
    }
}
