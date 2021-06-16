using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;

        List<int> rounds = new List<int>();
        List<MatchupModel> selectedMatchups = new List<MatchupModel>();
        //BindingList<int> rounds = new BindingList<int>();
        //BindingList<MatchupModel> selectedMatchups = new BindingList<MatchupModel>();


        BindingSource roundsBinding = new BindingSource();
        BindingSource matchupsBinding = new BindingSource();

        /// <summary>
        /// Form Initilizations
        /// </summary>
        /// <param name="model"></param>
        public TournamentViewerForm(TournamentModel model)
        {
            InitializeComponent();

          
            tournament = model;

            tournament.OnTournamentComplete += Tournament_OnTournamentComplete;

            LoadFormData();
            LoadRounds();
        }


        /// <summary>
        /// Close the form when event for TournamentComplete occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tournament_OnTournamentComplete(object sender, DateTime e)
        {
            this.Close();
        }

        /// <summary>
        /// Load Tournament Name to the form
        /// </summary>
        private void LoadFormData()
        {
            tournamentName.Text = tournament.TournamentName;

        }
        /// <summary>
        /// Wire up lists for the lists of Rounds
        /// </summary>
        private void WireUpRoundsLists()
        {
            //roundDropDown.DataSource = null;
            roundsBinding.DataSource = rounds;
            roundDropDown.DataSource = roundsBinding;


        }

        /// <summary>
        /// Wire up lists for the lists of Matchups
        /// </summary>
        private void WireUpMatchupList()
        {
            //matchupListBox.DataSource = null;
            matchupsBinding.DataSource = selectedMatchups;
            matchupListBox.DataSource = matchupsBinding;
            matchupListBox.DisplayMember = "DisplayName";

        }

        /// <summary>
        /// Load data for the list of Rounds
        /// </summary>
        private void LoadRounds()
        {
            rounds = new List<int>();
            //rounds.Clear();
            rounds.Add(1);
            int currRound = 1;

            foreach( List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    rounds.Add(currRound);
                }
            }
            WireUpRoundsLists();
            LoadMatchups();
        }

        /// <summary>
        /// Event handler for when roundDropDown selects another item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        /// <summary>
        /// Load data for the list of matchups
        /// </summary>
        private void LoadMatchups()
        {
            int round = (int)roundDropDown.SelectedItem;

            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    if (unplayedOnlyCheckbox.Checked)
                    {
                        List<MatchupModel> unplayedMatchups = new List<MatchupModel>();
                        foreach (MatchupModel m in matchups)
                        {
                            if (m.Winner == null)
                            {
                                unplayedMatchups.Add(m);
                            }
                        }

                        selectedMatchups = unplayedMatchups;
                        //selectedMatchups = new BindingList<MatchupModel>(unplayedMatchups);

                        break;

                    }
                    else
                    {
                        selectedMatchups = matchups;
                        //selectedMatchups = new BindingList<MatchupModel>(matchups);
                        break;
                    }

                }
            }

            WireUpMatchupList();
            LoadMatchupInfo();
        }

        /// <summary>
        /// Load data for the details about the selected matchup
        /// </summary>
        private void LoadMatchupInfo()
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            if (m == null)
            {
                DisplayMatchupInfo();
                return;
            }

            for (int i=0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[i].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[i].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = m.Entries[i].Score.ToString();

                        teamTwoName.Text = "<bye>";
                        teamTwoScoreValue.Text = "";
                    }
                    else
                    {
                        teamOneName.Text = "Not Yet Set";
                        teamOneScoreValue.Text = "";
                    }
                }
                if (i == 1)
                {
                    if (m.Entries[i].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[i].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = m.Entries[i].Score.ToString();
                    }
                    else
                    {
                        teamTwoName.Text = "Not Yet Set";
                        teamTwoScoreValue.Text = "";
                    }
                }
            }

            DisplayMatchupInfo();
        }
        /// <summary>
        /// Displays matchup info only if one is selected
        /// </summary>
        private void DisplayMatchupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);

            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;
            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;
            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }
        /// <summary>
        /// Event handler for when matchupListBox selects another item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchupInfo();
        }

        /// <summary>
        /// Event handler for when unplayedOnly checkbox state changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unplayedOnlyCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups();
        }

        /// <summary>
        /// Validate the data for the score on the form
        /// </summary>
        /// <returns></returns>
        private (bool, string) ValidateScore()
        {
            bool output = true;
            string error = "";

            double teamOneScore = 0;
            double teamTwoScore = 0;

            bool scoreOneValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);
            bool scoreTwoValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

            if (!scoreOneValid)
            {
                output = false;
                error = "The Score One value is not a valid number.";
            }
            else if (!scoreTwoValid)
            {
                output = false;
                error = "The Score Two value is not a valid number.";
            }
            else if (teamOneScore == teamTwoScore && teamOneScore == 0)
            {
                output = false;
                error = "You did not enter a score for either team.";
            }
            else if (teamOneScore == teamTwoScore)
            {
                output = false;
                error = "We do not allow ties in this application.";
            }

            return (output, error);
        }
        /// <summary>
        /// Event handler for when Score button is pressed
        /// Check if score is valid and update db data is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scoreButton_Click(object sender, EventArgs e)
        {
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            (bool validData, string err) = ValidateScore();

            if (!validData)
            {
                MessageBox.Show($"Input Error: {err} ");
                return;
                
            }
            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (m.Entries[i].TeamCompeting != null)
                    {
                        teamOneScore = double.Parse(teamOneScoreValue.Text);
                        m.Entries[i].Score = teamOneScore;

                    }
                }
                if (i == 1)
                {
                    if (m.Entries[i].TeamCompeting != null)
                    {
                        teamTwoScore = double.Parse(teamTwoScoreValue.Text);
                        m.Entries[i].Score = teamTwoScore;

                    }
                }
            }

            try
            {
                TournamentLogic.UpdateTournamentResults(tournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application had the following error: {ex.Message}");
                return;
            }
      
            LoadMatchups();
        }

    }
}
