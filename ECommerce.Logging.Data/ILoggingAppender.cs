namespace ECommerce.Logging.Data
{
    public interface ILoggingAppender
    {
        void InitializeAppender();
        void Log(LoggingItem loggingItem);
    }
}