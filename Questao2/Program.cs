using Newtonsoft.Json;
using Questao2;

public class Program
{
	public static async Task Main()
	{
		string teamName = "Paris Saint-Germain";
		int year = 2013;
		int totalGoals = await GetTotalScoredGoals(teamName, year);

		Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

		teamName = "Chelsea";
		year = 2014;
		totalGoals = await GetTotalScoredGoals(teamName, year);

		Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

		// Output expected:
		// Team Paris Saint - Germain scored 109 goals in 2013
		// Team Chelsea scored 92 goals in 2014
	}

	public static async Task<int> GetTotalScoredGoals(string team, int year)
	{
		int totalGoals = await SumScoredGoalsByDemand(EnumDemand.Home, team, year);
		totalGoals += await SumScoredGoalsByDemand(EnumDemand.Away, team, year);
		return totalGoals;
	}

	private static async Task<int> SumScoredGoalsByDemand(EnumDemand demand, string team, int year)
	{
		using (HttpClient client = new HttpClient())
		{
			Uri baseUrl = new Uri("https://jsonmock.hackerrank.com/api/football_matches");
			int totalGoals = 0;
			int currentPage = 1;
			bool hasMorePages = true;
			while (hasMorePages)
			{
				string url = baseUrl + $"?year={year}&team{(int)demand}={team}&page={currentPage}";
				HttpResponseMessage response = await client.GetAsync(url);
				string responseData = await response.Content.ReadAsStringAsync();

				var football = JsonConvert.DeserializeObject<ResponseFootball>(responseData);
				if (football == null || football.Data == null)
				{
					break;
				}
				totalGoals += football.Data.Sum(f => int.Parse(demand == EnumDemand.Home ? f.Team1Goals : f.Team2Goals));
				hasMorePages = currentPage < football.Total_pages;

				currentPage++;
			}
			return totalGoals;
		}
	}
}