using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        // TODO - Make the CreatePrize method actually to save to the Text File

        /// <summary>
        /// Save a new prize to the database
        /// </summary>
        /// <param name="model"> The prize information. </param>
        /// <returns> The prize information, including the unique identifiers.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            model.Id = 1;

            return model;
        }
    }
}
