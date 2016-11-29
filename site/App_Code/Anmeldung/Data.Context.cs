using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace MyHaflinger.Anmeldung.Data
{
    public static class DbFactory
    {
        public static AnmeldungsDbContext CreateContext(HttpServerUtilityBase server)
        {
            return new AnmeldungsDbContext(server.MapPath("~/App_Data/anmeldungen.db"));
        }
    }

    public class AnmeldungsDbContext : SQLiteConnection
    {
        public AnmeldungsDbContext(string path) : base(path)
		{
            CreateTable<EmailChallenge>();
        }

        public void RegisterEmailChallenge(string emailadress, string guid, DateTime registrationTime)
        {
            var newKey = this.Insert(new EmailChallenge()
            {
                EmailAddress = emailadress,
                ChallengeGuid = guid,
                ChallengeRequested = registrationTime
            });
        }
    }
}