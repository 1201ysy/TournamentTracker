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
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();

        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();

        ITeamRequester callingForm;
        /// <summary>
        /// Initialize Form
        /// </summary>
        /// <param name="caller"></param>
        public CreateTeamForm(ITeamRequester caller)
        {
            InitializeComponent();
            callingForm = caller;
            //CreateSampleData();
            WireUpLists();
        }
        /// <summary>
        /// Create Sample data to be used for dev
        /// </summary>
        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "FF", LastName = "LL" });
            availableTeamMembers.Add(new PersonModel { FirstName = "GG", LastName = "HH" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "AA", LastName = "CC" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "BB", LastName = "DD" });
        }

        /// <summary>
        /// Wire up the data for dropdwons and listbox with updated data
        /// </summary>
        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";

        }
        /// <summary>
        /// Validates the form for Create member button
        /// </summary>
        /// <returns></returns>
        private bool ValidateForm()
        {
            bool output = true;
            if (firstNameValue.Text.Length == 0)
            {
                output = false;
            }
            if (lastNameValue.Text.Length == 0)
            {
                output = false;
            }
            if (emailValue.Text.Length == 0)
            {
                output = false;
            }
            if (cellphoneValue.Text.Length == 0)
            {
                output = false;
            }

            return output;
        }
        /// <summary>
        /// Add new member data and add them to selected team member list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel model = new PersonModel();

                model.FirstName = firstNameValue.Text;
                model.LastName = lastNameValue.Text;
                model.EmailAddress = emailValue.Text;
                model.CellphoneNumber = cellphoneValue.Text;

                GlobalConfig.Connection.CreatePerson(model);

                selectedTeamMembers.Add(model);
                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again.");
            }
        }
        /// <summary>
        /// Add an selected member on to a team
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void addMemeberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);

                WireUpLists();
            }
        }
        /// <summary>
        /// Remove selected member from the team (list box)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);

                WireUpLists();
            }

        }
        /// <summary>
        /// Create team with the selcted team memeber list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel model = new TeamModel();

            model.TeamName = teamNameValue.Text;
            model.TeamMembers = selectedTeamMembers;

            GlobalConfig.Connection.CreateTeam(model);

            callingForm.TeamComplete(model);
            this.Close();
        }
    }
}
