using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace MyHaflinger.Treffen.Db
{
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
                if (null == challenge.FirstTokenRedemption)
                {
                    challenge.FirstTokenRedemption = DateTime.UtcNow;
                    this.Update(challenge);
                }

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

        public List<EmailChallenge> GetEmailChallenges()
        {
            return this.Table<EmailChallenge>().OrderByDescending(r => r.ChallengeRequested).ToList();
        }
    }
}