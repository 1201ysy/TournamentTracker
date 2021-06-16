using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        /// <summary>
        /// Creates rounds information for the tournament
        /// </summary>
        /// <param name="model"></param>
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));
            CreateOtherRounds(model, rounds);

        }
        /// <summary>
        /// Randomize team order given a List<TeamModel> teams
        /// </summary>
        /// <param name="teams"></param>
        /// <returns></returns>
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// Calculates the number of byes in the tournaments
        /// </summary>
        /// <param name="rounds"></param>
        /// <param name="numberOfTeams"></param>
        /// <returns></returns>
        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            //Math.Pow(2, rounds)
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;
            return output;

        }

        /// <summary>
        /// Calcaulates the number of rounds for the tournament
        /// </summary>
        /// <param name="teamCount"></param>
        /// <returns></returns>
        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;

            }

            return output;
        }

        /// <summary>
        /// Create first round for the tournament
        /// special case due to handling byes
        /// </summary>
        /// <param name="byes"></param>
        /// <param name="teams"></param>
        /// <returns></returns>
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();
                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Create all other round (except the first) for the tournament
        /// </summary>
        /// <param name="model"></param>
        /// <param name="rounds"></param>
        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> prevRound = model.Rounds[0];
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in prevRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });


                    if ( currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();

                    }
                }
                model.Rounds.Add(currRound);

                round += 1;
                prevRound = currRound;
                currRound = new List<MatchupModel>();
            }



        }

        /// <summary>
        /// Determines winner based on their score information for the list of matchups in the List<MatchupModel>
        /// </summary>
        /// <param name="matchups"></param>
        private static void ScoreMatchups(List<MatchupModel> matchups)
        {
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel m in matchups)
            {
                if (m.Entries.Count == 1) // for byes
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }

                if (greaterWins == "0") // low score wins
                {
                    if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties in this application");
                    }
                }
                else // high score wins
                {
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties in this application");
                    }
                }
            }


        }

        /// <summary>
        /// Advance winners to the next round of the tournament save this information to the database.
        /// </summary>
        /// <param name="matchups"></param>
        /// <param name="tournament"></param>
        private static void AdvanceWinners(List<MatchupModel> matchups, TournamentModel tournament)
        {
            foreach (MatchupModel m in matchups)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel rm in round)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {

                            if (me.ParentMatchup != null)
                            {
                                if (me.ParentMatchup.Id == m.Id)
                                {
                                    me.TeamCompeting = m.Winner;
                                    GlobalConfig.Connection.UpdateMatchup(rm);
                                }
                            }
                        }
                    }

                }

            }
        }


        
        /// <summary>
        /// Updates Tournament results : Matchup results
        /// Sends emails to participants when new round has started (when current round has finsihed)
        /// Also checks if the entire tournament is complete and sends out result emails
        /// </summary>
        /// <param name="model"></param>
        public static void UpdateTournamentResults(TournamentModel model)
        {
            List<MatchupModel> toScore = new List<MatchupModel>();
            int startingRound = CheckCurrentRound(model);

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    if (rm.Winner == null && (rm.Entries.Any(x=>x.Score != 0) || rm.Entries.Count ==1))
                    {
                        toScore.Add(rm);
                    }
                }
            }

            ScoreMatchups(toScore);
            AdvanceWinners(toScore, model);

            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));

            int endingRound = CheckCurrentRound(model);

            if (endingRound > startingRound)
            {
                Console.WriteLine("Send emails");
                //AlertUsersToNewRound(model);

            }


        }

        /// <summary>
        /// Sends email alert when new round has started
        /// </summary>
        /// <param name="team"></param>
        /// <param name="competitor"></param>
        private static void AlertPersonToNewRound(TeamModel team, MatchupEntryModel competitor)
        {

          
            List<string> toAddr = new List<string>();
            string subject = "";
            StringBuilder body = new StringBuilder();

            if (competitor != null)
            {
                subject = $"You have a new matchup with {competitor.TeamCompeting.TeamName}";

                body.AppendLine("<h1> You have a new matchup</h1>");
                body.Append("<strong> Competitor: </strong>");
                body.Append(competitor.TeamCompeting.TeamName);
                body.AppendLine();
                body.AppendLine();
                body.Append("Have a great time!");
                body.Append("~Tournament Tracker");
            }
            else
            {
                subject = "You have a bye week this round";

                body.Append("Enjoy your round off!");
                body.Append("~Tournament Tracker");
            }

            foreach (PersonModel teamMember in team.TeamMembers)
            {
                toAddr.Add(teamMember.EmailAddress);
            }

            EmailLogic.SendEmail(toAddr, new List<string>() , subject, body.ToString());
        }

        /// <summary>
        /// Find those who are still participating in the new round and alert them through email 
        /// </summary>
        /// <param name="model"></param>
        public static void AlertUsersToNewRound(TournamentModel model)
        {
            int currentRoundNumber = CheckCurrentRound(model);
            List<MatchupModel> currentRound = model.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();

            foreach (MatchupModel matchup in currentRound)
            {
                foreach (MatchupEntryModel me in matchup.Entries)
                {
                    AlertPersonToNewRound(me.TeamCompeting, matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());

                }
            }
        }

        /// <summary>
        /// Check the currently progressing rounds or if the tournament is complete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static int CheckCurrentRound(TournamentModel model)
        {
            int output = 1;

            foreach (List<MatchupModel> round in model.Rounds)
            {
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
                else
                {
                    return output;
                }
            }

            //Tournament is complete

            CompleteTournament(model);

            return output - 1;

        }

        /// <summary>
        /// Completes the Tournament :  Update database, calculates prizes and send out emails to all participants
        /// </summary>
        /// <param name="model"></param>
        private static void CompleteTournament(TournamentModel model)
        {
            GlobalConfig.Connection.CompleteTournament(model);
            TeamModel winners = model.Rounds.Last().First().Winner;
            TeamModel runnerup = model.Rounds.Last().First().Entries.Where(x => x.TeamCompeting != winners).First().TeamCompeting;

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;

            if (model.Prizes.Count > 0)
            {
                decimal totalPrize = model.EnteredTeams.Count * model.EntryFee;
                PrizeModel firstPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
                PrizeModel secondPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizePayout(totalPrize);
                }
                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalPrize);

                }
            }

            string fromAddr = GlobalConfig.AppKeyLookup("senderEmail");
            List<string> toAddr = new List<string>();
            string subject = "";
            StringBuilder body = new StringBuilder();

            subject = $"In {model.TournamentName}, {winners.TeamName} has won!";

            body.AppendLine("<h1> We have a WINNER!</h1>");
            body.AppendLine("<p> Congratulations to our winner on a great tournament.</p>");
            body.AppendLine("<br />");
            if (winnerPrize > 0)
            {
                body.AppendLine($"<p>{winners.TeamName} will receive ${winnerPrize}.</p>");
            }
            if (runnerUpPrize > 0)
            {
                body.AppendLine($"<p>{runnerup.TeamName} will receive ${runnerUpPrize}.</p>");
            }
            body.AppendLine("<p>Thanks for a great tournament everyone!</p>");
            body.AppendLine("<p>~Tournament Tracker</p>");


            List<string> bcc = new List<string>();

            foreach (TeamModel t in model.EnteredTeams)
            {
                foreach (PersonModel p in t.TeamMembers)
                {
                    if (p.EmailAddress.Length > 0)
                    {
                        bcc.Add(p.EmailAddress);
                    }
                }
            }

            EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());

            model.CompleteTournament();

        }



        /// <summary>
        /// Calculates the prize payout based on the PrizeModel
        /// </summary>
        /// <param name="prize"></param>
        /// <param name="totalPrize"></param>
        /// <returns></returns>
        private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalPrize)
        {
            decimal output = 0;
            if (prize.PrizeAmount >0)
            {
                output = prize.PrizeAmount;
            }
            else
            {
                output = Decimal.Multiply(totalPrize, Convert.ToDecimal(prize.PrizePercentage / 100));
            }
            return output;
        }

    }
}
