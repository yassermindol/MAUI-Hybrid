
using System.Text;
using ExpenseTracker.Helpers;

namespace ExpenseTracker.Services;

public class LogService
{
    readonly StringBuilder stringBuilder = new StringBuilder();
    private LogService() { }
    private static Lazy<LogService> instance = new Lazy<LogService>(() => new LogService());
    public static LogService Instance
    {
        get => instance.Value;
    }

    private string GetPrintableLogMsg(byte[] rawByteArray, bool allBinary, int codePageID)
    {
        string lsPrintable = "";
        try
        {
            for (int liCounter = 0; liCounter < rawByteArray.Length; liCounter++)
            {
                if (allBinary || rawByteArray[liCounter] < 32 || rawByteArray[liCounter] > 127)
                    lsPrintable += "{0x" + rawByteArray[liCounter].ToString("X2") + "}";
                else
                    lsPrintable += Encoding.GetEncoding(codePageID).GetString(rawByteArray, liCounter, 1);

            }
        }
        catch
        {
        }
        return (lsPrintable);
    }

    public void Print(byte[] rawByteArray, bool allBinary, int codePageID = 1252)
    {
        lock (this)
        {
            string msg = GetPrintableLogMsg(rawByteArray, allBinary, codePageID);
            Print(msg);
        }
    }

    public void Print(string header, byte[] rawByteArray, bool allBinary, int codePageID = 1252)
    {
        try
        {
            lock (this)
            {
                string data = GetPrintableLogMsg(rawByteArray, allBinary, codePageID);
                Print(header + data);
            }
        }
        catch
        {
        }
    }

    public void Print(string msg)
    {
        lock (this)
        {
            DebugHelper.WriteLine(Time + ": " + msg);
            //AccessStringBuilder(StringBuilderAccessType.Append, msg);
        }
    }

    public void Print(Exception ex)
    {
        lock (this)
        {
            Print("Exception: " + ex.GetType());
            if (ex.Message != null)
                Print("Exception Message: " + ex.Message);
            Print("Exception Stack Trace: " + ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print("InnerException: " + ex.InnerException.GetType());
                if (ex.InnerException.Message != null)
                    Print("InnerException  Message: " + ex.InnerException.Message);
                Print("InnerException Stack Trace: " + ex.InnerException.StackTrace);
                if (ex.InnerException.InnerException != null)
                {
                    Print("InnerException.InnerException: " + ex.InnerException.InnerException.GetType());
                    Print("InnerException.InnerException Message: " + ex.InnerException.InnerException.Message);
                    Print("InnerException.InnerException Stack Trace: " + ex.InnerException.InnerException.StackTrace);
                }
            }
        }
    }

    public void Clear()
    {
        Print("Clearing Logs.");
        AccessStringBuilder(StringBuilderAccessType.Clear);
    }

    private string AccessStringBuilder(StringBuilderAccessType accessType, string? data = null)
    {
        string str = null;
        lock (this)
        {
            if (accessType == StringBuilderAccessType.Append)
            {
                stringBuilder.Append(Time + ": " + data + "\r\n");
            }
            else if (accessType == StringBuilderAccessType.Clear)
            {
                stringBuilder.Clear();
            }
            else if (accessType == StringBuilderAccessType.ToString)
            {
                str = stringBuilder.ToString();
            }
        }
        return str;
    }

    public async Task SubmitLogToAppCenterAsync(Exception? exception = null, Dictionary<string, string>? deviceParam = null)
    {
        Print("Submitting log to App center.");
        try
        {
            if (await IsShouldSumitLog())
            {
                /*
                string user = Settings.Account.Instance.Username;
                if (exception == null)
                    exception = new Exception(user);
                else
                    Print(exception);

                if (deviceParam == null)
                    deviceParam = new Dictionary<string, string>();

                deviceParam.Add("User", user);

                string strData = AccessStringBuilder(StringBuilderAccessType.ToString);
                AppCenter.SetUserId(user);
                ErrorAttachmentLog log = ErrorAttachmentLog.AttachmentWithText(strData, user + ".txt");
                AppCenterHelper.TrackError(exception, deviceParam, log);
                */
            }
        }
        catch
        {
        }
        finally
        {
            //Clear();
        }
    }

    private async Task<bool> IsShouldSumitLog()
    {
        //await Task.Delay(10);
        //LogConfiguration config = CheckForLogService.Instance.Config;

        bool shouldSubmit = true;
        /*
        if (!config.EnableLogging)
            return false;

        if (config.LogIfTransactionFailedOnly)
        {
            if (AppSettings.Instance.UploadPass)
                shouldSubmit = false;
        }
        else
        {
            shouldSubmit = true;
        }

        if (shouldSubmit && config.LogOnlySelectedPhoneModels)
        {
            shouldSubmit = false;
            foreach (string model in config.PhoneModels)
            {
                if (model.ToUpper().Equals(DeviceInfo.Model.ToUpper()))
                {
                    shouldSubmit = true;
                    break;
                }
            }
        }
        */
        return shouldSubmit;
    }

    string Time
    {
        get
        {
            return DateTime.Now.ToString("yyyy-MM-dd-Thh-mm-ss-tt");
        }
    }

    enum StringBuilderAccessType { Append, ToString, Clear }
}