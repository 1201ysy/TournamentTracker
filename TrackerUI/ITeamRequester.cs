using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerUI
{
    /// <summary>
    /// Interface to communicate between forms 
    /// </summary>
    public interface ITeamRequester
    {
        void TeamComplete(TeamModel model);
    }
}
