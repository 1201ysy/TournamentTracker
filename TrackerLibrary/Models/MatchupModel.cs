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
        /// ID from the database that will be used to identify the winner
        /// </summary>
        public int WinnerId { get; set; }

        /// <summary>
        /// Reprsents the team that has is the winner of the matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        /// <summary>
        /// Represents which Round number the matchup is part of.
        /// </summary>
        public int MatchupRound { get; set; }

        /// <summary>
        /// DisplayName to be shown in forms
        /// </summary>
        public string DisplayName
        {
            get
            {
                string output = "";
                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs. {me.TeamCompeting.TeamName}";
                        }
                    }
                    else
                    {
                        output = "Matchup Not Yet Determined";
                        break;
                    }
                }
                return output;
            }
        }

    }
}
