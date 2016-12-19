using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;
using System.Linq.Expressions;

namespace MyHaflinger.Anmeldung.Data
{
    // https://github.com/praeclarum/sqlite-net
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
            CreateTable<Registration>();
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

        public string GetEmailForChallengeToken(string token)
        {
            var challenge = Table<EmailChallenge>().Where(c => c.ChallengeGuid == token).FirstOrDefault();

            if (null != challenge)
            {
                return challenge.EmailAddress;
            }

            return "";
        }

        public Registration RegisterParticipant(Registration reg)
        {
            var newKey = this.Insert(reg);
            return reg;
        }

        public List<Registration> GetRegisteredParticipants()
        {
            return this.Table<Registration>().OrderByDescending(r => r.Id).ToList();
        }

        public Registration GetRegisteredParticipant(int id)
        {
            var reg = Table<Registration>().Where(c => c.Id == id).FirstOrDefault();
            return reg;
        }

        public int UpdateRegisteredParticipant(Registration reg)
        {
            return this.Update(reg);
        }
    }
}