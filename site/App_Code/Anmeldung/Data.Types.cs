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

    public class Registration
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Auto-filled fields (from step 1 challenge)
        public string EmailAddress { get; set; }
        public DateTime RegisteredAt { get; set; }

        // Basic registration data
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string NumberPlate { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }

        // Payment-related data, both *including* driver
        public int PParticipantsFriday { get; set; }
        public int PParticipantsSatSun { get; set; }

        // Statistics data
        public int StatParticipantsSunday { get; set; }

        // Internal data
        public bool IntPaymentReceived { get; set; }
        public string IntNotes { get; set; }

        // Computed, non-persisted properties
        [Ignore]
        public int TotalPrice {  get { return this.GetTotalPrice(); } }
    }
}