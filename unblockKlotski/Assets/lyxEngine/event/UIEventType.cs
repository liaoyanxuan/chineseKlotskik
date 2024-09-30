using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sw.game.evt
{
    public class UIEventType
    {
       

        public const string TAB_FUNCTION = "TAB_FUNCTION";//显示下载地图的loading

        //提示对象框的事件
		public const string ALERTPANEL_HINT_CONFIRM = "ALERTPANEL_HINT_CONFIRM";//提示对话框确认
		public const string ALERTPANEM_HINT_APPLY = "ALERTPANEM_HINT_APPLY"; //提示对话框应用
   

		public const string ALERTPANEL_EDIT_FINISH_CONFIRM = "ALERTPANEL_EDIT_FINISH_CONFIRM";//提示对话框确认
		public const string ALERTPANEL_EDIT_RENEW_CONFIRM = "ALERTPANEL_EDIT_RENEW_CONFIRM";//提示对话框确认


        public const string ALERTPANEL_CONFIRM = "ALERTPANEL_CONFIRM";//通用对话框
		public const string ALERTPANEL_CANCEL = "ALERTPANEL_CANCEL";//提示对话框取消


        public const string CHANGE_STATE = "CHANGE_STATE";
        public const string CHANGE_STATE_ON = "CHANGE_STATE_ON";

        public const string INFER_STATE_CHANGE = "INFER_STATE_CHANGE";
        public const string INFER_BACK_CLICK = "INFER_BACK_CLICK";

        public const string SHOW_DOWNLOAD_MAP_TIPS = "SHOW_DOWNLOAD_MAP_TIPS";//显示下载地图的loading
        public const string WELCOME_FRAME = "WELCOME_FRAME";//欢迎框
		public const string NEW_GAME_CHOSE = "NEW_GAME_CHOSE";//新游戏
        public const string CONTINUE_GAME = "WELCOME_FRAME";//继续游戏

        public const string MISS_CAND_BACK_HIDE = "MISS_CAND_BACK_HIDE";
        public const string MISS_CAND_BACK_SHOW = "MISS_CAND_BACK_SHOW";

        public const string COLOR_SETTING_CHANGE = "COLOR_SETTING_CHANGE"; //颜色设置改变
        public const string COLOR_SETTING_CHANGE_STAGE2 = "COLOR_SETTING_CHANGE_STAGE2"; //颜色设置改变

        public const string VALUE_BLOD_CHANGE = "VALUE_BLOD_CHANGE"; //Value粗体设置
        public const string CANDIDATE_BLOD_CHANGE = "CANDIDATE_BLOD_CHANGE"; //Candidate粗体设置
        public const string SAME_VALUE_SHOW_CHANGE = "SAME_VALUE_SHOW"; //相同选数高亮
        public const string BUDDIES_SHOW_CHANGE = "BUDDIES_SHOW_CHANGE"; //相同选数高亮
        public const string GRID_4_BG_CHANGE = "GRID_4_BG"; //grid4
        public const string TIMER_SWITCH_CHANGE = "TIMER_SWITCH_CHANGE";  //计时器开关更新；
        public const string REDO_BTN_CHANGE = "REDO_BTN_CHANGE"; //重做开关；
        public const string ERROR_COUNT_CHANGE = "ERROR_COUNT_CHANGE"; //错误检查；


        public const string ALERTPANEL_EDIT_ERROROVER_CONFIRM = "ALERTPANEL_EDIT_ERROROVER_CONFIRM";//继续游戏
        public const string ALERTPANEL_EDIT_ERROROVER_NEWGAME = "ALERTPANEL_EDIT_ERROROVER_NEWGAME";//继续游戏


        public const string LOCK_STATE_OFF = "LOCK_STATE_OFF";   //连填锁定状态关闭
        public const string HELP_BTN_SWITCH_CHANGE = "HELP_BTN_SWITCH_CHANGE";  //帮助按钮变化


        public const string BRIGHTNESS_SETTING_CHANGE = "BRIGHTNESS_SETTING_CHANGE"; //亮度设置改变

        public const string cacheItemDataChange = "cacheItemDataChange";

        public const string playingSudokuChange = "playingSudokuChange";

        public const string sysToggleChange = "sysToggleChange";

        public const string selfToggleChange = "selfToggleChange";


    }
}
