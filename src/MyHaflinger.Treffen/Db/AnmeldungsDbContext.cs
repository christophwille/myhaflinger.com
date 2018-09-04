using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace MyHaflinger.Treffen.Db
{
	public class AnmeldungsDbContext : SQLiteAsyncConnection
	{
		public AnmeldungsDbContext(string path) : base(path)
		{
		}

		public async Task CreateTablesAsync()
		{
			await CreateTablesAsync<EmailChallenge, Registration>().ConfigureAwait(false);
		}

		public async Task RegisterEmailChallengeAsync(string emailadress, string guid, DateTime registrationTime)
		{
			var newKey = await this.InsertAsync(new EmailChallenge()
			{
				EmailAddress = emailadress,
				ChallengeGuid = guid,
				ChallengeRequested = registrationTime
			}).ConfigureAwait(false);
		}

		public async Task<string> GetEmailForChallengeTokenAsync(string token)
		{
			var challenge = await Table<EmailChallenge>().Where(c => c.ChallengeGuid == token).FirstOrDefaultAsync().ConfigureAwait(false);

			if (null != challenge)
			{
				if (null == challenge.FirstTokenRedemption)
				{
					challenge.FirstTokenRedemption = DateTime.UtcNow;
					int changed = await this.UpdateAsync(challenge).ConfigureAwait(false);
				}

				return challenge.EmailAddress;
			}

			return "";
		}

		public async Task<Registration> RegisterParticipantAsync(Registration reg)
		{
			var newKey = await this.InsertAsync(reg).ConfigureAwait(false);
			return reg;
		}

		public async Task<List<Registration>> GetRegisteredParticipantsAsync()
		{
			return await this.Table<Registration>().OrderByDescending(r => r.Id).ToListAsync().ConfigureAwait(false);
		}

		public async Task<Registration> GetRegisteredParticipantAsync(int id)
		{
			var reg = await Table<Registration>().Where(c => c.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);
			return reg;
		}

		public async Task<int> UpdateRegisteredParticipantAsync(Registration reg)
		{
			return await this.UpdateAsync(reg).ConfigureAwait(false);
		}

		public async Task<List<EmailChallenge>> GetEmailChallengesAsync()
		{
			return await this.Table<EmailChallenge>().OrderByDescending(r => r.ChallengeRequested).ToListAsync().ConfigureAwait(false);
		}
	}
}