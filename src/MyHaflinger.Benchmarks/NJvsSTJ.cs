using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace MyHaflinger.Benchmarks
{
	public class LogonUser
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}

	[MemoryDiagnoser] // https://adamsitnik.com/the-new-Memory-Diagnoser/
	public class NJvsSTJ
	{
		private readonly string _jsonFilePath = "AnmeldungAccounts.json";

		[Benchmark]
		public void NJ()
		{
			string fileContents = File.ReadAllText(_jsonFilePath);
			var definedAccounts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LogonUser>>(fileContents);
		}

		[Benchmark]
		public async Task STJAsync()
		{
			using (var stream = File.OpenRead(_jsonFilePath))
			{
				var options = new JsonSerializerOptions
				{
					AllowTrailingCommas = true,
					PropertyNameCaseInsensitive = true
				};

				var definedAccounts = await System.Text.Json.JsonSerializer.DeserializeAsync<List<LogonUser>>(stream, options);
				stream.Close();
			}
		}

		[Benchmark]
		public void STJ()
		{
			string fileContents = File.ReadAllText(_jsonFilePath);

			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				PropertyNameCaseInsensitive = true
			};

			var definedAccounts = System.Text.Json.JsonSerializer.Deserialize<List<LogonUser>>(fileContents, options);
		}
	}
}
