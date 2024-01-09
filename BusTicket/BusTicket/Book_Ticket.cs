using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusTicket
{
    public partial class Splash : Form
    {
        // Connection string to the database
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BusTicketDB1;Integrated Security=True";
        private SqlCommand cmd;
        private SqlDataAdapter adapt;

        // Constructor for the form
        public Splash()
        {
            InitializeComponent();

            // Populate DataGridView with ticket data and rename columns
            dataGridView1.DataSource = GetTicketData();
            RenameDataGridViewColumns();

            // Display success message
            MessageBox.Show("Refreshed Bookings!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Event handler for booking a ticket
        private void BkTicket_Click(object sender, EventArgs e)
        {
            // Database connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query for inserting a new ticket
                string insertQuery = "INSERT INTO BookedTickets (PassengerName, Destination, DepartureDate, TicketPrice) " +
                                    "VALUES (@PassengerName, @Destination, @DepartureDate, @TicketPrice)";

                // Execute the SQL command
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    try
                    {
                        // Get input values from textboxes
                        string passenger = fullName.Text.Trim();
                        string dest = destination.Text.Trim();
                        DateTime departure = datepick.Value;
                        string priceTicket = price.Text.Trim();

                        // Validate input values
                        if (string.IsNullOrEmpty(passenger))
                        {
                            throw new NullValueException("Full Name");
                        }
                        if (string.IsNullOrEmpty(dest))
                        {
                            throw new NullValueException("Destination");
                        }
                        if (string.IsNullOrEmpty(priceTicket))
                        {
                            throw new NullValueException("Ticket Price");
                        }

                        // Convert departure to DateTime
                        DateTime dateofDeparture = Convert.ToDateTime(departure);

                        // Set parameters for the SQL command
                        command.Parameters.AddWithValue("@PassengerName", passenger);
                        command.Parameters.AddWithValue("@Destination", dest);
                        command.Parameters.AddWithValue("@DepartureDate", dateofDeparture);
                        command.Parameters.AddWithValue("@TicketPrice", priceTicket);

                        // Execute the command
                        command.ExecuteNonQuery();

                        // Display success message
                        MessageBox.Show("Data added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear input textboxes
                        fullName.Text = "";
                        destination.Text = "";
                        price.Text = "";
                    }
                    catch (NullValueException ex)
                    {
                        // Display error message for null or empty values
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        // Display generic error message for other exceptions
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Event handler for editing a ticket
        private void EdtTicket_Click(object sender, EventArgs e)
        {
            // Check if IDBox is empty or null
            if (IDBox.Text == "" || IDBox.Text == null)
            {
                try
                {
                    // Check if a row is selected in the DataGridView
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        // Get the selected row and ticket ID
                        DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                        int ticketID = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                        // Fetch ticket details based on the ticketID
                        TicketDetails ticketDetails = GetTicketDetails(ticketID);

                        // Display the details in the corresponding TextBox controls
                        fullName.Text = ticketDetails.PassengerName;
                        destination.Text = ticketDetails.Destination;
                        datepick.Value = ticketDetails.DepartureDate;
                        price.Text = ticketDetails.TicketPrice.ToString();
                        IDBox.Text = ticketDetails.ID.ToString();
                    }
                    else
                    {
                        // Throw a custom exception if no row is selected
                        throw new RowSelection();
                    }
                }
                catch (NullValueException ex)
                {
                    // Display error message for null or empty values
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (RowSelection ex)
                {
                    // Display error message for no row selected
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Display generic error message for other exceptions
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Database connection for updating an existing ticket
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query for updating a ticket
                    string updateQuery = "UPDATE BookedTickets " +
                                         "SET PassengerName = @PassengerName, " +
                                         "    Destination = @Destination, " +
                                         "    DepartureDate = @DepartureDate, " +
                                         "    TicketPrice = @TicketPrice " +
                                         "WHERE ID = @TicketID"; // Assuming TicketID is the primary key

                    // Execute the SQL command
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        try
                        {
                            // Retrieve the existing ticket ID (replace this with your actual logic)
                            int ticketID = Convert.ToInt32(IDBox.Text); // Replace with your logic to get the ticket ID

                            // Get input values from textboxes
                            string passenger = fullName.Text.Trim();
                            string dest = destination.Text.Trim();
                            DateTime departure = datepick.Value;
                            string priceTicket = price.Text.Trim();

                            // Validate input values
                            if (string.IsNullOrEmpty(passenger))
                            {
                                throw new NullValueException("Full Name");
                            }
                            if (string.IsNullOrEmpty(dest))
                            {
                                throw new NullValueException("Destination");
                            }
                            if (string.IsNullOrEmpty(priceTicket))
                            {
                                throw new NullValueException("Ticket Price");
                            }

                            // Convert departure to DateTime
                            DateTime dateOfDeparture = Convert.ToDateTime(departure);

                            // Set parameters for the SQL command
                            command.Parameters.AddWithValue("@PassengerName", passenger);
                            command.Parameters.AddWithValue("@Destination", dest);
                            command.Parameters.AddWithValue("@DepartureDate", dateOfDeparture);
                            command.Parameters.AddWithValue("@TicketPrice", priceTicket);
                            command.Parameters.AddWithValue("@TicketID", ticketID); // Assuming TicketID is the primary key

                            // Execute the command
                            command.ExecuteNonQuery();

                            // Display success message
                            MessageBox.Show("Data updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Clear input textboxes and IDBox
                            IDBox.Text = "";
                            fullName.Text = "";
                            destination.Text = "";
                            price.Text = "";
                        }
                        catch (NullValueException ex)
                        {
                            // Display error message for null or empty values
                            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            // Display generic error message for other exceptions
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // Event handler for canceling a ticket
        private void CnlTicket_Click(object sender, EventArgs e)
        {
            // Database connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int ticketIDToDelete = 0;
                try
                {
                    // Check if a row is selected in the DataGridView
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        // Get the selected row and ticket ID
                        DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                        int ticketID = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                        ticketIDToDelete = ticketID;
                    }
                    else
                    {
                        // Throw a custom exception if no row is selected
                        throw new RowSelection();
                    }

                    // Display a confirmation dialog
                    DialogResult result = MessageBox.Show("Are you sure you want to cancel this ticket?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // SQL query for deleting a ticket
                        string deleteQuery = "DELETE FROM BookedTickets WHERE ID = @TicketID";

                        // Execute the SQL command
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            try
                            {
                                // Set parameters for the SQL command
                                command.Parameters.AddWithValue("@TicketID", ticketIDToDelete);

                                // Execute the command
                                command.ExecuteNonQuery();

                                // Display success message
                                MessageBox.Show("Ticket canceled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Clear IDBox
                                IDBox.Text = "";
                            }
                            catch (Exception ex)
                            {
                                // Display error message for delete operation
                                MessageBox.Show($"An error occurred while canceling the ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        // Display message if ticket cancellation is canceled by the user
                        MessageBox.Show("Ticket cancellation canceled by user.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (RowSelection ex)
                {
                    // Display error message for no row selected
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Display generic error message for other exceptions
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // This method is called when the "RefBooking" button is clicked.
        private void RefBooking_Click(object sender, EventArgs e)
        {
            try
            {
                // Refresh the DataGridView with updated ticket data.
                dataGridView1.DataSource = GetTicketData();

                // Rename DataGridView columns for better clarity.
                RenameDataGridViewColumns();

                // Display a success message.
                MessageBox.Show("Refreshed Bookings!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Display an error message if an exception occurs during the refresh.
                MessageBox.Show($"An error occurred while refreshing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Retrieve all ticket data from the database.
        private DataTable GetTicketData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to select all columns from the "BookedTickets" table.
                string selectQuery = "SELECT * FROM BookedTickets";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        // Create a DataTable to store the fetched data.
                        DataTable dataTable = new DataTable();

                        // Fill the DataTable with data from the database.
                        adapter.Fill(dataTable);

                        // Return the populated DataTable.
                        return dataTable;
                    }
                }
            }
        }

        // Rename DataGridView columns to provide user-friendly names.
        private void RenameDataGridViewColumns()
        {
            // Assuming dataGridView1 is the name of your DataGridView.
            dataGridView1.Columns["ID"].HeaderText = "ID";
            dataGridView1.Columns["PassengerName"].HeaderText = "Fullname";
            dataGridView1.Columns["Destination"].HeaderText = "Destination";
            dataGridView1.Columns["DepartureDate"].HeaderText = "Departure Date";
            dataGridView1.Columns["TicketPrice"].HeaderText = "Ticket Price";
        }

        // Retrieve ticket details for a specific ticket ID from the database.
        private TicketDetails GetTicketDetails(int ticketID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to select all columns from the "BookedTickets" table for a specific ticket ID.
                string selectQuery = "SELECT * FROM BookedTickets WHERE ID = @TicketID";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    // Add the ticketID as a parameter to the SQL query.
                    command.Parameters.AddWithValue("@TicketID", ticketID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Create a TicketDetails object with values from the database.
                            return new TicketDetails
                            {
                                ID = (int)reader["ID"],
                                PassengerName = reader["PassengerName"].ToString(),
                                Destination = reader["Destination"].ToString(),
                                DepartureDate = (DateTime)reader["DepartureDate"],
                                TicketPrice = (decimal)reader["TicketPrice"]
                            };
                        }
                    }
                }
            }

            // Return null if the ticket ID is not found.
            return null;
        }


    }
}
// Custom exception class representing an exception thrown when a null or empty value is encountered.
class NullValueException : Exception
{
    // Constructor for the NullValueException class.
    // Takes the field name as a parameter and generates an exception message.
    public NullValueException(string field) : base($"The value for field '{field}' cannot be null or empty.")
    {
        // Initialization code
    }
}

// Custom exception class representing an exception thrown when no row is selected for editing.
class RowSelection : Exception
{
    // Constructor for the RowSelection class.
    // Generates an exception message indicating that a row must be selected for editing.
    public RowSelection() : base("Select a row to be able to edit")
    {
        // Initialization code
    }
}

