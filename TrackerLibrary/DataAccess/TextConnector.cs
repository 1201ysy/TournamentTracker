using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {

        /// <summary>
        /// Save a new prize to the text file 
        /// </summary>
        /// <param name="model"> The prize information. </param>
        /// <returns> The prize information, including the unique identifiers.</returns>
        public void CreatePrize(PrizeModel model)
        {

            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            } 

            model.Id = currentId;

            prizes.Add(model);

            prizes.SaveToPrizeModelFile();

            return;
        }
        /// <summary>
        /// Save a new person to the text file 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();


            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            people.Add(model);

            people.SaveToPersonModelFile();

            return;
        }
        /// <summary>
        /// Get all person from the PersonModel csv text file 
        /// </summary>
        /// <returns></returns>
        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }
        /// <summary>
        /// Save a new team to the text file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamModelFile();

            return;

        }
        /// <summary>
        /// Get all teams from the TeamModel csv text file 
        /// </summary>
        /// <returns></returns>
        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }
        /// <summary>
        /// Save a new tournament to the text file
        /// </summary>
        /// <param name="model"></param>
        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels();

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentModelFile();

            TournamentLogic.UpdateTournamentResults(model);

            return;
        }
        /// <summary>
        /// Get all tournament from the text file
        /// </summary>
        /// <returns></returns>
        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels();
        }

        /// <summary>
        /// Update matchup information in the text file
        /// </summary>
        /// <param name="model"></param>
        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }

        /// <summary>
        /// Update the tournament state to complete
        /// </summary>
        /// <param name="model"></param>
        public void CompleteTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels();

            tournaments.Remove(model);

            tournaments.SaveToTournamentModelFile();

            TournamentLogic.UpdateTournamentResults(model);

            return;
        }
    }
}
