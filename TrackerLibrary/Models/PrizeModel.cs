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
        /// <summary>
        /// Constructor for PrizeModel that accepts all strings and handle data type conversions correspondingly 
        /// </summary>
        /// <param name="placeName"></param>
        /// <param name="placeNumber"></param>
        /// <param name="prizeAmount"></param>
        /// <param name="prizePercentage"></param>
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
