using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerUI
{
    /// <summary>
    /// Interface to communicate between forms 
    /// </summary>
    public interface IPrizeRequester
    {
        void PrizeComplete(PrizeModel model);
    }
}
