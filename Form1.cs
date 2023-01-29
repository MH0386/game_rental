using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GameRental
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gameRentalDataSet.RENT' table. You can move, or remove it, as needed.
            rENTTableAdapter.Fill(gameRentalDataSet.RENT);
            // TODO: This line of code loads data into the 'gameRentalDataSet.GAME' table. You can move, or remove it, as needed.
            gAMETableAdapter.Fill(gameRentalDataSet.GAME);
            // TODO: This line of code loads data into the 'gameRentalDataSet.CLIENT' table. You can move, or remove it, as needed.
            cLIENTTableAdapter.Fill(gameRentalDataSet.CLIENT);
            ActiveControl = logInUserName;
            NavBar.Hide();
            expandBtn.Hide();
        }
        
        private void addUser_Click(object sender, EventArgs e)
        {
            if (FName.Text == "")
            {
                MessageBox.Show(@"Please Enter Your First Name", @"Error");
                return;
            }

            if (LName.Text == "")
            {
                MessageBox.Show(@"Please Enter Your Last Name", @"Error");
                return;
            }

            if (userName.Text == "")
            {
                MessageBox.Show(@"Please Enter a Valid User Name", @"Error");
                return;
            }

            if (Password.Text == "")
            {
                MessageBox.Show(@"Please Enter a Valid Password", @"Error");
                return;
            }

            if (Password.TextLength < 5)
            {
                MessageBox.Show(@"Please Enter a Longer Password", @"Error");
                return;
            }


            SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText = $"INSERT INTO CLIENT(FNAME, LNAME, USERNAME, PASSWORD, ADMIN) VALUES('{FName.Text}', ' {LName.Text} ', ' {userName.Text} ', ' {Password.Text} ',  {(isAdmin.Checked ? "1" : "0")} )";

            int success;

            try
            {
                success = sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            MessageBox.Show((success == 1 ? "User Has Been Added Successfully" : "User Name Has Been Already Taken"));

            sConnetion.Close();

            if (Authority2.Text == @"Unknown")
            {
                LogInPanel.BringToFront();
                LogInPanel.Show();
                ActiveControl = logInUserName;
            }

            addUserPanel.SendToBack();
        }

        private string _username;
        private string _password;
        private string _firstName;
        private string _lastName;

        private void CheckUser()
        {
            _username = logInUserName.Text;
            _password = logInPassword.Text;

            if (_username.Length > 0 && _password.Length > 0)
            {
                SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                SqlCommand sCommand = new SqlCommand();
                sCommand.Connection = sConnetion;

                sConnetion.Open();


                sCommand.CommandText =
                    $"SELECT PASSWORD, ADMIN, FName, LName FROM CLIENT WHERE USERNAME = '{_username}'";

                SqlDataReader sReader = sCommand.ExecuteReader();

                if (sReader.Read())
                {
                    string psd = sReader["PASSWORD"].ToString();
                    string isAdmin = sReader["ADMIN"].ToString();
                    _firstName = sReader["FName"].ToString();
                    _lastName = sReader["LName"].ToString();

                    sConnetion.Close();

                    if (_password == psd)
                    {
                        LogInPanel.Hide();
                        Back.Hide();
                        Authority2.Text = (isAdmin == "True" ? "Admin" : "Client");
                        currentUserAuthority.Text = Authority2.Text;

                        currentUserName2.Text = $@"{_firstName} {_lastName}";
                        currentUserName.Text = currentUserName2.Text;

                        logInUserName.Text = "";
                        logInPassword.Text = "";

                        NavBar.Show();
                        expandBtn.Show();
                        if (Authority2.Text == @"Admin")
                        {
                            sCommand = new SqlCommand("SELECT * FROM CLIENT");
                            SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                            sCommand.Connection = sConnection;
                            SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                            DataTable dt = new DataTable();
                            sDataAdapter.Fill(dt);
                            dataGridView1.DataSource = dt;

                            AddRenting.Hide();
                            AddNewGame.Show();
                            AddNewUser.Width = 318;
                            AddNewUser.Location = new Point(9, 3);

                            AddNewGame.Width = 318;
                            AddNewGame.Location = new Point(330, 3);

                            dataGridView1.AllowUserToAddRows = true;
                            dataGridView1.AllowUserToDeleteRows = true;
                            dataGridView1.ReadOnly = false;

                            dataGridView2.AllowUserToAddRows = true;
                            dataGridView2.AllowUserToDeleteRows = true;
                            dataGridView2.ReadOnly = false;

                            DeleteGame.Visible = true;

                            dataGridView3.AllowUserToAddRows = true;
                            dataGridView3.AllowUserToDeleteRows = true;
                            dataGridView3.ReadOnly = false;

                            DeleteGame.Visible = true;
                            button2.Visible = true;

                            return;
                        }

                        AddNewGame.Hide();
                        AddRenting.Show();
                        AddNewUser.Width = 318;
                        AddNewUser.Location = new Point(9, 3);

                        AddRenting.Width = 318;
                        AddRenting.Location = new Point(330, 3);

                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.AllowUserToDeleteRows = false;
                        dataGridView1.ReadOnly = true;

                        dataGridView2.AllowUserToAddRows = false;
                        dataGridView2.AllowUserToDeleteRows = false;
                        dataGridView2.ReadOnly = true;

                        DeleteGame.Visible = false;

                        dataGridView3.AllowUserToAddRows = false;
                        dataGridView3.AllowUserToDeleteRows = false;
                        dataGridView3.ReadOnly = true;

                        DeleteGame.Visible = false;
                        button2.Visible = false;

                        return;
                    }
                }
                MessageBox.Show(@"Wrong User Name or Password", @"Error");

            }
            else
            {
                MessageBox.Show(@"Please Enter Your User Name & Password", @"Error");
            }
        }

        private static bool IsFloatOrInt(string value)
        {
            int intValue;
            float floatValue;
            return int.TryParse(value, out intValue) || float.TryParse(value, out floatValue);
        }

        private void AddGame()
        {
            if (gName.TextLength == 0)
            {
                MessageBox.Show(@"Please Enter The Name Of The Game", @"Error");
                return;
            }

            if (gCat.TextLength == 0)
            {
                MessageBox.Show(@"Please Enter The Category Name Of This Game", @"Error");
                return;
            }

            if (gRent.TextLength == 0)
            {
                MessageBox.Show(@"Please Enter The Rent Price Of The Game", @"Error");
                return;
            }

            if (!(IsFloatOrInt(gRent.Text)))
            {
                MessageBox.Show(@"Please Enter a Numeric Value for the Price Of The Game", @"Error");
                return;
            }

            if (gVen.TextLength == 0)
            {
                MessageBox.Show(@"Please Enter The Vendor Name Of The Game", @"Error");
                return;
            }

            SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText =
                $"INSERT INTO GAME (GNAME, CATEGORY, RENT_PRICE_PER_DAY, VENDORNAME, RELEASEDATE) VALUES ('{gName.Text}', '{gCat.Text}', {gRent.Text}, '{gVen.Text}', '{gDateReleased.Text}')";

            try
            {
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
                return;
            }

            MessageBox.Show(@"Successfully Added", @"Success");
            homePanel.BringToFront();

            sConnetion.Close();
        }

        private void homePage_Click(object sender, EventArgs e)
        {
            homePanel.BringToFront();
        }

        private void usersPage_Click(object sender, EventArgs e)
        {
            usersPanel.BringToFront();
            cLIENTTableAdapter.Fill(gameRentalDataSet.CLIENT);

            if (Authority2.Text != @"Client") return;
            SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE USERNAME = '" + logInUserName.Text + "'");
            SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            sCommand.Connection = sConnection;

            SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

            DataTable dt = new DataTable();
            sDataAdapter.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.ReadOnly = true;
        }

        private void gamesPage_Click(object sender, EventArgs e)
        {
            GamesPanel.BringToFront();
            gAMETableAdapter.Fill(gameRentalDataSet.GAME);
        }

        private void rentsPage_Click(object sender, EventArgs e)
        {
            SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT");
            SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            sCommand.Connection = sConnection;
            SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
            DataTable dt = new DataTable();
            sDataAdapter.Fill(dt);
            dataGridView3.DataSource = dt;
            RentsPanel.BringToFront();
        }

        private void aboutPage_Click(object sender, EventArgs e)
        {
            AboutUSPanel.BringToFront();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void restoreBtn_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void minBtn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void AddNewUser_Click(object sender, EventArgs e)
        {
            addUserPanel.BringToFront();
            ActiveControl = FNameLabel;
        }

        private void expandBtn_Click(object sender, EventArgs e)
        {
            if (NavBar.Width == 65)
            {
                NavBar.Width = 157;

                homePage.RightToLeft = RightToLeft.Yes;
                usersPage.RightToLeft = RightToLeft.Yes;
                gamesPage.RightToLeft = RightToLeft.Yes;
                rentsPage.RightToLeft = RightToLeft.Yes;
                aboutPage.RightToLeft = RightToLeft.Yes;
                signUp.Visible = true;
                CurrentUser.Visible = true;
                currentUserName.Visible = true;
            }
            else
            {
                NavBar.Width = 65;

                homePage.RightToLeft = RightToLeft.No;
                usersPage.RightToLeft = RightToLeft.No;
                gamesPage.RightToLeft = RightToLeft.No;
                rentsPage.RightToLeft = RightToLeft.No;
                aboutPage.RightToLeft = RightToLeft.No;
                signUp.Visible = false;
                CurrentUser.Visible = false;
                currentUserName.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckUser();
        }

        private void logInUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckUser();
            }
        }

        private void currentUserName_Click(object sender, EventArgs e)
        {
            addUserPanel.BringToFront();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            LogInPanel.BringToFront();
            ActiveControl = logInUserName;
        }

        private void UpdateInfo_Click(object sender, EventArgs e)
        {
            FN2.Text = _firstName;
            LN2.Text = _lastName;
            if (Authority2.Text == @"Admin")
            {
                isAdmin2.Checked = true;
            }
            UN2.Text = _username;
            PW2.Text = _password;

            EditDataPanel.BringToFront();
        }

        private void UpdateInformation()
        {
            SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText = "UPDATE CLIENT SET FNAME = '" + FN2.Text + "' WHERE USERNAME = '" + _username + "'; UPDATE CLIENT SET LNAME = '" + LN2.Text + "'  WHERE USERNAME = '" + _username + "' ; UPDATE CLIENT SET PASSWORD = '" + PW2.Text + "' WHERE USERNAME = '" + _username + "'; UPDATE CLIENT SET ADMIN = " + (isAdmin2.Checked ? 1 : 0) + " WHERE USERNAME = '" + _username + "';";

            try
            {
                sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
                return;
            }

            MessageBox.Show(@"Successfully Edited", @"Success");
            homePanel.BringToFront();

            currentUserName.Text = FN2.Text + ' ' + LN2.Text;
            currentUserName2.Text = currentUserName.Text;

            currentUserAuthority.Text = (isAdmin2.Checked ? "Admin" : "Client");
            Authority2.Text = currentUserAuthority.Text;

            sConnetion.Close();
        }

        private void UpdateGameInfo()
        {
            SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            int numOfRowsAffected;

            sCommand.CommandText = "UPDATE GAME SET CATEGORY = '" + GameCategory.Text + "' WHERE GNAME = '" + GameName.Text + "'; UPDATE GAME SET RENT_PRICE_PER_DAY = '" + GamePrice.Text + "' WHERE GNAME = '" + GameName.Text + "'; UPDATE GAME SET VENDORNAME = '" + GameVendor.Text + "' WHERE GNAME = '" + GameName.Text + "'; UPDATE GAME SET RELEASEDATE = '" + GameDate.Text + "' WHERE GNAME = '" + GameName.Text + "';";

            try
            {
                numOfRowsAffected = sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
                return;
            }

            if (numOfRowsAffected > 0)
            {
                MessageBox.Show(@"Successfully Edited", @"Success");
                homePanel.BringToFront();
            }
            else
            {
                MessageBox.Show(@"Wrong Game Name", @"Error");
            }


            sConnetion.Close();
        }

        private void DeleteAGame()
        {
            SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText = "DELETE FROM Game WHERE GNAME = '" + GameToDelete.Text + "';";

            int numOfRowsAffected;
            try
            {
                numOfRowsAffected = sCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
                return;
            }

            if (numOfRowsAffected > 0)
            {
                MessageBox.Show(@"Successfully Deleted", @"Success");
                GameToDelete.Text = "";
                gAMETableAdapter.Fill(gameRentalDataSet.GAME);
            }
            else
            {
                MessageBox.Show(@"No Game With Such Name", @"Error");
            }

            sConnetion.Close();

            GameToDelete.Visible = false;
        }

        private void ApplyChanges_Click(object sender, EventArgs e)
        {
            UpdateInformation();
        }

        private void DeleteMyInfo_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(@"Continue Deleting Your Account?", @"Sure?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                SqlCommand sCommand = new SqlCommand();
                sCommand.Connection = sConnetion;

                sConnetion.Open();

                sCommand.CommandText = $"DELETE FROM CLIENT WHERE USERNAME = '{_username}';";


                try
                {
                    sCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Error");
                    return;
                }

                MessageBox.Show(@"Successfully Deleted", @"Success");
                NavBar.Hide();
                LogInPanel.BringToFront();
                LogInPanel.Show();
                ActiveControl = logInUserName;

                currentUserName.Text = FN2.Text + ' ' + LN2.Text;
                currentUserName2.Text = currentUserName.Text;

                currentUserAuthority.Text = (isAdmin2.Checked ? "Admin" : "Client");
                Authority2.Text = currentUserAuthority.Text;

                sConnetion.Close();
            }
            else
            {
            }
        }

        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(@"Log Out From Your Account?", @"Sure?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                NavBar.Hide();
                LogInPanel.BringToFront();
                LogInPanel.Show();
                ActiveControl = logInUserName;
            }
            else
            {
            }
        }

        private void FN2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateInformation();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddGame();
        }

        private void gName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddGame();
            }
        }

        private void AddNewGame_Click(object sender, EventArgs e)
        {
            NewGamePanel.BringToFront();
        }

        private void SearchGameBtn_Click(object sender, EventArgs e)
        {
            SearchGameBtn.Hide();
            ActiveControl = GamesFilters;
        }

        private void GamesFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamesFilters.Text == @"None")
            {
                SearchGameBtn.Show();

                gameSearchBx1.Visible = false;

                gameSearchBx2.Visible = false;
                gameSearchBx3.Visible = false;

                GameDateFrom.Visible = false;
                GameDateTo.Visible = false;

                Attribute.Visible = false;

                Between.Visible = false;
                And.Visible = false;

                gAMETableAdapter.Fill(gameRentalDataSet.GAME);
                dataGridView2.DataSource = gAMETableAdapter.GetData();
            }
            else if (GamesFilters.Text == @"Name")
            {
                gameSearchBx1.Visible = true;

                gameSearchBx2.Visible = false;
                gameSearchBx3.Visible = false;

                GameDateFrom.Visible = false;
                GameDateTo.Visible = false;

                Attribute.Text = GamesFilters.Text + ':';
                Attribute.Visible = true;

                Between.Visible = false;
                And.Visible = false;
            }
            else if (GamesFilters.Text == @"Category")
            {
                gameSearchBx1.Visible = true;

                gameSearchBx2.Visible = false;
                gameSearchBx3.Visible = false;

                GameDateFrom.Visible = false;
                GameDateTo.Visible = false;

                Attribute.Text = GamesFilters.Text + ':';
                Attribute.Visible = true;

                Between.Visible = false;
                And.Visible = false;
            }
            else if (GamesFilters.Text == @"Price")
            {
                gameSearchBx1.Visible = false;

                gameSearchBx2.Visible = true;
                gameSearchBx3.Visible = true;

                GameDateFrom.Visible = false;
                GameDateTo.Visible = false;

                Attribute.Visible = false;

                Between.Visible = true;
                And.Visible = true;
            }
            else if (GamesFilters.Text == @"Vendor")
            {
                gameSearchBx1.Visible = true;

                gameSearchBx2.Visible = false;
                gameSearchBx3.Visible = false;

                GameDateFrom.Visible = false;
                GameDateTo.Visible = false;

                Attribute.Text = GamesFilters.Text + ':';
                Attribute.Visible = true;

                Between.Visible = false;
                And.Visible = false;
            }
            else if (GamesFilters.Text == @"Release Date")
            {
                gameSearchBx1.Visible = false;

                GameDateFrom.Visible = true;
                GameDateTo.Visible = true;

                Attribute.Visible = false;

                Between.Visible = true;
                And.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditGamePanel.BringToFront();
        }

        private void GamesFilters_Leave(object sender, EventArgs e)
        {
            if (GamesFilters.Text == @"None")
            {
                SearchGameBtn.Show();
            }
        }

        private string ReturnName()
        {
            switch (GamesFilters.Text)
            {
                case "Name":
                    return "GNAME";
                case "Vendor":
                    return "VENDORNAME";
                default:
                    return "CATEGORY";
            }
        }

        private void SearchGame()
        {
            switch (GamesFilters.Text)
            {
                case @"Name":
                case @"Vendor":
                case @"Category":
                {
                    SqlCommand sCommand = new SqlCommand(
                        $"SELECT * FROM GAME WHERE {ReturnName()} LIKE '%{gameSearchBx1.Text}%'");

                    SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                    sCommand.Connection = sConnection;

                    SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                    DataTable dt = new DataTable();

                    sDataAdapter.Fill(dt);
                    dataGridView2.DataSource = dt;
                    dataGridView2.ReadOnly = true;
                    break;
                }
                case @"Price":
                {
                    SqlCommand sCommand;


                    if (gameSearchBx2.TextLength > 0 && gameSearchBx3.TextLength > 0)
                    {
                        sCommand = new SqlCommand("select * from GAME where RENT_PRICE_PER_DAY >= " + gameSearchBx2.Text + " AND RENT_PRICE_PER_DAY <= " + gameSearchBx3.Text);
                    }
                    else if (gameSearchBx2.TextLength > 0 && gameSearchBx3.TextLength == 0)
                    {
                        sCommand = new SqlCommand("select * from GAME where RENT_PRICE_PER_DAY >= " + gameSearchBx2.Text);
                    }
                    else if (gameSearchBx2.TextLength == 0 && gameSearchBx3.TextLength > 0)
                    {
                        sCommand = new SqlCommand("select * from GAME where RENT_PRICE_PER_DAY <= " + gameSearchBx3.Text);
                    }
                    else
                    {
                        gAMETableAdapter.Fill(gameRentalDataSet.GAME);
                        return;
                    }
                    SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                    sCommand.Connection = sConnection;

                    SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                    DataTable dt = new DataTable();
                    sDataAdapter.Fill(dt);
                    dataGridView2.DataSource = dt;
                    dataGridView2.ReadOnly = true;
                    break;
                }
                case @"Release Date":
                {
                    SqlCommand sCommand = new SqlCommand(
                        $"select * from GAME where RELEASEDATE >= '{GameDateFrom.Text}' and RELEASEDATE <= '{GameDateTo.Text}'");


                    SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                    sCommand.Connection = sConnection;

                    SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                    DataTable dt = new DataTable();
                    sDataAdapter.Fill(dt);
                    dataGridView2.DataSource = dt;
                    dataGridView2.ReadOnly = true;
                    break;
                }
            }
        }

        private void GameDateTo_ValueChanged(object sender, EventArgs e)
        {
            SearchGame();
        }

        private void gameSearchBx1_KeyDown(object sender, EventArgs e)
        {
            SearchGame();
        }

        private void GameName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateGameInfo();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateGameInfo();
        }

        private void DeleteGame_Click(object sender, EventArgs e)
        {
            if (GameToDelete.Visible == false)
            {
                GameToDelete.Visible = true;
            }
            else
            {
                if (GameToDelete.TextLength > 0)
                {
                    DeleteAGame();
                }

                GameToDelete.Visible = false;
            }
        }

        private void GameToDelete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && GameToDelete.TextLength > 0)
            {
                DeleteAGame();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (GameToDelete.Visible)
            {
                GameToDelete.Text = dataGridView2.SelectedCells[0].Value.ToString();
            }
        }

        private void RentsSearch_TextChanged(object sender, EventArgs e)
        {
            switch (RentsSearch.Text)
            {
                case @"Game Name":
                case @"User Name":
                    RentSearchAttr.Text = RentsSearch.Text + ':';
                    RentSearchAttr.Visible = true;

                    RentsTextBox.Visible = true;

                    RentsDateFrom.Visible = false;
                    RentsDateTo.Visible = false;

                    AllReturnedGames.Visible = false;
                    AllNotReturnedGames.Visible = false;

                    Between2.Visible = false;
                    And2.Visible = false;
                    break;
                case "Return Date":
                case "Date":
                    RentSearchAttr.Visible = false;

                    RentsTextBox.Visible = false;

                    RentsDateFrom.Visible = true;
                    RentsDateTo.Visible = true;

                    AllReturnedGames.Visible = false;
                    AllNotReturnedGames.Visible = false;

                    Between2.Visible = true;
                    And2.Visible = true;
                    break;
                case "Rent Status":
                    RentSearchAttr.Visible = false;

                    RentsTextBox.Visible = false;

                    RentsDateFrom.Visible = false;
                    RentsDateTo.Visible = false;

                    AllReturnedGames.Visible = true;
                    AllNotReturnedGames.Visible = true;

                    Between2.Visible = false;
                    And2.Visible = false;
                    break;
                default:
                {
                    RentsSerachBtn.Visible = true;

                    RentSearchAttr.Visible = false;

                    RentsTextBox.Visible = false;

                    RentsDateFrom.Visible = false;
                    RentsDateTo.Visible = false;

                    AllReturnedGames.Visible = false;
                    AllNotReturnedGames.Visible = false;

                    Between2.Visible = false;
                    And2.Visible = false;

                    SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT");
                    SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                    sCommand.Connection = sConnection;
                    SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                    DataTable dt = new DataTable();
                    sDataAdapter.Fill(dt);
                    dataGridView3.DataSource = dt;
                    break;
                }
            }
        }

        private void RentsTextBox_TextChanged(object sender, EventArgs e)
        {
            switch (RentsSearch.Text)
            {
                case "Game Name":
                {
                    SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE GNAME LIKE '%" + RentsTextBox.Text + "%'");
                    SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                    sCommand.Connection = sConnection;
                    SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                    DataTable dt = new DataTable();
                    sDataAdapter.Fill(dt);
                    dataGridView3.DataSource = dt;
                    break;
                }
                case "User Name":
                {
                    SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE USERNAME LIKE '%" + RentsTextBox.Text + "%'");
                    SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                    sCommand.Connection = sConnection;
                    SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                    DataTable dt = new DataTable();
                    sDataAdapter.Fill(dt);
                    dataGridView3.DataSource = dt;
                    break;
                }
            }
        }

        private void AllNotReturnedGames_CheckedChanged(object sender, EventArgs e)
        {
            if (RentsSearch.Text == @"Rent Status")
            {
                SqlCommand sCommand = AllReturnedGames.Checked ? new SqlCommand("SELECT * FROM RENT WHERE ISRETURNED = 1") : new SqlCommand("SELECT * FROM RENT WHERE ISRETURNED != 1");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
        }

        private void AddRenting_Click(object sender, EventArgs e)
        {
            AddRentingPanel.BringToFront();
        }

        private void dataGridView4_SelectionChanged(object sender, EventArgs e)
        {
            NewRentGameSearch.Text = dataGridView4.SelectedCells[0].Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(@"Continue Renting Process?", @"Sure?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                DateTime dt1 = DateTime.Now.Date;

                SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                SqlCommand sCommand = new SqlCommand();
                sCommand.Connection = sConnetion;

                sConnetion.Open();

                sCommand.CommandText = "INSERT INTO RENT(GNAME, USERNAME, DATE) VALUES('" + NewRentGameSearch.Text + "', '" + _username + "', '" + dt1 + "')";

                int success;

                try
                {
                    success = sCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                MessageBox.Show((success == 1 ? "Process Has Been Done Successfully" : "No Game Has Such Name"));

                sConnetion.Close();
            }
            else
            {
            }
        }

        private void GetUpdatedReport()
        {
            ReportText.Text = @"The Most Interesting Game is ";

            SqlConnection sConnetion = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText = "SELECT TOP 1 GNAME AS TOPGAME, COUNT(USERNAME) AS RENTERS FROM RENT GROUP BY GNAME ORDER BY COUNT(USERNAME) DESC";

            SqlDataReader sReader = sCommand.ExecuteReader();

            if (sReader.Read())
            {
                ReportText.Text = ReportText.Text + sReader["TOPGAME"] + @" With " + sReader["RENTERS"] + " Renters.\r\n\r\n";
            }

            sConnetion.Close();

            //----------------------------------------------------
            ReportText.Text += "The Games That Didn't Have Any Renting in The Last 30 Days Are:\r\n";

            SqlConnection sConnetion1 = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand1 = new SqlCommand();
            sCommand1.Connection = sConnetion1;

            sConnetion1.Open();

            sCommand1.CommandText = "SELECT GNAME FROM GAME  EXCEPT SELECT GNAME FROM RENT WHERE RENT.DATE >= DATEADD(DAY, -30, GETDATE())";

            SqlDataReader sReader1 = sCommand1.ExecuteReader();

            while (sReader1.Read())
            {
                ReportText.Text = ReportText.Text + sReader1["GNAME"] + @", ";
            }
            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------
            ReportText.Text += "The Games That Didn't Have Any Renting in The Last Month Are:\r\n";

            SqlConnection sConnetion2 = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand2 = new SqlCommand();
            sCommand2.Connection = sConnetion2;

            sConnetion2.Open();

            sCommand2.CommandText = "SELECT GNAME FROM GAME EXCEPT SELECT GNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE())";

            SqlDataReader sReader2 = sCommand2.ExecuteReader();

            while (sReader2.Read())
            {
                ReportText.Text = ReportText.Text + sReader2["GNAME"] + @", ";
            }
            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------
            ReportText.Text += @"The Client With The Maximum Number Of Renting Last Month is: ";

            SqlConnection sConnetion3 = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand3 = new SqlCommand();
            sCommand3.Connection = sConnetion3;

            sConnetion3.Open();

            sCommand3.CommandText = "SELECT CLIENT.FNAME AS FN, CLIENT.LNAME AS LN, COUNT(GNAME) AS RENTS FROM CLIENT, RENT WHERE CLIENT.USERNAME = RENT.USERNAME AND RENT.USERNAME IN(SELECT TOP 1 USERNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()) GROUP BY USERNAME ORDER BY COUNT(GNAME) DESC) GROUP BY FNAME, LNAME";

            SqlDataReader sReader3 = sCommand3.ExecuteReader();

            if (sReader3.Read())
            {
                ReportText.Text =
                    $"{ReportText.Text}{sReader3["FN"]} {sReader3["LN"]} With {sReader3["RENTS"]} Rentings.\r\n\r\n";
            }

            //----------------------------------------------------
            ReportText.Text += @"The Vendor With The Maximum Renting out Last Month is: ";

            SqlConnection sConnetion4 = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand4 = new SqlCommand();
            sCommand4.Connection = sConnetion4;

            sConnetion4.Open();

            sCommand4.CommandText = "SELECT TOP 1 VENDORNAME AS VENDOR, COUNT(RENT.USERNAME) AS RENTS FROM RENT JOIN GAME ON RENT.GNAME = GAME.GNAME WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()) GROUP BY VENDORNAME ORDER BY COUNT(VENDORNAME) DESC";

            SqlDataReader sReader4 = sCommand4.ExecuteReader();

            if (sReader4.Read())
            {
                ReportText.Text = ReportText.Text + sReader4["VENDOR"] + @" With " + sReader4["RENTS"] + " Rentings.\r\n\r\n";
            }

            //----------------------------------------------------
            ReportText.Text += "The Vendors Whose Games Didn't Have Any Renting in The Last Month Are:\r\n";

            SqlConnection sConnetion5 = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand5 = new SqlCommand();
            sCommand5.Connection = sConnetion5;

            sConnetion5.Open();

            sCommand5.CommandText = "SELECT GAME.VENDORNAME AS VENDOR FROM GAME EXCEPT SELECT GAME.VENDORNAME FROM GAME WHERE GNAME IN(SELECT GNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()))";

            SqlDataReader sReader5 = sCommand5.ExecuteReader();

            while (sReader5.Read())
            {
                ReportText.Text = ReportText.Text + sReader5["VENDOR"] + @", ";
            }
            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------
            ReportText.Text += "The Vendors Who Dind't Add Any Games Last Year Are:\r\n";

            SqlConnection sConnetion6 = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
            SqlCommand sCommand6 = new SqlCommand();
            sCommand6.Connection = sConnetion6;

            sConnetion6.Open();

            sCommand6.CommandText = "SELECT GAME.VENDORNAME AS VENDOR FROM GAME EXCEPT SELECT GAME.VENDORNAME FROM GAME WHERE GNAME IN(SELECT GNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()))";

            SqlDataReader sReader6 = sCommand6.ExecuteReader();

            while (sReader6.Read())
            {
                ReportText.Text = ReportText.Text + sReader6["VENDOR"] + @", ";
            }
            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------

            ReportText.Text += "End Of Report\r\n    Thanks!";
        }

        private void ViewReport_Click(object sender, EventArgs e)
        {
            QuickReportPanel.BringToFront();
            GetUpdatedReport();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "Sql Statements.txt");
        }

        private void RentsSearchBtn_Click(object sender, EventArgs e)
        {
            RentsSerachBtn.Visible = false;
        }

        private void UserSearchBtn_Click(object sender, EventArgs e)
        {
            UserSearchBtn.Visible = false;
        }

        private void UserSearchBox_Leave(object sender, EventArgs e)
        {
            UserSearchBtn.Visible = true;
            UserSearchAttr.Visible = false;
        }

        private void UserSearchOption_TextChanged(object sender, EventArgs e)
        {
            UserSearchAttr.Text = $@"{UserSearchOption.Text}:";

            if (UserSearchOption.Text == @"First Name" || UserSearchOption.Text == @"Last Name" || UserSearchOption.Text == @"User Name")
            {

                UserSearchAttr.Visible = true;

                AllAdmins.Visible = false;
                NotAdmins.Visible = false;

                UserSearchBox.Visible = true;

            }
            else if(UserSearchOption.Text == @"Authority")
            {

                UserSearchBtn.Visible = true;

                AllAdmins.Visible = true;
                NotAdmins.Visible = true;

                UserSearchBox.Visible = false;
            }
            else
            {
                UserSearchAttr.Visible = false;

                AllAdmins.Visible = false;
                NotAdmins.Visible = false;

                UserSearchBox.Visible = false;

                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
        }

        private void UserSearchBox_TextChanged(object sender, EventArgs e)
        {
            if (UserSearchOption.Text == @"First Name")
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE FNAME LIKE '%" + UserSearchBox.Text + "%'");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
            else if (UserSearchOption.Text == @"Last Name")
            {
                SqlCommand sCommand = new SqlCommand($"SELECT * FROM CLIENT WHERE LNAME LIKE '%{UserSearchBox.Text}%'");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
            else if (UserSearchOption.Text == @"User Name")
            {
                SqlCommand sCommand = new SqlCommand(
                    $"SELECT * FROM CLIENT WHERE USERNAME LIKE '%{UserSearchBox.Text}%'");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
        }

        private void AllAdmins_CheckedChanged(object sender, EventArgs e)
        {
            if(AllAdmins.Checked)
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE ADMIN = 1");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
        }

        private void NotAdmins_CheckedChanged(object sender, EventArgs e)
        {
            if(NotAdmins.Checked)
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE ADMIN != 1");

                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
        }

        private void RentsDateTo_ValueChanged(object sender, EventArgs e)
        {
            if(RentsSearch.Text == @"Date")
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE DATE >= '" + RentsDateFrom.Text + "' AND DATE <= '" + RentsDateTo.Text + "'");
                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
            else
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE RETURNDATE >= '" + RentsDateFrom.Text + "' AND DATE <= '" + RentsDateTo.Text + "'");
                SqlConnection sConnection = new SqlConnection("Data Source=.;Initial Catalog=gameRental;Integrated Security=True");
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
            
        }

        private void n1_Click(object sender, EventArgs e)
        {

        }
    }
}
    

