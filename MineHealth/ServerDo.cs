using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Utility;
using System.Threading;

namespace MineHealth
{
    public class ServerDo
    {
        private TcpClient client;
        private int clientNo;
        string strMsg;
        public event EventHandler ClientExit;
        public ServerDo(TcpClient client, int clientno)
        {
            this.client = client;
            this.clientNo = clientno;
        }

        public void DoStart()
        {
            Running();
        }

        private void Running()
        {
            NetworkStream ns = client.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            ns.ReadTimeout = 10000;

            string welcome = "Server Connnect Success!";
            sw.WriteLine(welcome);
            sw.Flush();
            try
            {

                while (true)
                {
                    strMsg = sr.ReadLine();

                    if (string.IsNullOrEmpty(strMsg))
                    {
                        ClientExit?.Invoke(client, EventArgs.Empty);
                        break;
                    }

                    Console.WriteLine("ME: {0}", strMsg);
                    if (strMsg == "exit")  //exit 메시지 수신시 종료하기
                    {
                        ClientExit?.Invoke(client, EventArgs.Empty);
                        break;
                    }

                    // 로그인 여부, LOGIN [핸드폰번호],[비밀번호]
                    if (strMsg.Contains("LOGIN "))
                    {
                        strMsg = strMsg.Replace("LOGIN ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 2)
                        {
                            var tempPhone = splitStr[0];
                            var tempPassword = splitStr[1];
                            var value = SQLHelper.RequestUserPswd(tempPhone);

                            if (string.IsNullOrWhiteSpace(value))
                            {
                                strMsg = "Failed,1";    // 실패, 등록되지 않은 핸드폰번호
                            }
                            else if (tempPassword == value)
                            {
                                strMsg = "LOGIN OK";    // 성공
                            }
                            else
                            {
                                strMsg = "Failed,2";    // 실패, 비밀번호가 다름.
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";        // 실패, 핸드폰번호,비밀번호가 모두 입력되지 않음.
                        }
                    }

                    // 회원가입, SIGNIN [핸드폰번호],[비밀번호],[생년월일],[닉네임],[성별]
                    else if (strMsg.Contains("SIGNIN "))
                    {
                        strMsg = strMsg.Replace("SIGNIN ", "");
                        var splitStr = strMsg.Split(',');
                        if (string.IsNullOrEmpty(splitStr[0]) || string.IsNullOrEmpty(splitStr[1]) || string.IsNullOrEmpty(splitStr[2]) || string.IsNullOrEmpty(splitStr[3]) || string.IsNullOrEmpty(splitStr[4]))
                        {
                            strMsg = "Failed,3";
                        }
                        else if (SQLHelper.CheckDuplicationPhone(splitStr[0]) > 0)
                        {
                            strMsg = "Failed,1";   // 실패, 이미 등록된 핸드폰번호
                        }
                        else if (splitStr.Length == 5)
                        {
                            var temp = SQLHelper.RequestSignIn(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]);
                            if (temp != -1)
                            {
                                strMsg = "SIGNIN OK";
                            }
                            else
                            {
                                strMsg = "Failed,-1";
                            }
                        }
                        else
                        {
                            strMsg = "Failed, 3";
                        }
                    }

                    // 중복체크, DUPLICATION [핸드폰번호]
                    else if (strMsg.Contains("DUPLICATION "))
                    {
                        strMsg = strMsg.Replace("DUPLICATION ", "");
                        var value = SQLHelper.CheckDuplicationPhone(strMsg);

                        if (string.IsNullOrEmpty(strMsg))
                        {
                            strMsg = "Failed,1";    // 올바르지않은 핸드폰번호 입력
                        }
                        if (value == 0)
                        {
                            strMsg = "NOT Duplicate";   // 중복 아님
                        }
                        else if (value == -1)
                        {
                            strMsg = "Failed,-1";
                        }
                        else
                        {
                            strMsg = "Duplicate";       // 중복
                        }
                    }

                    // 비밀번호변경, CHANGEPWD [핸드폰번호],[비밀번호],[새로운비밀번호]
                    else if (strMsg.Contains("CHANGEPWD "))
                    {
                        strMsg = strMsg.Replace("CHANGEPWD ", "");
                        var splitStr = strMsg.Split(',');
                        if (SQLHelper.CheckDuplicationPhone(splitStr[0]) == 0)
                        {
                            strMsg = "Failed,1";   // 실패, 등록되지 않은 핸드폰번호
                        }
                        else if (splitStr.Length == 3)
                        {
                            if (splitStr[1] != SQLHelper.RequestUserPswd(splitStr[0]))
                            {
                                strMsg = "Failed,2";
                            }
                            else if (splitStr[1] == splitStr[2])
                            {
                                strMsg = "Failed,3";
                            }
                            else
                            {
                                if (SQLHelper.RequestChangePassword(splitStr[0], splitStr[1], splitStr[2]) == -1)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else
                                {
                                    strMsg = "CHANGEPWD OK";
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,4";
                        }
                    }

                    // 유저정보수정, UPDATEUSERINFO [핸드폰번호],[비밀번호],[생년월일],[닉네임],[성별]
                    else if (strMsg.Contains("USERINFOUPDATE "))
                    {
                        strMsg = strMsg.Replace("USERINFOUPDATE ", "");
                        var splitStr = strMsg.Split(',');
                        if (SQLHelper.CheckDuplicationPhone(splitStr[0]) == 0)
                        {
                            strMsg = "Failed,1";   // 실패, 등록되지 않은 핸드폰번호
                        }
                        else if (splitStr.Length == 5)
                        {
                            if (splitStr[1] != SQLHelper.RequestUserPswd(splitStr[0]))
                            {
                                strMsg = "Failed,2";    // 실패, 일치하지 않는 비밀번호
                            }
                            else
                            {
                                var value = SQLHelper.RequestUpdateUserInfo(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]);
                                if (value == -1)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else
                                {
                                    strMsg = "UPDATEUSERINFO OK";
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";
                        }
                    }

                    // 유저정보조회, USERINFO [핸드폰번호],[비밀번호]
                    else if (strMsg.Contains("USERINFO "))
                    {
                        strMsg = strMsg.Replace("USERINFO ", "");
                        var splitStr = strMsg.Split(',');
                        if (SQLHelper.CheckDuplicationPhone(splitStr[0]) == 0)
                        {
                            strMsg = "Failed,1";   // 실패, 등록되지 않은 핸드폰번호
                        }
                        else if (splitStr.Length == 2)
                        {
                            if (splitStr[1] != SQLHelper.RequestUserPswd(splitStr[0]))
                            {
                                strMsg = "Failed,2";
                            }
                            else
                            {
                                var value = SQLHelper.RequestUserInfo(splitStr[0], splitStr[1]);
                                if (value == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (value == string.Empty)
                                {
                                    strMsg = "Failed,3";
                                }
                                else
                                {
                                    strMsg = value;
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,4";
                        }
                    }

                    // 유저검사일조회, USERTESTDATE [핸드폰번호],[조회개수]
                    else if (strMsg.Contains("USERTESTDATE "))
                    {
                        strMsg = strMsg.Replace("USERTESTDATE ", "");
                        var splitStr = strMsg.Split(',');
                        if (SQLHelper.CheckDuplicationPhone(splitStr[0]) == 0)
                        {
                            strMsg = "Failed,1";   // 실패, 등록되지 않은 핸드폰번호
                        }
                        else if (splitStr.Length == 2)
                        {
                            var value = SQLHelper.RequestTestDateTime(splitStr[0], splitStr[1]);

                            if (value == null)
                            {
                                strMsg = "Failed,-1";
                            }
                            else
                            {
                                strMsg = value;
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }

                    // 유저TESTID조회, USERTESTID [핸드폰번호],[검사일]
                    else if (strMsg.Contains("USERTESTID "))
                    {
                        strMsg = strMsg.Replace("USERTESTID ", "");
                        var splitStr = strMsg.Split(',');
                        if (SQLHelper.CheckDuplicationPhone(splitStr[0]) == 0)
                        {
                            strMsg = "Failed,1";   // 실패, 등록되지 않은 핸드폰번호
                        }
                        else if (splitStr.Length == 2)
                        {
                            var value = SQLHelper.RequestTestId(splitStr[0], splitStr[1]);
                            if (value == null)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (value == "")
                            {
                                strMsg = "Failed,2";    // 데이터가 존재하지 않음.
                            }
                            else
                            {
                                strMsg = value;
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }

                    // 유저 점수 조회, USERSCORE [카테고리],[TESTID]
                    else if (strMsg.Contains("USERSCORE "))
                    {
                        strMsg = strMsg.Replace("USERSCORE ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 2)
                        {
                            if (new List<string> { "QA", "QB", "QC", "PA", "PB" }.Contains(splitStr[0]) == false)
                            {
                                strMsg = "Failed,1";   // 실패, 등록되지 않은 카테고리
                            }
                            else
                            {
                                var value = SQLHelper.RequestScore(splitStr[0], splitStr[1]);
                                if (value == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (value == "")
                                {
                                    strMsg = "Failed,2";
                                }
                                else
                                {
                                    strMsg = value;
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }

                    // 유저 레벨 조회, USERLEVEL [카테고리],[TESTID]
                    else if (strMsg.Contains("USERLEVEL "))
                    {
                        strMsg = strMsg.Replace("USERLEVEL ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 2)
                        {
                            if (new List<string> { "QA", "QB", "QC", "PA", "PB" }.Contains(splitStr[0]) == false)
                            {
                                strMsg = "Failed,1";   // 실패, 등록되지 않은 카테고리
                            }
                            else
                            {
                                var value = SQLHelper.RequestLevel(splitStr[0], splitStr[1]);
                                if (value == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (value == "")
                                {
                                    strMsg = "Failed,2";
                                }
                                else
                                {
                                    strMsg = value;
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }

                    // 유저 코멘트 조회, USERCOMMENT [카테고리],[TESTID]
                    else if (strMsg.Contains("USERCOMMENT "))
                    {
                        strMsg = strMsg.Replace("USERCOMMENT ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 2)
                        {
                            if (new List<string> { "QA", "QB", "QC", "PA", "PB" }.Contains(splitStr[0]) == false)
                            {
                                strMsg = "Failed,1";   // 실패, 등록되지 않은 카테고리
                            }
                            else
                            {
                                var value = SQLHelper.RequestComment(splitStr[0], splitStr[1]);
                                if (value == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (value == "")
                                {
                                    strMsg = "Failed,2";
                                }
                                else
                                {
                                    strMsg = value;
                                }
                            }

                        }
                        else
                        {
                            strMsg = "Failed,3";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }

                    // 유저 추천 링크 조회, USERLINK [카테고리],[TESTID]
                    else if (strMsg.Contains("USERLINK "))
                    {
                        strMsg = strMsg.Replace("USERLINK ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 2)
                        {
                            if (new List<string> { "MINE", "HEALTH" }.Contains(splitStr[0]) == false)
                            {
                                strMsg = "Failed,1";   // 실패, 등록되지 않은 카테고리
                            }
                            else
                            {
                                var value = SQLHelper.RequestPersonalLink(splitStr[0], splitStr[1]);
                                if (value == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (value == "")
                                {
                                    strMsg = "Failed,2";
                                }
                                else
                                {
                                    strMsg = value;
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";   // 실패, 모든 정보가 입력되지 않음.
                        }

                    }

                    // 전체 링크 조회, ALLLINK [카테고리]
                    else if (strMsg.Contains("ALLLINK "))
                    {
                        strMsg = strMsg.Replace("ALLLINK ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 1)
                        {
                            if (new List<string> { "MINE", "HEALTH" }.Contains(splitStr[0]) == false)
                            {
                                strMsg = "Failed,1";   // 실패, 등록되지 않은 카테고리
                            }
                            else
                            {
                                var value = SQLHelper.RequestAllLink(splitStr[0]);
                                if (value == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (value == "")
                                {
                                    strMsg = "Failed,2";
                                }
                                else
                                {
                                    strMsg = value;
                                }
                            }
                        }
                        else
                        {
                            strMsg = "Failed,3";   // 실패, 모든 정보가 입력되지 않음.
                        }

                    }

                    // 유저 테스트 기록 입력, USERLOGINPUT [핸드폰번호],[검사일],[테스트장소]
                    else if (strMsg.Contains("USERLOGINPUT "))
                    {
                        strMsg = strMsg.Replace("USERLOGINPUT ", "");
                        var splitStr = strMsg.Split(',');

                        if (splitStr.Length == 3)
                        {
                            var value = SQLHelper.InsertUserLog(splitStr[0], splitStr[1], splitStr[2]);
                            if (value == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (value == 1)
                            {
                                var testIDValue = SQLHelper.RequestTestId(splitStr[0], splitStr[1]);

                                SQLHelper.InsertQuestion("QA", testIDValue, null, "0");
                                SQLHelper.InsertQuestion("QB", testIDValue, null, "0");
                                SQLHelper.InsertQuestion("QC", testIDValue, null, "0");
                                SQLHelper.InsertEmotion("EA", testIDValue, "");
                                SQLHelper.InsertEmotion("EB", testIDValue, "");
                                SQLHelper.InsertEmotion("EC", testIDValue, "");
                                SQLHelper.InsertEmotion("ED", testIDValue, "");

                                SQLHelper.InsertPose("PA", testIDValue, null, "0");
                                SQLHelper.InsertPose("PB", testIDValue, null, "0");

                                if (testIDValue == null)
                                {
                                    strMsg = "Failed,-1";
                                }
                                else if (testIDValue == "")
                                {
                                    strMsg = "Failed,3";    // 데이터가 존재하지 않음.
                                }
                                else
                                {
                                    strMsg = testIDValue;
                                }
                            }
                            else if (value == 0)
                            {
                                strMsg = "Failed,1";    // 데이터가 입력되지 않음.
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }

                    }
                    // 질문A 테스트 기록 입력, QAINSERT [TESTID],[Answer0~9],[Score]
                    else if (strMsg.Contains("QAINSERT "))
                    {
                        strMsg = strMsg.Replace("QAINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 12)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 10; i++)
                            {
                                list.Add(splitStr[i + 1]);
                            }
                            var tempResult = SQLHelper.UpdateQuestion("QA", splitStr[0], list, splitStr[11]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "QAINSERT OK";
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }
                    // 질문B 테스트 기록 입력, QBINSERT [TESTID],[Answer0~6],[Score]
                    else if (strMsg.Contains("QBINSERT "))
                    {
                        strMsg = strMsg.Replace("QBINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 9)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 7; i++)
                            {
                                list.Add(splitStr[i + 1]);
                            }
                            var tempResult = SQLHelper.UpdateQuestion("QB", splitStr[0], list, splitStr[8]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "QBINSERT OK";
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }
                    // 질문C 테스트 기록 입력, QCINSERT [TESTID],[Answer0~9],[Score]
                    else if (strMsg.Contains("QCINSERT "))
                    {
                        strMsg = strMsg.Replace("QCINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 12)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 10; i++)
                            {
                                list.Add(splitStr[i + 1]);
                            }
                            var tempResult = SQLHelper.UpdateQuestion("QC", splitStr[0], list, splitStr[11]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "QCINSERT OK";
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }
                    // 자세A 테스트 기록 입력, PAINSERT [TESTID],[POINT0~33],[SavePath],[Score]
                    else if (strMsg.Contains("PAINSERT "))
                    {
                        strMsg = strMsg.Replace("PAINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 71)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 68; i = i + 2)
                            {
                                list.Add(splitStr[i + 1] + "," +splitStr[i + 2]);
                            }
                            string cal_score = Cal_YAData(list);
                            var tempResult = SQLHelper.UpdatePose("PA", splitStr[0], list, splitStr[69], cal_score);
                            SQLHelper.SaveScoreDatas(splitStr[0], SQLHelper.ScoreCategory.FrontAngle, cal_score);
                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "PAINSERT OK";
                            }
                        }
                        else if (splitStr.Length == 67)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 64; i = i + 2)
                            {
                                list.Add(splitStr[i + 1] + "," + splitStr[i + 2]);
                            }
                            list.Add("<0, 0>");
                            list.Add("<0, 0>");
                            var tempResult = SQLHelper.UpdatePose("PA", splitStr[0], list, splitStr[65], splitStr[66]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "PAINSERT OK";
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }
                    // 자세B 테스트 기록 입력, PBINSERT [TESTID],[POINT0~33],[SavePath],[SCORE]
                    else if (strMsg.Contains("PBINSERT "))
                    {
                        strMsg = strMsg.Replace("PBINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 71)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 68; i = i + 2)
                            {
                                list.Add(splitStr[i + 1] + "," + splitStr[i + 2]);
                            }

                            string cal_score_neck = Cal_TNData(list);
                            string cal_score_spine = Cal_RTData(list);

                            var tempResult = SQLHelper.UpdatePose("PB", splitStr[0], list, splitStr[69], $"{cal_score_neck}/{cal_score_spine}");

                            SQLHelper.SaveScoreDatas(splitStr[0], SQLHelper.ScoreCategory.SideNeck, cal_score_neck);
                            SQLHelper.SaveScoreDatas(splitStr[0], SQLHelper.ScoreCategory.SideAngle, cal_score_spine);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "PBINSERT OK";
                            }
                        }
                        else if (splitStr.Length == 67)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < 64; i = i + 2)
                            {
                                list.Add(splitStr[i + 1] + "," + splitStr[i + 2]);
                            }
                            list.Add("<0, 0>");
                            list.Add("<0, 0>");
                            var tempResult = SQLHelper.UpdatePose("PB", splitStr[0], list, splitStr[65], splitStr[66]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "PBINSERT OK";
                            }
                        }
                        else
                        {
                            strMsg = "Failed,2";   // 실패, 모든 정보가 입력되지 않음.
                        }
                    }

                    else if (strMsg.Contains("EAINSERT "))
                    {
                        strMsg = strMsg.Replace("EAINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 2)
                        {
                            var tempResult = SQLHelper.UpdateEmotion("EA", splitStr[0], splitStr[1]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "EAINSERT OK";
                            }
                        }

                    }

                    else if (strMsg.Contains("EBINSERT "))
                    {
                        strMsg = strMsg.Replace("EBINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 2)
                        {
                            var tempResult = SQLHelper.UpdateEmotion("EB", splitStr[0], splitStr[1]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "EBINSERT OK";
                            }
                        }

                    }

                    else if (strMsg.Contains("ECINSERT "))
                    {
                        strMsg = strMsg.Replace("ECINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 2)
                        {
                            var tempResult = SQLHelper.UpdateEmotion("EC", splitStr[0], splitStr[1]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "ECINSERT OK";
                            }
                        }

                    }

                    else if (strMsg.Contains("EDINSERT "))
                    {
                        strMsg = strMsg.Replace("EDINSERT ", "");
                        var splitStr = strMsg.Split(',');
                        if (splitStr.Length == 2)
                        {
                            var tempResult = SQLHelper.UpdateEmotion("ED", splitStr[0], splitStr[1]);

                            if (tempResult == -1)
                            {
                                strMsg = "Failed,-1";
                            }
                            else if (tempResult == 0)
                            {
                                strMsg = "Failed,1";
                            }
                            else if (tempResult == 1)
                            {
                                strMsg = "EDINSERT OK";
                            }
                        }

                    }

                    SendMessage(strMsg);
                    sw.WriteLine(strMsg);
                    sw.Flush();
                }
                sw.Close();
                sr.Close();
                ns.Close();

                sw.Dispose();
                sr.Dispose();
                ns.Dispose();
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
                ClientExit?.Invoke(client, EventArgs.Empty);
            }
            SendMessage("Client 연결 종료!");
        }

        private string Cal_TNData(List<string> datas)
        {
            string result = "";

            string tempA = datas[(int)JointId.EarLeft].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsA = tempA.Split(",");

            string tempB = datas[(int)JointId.Neck].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsB = tempB.Split(",");


            if (tempsA.Length != 2 || tempsB.Length != 2)
                return result;


            double EarLeftX = Convert.ToDouble(tempsA[0]);
            double EarLeftY = Convert.ToDouble(tempsA[1]);

            double C7X = Convert.ToDouble(tempsB[0]);
            double C7Y = Convert.ToDouble(tempsB[1]);

            var xa = EarLeftX;
            var ya = EarLeftY;
            var xb = C7X;
            var yb = C7Y;
            var xc = EarLeftX;
            var yc = C7Y;

            var theta = Math.Atan2(yb - ya, xb - xa) - Math.Atan2(yb - yc, xb - xc);
            theta = theta * 180 / Math.PI;

            result = theta.ToString("0.00");

            return result;
        }
        private string Cal_YAData(List<string> datas)
        {
            string result = "";

            string tempA = datas[(int)JointId.ClavicleLeft].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsA = tempA.Split(",");

            string tempB = datas[(int)JointId.ClavicleRight].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsB = tempB.Split(",");

            string tempC = datas[(int)JointId.SpineChest].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsC = tempC.Split(",");

            string tempD = datas[(int)JointId.SpineNavel].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsD = tempD.Split(",");


            if (tempsA.Length != 2 || tempsB.Length != 2 || tempsC.Length != 2 || tempsD.Length != 2)
                return result;


            double EClavicleRightX = Convert.ToDouble(tempsA[0]);
            double EClavicleRightY = Convert.ToDouble(tempsA[1]);
            double EClavicleLeftX = Convert.ToDouble(tempsB[0]);
            double EClavicleLeftY = Convert.ToDouble(tempsB[1]);
            double SpineChestX = Convert.ToDouble(tempsC[0]);
            double SpineChestY = Convert.ToDouble(tempsC[1]);
            double SpineNavelX = Convert.ToDouble(tempsD[0]);
            double SpineNavelY = Convert.ToDouble(tempsD[1]);


            var Axa = EClavicleRightX;
            var Aya = EClavicleRightY;
            var Axb = SpineChestX;
            var Ayb = SpineChestY;
            var Axc = SpineNavelX;
            var Ayc = SpineNavelY;

            var Atheta = Math.Atan2(Ayb - Aya, Axb - Axa) - Math.Atan2(Ayb - Ayc, Axb - Axc);
            Atheta = Atheta * 180 / Math.PI;


            var Bxa = EClavicleLeftX;
            var Bya = EClavicleLeftY;
            var Bxb = SpineChestX;
            var Byb = SpineChestY;
            var Bxc = SpineNavelX;
            var Byc = SpineNavelY;

            var Btheta = Math.Atan2(Byb - Bya, Bxb - Bxa) - Math.Atan2(Byb - Byc, Bxb - Bxc);
            Btheta = 360 - Btheta * 180 / Math.PI;


            //result = $"{Atheta.ToString("0.00")}:{Btheta.ToString("0.00")}";
            result = Math.Abs(Atheta - Btheta).ToString("0.00");

            return result;
        }

        private string Cal_RTData(List<string> datas)
        {
            string result = "";

            string tempA = datas[(int)JointId.Neck].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsA = tempA.Split(",");

            string tempB = datas[(int)JointId.Pelvis].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsB = tempB.Split(",");

            string tempC = datas[(int)JointId.Sacrum].Replace("<", "").Replace("(", "").Replace(")", "").Replace(">", "");
            string[] tempsC = tempC.Split(",");

            if (tempsA.Length != 2 || tempsB.Length != 2 || tempsC.Length != 2)
                return result;

            double NeckX = Convert.ToDouble(tempsA[0]);
            //double NeckY = Convert.ToDouble(tempsA[1]);
            double PelvisX = Convert.ToDouble(tempsB[0]);
            //double PelvisY = Convert.ToDouble(tempsB[1]);
            double SacrumX = Convert.ToDouble(tempsC[0]);
            //double SacrumY = Convert.ToDouble(tempsC[1]);

            var c7_sacrum_length = NeckX - SacrumX;
            var pelvis_sacrum_length = PelvisX - SacrumX;
            var ratio = c7_sacrum_length / pelvis_sacrum_length;


            result = ratio.ToString("0.00");

            return result;
        }
        /* private void Running()
        {

            NetworkStream ns = client.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            string welcome = "Server Connnect Success!";
            sw.WriteLine(welcome);
            sw.Flush();
            while (true)
            {
                strMsg = sr.ReadLine();
                Console.WriteLine("me: {0}", strMsg);
                if (strMsg == "exit")  //exit 메시지 수신시 종료하기
                {
                    ClientExit?.Invoke(client, EventArgs.Empty);
                    break;
                }

                // 로그인 여부, LOGIN[핸드폰번호],[비밀번호]
                if (strMsg.Contains("LOGIN"))
                {
                    strMsg = strMsg.Replace("LOGIN", "");
                    var splitStr = strMsg.Split(',');
                    if (splitStr.Length == 2)
                    {
                        strMsg = SQLHelper.RequestUserPswd(splitStr[0]);
                        if (string.IsNullOrEmpty(strMsg))
                        {
                            strMsg = "Failed, 2";
                        }
                        else if (strMsg == splitStr[1])
                        {
                            strMsg = "OK";
                        }
                        else
                        {
                            strMsg = "Failed, 1";
                        }
                    }
                    else
                        strMsg = "Failed, -1";
                }

                // 회원가입: SIGNIN[핸드폰번호],[비밀번호],[생년월일],[닉네임],[성별]
                if (strMsg.Contains("SIGNIN"))
                {
                    strMsg = strMsg.Replace("SIGNIN", "");
                    var splitStr = strMsg.Split(',');
                    if (SQLHelper.CheckDuplicationPhone(splitStr[0]) > 0)
                    {
                        strMsg = "Failed, 2";
                    }
                    else if (splitStr.Length == 5)
                    {
                        var temp = SQLHelper.RequestSignIn(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]);
                        if (temp == -1)
                            strMsg = "Failed, -1";
                        else
                            strMsg = "OK";
                    }
                    else
                    {
                        strMsg = "Failed, 3";
                    }
                }

                // 유저정보 조회, USERINFO[핸드폰번호]
                if (strMsg.Contains("USERINFO"))
                {
                    strMsg = strMsg.Replace("USERINFO", "");
                    var strTemp = SQLHelper.RequestUserInfo(strMsg);
                    if (strTemp is null)
                        strMsg = "Failed, -1";
                    else if (strTemp == string.Empty)
                        strMsg = "Failed, 1";
                    else
                        strMsg = strTemp;
                }

                // 회원정보수정: UPDATE[핸드폰번호],[비밀번호],[생년월일],[닉네임],[성별]
                if (strMsg.Contains("UPDATE"))
                {
                    strMsg = strMsg.Replace("UPDATE", "");
                    var splitStr = strMsg.Split(',');

                    if (splitStr.Length == 5)
                    {
                        var temp = SQLHelper.RequestUpdateUserInfo(splitStr[0], splitStr[1], splitStr[2], splitStr[3], splitStr[4]);

                        if (temp == -1)
                            strMsg = "UPDATE Failed";
                        else
                            strMsg = "UPDATE Success.";
                    }
                    else
                    {
                        strMsg = "This is Incorrect Information.";
                    }
                }

                // 등록된 핸드폰번호 중복검사: DUPLICATION[핸드폰번호]
                if (strMsg.Contains("DUPLICATION"))
                {
                    strMsg = strMsg.Replace("DUPLICATION", "");
                    strMsg = "Count: " + SQLHelper.CheckDuplicationPhone(strMsg).ToString();
                }

                // 테스트 일자 호출: USERTESTDATE[핸드폰번호] 또는 USERTESTDATE[핸드폰번호],[조회 수]
                if (strMsg.Contains("USERTESTDATE"))
                {
                    strMsg = strMsg.Replace("USERTESTDATE", "");
                    var splitStr = strMsg.Split(',');
                    if (splitStr.Length == 2)
                        strMsg = SQLHelper.RequestTestDateTime(splitStr[0], Convert.ToInt32(splitStr[1])).ToString();
                    else if (splitStr.Length == 1)
                        strMsg = SQLHelper.RequestTestDateTime(splitStr[0]).ToString();
                    else
                        strMsg = "This is Incorrect Information.";
                }

                // 설문조사 스코어 호출: QSCORE설문번호,핸드폰번호,테스트일자
                if (strMsg.Contains("QSCORE"))
                {
                    strMsg = strMsg.Replace("QSCORE", "");
                    var splitStr = strMsg.Split(',');

                    if (splitStr.Length == 3)
                        strMsg = SQLHelper.RequestScoreQuestion(splitStr[0], splitStr[1], splitStr[2]);
                    else
                        strMsg = "Failed, -1";
                }

                SendMessage(strMsg);
                sw.WriteLine(strMsg);
                sw.Flush();
            }

            sw.Close();
            sr.Close();
            ns.Close();

            SendMessage("Client 연결 종료!");
        }
        */

        public void SendMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
                Console.WriteLine("client#{0}: {1}, {2}", clientNo, msg, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
        }
    }

    public enum JointId
    {
        //
        // 요약:
        //     Pelvis
        Pelvis = 0,
        //
        // 요약:
        //     Spine navel
        SpineNavel = 1,
        //
        // 요약:
        //     Spine chest
        SpineChest = 2,
        //
        // 요약:
        //     Neck
        Neck = 3,
        //
        // 요약:
        //     Left clavicle
        ClavicleLeft = 4,
        //
        // 요약:
        //     Left shoulder
        ShoulderLeft = 5,
        //
        // 요약:
        //     Left elbow
        ElbowLeft = 6,
        //
        // 요약:
        //     Left wrist
        WristLeft = 7,
        //
        // 요약:
        //     Left hand
        HandLeft = 8,
        //
        // 요약:
        //     Left hand tip
        HandTipLeft = 9,
        //
        // 요약:
        //     Left thumb
        ThumbLeft = 10,
        //
        // 요약:
        //     Right clavicle
        ClavicleRight = 11,
        //
        // 요약:
        //     Right shoulder
        ShoulderRight = 12,
        //
        // 요약:
        //     Right elbow
        ElbowRight = 13,
        //
        // 요약:
        //     Right wrist
        WristRight = 14,
        //
        // 요약:
        //     Right hand
        HandRight = 15,
        //
        // 요약:
        //     Right hand tip
        HandTipRight = 16,
        //
        // 요약:
        //     Right thumb
        ThumbRight = 17,
        //
        // 요약:
        //     Left hip
        HipLeft = 18,
        //
        // 요약:
        //     Left knee
        KneeLeft = 19,
        //
        // 요약:
        //     Left ankle
        AnkleLeft = 20,
        //
        // 요약:
        //     Left foot
        FootLeft = 21,
        //
        // 요약:
        //     Right hip
        HipRight = 22,
        //
        // 요약:
        //     Right knee
        KneeRight = 23,
        //
        // 요약:
        //     Right ankle
        AnkleRight = 24,
        //
        // 요약:
        //     Right foot
        FootRight = 25,
        //
        // 요약:
        //     Head
        Head = 26,
        //
        // 요약:
        //     Nose
        Nose = 27,
        //
        // 요약:
        //     Left eye
        EyeLeft = 28,
        //
        // 요약:
        //     Left ear
        EarLeft = 29,
        //
        // 요약:
        //     Right eye
        EyeRight = 30,
        //
        // 요약:
        //     Right ear
        EarRight = 31,
        //
        // 요약:
        //     Right ear
        Sacrum = 32,
        //
        // 요약:
        //     Right ear
        C7 = 33,
        //
        // 요약:
        //     Number of different joints defined in this enumeration.
        Count = 34
    }
}
