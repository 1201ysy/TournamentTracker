using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one prize for the tournament
    /// </summary>
    public class PrizeModel
    {

        /// <summary>
        /// The unique identifier for the prize.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the Place number for the prize
        /// </summary>
        public int PlaceNumber { get; set; }
        /// <summary>
        /// Represent the word name (ex. Champion, runner-up) for the placing
        /// </summary>
        public string PlaceName { get; set; }
        /// <summary>
        /// Represents the prize amount in $
        /// </summary>
        public decimal PrizeAmount { get; set; }
        /// <summary>
        /// Represents the prize as a percentage of total prize pool $
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }
        public PrizeModel (string placeName, string placeNumber, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;

        }

    }
}
