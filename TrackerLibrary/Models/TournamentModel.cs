using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one tournament in the tournament tracker
    /// </summary>
    public class TournamentModel
    {
        /// <summary>
        /// Event for Tournament Complete
        /// </summary>
        public event EventHandler<DateTime> OnTournamentComplete;

        /// <summary>
        /// Method to start the event "Tournament Complete"
        /// </summary>
        public void CompleteTournament()
        {
            OnTournamentComplete?.Invoke(this, DateTime.Now);
        }
        /// <summary>
        /// The unique identifier for the Tournament model.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Reprsents the name of the Tournament
        /// </summary>
        public string TournamentName { get; set; }

        /// <summary>
        /// Represenets the entry fee to enter the tournament
        /// </summary>
        public decimal EntryFee { get; set; }
        /// <summary>
        /// Represents the list of teams entering the tournament
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// Represents the list of prizes for the tournmanet
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// Represents the list of rounds and all its matchup (as list) for the corresponding round in the tournament
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }
}
