using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

public class ConfigurationService
{
    private string GetSetting(string settingName)
    {
        return ConfigurationManager.AppSettings[settingName];
    }
    
    public string ContactFormTo
    {
        get
        {
            return GetSetting("ContactFormTo");
        }
    }

    public string MailFromAddress
    {
        get
        {
            return GetSetting("MailFromAddress");
        }
    }

    public string SmtpUsername
    {
        get
        {
            return GetSetting("SmtpUsername");
        }
    }

    public string SmtpPassword
    {
        get
        {
            return GetSetting("SmtpPassword");
        }
    }

    public string SmtpHost
    {
        get
        {
            return GetSetting("SmtpHost");
        }
    }

    public string SmtpPort
    {
        get
        {
            return GetSetting("SmtpPort");
        }
    }

    private bool ParseBooleanSetting(string settingName, bool defaultValue = false)
    {
        string setting = GetSetting(settingName);
        bool bValue = defaultValue;
        bool.TryParse(setting, out bValue);
        return bValue;
    }

    //public bool SomeBooleanProperty
    //{
    //    get
    //    {
    //        return ParseBooleanSetting("SomeBooleanProperty");
    //    }
    //}
}