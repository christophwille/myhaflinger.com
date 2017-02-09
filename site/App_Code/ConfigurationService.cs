using System;
using System.Collections.Generic;
using System.Configuration;

public class ConfigurationService
{
    public string ContactFormTo => GetSetting("ContactFormTo");

    public string MailFromAddress => GetSetting("MailFromAddress");

    public string SmtpUsername => GetSetting("SmtpUsername");

    public string SmtpPassword => GetSetting("SmtpPassword");

    public string SmtpHost => GetSetting("SmtpHost");

    public string SmtpPort => GetSetting("SmtpPort");



    private string GetSetting(string settingName)
    {
        return ConfigurationManager.AppSettings[settingName];
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