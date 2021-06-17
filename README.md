# TournamentTracker
Exploring C#


Windows Form Application called TournamentTracker.

Requirements:
1. Tracks game played and their outcome (who won)
2. Multiple competitors play in the tournament
3. Creates a tournament plan (who plays in what order)
4. Schedules games
5. A single loss eliminates a player
6. The last player standing is the winner

Functions:
- Create new Tournament
  - with some Tournament Name (required)
  - Create new team / add existing teams to Tournament (required)
    - Add new member / add existing members to Teams
  - Set Entry Fee to enter the tournament in $ (optional)
  - Create new Prize using Entry Fee (optional)
    - 1st & 2nd prize only   
- Once all Tournament information is entered and new tournament is create:
  - Sets up all the matchups (randomized) and if not enough teams participating, add byes in the first round
  - Using the tournament viewer:
    - Filter matches by round # and unplayed matches
    - Users can enter match results using the viewer
    - match winners are advanced to the next round until tournamanet is finished (last team standing)
- Sends Emails when each round is complete and new round is starting
- Sends Emails for the result when the tournament is finished with the winner

Database supported for the Tournament:
- SQL : Microsoft SQL Server + SSMS
- TextFile : csv files 
