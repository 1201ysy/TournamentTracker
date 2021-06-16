using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";
        /// <summary>
        /// Save a new prize to the database
        /// </summary>
        /// <param name="model"> The prize information. </param>
        /// <returns> The prize information, including the unique identifiers.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new Microsoft.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePercentage);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                return model;
            }
        }

        /// <summary>
        /// Save a new person to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new Microsoft.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", model.FirstName);
                p.Add("@LastName", model.LastName);
                p.Add("@EmailAddress", model.EmailAddress);
                p.Add("@CellphoneNumber", model.CellphoneNumber);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                return model;
            }
        }
        /// <summary>
        /// Get all people from the database
        /// </summary>
        /// <returns></returns>
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new Microsoft.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }
        /// <summary>
        /// Save a new team to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new Microsoft.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("@TeamName", model.TeamName);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");


                foreach (PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", model.Id);
                    p.Add("@PersonId", tm.Id);

                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }
        }
        /// <summary>
        /// Get all teams from the database
        /// </summary>
        /// <returns></returns>
        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new Microsoft.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);

                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return output;
        }

        public TournamentModel CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new Microsoft.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                // Save Tournament
                var p = new DynamicParameters();
                p.Add("@TournamentName", model.TournamentName);
                p.Add("@EntryFee", model.EntryFee);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                // Save TournamentPrizes
                foreach (PrizeModel pz in model.Prizes)
                {
                    p = new DynamicParameters();
                    p.Add("@TournamentId", model.Id);
                    p.Add("@PrizeId", pz.Id);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
                }

                // Save TournamentEntries
                foreach (TeamModel tm in model.EnteredTeams)
                {
                    p = new DynamicParameters();
                    p.Add("@TournamentId", model.Id);
                    p.Add("@TeamId", tm.Id);
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);

                }

                // Save TournamentRounds
                foreach (List<MatchupModel> round in model.Rounds)
                {

                    foreach (MatchupModel matchup in round)
                    {

                        p = new DynamicParameters();
                        p.Add("@TournamentId", model.Id);
                        p.Add("@MatchupRound", matchup.MatchupRound);
                        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchup_Insert", p, commandType: CommandType.StoredProcedure);

                        model.Id = p.Get<int>("@id");

                        foreach (MatchupEntryModel entry in matchup.Entries)
                        {
                            p = new DynamicParameters();
                            p.Add("@MatchupId", matchup.Id);
                            if (entry.ParentMatchup != null)
                            {
                                p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                            }
                            else
                            {
                                p.Add("@ParentMatchupId", null);
                            }

                            if (entry.TeamCompeting != null)
                            {
                                p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                            }
                            else
                            {
                                p.Add("@TeamCompetingId", null);
                            }
                            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                            connection.Execute("dbo.spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);

                        }


                    }

                }



                return model;
            }
        }
    }
}
