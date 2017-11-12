using System;
using SQLite;

namespace MyHaflinger.Treffen.Db
{
    public class EmailChallenge
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string EmailAddress { get; set; }
        public string ChallengeGuid { get; set; }
        public DateTime ChallengeRequested { get; set; }

        public DateTime? FirstTokenRedemption { get; set; }
    }
}