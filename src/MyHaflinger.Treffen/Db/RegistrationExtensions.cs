﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyHaflinger.Treffen;

namespace MyHaflinger.Treffen.Db
{
    public static class RegistrationExtensions
    {
        public static int GetTotalPrice(this Registration reg)
        {
            int price = reg.PParticipantsFriday * AnmeldungSettings.PricePerParticipantFriday;

            if (reg.PParticipantsSatSun > 0)
            {
                price += AnmeldungSettings.PricePerCar + (reg.PParticipantsSatSun - 1) * AnmeldungSettings.PricePerPassenger;
            }

            return price;
        }
    }
}