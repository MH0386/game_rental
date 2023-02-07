using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GameRental
{
    public partial class Form1 : Form
    {
        //Minimize from taskbar
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;

        ////Moving by Drag
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        string ConString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\GameRental.mdf;Integrated Security=True";

        string FirstName;
        string LastName;
        string pw;

        string UN;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true); // this is to avoid visual artifacts
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gameRentalDataSet.RENT' table. You can move, or remove it, as needed.
            this.rENTTableAdapter.Fill(this.gameRentalDataSet.RENT);
            // TODO: This line of code loads data into the 'gameRentalDataSet.CLIENT' table. You can move, or remove it, as needed.
            this.cLIENTTableAdapter.Fill(this.gameRentalDataSet.CLIENT);
            // TODO: This line of code loads data into the 'gameRentalDataSet.GAME' table. You can move, or remove it, as needed.
            this.gAMETableAdapter.Fill(this.gameRentalDataSet.GAME);


            this.ActiveControl = logInUserName;
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


            SqlConnection sConnetion = new SqlConnection(ConString);
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText = $"INSERT INTO CLIENT(FNAME, LNAME, USERNAME, PASSWORD, ADMIN) VALUES('" +
                                   FName.Text + "', '" + LName.Text + "', '" + userName.Text + "', '" + Password.Text +
                                   "', " +
                                   (isAdmin.Checked ? "1" : "0") + ")";

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

            if (Authority2.Text == "Unkown")
            {
                LogInPanel.BringToFront();
                LogInPanel.Show();
                this.ActiveControl = logInUserName;
            }

            addUserPanel.SendToBack();
        }

        public void checkUser()
        {
            UN = logInUserName.Text;
            pw = logInPassword.Text;

            if (UN.Length > 0 && pw.Length > 0)
            {
                SqlConnection sConnetion = new SqlConnection(ConString);
                SqlCommand sCommand = new SqlCommand();
                sCommand.Connection = sConnetion;

                sConnetion.Open();


                sCommand.CommandText = "SELECT PASSWORD, ADMIN, FName, LName FROM CLIENT WHERE USERNAME = '" + UN + "'";

                SqlDataReader sReader;
                sReader = sCommand.ExecuteReader();

                if (sReader.Read())
                {
                    string psd = sReader["PASSWORD"].ToString();
                    string isAdmin = sReader["ADMIN"].ToString();
                    FirstName = sReader["FName"].ToString();
                    LastName = sReader["LName"].ToString();

                    sConnetion.Close();

                    if (pw == psd)
                    {
                        LogInPanel.Hide();
                        Back.Hide();
                        Authority2.Text = (isAdmin == "True" ? "Admin" : "Client");
                        currentUserAuthority.Text = Authority2.Text;

                        currentUserName2.Text = FirstName + ' ' + LastName;
                        currentUserName.Text = currentUserName2.Text;

                        logInUserName.Text = "";
                        logInPassword.Text = "";

                        NavBar.Show();
                        expandBtn.Show();
                        if (Authority2.Text == "Admin")
                        {
                            sCommand = new SqlCommand("SELECT * FROM CLIENT");
                            SqlConnection sConnection = new SqlConnection(ConString);
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

                            UserSearchBtn.Visible = true;
                            UserSearchOption.Visible = true;

                            AddGamebtn2.Visible = true;

                            AddRentingbtn2.Visible = false;

                            return;
                        }
                        else
                        {
                            sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE USERNAME = '" + UN + "'");
                            SqlConnection sConnection = new SqlConnection(ConString);
                            sCommand.Connection = sConnection;
                            SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                            DataTable dt = new DataTable();
                            sDataAdapter.Fill(dt);
                            dataGridView1.DataSource = dt;

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

                            UserSearchBtn.Visible = false;
                            UserSearchOption.Visible = false;

                            AddGamebtn2.Visible = false;

                            AddRentingbtn2.Visible = true;

                            return;
                        }
                    }
                }

                MessageBox.Show(@"Wrong User Name or Password", @"Error");
            }
            else
            {
                MessageBox.Show(@"Please Enter Your User Name & Password", @"Error");
            }
        }

        public bool isFloatOrInt(string value)
        {
            int intValue;
            float floatValue;

            return Int32.TryParse(value, out intValue) || float.TryParse(value, out floatValue);
        }

        public void addGame()
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

            if (!(isFloatOrInt(gRent.Text)))
            {
                MessageBox.Show(@"Please Enter a Numeric Value for the Price Of The Game", @"Error");
                return;
            }

            if (gVen.TextLength == 0)
            {
                MessageBox.Show(@"Please Enter The Vendor Name Of The Game", @"Error");
                return;
            }

            SqlConnection sConnetion = new SqlConnection(ConString);
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText =
                "INSERT INTO GAME (GNAME, CATEGORY, RENT_PRICE_PER_DAY, VENDORNAME, RELEASEDATE) VALUES ('" +
                gName.Text + "', '" + gCat.Text + "', " + gRent.Text + ", '" + gVen.Text + "', '" + gDateReleased.Text +
                "')";

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

            if (Authority2.Text == "Client")
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE USERNAME = '" + UN + "'");
                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void gamesPage_Click(object sender, EventArgs e)
        {
            GamesPanel.BringToFront();

            SqlCommand sCommand = new SqlCommand("SELECT * FROM GAME");
            SqlConnection sConnection = new SqlConnection(ConString);
            sCommand.Connection = sConnection;
            sConnection.Open();

            SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
            //sDataAdapter.Equals(dataGridView2.SelectedRows[0]);

            DataTable dt = new DataTable();
            sDataAdapter.Fill(dt);
            dataGridView2.DataSource = dt;
            dataGridView2.ReadOnly = true;
        }

        private void rentsPage_Click(object sender, EventArgs e)
        {
            SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE USERNAME = '" + UN + "'");
            SqlConnection sConnection = new SqlConnection(ConString);
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
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void minBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void AddNewUser_Click(object sender, EventArgs e)
        {
            addUserPanel.BringToFront();
            this.ActiveControl = FNameLabel;
        }

        private void expandBtn_Click(object sender, EventArgs e)
        {
            if (NavBar.Width == 65)
            {
                NavBar.Width = 157;

                homePage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                usersPage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                gamesPage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                rentsPage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                aboutPage.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                signUp.Visible = true;
                CurrentUser.Visible = true;
                currentUserName.Visible = true;
            }
            else
            {
                NavBar.Width = 65;

                homePage.RightToLeft = System.Windows.Forms.RightToLeft.No;
                usersPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
                gamesPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
                rentsPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
                aboutPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
                signUp.Visible = false;
                CurrentUser.Visible = false;
                currentUserName.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkUser();
        }

        private void logInUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                checkUser();
            }
        }

        private void currentUserName_Click(object sender, EventArgs e)
        {
            addUserPanel.BringToFront();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            LogInPanel.BringToFront();
            this.ActiveControl = logInUserName;
        }

        private void UpdateInfo_Click(object sender, EventArgs e)
        {
            FN2.Text = FirstName;
            LN2.Text = LastName;
            if (Authority2.Text == "Admin")
            {
                isAdmin2.Checked = true;
            }

            UN2.Text = UN;
            PW2.Text = pw;

            EditDataPanel.BringToFront();
        }

        public void updateInfo()
        {
            SqlConnection sConnetion = new SqlConnection(ConString);
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            sCommand.CommandText = "UPDATE CLIENT SET FNAME = '" + FN2.Text + "' WHERE USERNAME = '" + UN +
                                   "'; UPDATE CLIENT SET LNAME = '" + LN2.Text + "'  WHERE USERNAME = '" + UN +
                                   "' ; UPDATE CLIENT SET PASSWORD = '" + PW2.Text + "' WHERE USERNAME = '" + UN +
                                   "'; UPDATE CLIENT SET ADMIN = " + (isAdmin2.Checked ? 1 : 0) +
                                   " WHERE USERNAME = '" + UN + "';";

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

        public void updateGameInfo()
        {
            SqlConnection sConnetion = new SqlConnection(ConString);
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            int numOfRowsAffected;

            sCommand.CommandText = "UPDATE GAME SET CATEGORY = '" + GameCategory.Text + "' WHERE GNAME = '" +
                                   GameName.Text + "'; UPDATE GAME SET RENT_PRICE_PER_DAY = '" + GamePrice.Text +
                                   "' WHERE GNAME = '" + GameName.Text + "'; UPDATE GAME SET VENDORNAME = '" +
                                   GameVendor.Text + "' WHERE GNAME = '" + GameName.Text +
                                   "'; UPDATE GAME SET RELEASEDATE = '" + GameDate.Text + "' WHERE GNAME = '" +
                                   GameName.Text + "';";

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

        public void DeleteAGame()
        {
            SqlConnection sConnetion = new SqlConnection(ConString);
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
                //
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
            updateInfo();
        }

        private void DeleteMyInfo_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Continue Deleting Your Account?", "Sure?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                SqlConnection sConnetion = new SqlConnection(ConString);
                SqlCommand sCommand = new SqlCommand();
                sCommand.Connection = sConnetion;

                sConnetion.Open();

                sCommand.CommandText = "DELETE FROM CLIENT WHERE USERNAME = '" + UN + "';";


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
                this.ActiveControl = logInUserName;

                currentUserName.Text = FN2.Text + ' ' + LN2.Text;
                currentUserName2.Text = currentUserName.Text;

                currentUserAuthority.Text = (isAdmin2.Checked ? "Admin" : "Client");
                Authority2.Text = currentUserAuthority.Text;

                sConnetion.Close();
            }
            else
            {
                return;
            }
        }

        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Log Out From Your Account?", "Sure?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                NavBar.Hide();
                LogInPanel.BringToFront();
                LogInPanel.Show();
                this.ActiveControl = logInUserName;
            }
            else
            {
                return;
            }
        }

        private void FN2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateInfo();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addGame();
        }

        private void gName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                addGame();
            }
        }

        private void AddNewGame_Click(object sender, EventArgs e)
        {
            NewGamePanel.BringToFront();
        }

        private void SearchGameBtn_Click(object sender, EventArgs e)
        {
            SearchGameBtn.Hide();
            this.ActiveControl = GamesFilters;
        }

        private void GamesFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamesFilters.Text == "None")
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

                //
            }
            else if (GamesFilters.Text == "Name")
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
            else if (GamesFilters.Text == "Category")
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
            else if (GamesFilters.Text == "Price")
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
            else if (GamesFilters.Text == "Vendor")
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
            else if (GamesFilters.Text == "Release Date")
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
            if (GamesFilters.Text == "None")
            {
                SearchGameBtn.Show();
            }
        }

        public String returnName()
        {
            if (GamesFilters.Text == "Name")
            {
                return "GNAME";
            }
            else if (GamesFilters.Text == "Vendor")
            {
                return "VENDORNAME";
            }
            else
            {
                return "CATEGORY";
            }
        }

        //***********************


        public void searchGame()
        {
            if (GamesFilters.Text == "Name" || GamesFilters.Text == "Vendor" || GamesFilters.Text == "Category")
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM GAME WHERE " + returnName() + " LIKE '%" +
                                                     gameSearchBx1.Text + "%'");

                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView2.DataSource = dt;
                dataGridView2.ReadOnly = true;
            }
            else if (GamesFilters.Text == "Price")
            {
                SqlCommand sCommand;


                if (gameSearchBx2.TextLength > 0 && gameSearchBx3.TextLength > 0)
                {
                    sCommand = new SqlCommand("select * from GAME where RENT_PRICE_PER_DAY >= " + gameSearchBx2.Text +
                                              " AND RENT_PRICE_PER_DAY <= " + gameSearchBx3.Text);
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
                    //
                    return;
                }

                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView2.DataSource = dt;
                dataGridView2.ReadOnly = true;
            }
            else if (GamesFilters.Text == "Release Date")
            {
                SqlCommand sCommand;

                sCommand = new SqlCommand("select * from GAME where RELEASEDATE >= '" + GameDateFrom.Text +
                                          "' and RELEASEDATE <= '" + GameDateTo.Text + "'");


                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView2.DataSource = dt;
                dataGridView2.ReadOnly = true;
            }
        }

        private void GameDateTo_ValueChanged(object sender, EventArgs e)
        {
            searchGame();
        }

        private void gameSearchBx1_KeyDown(object sender, EventArgs e)
        {
            searchGame();
        }

        private void GameName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateGameInfo();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            updateGameInfo();
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
            if (GameToDelete.Visible == true)
            {
                GameToDelete.Text = dataGridView2.SelectedCells[0].Value.ToString();
            }
        }

        private void RentsSearch_TextChanged(object sender, EventArgs e)
        {
            if (RentsSearch.Text == "Game Name" || RentsSearch.Text == "User Name")
            {
                RentSearchAttr.Text = RentsSearch.Text + ':';
                RentSearchAttr.Visible = true;

                RentsTextBox.Visible = true;

                RentsDateFrom.Visible = false;
                RentsDateTo.Visible = false;

                AllReturnedGames.Visible = false;
                AllNotReturnedGames.Visible = false;

                Between2.Visible = false;
                And2.Visible = false;

                RentsTextBox.Focus();
            }
            else if (RentsSearch.Text == "Return Date" || RentsSearch.Text == "Date")
            {
                RentSearchAttr.Visible = false;

                RentsTextBox.Visible = false;

                RentsDateFrom.Visible = true;
                RentsDateTo.Visible = true;

                AllReturnedGames.Visible = false;
                AllNotReturnedGames.Visible = false;

                Between2.Visible = true;
                And2.Visible = true;
            }
            else if (RentsSearch.Text == "Rent Status")
            {
                RentSearchAttr.Visible = false;

                RentsTextBox.Visible = false;

                RentsDateFrom.Visible = false;
                RentsDateTo.Visible = false;

                AllReturnedGames.Visible = true;
                AllNotReturnedGames.Visible = true;

                Between2.Visible = false;
                And2.Visible = false;
            }
            else
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
                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
        }

        private void RentsTextBox_TextChanged(object sender, EventArgs e)
        {
            if (RentsSearch.Text == "Game Name")
            {
                SqlCommand sCommand =
                    new SqlCommand("SELECT * FROM RENT WHERE GNAME LIKE '%" + RentsTextBox.Text + "%'");
                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
            else if (RentsSearch.Text == "User Name")
            {
                SqlCommand sCommand =
                    new SqlCommand("SELECT * FROM RENT WHERE USERNAME LIKE '%" + RentsTextBox.Text + "%'");
                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
        }

        private void AllNotReturnedGames_CheckedChanged(object sender, EventArgs e)
        {
            if (RentsSearch.Text == "Rent Status")
            {
                SqlCommand sCommand;

                if (AllReturnedGames.Checked)
                {
                    sCommand = new SqlCommand("SELECT * FROM RENT WHERE ISRETURNED = 1");
                }
                else
                {
                    sCommand = new SqlCommand("SELECT * FROM RENT WHERE ISRETURNED != 1");
                }

                SqlConnection sConnection = new SqlConnection(ConString);
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
            DialogResult dr = MessageBox.Show("Continue Renting Process?", "Sure?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                DateTime dt1 = new DateTime();
                dt1 = DateTime.Now.Date;

                SqlConnection sConnetion = new SqlConnection(ConString);
                SqlCommand sCommand = new SqlCommand();
                sCommand.Connection = sConnetion;

                sConnetion.Open();

                sCommand.CommandText = "INSERT INTO RENT(GNAME, USERNAME, DATE) VALUES('" + NewRentGameSearch.Text +
                                       "', '" + UN + "', '" + dt1 + "')";

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
                return;
            }
        }

        public void getUpdatedReport()
        {
            ReportText.Text = "The Most Interesting Game is ";

            SqlConnection sConnetion = new SqlConnection(ConString);
            SqlCommand sCommand = new SqlCommand();
            sCommand.Connection = sConnetion;

            sConnetion.Open();

            SqlDataReader sReader;

            sCommand.CommandText =
                "SELECT TOP 1 GNAME AS TOPGAME, COUNT(USERNAME) AS RENTERS FROM RENT GROUP BY GNAME ORDER BY COUNT(USERNAME) DESC";

            sReader = sCommand.ExecuteReader();

            if (sReader.Read())
            {
                ReportText.Text = ReportText.Text + sReader["TOPGAME"].ToString() + " With " +
                                  sReader["RENTERS"].ToString() + " Renters.\r\n\r\n";
            }

            sConnetion.Close();

            //----------------------------------------------------
            ReportText.Text += "The Games That Didn't Have Any Renting in The Last 30 Days Are:\r\n";

            SqlConnection sConnetion1 = new SqlConnection(ConString);
            SqlCommand sCommand1 = new SqlCommand();
            sCommand1.Connection = sConnetion1;

            sConnetion1.Open();

            SqlDataReader sReader1;

            sCommand1.CommandText =
                "SELECT GNAME FROM GAME  EXCEPT SELECT GNAME FROM RENT WHERE RENT.DATE >= DATEADD(DAY, -30, GETDATE())";

            sReader1 = sCommand1.ExecuteReader();

            while (sReader1.Read())
            {
                ReportText.Text = ReportText.Text + sReader1["GNAME"].ToString() + ", ";
            }

            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------
            ReportText.Text += "The Games That Didn't Have Any Renting in The Last Month Are:\r\n";

            SqlConnection sConnetion2 = new SqlConnection(ConString);
            SqlCommand sCommand2 = new SqlCommand();
            sCommand2.Connection = sConnetion2;

            sConnetion2.Open();

            SqlDataReader sReader2;

            sCommand2.CommandText =
                "SELECT GNAME FROM GAME EXCEPT SELECT GNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE())";

            sReader2 = sCommand2.ExecuteReader();

            while (sReader2.Read())
            {
                ReportText.Text = ReportText.Text + sReader2["GNAME"].ToString() + ", ";
            }

            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------
            ReportText.Text += "The Client With The Maximum Number Of Rentings Last Month is:\r\n";

            SqlConnection sConnetion3 = new SqlConnection(ConString);
            SqlCommand sCommand3 = new SqlCommand();
            sCommand3.Connection = sConnetion3;

            sConnetion3.Open();

            SqlDataReader sReader3;

            sCommand3.CommandText =
                "SELECT CLIENT.FNAME AS FN, CLIENT.LNAME AS LN, COUNT(GNAME) AS RENTS FROM CLIENT, RENT WHERE CLIENT.USERNAME = RENT.USERNAME AND RENT.USERNAME IN(SELECT TOP 1 USERNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()) GROUP BY USERNAME ORDER BY COUNT(GNAME) DESC) GROUP BY FNAME, LNAME";

            sReader3 = sCommand3.ExecuteReader();

            if (sReader3.Read())
            {
                ReportText.Text = ReportText.Text + sReader3["FN"].ToString() + ' ' + sReader3["LN"].ToString() +
                                  " With " + sReader3["RENTS"].ToString() + " Rentings.\r\n\r\n";
            }

            //----------------------------------------------------
            ReportText.Text += "The Vendor With The Maximum Renting out Last Month is:\r\n";

            SqlConnection sConnetion4 = new SqlConnection(ConString);
            SqlCommand sCommand4 = new SqlCommand();
            sCommand4.Connection = sConnetion4;

            sConnetion4.Open();

            SqlDataReader sReader4;

            sCommand4.CommandText =
                "SELECT TOP 1 VENDORNAME AS VENDOR, COUNT(RENT.USERNAME) AS RENTS FROM RENT JOIN GAME ON RENT.GNAME = GAME.GNAME WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()) GROUP BY VENDORNAME ORDER BY COUNT(VENDORNAME) DESC";

            sReader4 = sCommand4.ExecuteReader();

            if (sReader4.Read())
            {
                ReportText.Text = ReportText.Text + sReader4["VENDOR"].ToString() + " With " +
                                  sReader4["RENTS"].ToString() + " Rentings.\r\n\r\n";
            }

            //----------------------------------------------------
            ReportText.Text += "The Vendors Whose Games Didn't Have Any Renting in The Last Month Are:\r\n";

            SqlConnection sConnetion5 = new SqlConnection(ConString);
            SqlCommand sCommand5 = new SqlCommand();
            sCommand5.Connection = sConnetion5;

            sConnetion5.Open();

            SqlDataReader sReader5;

            sCommand5.CommandText =
                "SELECT GAME.VENDORNAME AS VENDOR FROM GAME EXCEPT SELECT GAME.VENDORNAME FROM GAME WHERE GNAME IN(SELECT GNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()))";

            sReader5 = sCommand5.ExecuteReader();

            while (sReader5.Read())
            {
                ReportText.Text = ReportText.Text + sReader5["VENDOR"].ToString() + ", ";
            }

            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------
            ReportText.Text += "The Vendors Who Dind't Add Any Games Last Year Are:\r\n";

            SqlConnection sConnetion6 = new SqlConnection(ConString);
            SqlCommand sCommand6 = new SqlCommand();
            sCommand6.Connection = sConnetion6;

            sConnetion6.Open();

            SqlDataReader sReader6;

            sCommand6.CommandText =
                "SELECT GAME.VENDORNAME AS VENDOR FROM GAME EXCEPT SELECT GAME.VENDORNAME FROM GAME WHERE GNAME IN(SELECT GNAME FROM RENT WHERE DATEPART(MONTH FROM RENT.DATE) = DATEPART(MONTH FROM GETDATE()) - 1 AND DATEPART(YEAR FROM RENT.DATE) = DATEPART(YEAR FROM GETDATE()))";

            sReader6 = sCommand6.ExecuteReader();

            while (sReader6.Read())
            {
                ReportText.Text = ReportText.Text + sReader6["VENDOR"].ToString() + ", ";
            }

            ReportText.Text = ReportText.Text.Remove(ReportText.Text.Length - 2);
            ReportText.Text += ".\r\n\r\n";

            //----------------------------------------------------

            ReportText.Text += "End Of Report\r\n    Thanks!";
        }

        private void ViewReport_Click(object sender, EventArgs e)
        {
            QuickReportPanel.BringToFront();
            getUpdatedReport();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", "Sql Statements.txt");
        }

        private void RentsSerachBtn_Click(object sender, EventArgs e)
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
            UserSearchAttr.Text = UserSearchOption.Text + ':';

            if (UserSearchOption.Text == "First Name" || UserSearchOption.Text == "Last Name" ||
                UserSearchOption.Text == "User Name")
            {
                UserSearchAttr.Visible = true;

                AllAdmins.Visible = false;
                NotAdmins.Visible = false;

                UserSearchBox.Visible = true;
            }
            else if (UserSearchOption.Text == "Authority")
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

                SqlConnection sConnection = new SqlConnection(ConString);
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
            if (UserSearchOption.Text == "First Name")
            {
                SqlCommand sCommand =
                    new SqlCommand("SELECT * FROM CLIENT WHERE FNAME LIKE '%" + UserSearchBox.Text + "%'");

                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
            else if (UserSearchOption.Text == "Last Name")
            {
                SqlCommand sCommand =
                    new SqlCommand("SELECT * FROM CLIENT WHERE LNAME LIKE '%" + UserSearchBox.Text + "%'");

                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;

                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);

                DataTable dt = new DataTable();

                sDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.ReadOnly = true;

                sConnection.Close();
            }
            else if (UserSearchOption.Text == "User Name")
            {
                SqlCommand sCommand =
                    new SqlCommand("SELECT * FROM CLIENT WHERE USERNAME LIKE '%" + UserSearchBox.Text + "%'");

                SqlConnection sConnection = new SqlConnection(ConString);
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
            if (AllAdmins.Checked)
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE ADMIN = 1");

                SqlConnection sConnection = new SqlConnection(ConString);
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
            if (NotAdmins.Checked)
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM CLIENT WHERE ADMIN != 1");

                SqlConnection sConnection = new SqlConnection(ConString);
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
            if (RentsSearch.Text == "Date")
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE DATE >= '" + RentsDateFrom.Text +
                                                     "' AND DATE <= '" + RentsDateTo.Text + "'");
                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
            else
            {
                SqlCommand sCommand = new SqlCommand("SELECT * FROM RENT WHERE RETURNDATE >= '" + RentsDateFrom.Text +
                                                     "' AND DATE <= '" + RentsDateTo.Text + "'");
                SqlConnection sConnection = new SqlConnection(ConString);
                sCommand.Connection = sConnection;
                SqlDataAdapter sDataAdapter = new SqlDataAdapter(sCommand);
                DataTable dt = new DataTable();
                sDataAdapter.Fill(dt);
                dataGridView3.DataSource = dt;
            }
        }

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
    }
}