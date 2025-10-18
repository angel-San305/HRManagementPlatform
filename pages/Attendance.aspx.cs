using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRManagement.Pages
{
    public partial class Attendance : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["Username"] == null || Session["UserID"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Check user role and adjust UI accordingly
                string userRole = Session["UserRole"]?.ToString() ?? "";
                ConfigureUIForRole(userRole);

                LoadEmployeeDropdowns();
                SetDefaultDates();
                LoadAttendanceSummary();
                LoadAttendanceRecords();
            }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            string role = Session["UserRole"]?.ToString();

            if (role == "Employee")
            {
                this.MasterPageFile = "~/EmployeeSite.Master";
            }
            else
            {
                this.MasterPageFile = "~/Site.Master"; // Default to Admin
            }
        }


        private void ConfigureUIForRole(string role)
        {
            // If user is an Employee, hide certain controls
            if (role == "Employee")
            {
                // Hide the employee filter dropdown for employees
                ddlFilterEmployee.Enabled = false;
                ddlFilterEmployee.Visible = false;

                // Optionally hide the filter employee label
                if (ddlFilterEmployee.Parent.FindControl("lblFilterEmployee") != null)
                {
                    ddlFilterEmployee.Parent.FindControl("lblFilterEmployee").Visible = false;
                }

                // Hide export button for employees (optional)
                // btnExportAttendance.Visible = false;
            }
        }

        private void LoadEmployeeDropdowns()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString() ?? "";
                int userId = Convert.ToInt32(Session["UserID"]);

                string query = "";

                // If Employee role, only load their own record
                if (userRole == "Employee")
                {
                    query = @"SELECT e.EmployeeID, e.FirstName + ' ' + e.LastName AS FullName 
                            FROM Employees e
                            WHERE e.IsActive = 1 AND e.UserID = @UserID
                            ORDER BY e.FirstName, e.LastName";
                }
                else // Admin or HR can see all employees
                {
                    query = @"SELECT EmployeeID, FirstName + ' ' + LastName AS FullName 
                            FROM Employees 
                            WHERE IsActive = 1 
                            ORDER BY FirstName, LastName";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (userRole == "Employee")
                        {
                            cmd.Parameters.AddWithValue("@UserID", userId);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // For add/edit form - Employee dropdown in modal
                            ddlEmployee.DataSource = dt;
                            ddlEmployee.DataTextField = "FullName";
                            ddlEmployee.DataValueField = "EmployeeID";
                            ddlEmployee.DataBind();

                            // If Employee, auto-select their record and disable dropdown
                            if (userRole == "Employee" && dt.Rows.Count > 0)
                            {
                                ddlEmployee.SelectedValue = dt.Rows[0]["EmployeeID"].ToString();
                                ddlEmployee.Enabled = false; // Disable so they can't change it
                            }
                            else // Admin or HR - keep dropdown enabled
                            {
                                ddlEmployee.Items.Insert(0, new ListItem("Select Employee", ""));
                                ddlEmployee.Enabled = true; // Make sure it's enabled for Admin/HR
                            }

                            // For filter dropdown (only for Admin/HR)
                            if (userRole == "Admin" || userRole == "HR")
                            {
                                // Clone the datatable for filter dropdown
                                DataTable dtFilter = dt.Copy();
                                ddlFilterEmployee.DataSource = dtFilter;
                                ddlFilterEmployee.DataTextField = "FullName";
                                ddlFilterEmployee.DataValueField = "EmployeeID";
                                ddlFilterEmployee.DataBind();
                                ddlFilterEmployee.Items.Insert(0, new ListItem("All Employees", ""));
                                ddlFilterEmployee.Enabled = true;
                            }
                            else // Employee
                            {
                                // For employees, set their own ID as filter
                                ddlFilterEmployee.DataSource = dt;
                                ddlFilterEmployee.DataTextField = "FullName";
                                ddlFilterEmployee.DataValueField = "EmployeeID";
                                ddlFilterEmployee.DataBind();
                                if (dt.Rows.Count > 0)
                                {
                                    ddlFilterEmployee.SelectedValue = dt.Rows[0]["EmployeeID"].ToString();
                                }
                                ddlFilterEmployee.Enabled = false; // Disable for employees
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading employees: " + ex.Message, "danger");
            }
        }

        private void SetDefaultDates()
        {
            txtFromDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtAttendanceDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void LoadAttendanceSummary()
        {
            try
            {
                DateTime today = DateTime.Today;
                string userRole = Session["UserRole"]?.ToString() ?? "";
                int userId = Convert.ToInt32(Session["UserID"]);

                string query = "";

                // If Employee, only show their own summary
                if (userRole == "Employee")
                {
                    query = @"SELECT 
                            SUM(CASE WHEN a.Status = 'Present' THEN 1 ELSE 0 END) AS PresentToday,
                            SUM(CASE WHEN a.Status = 'Late' THEN 1 ELSE 0 END) AS LateToday,
                            SUM(CASE WHEN a.Status = 'Absent' THEN 1 ELSE 0 END) AS AbsentToday,
                            COUNT(*) AS TotalToday
                            FROM Attendance a
                            INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                            WHERE a.AttendanceDate = @Today AND e.UserID = @UserID";
                }
                else
                {
                    query = @"SELECT 
                            SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) AS PresentToday,
                            SUM(CASE WHEN Status = 'Late' THEN 1 ELSE 0 END) AS LateToday,
                            SUM(CASE WHEN Status = 'Absent' THEN 1 ELSE 0 END) AS AbsentToday,
                            COUNT(*) AS TotalToday
                            FROM Attendance 
                            WHERE AttendanceDate = @Today";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Today", today);
                        if (userRole == "Employee")
                        {
                            cmd.Parameters.AddWithValue("@UserID", userId);
                        }

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblPresentToday.Text = reader["PresentToday"].ToString();
                                lblLateToday.Text = reader["LateToday"].ToString();
                                lblAbsentToday.Text = reader["AbsentToday"].ToString();
                                lblTotalRecords.Text = reader["TotalToday"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading attendance summary: " + ex.Message, "danger");
            }
        }

        private void LoadAttendanceRecords()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString() ?? "";
                int userId = Convert.ToInt32(Session["UserID"]);

                StringBuilder query = new StringBuilder();
                query.Append(@"SELECT a.AttendanceID, a.AttendanceDate, a.Status, a.Hours, a.Notes,
                              e.FirstName + ' ' + e.LastName AS EmployeeName,
                              e.EmployeeID
                              FROM Attendance a
                              INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                              WHERE 1=1");

                var parameters = new List<SqlParameter>();

                // If Employee, only show their own records
                if (userRole == "Employee")
                {
                    query.Append(" AND e.UserID = @UserID");
                    parameters.Add(new SqlParameter("@UserID", userId));
                }
                // Apply employee filter (for Admin/HR)
                else if (!string.IsNullOrEmpty(ddlFilterEmployee.SelectedValue))
                {
                    query.Append(" AND a.EmployeeID = @EmployeeID");
                    parameters.Add(new SqlParameter("@EmployeeID", ddlFilterEmployee.SelectedValue));
                }

                // Apply date filters
                if (!string.IsNullOrEmpty(txtFromDate.Text))
                {
                    query.Append(" AND a.AttendanceDate >= @FromDate");
                    parameters.Add(new SqlParameter("@FromDate", DateTime.Parse(txtFromDate.Text)));
                }

                if (!string.IsNullOrEmpty(txtToDate.Text))
                {
                    query.Append(" AND a.AttendanceDate <= @ToDate");
                    parameters.Add(new SqlParameter("@ToDate", DateTime.Parse(txtToDate.Text)));
                }

                query.Append(" ORDER BY a.AttendanceDate DESC, e.FirstName");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvAttendance.DataSource = dt;
                            gvAttendance.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading attendance records: " + ex.Message, "danger");
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadAttendanceRecords();
            LoadAttendanceSummary();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";

            if (userRole != "Employee")
            {
                ddlFilterEmployee.SelectedIndex = 0;
            }

            SetDefaultDates();
            LoadAttendanceRecords();
            LoadAttendanceSummary();
        }

        protected void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateAttendanceForm())
                {
                    if (string.IsNullOrEmpty(hdnAttendanceID.Value))
                    {
                        AddAttendance();
                    }
                    else
                    {
                        UpdateAttendance();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving attendance: " + ex.Message, "danger");
            }
        }

        protected void btnAddAttendance_Click(object sender, EventArgs e)
        {
            ClearForm();
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal",
                "$('#addAttendanceModal').modal('show');", true);
        }

        private void AddAttendance()
        {
            // Validate that employee can only add their own attendance
            if (!ValidateEmployeePermission(int.Parse(ddlEmployee.SelectedValue)))
            {
                ShowMessage("You can only mark your own attendance.", "danger");
                return;
            }

            // Check if attendance already exists for this employee and date
            if (AttendanceExists(int.Parse(ddlEmployee.SelectedValue), DateTime.Parse(txtAttendanceDate.Text)))
            {
                ShowMessage("Attendance record already exists for this employee on the selected date.", "warning");
                return;
            }

            string query = @"INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, Hours, Notes)
                   VALUES (@EmployeeID, @AttendanceDate, @Status, @Hours, @Notes)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", ddlEmployee.SelectedValue);
                    cmd.Parameters.AddWithValue("@AttendanceDate", DateTime.Parse(txtAttendanceDate.Text));
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@Hours", string.IsNullOrEmpty(txtHours.Text) ? (object)8.0m : (object)decimal.Parse(txtHours.Text));
                    cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? DBNull.Value : (object)txtNotes.Text.Trim());

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ClearForm();
                        LoadAttendanceRecords();
                        LoadAttendanceSummary();
                        ShowMessage("Attendance marked successfully!", "success");

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hideModal",
                            "var modal = bootstrap.Modal.getInstance(document.getElementById('addAttendanceModal')); modal.hide();", true);
                    }
                }
            }
        }

        private void UpdateAttendance()
        {
            // Validate that employee can only update their own attendance
            if (!ValidateEmployeePermission(int.Parse(ddlEmployee.SelectedValue)))
            {
                ShowMessage("You can only update your own attendance.", "danger");
                return;
            }

            string query = @"UPDATE Attendance 
                   SET EmployeeID = @EmployeeID, AttendanceDate = @AttendanceDate, 
                       Status = @Status, Hours = @Hours, Notes = @Notes
                   WHERE AttendanceID = @AttendanceID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AttendanceID", hdnAttendanceID.Value);
                    cmd.Parameters.AddWithValue("@EmployeeID", ddlEmployee.SelectedValue);
                    cmd.Parameters.AddWithValue("@AttendanceDate", DateTime.Parse(txtAttendanceDate.Text));
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@Hours", string.IsNullOrEmpty(txtHours.Text) ? (object)8.0m : (object)decimal.Parse(txtHours.Text));
                    cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtNotes.Text) ? DBNull.Value : (object)txtNotes.Text.Trim());

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ClearForm();
                        LoadAttendanceRecords();
                        LoadAttendanceSummary();
                        ShowMessage("Attendance updated successfully!", "success");

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hideModal",
                            "var modal = bootstrap.Modal.getInstance(document.getElementById('addAttendanceModal')); modal.hide();", true);
                    }
                }
            }
        }

        private bool ValidateEmployeePermission(int employeeId)
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";

            // Admin and HR can mark/edit anyone's attendance
            if (userRole == "Admin" || userRole == "HR")
            {
                return true;
            }

            // Employees can only mark their own
            if (userRole == "Employee")
            {
                int userId = Convert.ToInt32(Session["UserID"]);

                // Check if the selected employee belongs to the logged-in user
                string query = "SELECT COUNT(*) FROM Employees WHERE EmployeeID = @EmployeeID AND UserID = @UserID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        conn.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }

            return false;
        }

        protected void txtFromDate_TextChanged(object sender, EventArgs e)
        {
            LoadAttendanceRecords();
            LoadAttendanceSummary();
        }

        protected void txtToDate_TextChanged(object sender, EventArgs e)
        {
            LoadAttendanceRecords();
            LoadAttendanceSummary();
        }

        protected void ddlFilterEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAttendanceRecords();
            LoadAttendanceSummary();
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttendance.PageIndex = e.NewPageIndex;
            LoadAttendanceRecords();
        }

        protected void gvAttendance_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int attendanceId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditAttendance":
                    EditAttendance(attendanceId);
                    break;
                case "DeleteAttendance":
                    DeleteAttendance(attendanceId);
                    break;
            }
        }

        private void EditAttendance(int attendanceId)
        {
            try
            {
                string query = @"SELECT a.*, e.FirstName + ' ' + e.LastName AS EmployeeName, e.UserID
                       FROM Attendance a
                       INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                       WHERE a.AttendanceID = @AttendanceID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttendanceID", attendanceId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Check permission before allowing edit
                                int employeeUserId = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : 0;
                                string userRole = Session["UserRole"]?.ToString() ?? "";
                                int currentUserId = Convert.ToInt32(Session["UserID"]);

                                if (userRole == "Employee" && employeeUserId != currentUserId)
                                {
                                    ShowMessage("You can only edit your own attendance records.", "danger");
                                    return;
                                }

                                hdnAttendanceID.Value = reader["AttendanceID"].ToString();
                                ddlEmployee.SelectedValue = reader["EmployeeID"].ToString();
                                txtAttendanceDate.Text = Convert.ToDateTime(reader["AttendanceDate"]).ToString("yyyy-MM-dd");
                                ddlStatus.SelectedValue = reader["Status"].ToString();
                                txtHours.Text = reader["Hours"].ToString();
                                txtNotes.Text = reader["Notes"]?.ToString() ?? "";

                                lblModalTitle.Text = "Edit Attendance Record";
                                btnSaveAttendance.Text = "Update Attendance";

                                string script = @"
                                setTimeout(function() {
                                    var modalEl = document.getElementById('addAttendanceModal');
                                    if (modalEl) {
                                        var modal = new bootstrap.Modal(modalEl, {
                                            backdrop: 'static',
                                            keyboard: false
                                        });
                                        modal.show();
                                    }
                                }, 100);";

                                Page.ClientScript.RegisterStartupScript(this.GetType(), "showEditModal", script, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading attendance details: " + ex.Message, "danger");
            }
        }

        private void DeleteAttendance(int attendanceId)
        {
            try
            {
                // Check permission before deleting
                string checkQuery = @"SELECT e.UserID FROM Attendance a 
                                    INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID 
                                    WHERE a.AttendanceID = @AttendanceID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Verify permission
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@AttendanceID", attendanceId);
                        object result = checkCmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int employeeUserId = Convert.ToInt32(result);
                            string userRole = Session["UserRole"]?.ToString() ?? "";
                            int currentUserId = Convert.ToInt32(Session["UserID"]);

                            if (userRole == "Employee" && employeeUserId != currentUserId)
                            {
                                ShowMessage("You can only delete your own attendance records.", "danger");
                                return;
                            }
                        }
                    }

                    // Proceed with deletion
                    string query = "DELETE FROM Attendance WHERE AttendanceID = @AttendanceID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttendanceID", attendanceId);
                        int deleteResult = cmd.ExecuteNonQuery();

                        if (deleteResult > 0)
                        {
                            LoadAttendanceRecords();
                            LoadAttendanceSummary();
                            ShowMessage("Attendance record deleted successfully!", "success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting attendance record: " + ex.Message, "danger");
            }
        }

        protected void btnExportAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = GetAttendanceDataForExport();
                ExportToCSV(dt, "Attendance_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
            }
            catch (Exception ex)
            {
                ShowMessage("Error exporting data: " + ex.Message, "danger");
            }
        }

        private DataTable GetAttendanceDataForExport()
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";
            int userId = Convert.ToInt32(Session["UserID"]);

            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT e.FirstName + ' ' + e.LastName AS EmployeeName,
                           a.AttendanceDate, a.Status, a.Hours, a.Notes
                           FROM Attendance a
                           INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
                           WHERE 1=1");

            var parameters = new List<SqlParameter>();

            // If Employee, only export their own records
            if (userRole == "Employee")
            {
                query.Append(" AND e.UserID = @UserID");
                parameters.Add(new SqlParameter("@UserID", userId));
            }

            query.Append(" ORDER BY a.AttendanceDate DESC, e.FirstName");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private void ExportToCSV(DataTable dt, string filename)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={filename}");
                Response.Charset = "";
                Response.ContentType = "application/text";

                StringBuilder sb = new StringBuilder();

                // Add column headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // Add data rows
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        sb.Append(dt.Rows[i][k].ToString().Replace(",", ";"));
                        if (k < dt.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage("Error exporting to CSV: " + ex.Message, "danger");
            }
        }

        private bool AttendanceExists(int employeeId, DateTime date)
        {
            string query = "SELECT COUNT(*) FROM Attendance WHERE EmployeeID = @EmployeeID AND AttendanceDate = @AttendanceDate";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@AttendanceDate", date);
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private bool ValidateAttendanceForm()
        {
            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                ShowMessage("Please select an employee.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtAttendanceDate.Text))
            {
                ShowMessage("Please select attendance date.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlStatus.SelectedValue))
            {
                ShowMessage("Please select attendance status.", "danger");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            hdnAttendanceID.Value = "";

            string userRole = Session["UserRole"]?.ToString() ?? "";

            // Only clear employee selection for Admin/HR
            if (userRole != "Employee")
            {
                ddlEmployee.SelectedIndex = 0;
            }

            txtAttendanceDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            ddlStatus.SelectedIndex = 0;
            txtHours.Text = "";
            txtNotes.Text = "";
            lblModalTitle.Text = "Mark Attendance";
            btnSaveAttendance.Text = "Save Attendance";
        }

        protected string GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "present":
                    return "success";
                case "late":
                    return "warning";
                case "absent":
                    return "danger";
                default:
                    return "secondary";
            }
        }

        private void ShowMessage(string message, string type = "info")
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Visible = true;
        }
    }
}