#if UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.iOS;

public class NotificationInfo
{
    public int notificationId;
    public string title;
    public string text;
    public int day;
    public int hour;
    public int minute;
    public int second;
    public string smallIcon;
    public string largeIcon;
}

public class iOSNotificationSender : MonoBehaviour
{
    public static iOSNotificationSender instance;

    string smileEmoji = @"\U0001F60A😊,\U00002764❤️,\U0001F44D👍,\U0001F448👈,\U0001F449👉,\U0001F40E🐎,\U0001F3C6🏆,\U0001F339🌹,\U0001F389🎉,\U00002B50⭐";
    private  List<NotificationInfo> _notificationInfos= new List<NotificationInfo>();
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;

    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    private void initData() 
    {
        pushNotification(1, "快樂星期天", "玩三國華容道，越玩越聰明哦。🌟", 1, 12, 30);
        pushNotification(2, "周一，新的一周開始啦", "三國華容道，主公，你能幫曹操脫困嗎？🐎", 2, 12, 30);
        pushNotification(3, "周二，要元氣滿滿哦", "三大不可思議智力遊戲之一：三國華容道，越玩越聰明。💪", 3, 12, 30);
        pushNotification(4, "周三，祝你好運連連", "老少鹹宜，每天都玩一玩三國華容道，可以提高專註力與邏輯推理能哦。☘️", 4, 12, 30);
        pushNotification(5, "周四，開心快樂每一天", "朋友玩三國華容道，智商已經從80提高到120+。🎓", 5, 12, 30);
        pushNotification(6, "周五，心情愉快", "你知道在三國華容道中，關羽為什麼要放走曹操嗎？😊", 6, 12, 30);
        pushNotification(7, "周六，周末愉快哦", "安排關羽去守華容道的是諸葛孔明先生。🌹", 7, 12, 30);


        /*
        //测试通知
        pushNotification(713, "三国华容道713", "玩三国华容道，越玩越聪明哦。", 7, 12, 30);
        pushNotification(714, "三国华容道714", "玩三国华容道，越玩越聪明哦。", 7, 12, 40);
        pushNotification(715, "三国华容道715", "玩三国华容道，越玩越聪明哦。", 7, 12, 50);
        pushNotification(716, "三国华容道716", "玩三国华容道，越玩越聪明哦。", 7, 12, 58);//不存在60分


        pushNotification(71300, "三国华容道71300", "玩三国华容道，越玩越聪明哦。", 7, 13, 0);
        pushNotification(71310, "三国华容道71310", "玩三国华容道，越玩越聪明哦。", 7, 13, 10);
        pushNotification(71320, "三国华容道71320", "玩三国华容道，越玩越聪明哦。", 7, 13, 20);
        pushNotification(71330, "三国华容道71330", "玩三国华容道，越玩越聪明哦。", 7, 13, 30);
        pushNotification(71340, "三国华容道7130", "玩三国华容道，越玩越聪明哦。", 7, 13, 40);
        pushNotification(71350, "三国华容道71310", "玩三国华容道，越玩越聪明哦。", 7, 13, 50);
        pushNotification(71360, "三国华容道71320", "玩三国华容道，越玩越聪明哦。", 7, 13, 59); //不存在60分
       

        pushNotification(72, "三国华容道72", "玩三国华容道可以提高专注力与逻辑推理能哦。", 7, 13, 30);
        pushNotification(73, "三国华容道73", "三国华容道，主公，你能帮曹操脱困吗？", 7, 14, 30);
        pushNotification(74, "三国华容道74", "三大不可思议智力游戏之一：三国华容道，越玩越聪明。", 7, 15, 30);
        pushNotification(75, "三国华容道75", "老少咸宜，每天都玩一玩三国华容道。", 7, 16, 30);
        pushNotification(76, "三国华容道76", "朋友玩三国华容道，智商已经提高到120+。", 7, 17, 30);
        pushNotification(77, "三国华容道77", "你知道在三国华容道中，曹操和关羽是什么关系吗？", 7, 18, 30);
        pushNotification(78, "三国华容道78", "玩三国华容道，越玩越聪明哦。", 7, 19, 30);
        pushNotification(79, "三国华容道79", "玩三国华容道可以提高专注力与逻辑推理能哦。", 7, 20, 30);
        pushNotification(80, "三国华容道80", "三国华容道，主公，你能帮曹操脱困吗？", 7, 21, 30);
        pushNotification(81, "三国华容道81", "三大不可思议智力游戏之一：三国华容道，越玩越聪明。", 7, 22, 30);
        pushNotification(82, "三国华容道82", "老少咸宜，每天都玩一玩三国华容道。", 7, 23, 30);
        pushNotification(83, "三国华容道83", "朋友玩三国华容道，智商已经提高到120+。", 7, 23, 59);
        */

    }



    /// <summary>
    /// 压入通知数据
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="smallIconId"></param>
    /// <param name="largeIconId"></param>
    public void pushNotification(int nId,string title, string text, int day, int hour, int minute, string smallIconId = null, string largeIconId = null)
    {

        NotificationInfo notificationInfo = new NotificationInfo()
        {
            notificationId= nId,
            title = title,
            text = text,
            day = day,
            hour = hour,
            minute = minute,
            smallIcon = smallIconId,
            largeIcon = largeIconId
        };
        _notificationInfos.Add(notificationInfo);


    }



    public void requestNotification() 
    {
        StartCoroutine(RequestAuthorization());
    }

    IEnumerator RequestAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization:";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(res);

            //获得授权
            if (req.Granted)
            {
                ResetNotificationChannel();

                initData();

                ReSendNotification();
            }
        }
    }


    private  void ResetNotificationChannel()
    {
      
        iOSNotificationCenter.ApplicationBadge = 0;
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.RemoveAllScheduledNotifications();
    }

    protected  void ReSendNotification()
    {
        if (_notificationInfos != null && _notificationInfos.Count > 0)
        {
            Debug.Log("_notificationInfos:" + _notificationInfos.Count);
            for (var i = 0; i < _notificationInfos.Count; i++)
             {
                 SendNotification(_notificationInfos[i]);
             }
        }

    }




    private  void SendNotification(NotificationInfo notificationInfo)//string title, string text,TimeSpan timeInterval)
    {

        iOSNotificationCalendarTrigger calendarTrigger = new iOSNotificationCalendarTrigger()
        {
            Day = notificationInfo.day,
            Hour = notificationInfo.hour,
            Minute = notificationInfo.minute,
            UtcTime = false,
            Repeats = true
        };

        iOSNotification notification = new iOSNotification()
        {
            Identifier = "_notification_" + notificationInfo.notificationId,
            Title = notificationInfo.title,
            Body = notificationInfo.text,
            Badge = 1,
            ShowInForeground = false,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = calendarTrigger,
        };

        Debug.Log("SendNotification:Id:"+ notificationInfo.notificationId+ " day:"+ notificationInfo.day + " "+notificationInfo.title+"\n" + notificationInfo.text);
        iOSNotificationCenter.ScheduleNotification(notification);
    }

    

    

}
#endif