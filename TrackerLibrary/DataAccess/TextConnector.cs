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
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamFile = "TeamModel.csv";
        private const string TournamentFile = "TournamentModel.csv";
        private const string MatchupFile = "MatchupModel.csv";
        private const string MatchupEntriesFile = "MatchupEntriesModel.csv";


        /// <summary>
        /// Save a new prize to the text file 
        /// </summary>
        /// <param name="model"> The prize information. </param>
        /// <returns> The prize information, including the unique identifiers.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {

            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            } 

            model.Id = currentId;

            prizes.Add(model);

            prizes.SaveToPrizeModelFile(PrizesFile);

            return model;
        }
        /// <summary>
        /// Save a new person to the text file 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();


            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            people.Add(model);

            people.SaveToPersonModelFile(PeopleFile);

            return model;
        }
        /// <summary>
        /// Get all person from the PersonModel csv text file 
        /// </summary>
        /// <returns></returns>
        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }
        /// <summary>
        /// Save a new team to the text file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamModelFile(TeamFile);

            return model;

        }
        /// <summary>
        /// Get all teams from the TeamModel csv text file 
        /// </summary>
        /// <returns></returns>
        public List<TeamModel> GetTeam_All()
        {
            return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }

        public TournamentModel CreateTournament(TournamentModel model)
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

            tournaments.SaveToTournamentModelFile(TournamentFile);

            return model;
        }
    }
}
