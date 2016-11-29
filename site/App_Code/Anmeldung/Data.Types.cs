using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite;

namespace MyHaflinger.Anmeldung.Data
{
    public class EmailChallenge
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

		public string EmailAddress { get; set; }
		public string ChallengeGuid { get; set; }
		public DateTime ChallengeRequested { get; set; }
    }
}