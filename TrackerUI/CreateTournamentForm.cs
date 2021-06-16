using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequester, ITeamRequester
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();

        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;

            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }
        /// <summary>
        /// Create a new prize and add it to the prizes list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // call the CreatePrizeForm
            CreatePrizeForm frm = new CreatePrizeForm(this);
            frm.Show();

        }

        /// <summary>
        /// Interface Helper to
        /// get back from the form PrizeFrom
        /// Take the prizemodel and put it into our list of selected prizes
        /// </summary>
        /// <param name="model"></param>
        public void PrizeComplete(PrizeModel model)
        {
            selectedPrizes.Add(model);
            WireUpLists();
        }
        /// <summary>
        /// Interface Helper to
        /// get back from the form CreateTeam
        /// Take the teammodel and put it into our list of selected teams
        /// </summary>
        /// <param name="model"></param>

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }
        /// <summary>
        /// Create a new team and add it to the team list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // call the CreateTeamFrom
            CreateTeamForm frm = new CreateTeamForm(this);
            frm.Show();
        }
        /// <summary>
        ///  Remove selected team from the team list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (t != null)
            {
                selectedTeams.Remove(t);
                availableTeams.Add(t);

                WireUpLists();
            }
        }
        /// <summary>
        /// Remove selected prize from the prize list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;

            if (p != null)
            {
                selectedPrizes.Remove(p);

                WireUpLists();
            }
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {

            if (ValidateForm())
            {            
                // create tournament entry
                // create all of the prizes entries
                // create all of the team entries

                TournamentModel model = new TournamentModel();
                model.TournamentName = tournamentNameValue.Text;
                model.EntryFee = decimal.Parse(entryFeeValue.Text);
                model.Prizes = selectedPrizes;
                model.EnteredTeams = selectedTeams;

                TournamentLogic.CreateRounds(model);

                model = GlobalConfig.Connection.CreateTournament(model);



            }
            // create matchups

            // Order our list randomly of teams
            // Check if it is big enough - if not, add in byes : 2^n power
            // create first round of macthups
            // create every round after that - filling in only parentMatchup, rounds

        }

        private bool ValidateForm()
        {
            decimal fee = 0;
            bool feeValid = decimal.TryParse(entryFeeValue.Text, out fee);
            if (!feeValid)
            {
                MessageBox.Show("You need to enter a valid Entry Fee.", "Invalid Fee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }


    }
}
