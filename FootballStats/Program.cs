using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Newtonsoft.Json;

namespace FootballStats
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            var fileContents = ReadResults(fileName);
            fileName = Path.Combine(directory.FullName, "players.json");
            var players = DeserializePlayers(fileName);
            var topTenPlayers = GetTopTenPlayers(players);
            foreach (var player in topTenPlayers)
            {
                Console.WriteLine
                    (player.FirstName + " " + player.LastName 
                                      + "// PPG: " + player.PointsPerGame 
                                      + "// Team: " + player.TeamName);
            }

            fileName = Path.Combine(directory.FullName, "topten.json");
            SerializePlayersToFile(topTenPlayers, fileName);

            Console.ReadLine();
        }

        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<GameResult> ReadResults(string fileName)
        {

            var results = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null )
                {
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');

                    DateTime gameDate;

                    if (DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }

                    gameResult.TeamName = values[1];

                    HomeOrAway homeOrAway;
                    if (Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway;
                    }

                    int parseInt;
                    if (int.TryParse(values[3], out parseInt))
                    {
                        gameResult.Goals = parseInt;
                    }
                    if (int.TryParse(values[4], out parseInt))
                    {
                        gameResult.GoalAttempts = parseInt;
                    }
                    if (int.TryParse(values[5], out parseInt))
                    {
                        gameResult.ShotsOnGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }

                    double possessionPercent;
                    string convertDouble = values[7];
                    
                    if (double.TryParse(convertDouble, out possessionPercent))
                    {
                        gameResult.PossessionPercent = possessionPercent;
                    }

                    results.Add(gameResult);
                }
            }

            return results;

        }

        public static List<Player> DeserializePlayers(string fileName)
        {
            var players = new List<Player>();
            var serializer = new JsonSerializer();

            using (var reader = new StreamReader(fileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                players = serializer.Deserialize<List<Player>>(jsonReader);
            }

            return players;
        }

        public static List<Player> GetTopTenPlayers(List<Player> players)
        {
            var TopTenPlayers = new List<Player>();
            players.Sort(new PlayerComparer());
            int counter = 0;

            foreach (var player in players)
            {
                TopTenPlayers.Add(player);
                counter++;
                if (counter >= 10)
                {
                    break;
                }
            }
            return TopTenPlayers;
        }

        public static void SerializePlayersToFile(List<Player> players, string fileName)
        {
            var serializer = new JsonSerializer();

            using (var writer = new StreamWriter(fileName))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, players);
            }
        }
    }
}

