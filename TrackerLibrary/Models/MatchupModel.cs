using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one matchup in the Tournament
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier for the Matchup Model.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents two teams that are matched up.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        /// <summary>
        /// Reprsents the team that has is the winner of the matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents which Round number the matchup is part of.
        /// </summary>
        public int MatchupRound { get; set; }

    }
}
