namespace liaoyanxuan.common.interfaces
{
    public interface ILoggerHelper
    {
        void Debug(object message, bool isShowStack = true, int user = 0);

        void Error(object message, bool isShowStack = true);

        void Warning(object message, bool isShowStack = true);

        void LogWarning(string message,object arg0,bool isShowStack = true);

        void log1(string msg);

        void Info(object message, bool isShowStack = true);

        void Log(string str, params object[] param);

    }
}
