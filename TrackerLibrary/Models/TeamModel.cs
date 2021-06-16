using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one team in the tournament
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// The unique identifier for the Team model.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the team name
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// Represents the list of person on the team
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

    }
}
