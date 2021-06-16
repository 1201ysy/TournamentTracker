using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Returns the full file path of "fileName"
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FullFilePath(this string fileName) // PrizeModel.csv
        {
            return $"{ConfigurationManager.AppSettings["filepath"]}\\{fileName}";
        }


        /// <summary>
        /// Check if text file "file" exists and load the text file as a list of strings
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Converts the csv text to List<PrizeModel>
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);

                output.Add(p);
            }

            return output;
        }

        /// <summary>
        ///  Convert the prizes to List<string> and
        ///  save the List<string> to the text file "fileName"
        /// </summary>
        /// <param name="models"></param>
        /// <param name="fileaName"></param>
        public static void SaveToPrizeModelFile(this List<PrizeModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
            }

            File.WriteAllLines(GlobalConfig.PrizesFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Converts the csv text to List<PersonModel>
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellphoneNumber = cols[4];

                output.Add(p);
            }

            return output;
        }

        /// <summary>
        ///  Convert the people to List<string> and
        ///  save the List<string> to the text file "fileName"
        /// </summary>
        /// <param name="models"></param>
        /// <param name="fileaName"></param>

        public static void SaveToPersonModelFile(this List<PersonModel> models)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.CellphoneNumber}");
            }

            File.WriteAllLines(GlobalConfig.PeopleFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Converts the csv text to List<TeamModel>
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // id, team name, list of ids of team members seperated by pipe
            // e.g. 1, New team, 1|2|5


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(t);
            }

            return output;
        }

        /// <summary>
        ///  Convert the team to List<string> and
        ///  save the List<string> to the text file "fileName"
        /// </summary>
        /// <param name="models"></param>
        /// <param name="fileaName"></param>
        public static void SaveToTeamModelFile(this List<TeamModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {

                lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
            }

            File.WriteAllLines(GlobalConfig.TeamFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Convert the List<PersonModel> list to string
        /// Each PersonModel item is split by "|" characterin the string
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";
            
            if (people.Count == 0)
            {
                return "";
            }

            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }
       
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        /// <summary>
        /// Convert the List<string> to List<TournamentModel> 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines)
        {
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();



            // id, TournamentName, EntryFee, (id1|id2|id3|... Entered Teams), (id1|id2|id3| ... Prizes), (id1^id2^id3|Id4^id5^Id6| ... Rounds)


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TournamentModel tm = new TournamentModel();

                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                if (cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');

                    foreach (string id in prizeIds)
                    {
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    }
                }

                string[] rounds = cols[5].Split('|');
                List<MatchupModel> ms = new List<MatchupModel>();

                foreach (string round in rounds)
                {
                    string[] msTxt = round.Split('^');
                    ms = new List<MatchupModel>();
                    foreach (string id in msTxt)
                    {
                        ms.Add(matchups.Where(x => x.Id == int.Parse(id)).First());
                    }
                    tm.Rounds.Add(ms);
                }


                output.Add(tm);
            }

            return output;

        }

        /// <summary>
        /// Save Tournament Model information to text file
        /// </summary>
        /// <param name="models"></param>
        public static void SaveToTournamentModelFile(this List<TournamentModel> models)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in models)
            {

                lines.Add($"{tm.Id},{tm.TournamentName},{tm.EntryFee},{ConvertTeamListToString(tm.EnteredTeams)},{ConvertPrizeListToString(tm.Prizes)},{ConvertRoundListToString(tm.Rounds)}");

            }

            File.WriteAllLines(GlobalConfig.TournamentFile.FullFilePath(), lines);
        }

        /// <summary>
        /// Convert the List<TeamModel> to string in the following format "id1|id2|id3" 
        /// </summary>
        /// <param name="teams"></param>
        /// <returns></returns>
        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        /// <summary>
        /// Convert the List<PrizeModel> to string in the following format "id1|id2|id3"
        /// </summary>
        /// <param name="prizes"></param>
        /// <returns></returns>
        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        /// <summary>
        /// Convert the List< List<MatchupModel>> to string in the following format "roudn1|round2|round3"
        /// </summary>
        /// <param name="rounds"></param>
        /// <returns></returns>
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }

            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ConvertMatchupListToString(r)}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        /// <summary>
        /// Convert the List<MatchupModel> to string in the following format "id1^id2^id3"
        /// </summary>
        /// <param name="matchups"></param>
        /// <returns></returns>

        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id}^";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }

        /// <summary>
        /// Save the Round info (matchup,matchupentries) to text file
        /// </summary>
        /// <param name="models"></param>
        public static void SaveRoundsToFile(this TournamentModel models)
        {
            foreach (List<MatchupModel> round in models.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    matchup.SaveMatchupToFile();
                }
            }
        }

        /// <summary>
        /// Save MatchupModel to text file
        /// </summary>
        /// <param name="matchup"></param>
        public static void SaveMatchupToFile(this MatchupModel matchup)
        {
            
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            int currentId = 1;

            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }

            matchup.Id = currentId;

            matchups.Add(matchup);


            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveToMatchupEntriesFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertEntriesListToString(m.Entries)},{winner},{m.MatchupRound}");

            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
           
        }
        /// <summary>
        /// Update Matchup information in the file
        /// </summary>
        /// <param name="matchup"></param>
        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {

            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel(); 
            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }

            }
            matchups.Remove(oldMatchup);

            matchups.Add(matchup);




            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateToMatchupEntriesFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertEntriesListToString(m.Entries)},{winner},{m.MatchupRound}");

            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);

        }

        /// <summary>
        /// Convert the List<MatchupEntry> to string in the following format "id1|id2|id3" 
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        private static string ConvertEntriesListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }

            foreach (MatchupEntryModel e in entries)
            {
                output += $"{e.Id}|";
            }

            output = output.Substring(0, output.Length - 1);
            return output;
        }


        /// <summary>
        /// Save MatchupEntry model into text file
        /// </summary>
        /// <param name="entry"></param>
        public static void SaveToMatchupEntriesFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            int currentId = 1;

            if (entries.Count > 0)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }

            entry.Id = currentId;
            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                string team = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                if (e.TeamCompeting != null)
                {
                    team = e.TeamCompeting.Id.ToString();
                }

                lines.Add($"{e.Id},{team},{e.Score},{parent}");
                
            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);

        }

        /// <summary>
        /// Update MatchupEntry Model information in the text file
        /// </summary>
        /// <param name="entry"></param>
        public static void UpdateToMatchupEntriesFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();
            MatchupEntryModel oldEntry = new MatchupEntryModel();
            foreach (MatchupEntryModel me in entries)
            {
                if (me.Id == entry.Id)
                {
                    oldEntry = me;
                }

            }
            entries.Remove(oldEntry);

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel e in entries)
            {
                string parent = "";
                string team = "";
                if (e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                if (e.TeamCompeting != null)
                {
                    team = e.TeamCompeting.Id.ToString();
                }

                lines.Add($"{e.Id},{team},{e.Score},{parent}");

            }

            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);

        }

        /// <summary>
        /// Convert the List<string> to List<MatchupModel> 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            List<MatchupModel> output = new List<MatchupModel>();

            // id, entries (pipe delimited), winner, matchuprounds
            // e.g. 1, 1|2|5, 1, 1


            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel m = new MatchupModel();
                m.Id = int.Parse(cols[0]);
                m.Entries = ConvertStringToMatchupEntryModels(cols[1]);
                int winnerId = 0;
                if (int.TryParse(cols[2], out winnerId))
                {
                    m.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                else
                {
                    m.Winner = null;
                }
                m.MatchupRound = int.Parse(cols[3]);

                output.Add(m);
            }

            return output;
        }

        /// <summary>
        /// Get TeamModel by Id from the text file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();

            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels().First();
                }
            }
            return null;
        }

        /// <summary>
        /// Get MatchupModel by Id from the text file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }
            return null;

        }
        /// <summary>
        /// Convert the string to List<MatchupEntryModel> 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            string[] ids = input.Split('|');


            foreach (string id in ids)
            {
                foreach(string entry in entries)
                {
                    string[] cols = entry.Split(',');
                    if (cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
                //output.Add(entries.Where(x => x.Id == int.Parse(id)).First());
            }
            output = matchingEntries.ConvertToMatchupEntryModels();
            return output;

        }

        /// <summary>
        /// Convert the List<string> to List<MatchupEntryModel> 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupEntryModel m = new MatchupEntryModel();
                m.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                {
                    m.TeamCompeting = null;
                }
                else
                {
                    m.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }

                m.Score = double.Parse(cols[2]);

                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                {
                    m.ParentMatchup = LookupMatchupById(int.Parse(cols[3]));
                }
                else
                {
                    m.ParentMatchup = null;
                }

                output.Add(m);
            }

            return output;

        }
    }
}
