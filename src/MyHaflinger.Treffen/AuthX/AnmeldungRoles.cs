using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace MyHaflinger.Treffen.AuthX
{
	public static class AnmeldungRoles
	{
		public const string Kassier = "kassier";
		public const string Veranstaltungswart = "wart";

		public static bool IsKassier(this IPrincipal p)
		{
			return p.IsInRole(Kassier);
		}

		public static bool IsVeranstaltungswart(this IPrincipal p)
		{
			return p.IsInRole(Veranstaltungswart);
		}
	}
}
