using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;





namespace HRManagement.Pages
{
    public partial class Payroll : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            // Handle manual postbacks
            string eventTarget = Request["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(eventTarget))
            {
                switch (eventTarget)
                {
                    case "btnProcessCurrentMonth":
                        ProcessCurrentMonthPayroll();
                        break;
                    case "btnExportPayroll":
                        btnExportPayroll_Click(null, null);
                        break;
                }
            }

            if (!IsPostBack)
            {
                LoadEmployeeDropdowns();
                PopulateYearDropdowns();
                SetDefaultValues();
                LoadPayrollSummary();
                LoadPayrollRecords();
            }
        }


        private void ProcessCurrentMonthPayroll()
        {
            try
            {
                ShowMessage("Processing payroll based on attendance...", "info");

                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;
                int processedCount = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get all active employees
                    string employeeQuery = "SELECT EmployeeID, ISNULL(Salary, 0) as Salary FROM Employees WHERE IsActive = 1";
                    using (SqlCommand empCmd = new SqlCommand(employeeQuery, conn))
                    {
                        using (SqlDataReader reader = empCmd.ExecuteReader())
                        {
                            List<(int EmployeeID, decimal Salary)> employees = new List<(int, decimal)>();
                            while (reader.Read())
                            {
                                employees.Add((
                                    Convert.ToInt32(reader["EmployeeID"]),
                                    Convert.ToDecimal(reader["Salary"])
                                ));
                            }
                            reader.Close();

                            foreach (var emp in employees)
                            {
                                // Get days worked based on attendance
                                string attendanceQuery = @"SELECT COUNT(*) FROM Attendance
                                                   WHERE EmployeeID = @EmployeeID
                                                   AND MONTH(AttendanceDate) = @Month
                                                   AND YEAR(AttendanceDate) = @Year
                                                   AND Status IN ('Present','Late')";
                                int daysWorked = 22; // default
                                using (SqlCommand attCmd = new SqlCommand(attendanceQuery, conn))
                                {
                                    attCmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                                    attCmd.Parameters.AddWithValue("@Month", currentMonth);
                                    attCmd.Parameters.AddWithValue("@Year", currentYear);

                                    object result = attCmd.ExecuteScalar();
                                    if (result != null && result != DBNull.Value)
                                        daysWorked = Convert.ToInt32(result);
                                }

                                // Calculate net salary based on attendance
                                decimal netSalary = (emp.Salary * daysWorked) / 22m;

                                // Insert if payroll doesn't exist
                                string checkQuery = "SELECT COUNT(*) FROM Payroll WHERE EmployeeID = @EmployeeID AND Month = @Month AND Year = @Year";
                                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                                {
                                    checkCmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                                    checkCmd.Parameters.AddWithValue("@Month", currentMonth);
                                    checkCmd.Parameters.AddWithValue("@Year", currentYear);

                                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                                    if (exists == 0)
                                    {
                                        string insertQuery = @"INSERT INTO Payroll
                                                       (EmployeeID, Month, Year, BasicSalary, NetSalary, DaysWorked, ProcessedDate)
                                                       VALUES (@EmployeeID,@Month,@Year,@BasicSalary,@NetSalary,@DaysWorked,@ProcessedDate)";
                                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                        {
                                            insertCmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                                            insertCmd.Parameters.AddWithValue("@Month", currentMonth);
                                            insertCmd.Parameters.AddWithValue("@Year", currentYear);
                                            insertCmd.Parameters.AddWithValue("@BasicSalary", emp.Salary);
                                            insertCmd.Parameters.AddWithValue("@NetSalary", netSalary);
                                            insertCmd.Parameters.AddWithValue("@DaysWorked", daysWorked);
                                            insertCmd.Parameters.AddWithValue("@ProcessedDate", DateTime.Now);

                                            insertCmd.ExecuteNonQuery();
                                            processedCount++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                LoadPayrollRecords();
                LoadPayrollSummary();
                ShowMessage($"Successfully processed payroll for {processedCount} employees!", "success");
            }
            catch (Exception ex)
            {
                ShowMessage("Error processing payroll: " + ex.Message, "danger");
            }
        }

        
        private void LoadEmployeeDropdowns()
        {
            try
            {
                string query = @"SELECT e.EmployeeID, e.FirstName + ' ' + e.LastName AS FullName, e.Salary
                               FROM Employees e 
                               WHERE e.IsActive = 1 
                               ORDER BY e.FirstName, e.LastName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // For add/edit form
                        ddlEmployee.DataSource = dt;
                        ddlEmployee.DataTextField = "FullName";
                        ddlEmployee.DataValueField = "EmployeeID";
                        ddlEmployee.DataBind();
                        ddlEmployee.Items.Insert(0, new ListItem("Select Employee", ""));

                        // For filter
                        ddlFilterEmployee.DataSource = dt;
                        ddlFilterEmployee.DataTextField = "FullName";
                        ddlFilterEmployee.DataValueField = "EmployeeID";
                        ddlFilterEmployee.DataBind();
                        ddlFilterEmployee.Items.Insert(0, new ListItem("All Employees", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading employees: " + ex.Message, "danger");
            }
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlEmployee.SelectedValue))
                {
                    int employeeId = int.Parse(ddlEmployee.SelectedValue);
                    int month = int.Parse(ddlMonth.SelectedValue);
                    int year = int.Parse(ddlYear.SelectedValue);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // --- Get Salary ---
                        string salaryQuery = "SELECT Salary FROM Employees WHERE EmployeeID = @EmployeeID";
                        decimal basicSalary = 0;
                        using (SqlCommand cmd = new SqlCommand(salaryQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                                basicSalary = Convert.ToDecimal(result);
                        }
                        txtBasicSalary.Text = basicSalary.ToString("F2");

                        // --- Get Days Worked from Attendance ---
                        int daysWorked = GetDaysWorkedForMonth(conn, employeeId, month, year);
                        txtDaysWorked.Text = daysWorked.ToString();

                        // --- Auto-calculate Net Salary ---
                        decimal netSalary = CalculateNetSalary(basicSalary, daysWorked);
                        txtNetSalary.Text = netSalary.ToString("F2");
                    }
                }
                else
                {
                    txtBasicSalary.Text = "";
                    txtDaysWorked.Text = "";
                    txtNetSalary.Text = "";
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading employee details: " + ex.Message, "danger");
            }
        }

        [System.Web.Services.WebMethod]
        public static object GetEmployeeDetails(int employeeId, int month, int year)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get Salary from Employees table
                string salaryQuery = "SELECT Salary FROM Employees WHERE EmployeeID = @EmployeeID";
                decimal salary = 0;
                using (SqlCommand cmd = new SqlCommand(salaryQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        salary = Convert.ToDecimal(result);
                }

                // Get Days Worked from Attendance table
                string attendanceQuery = @"SELECT COUNT(*) 
                                   FROM Attendance 
                                   WHERE EmployeeID = @EmployeeID 
                                   AND MONTH(AttendanceDate) = @Month 
                                   AND YEAR(AttendanceDate) = @Year 
                                   AND (Status = 'Present' OR Status = 'Late')";
                int daysWorked = 0;
                using (SqlCommand cmd = new SqlCommand(attendanceQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    daysWorked = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Net Salary (assuming 30 days per month)
                decimal netSalary = (salary / 30) * daysWorked;

                return new { Salary = salary, DaysWorked = daysWorked, NetSalary = netSalary };
            }
        }



        [System.Web.Services.WebMethod]

        public static decimal GetEmployeeSalary(int employeeId)
        {
            decimal salary = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Salary FROM Employees WHERE EmployeeID = @EmployeeID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        salary = Convert.ToDecimal(result);
                }
            }
            return salary;
        }



        private void PopulateYearDropdowns()
        {
            int currentYear = DateTime.Now.Year;

            // Populate year dropdowns with current year ± 2 years
            for (int year = currentYear - 2; year <= currentYear + 2; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
                ddlFilterYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }

            ddlFilterYear.Items.Insert(0, new ListItem("All Years", ""));
            ddlYear.SelectedValue = currentYear.ToString();
            ddlFilterYear.SelectedValue = currentYear.ToString();
        }

        private void SetDefaultValues()
        {
            int currentMonth = DateTime.Now.Month;
            ddlMonth.SelectedValue = currentMonth.ToString();
            ddlFilterMonth.SelectedValue = currentMonth.ToString();
        }

        private void LoadPayrollSummary()
        {
            try
            {
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;


                string query = @"SELECT 
                               COUNT(*) AS ProcessedRecords,
                               ISNULL(SUM(NetSalary), 0) AS TotalPayroll,
                               ISNULL(AVG(NetSalary), 0) AS AverageSalary,
                               COUNT(DISTINCT EmployeeID) AS EmployeesPaid
                               FROM Payroll 
                               WHERE Month = @Month AND Year = @Year";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", currentMonth);
                        cmd.Parameters.AddWithValue("@Year", currentYear);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblProcessedRecords.Text = reader["ProcessedRecords"].ToString();
                                lblTotalPayroll.Text = "₱" + Convert.ToDecimal(reader["TotalPayroll"]).ToString("N2");
                                lblAverageSalary.Text = "₱" + Convert.ToDecimal(reader["AverageSalary"]).ToString("N2");
                                lblEmployeesPaid.Text = reader["EmployeesPaid"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading payroll summary: " + ex.Message, "danger");
            }
        }

        private void LoadPayrollRecords()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append(@"SELECT p.PayrollID, p.Month, p.Year, p.BasicSalary, p.NetSalary, 
                              p.DaysWorked, p.ProcessedDate,
                              e.FirstName + ' ' + e.LastName AS EmployeeName,
                              e.EmployeeID
                              FROM Payroll p
                              INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
                              WHERE 1=1");

                var parameters = new List<SqlParameter>();

                // Apply employee filter
                if (!string.IsNullOrEmpty(ddlFilterEmployee.SelectedValue))
                {
                    query.Append(" AND p.EmployeeID = @EmployeeID");
                    parameters.Add(new SqlParameter("@EmployeeID", ddlFilterEmployee.SelectedValue));
                }

                // Apply month filter
                if (!string.IsNullOrEmpty(ddlFilterMonth.SelectedValue))
                {
                    query.Append(" AND p.Month = @Month");
                    parameters.Add(new SqlParameter("@Month", ddlFilterMonth.SelectedValue));
                }

                // Apply year filter
                if (!string.IsNullOrEmpty(ddlFilterYear.SelectedValue))
                {
                    query.Append(" AND p.Year = @Year");
                    parameters.Add(new SqlParameter("@Year", ddlFilterYear.SelectedValue));
                }

                // Order by ProcessedDate DESC so the latest processed records show first
                query.Append(" ORDER BY p.ProcessedDate DESC, p.Year DESC, p.Month DESC, e.FirstName");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        if (parameters.Count > 0)
                            cmd.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvPayroll.DataSource = dt;
                            gvPayroll.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading payroll records: " + ex.Message, "danger");
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadPayrollRecords();
            LoadPayrollSummary();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            ddlFilterEmployee.SelectedIndex = 0;
            ddlFilterMonth.SelectedIndex = 0;
            ddlFilterYear.SelectedIndex = 0;
            LoadPayrollRecords();
            LoadPayrollSummary();
        }

        // --- FIXED: insert or update current month payroll, and always update ProcessedDate ---
        protected void btnProcessCurrentMonth_Click(object sender, EventArgs e)
        {
            ProcessCurrentMonthPayroll();
        }

        protected void btnExportPayroll_Click(object sender, EventArgs e)
        {
            try
            {
                // Debug: Log that method is called
                System.Diagnostics.Debug.WriteLine("Export button clicked!");

                // Show loading message
                ShowMessage("Preparing export...", "info");

                // Get real data from database
                DataTable dt = GetPayrollDataForExport(
                    string.IsNullOrEmpty(ddlFilterEmployee.SelectedValue) ? (int?)null : int.Parse(ddlFilterEmployee.SelectedValue),
                    string.IsNullOrEmpty(ddlFilterMonth.SelectedValue) ? (int?)null : int.Parse(ddlFilterMonth.SelectedValue),
                    string.IsNullOrEmpty(ddlFilterYear.SelectedValue) ? (int?)null : int.Parse(ddlFilterYear.SelectedValue)
                );

                if (dt.Rows.Count == 0)
                {
                    ShowMessage("No data found to export.", "warning");
                    return;
                }

                // Create CSV
                StringBuilder csv = new StringBuilder();

                // Headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    csv.Append(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1) csv.Append(",");
                }
                csv.AppendLine();

                // Data rows
                foreach (DataRow dataRow in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string value = dataRow[i]?.ToString() ?? "";
                        value = value.Replace("\"", "\"\""); // Escape quotes
                        csv.Append("\"" + value + "\"");
                        if (i < dt.Columns.Count - 1) csv.Append(",");
                    }
                    csv.AppendLine();
                }

                // Export
                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", "attachment; filename=PayrollExport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                Response.Write(csv.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage("Export error: " + ex.Message, "danger");
                System.Diagnostics.Debug.WriteLine("Export error: " + ex.ToString());
            }
        }


        protected void btnCalculateAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                pnlMessage.Visible = false;

                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                ShowMessage("Calculating attendance-based payroll...", "info");

                // Simple update for existing payroll records
                string updateQuery = @"UPDATE Payroll 
                             SET DaysWorked = 22, 
                                 NetSalary = BasicSalary, 
                                 ProcessedDate = @ProcessedDate
                             WHERE Month = @Month 
                             AND Year = @Year";

                int updatedCount = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", currentMonth);
                        cmd.Parameters.AddWithValue("@Year", currentYear);
                        cmd.Parameters.AddWithValue("@ProcessedDate", DateTime.Now);

                        conn.Open();
                        updatedCount = cmd.ExecuteNonQuery();
                    }
                }

                // Refresh the data
                LoadPayrollRecords();
                LoadPayrollSummary();

                ShowMessage($"Successfully updated {updatedCount} payroll records based on attendance for {DateTime.Now.ToString("MMMM yyyy")}!", "success");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error calculating attendance-based payroll: {ex.Message}", "danger");
                System.Diagnostics.Debug.WriteLine($"Attendance calculation error: {ex.ToString()}");
            }
        }


        protected void btnSavePayroll_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidatePayrollForm())
                {
                    if (string.IsNullOrEmpty(hdnPayrollID.Value))
                    {
                        AddPayroll();
                    }
                    else
                    {
                        UpdatePayroll();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving payroll: " + ex.Message, "danger");
            }
        }

        private void AddPayroll()
        {
            // Check if payroll already exists for this employee and period
            if (PayrollExists(int.Parse(ddlEmployee.SelectedValue), int.Parse(ddlMonth.SelectedValue), int.Parse(ddlYear.SelectedValue)))
            {
                ShowMessage("Payroll record already exists for this employee and period.", "warning");
                return;
            }

            string query = @"INSERT INTO Payroll (EmployeeID, Month, Year, BasicSalary, NetSalary, DaysWorked, ProcessedDate)
                   VALUES (@EmployeeID, @Month, @Year, @BasicSalary, @NetSalary, @DaysWorked, @ProcessedDate)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", ddlEmployee.SelectedValue);
                    cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                    cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                    cmd.Parameters.AddWithValue("@BasicSalary", decimal.Parse(txtBasicSalary.Text));

                    decimal netSalary;
                    if (string.IsNullOrEmpty(txtNetSalary.Text))
                    {
                        // Auto-calculate net salary
                        netSalary = CalculateNetSalary(decimal.Parse(txtBasicSalary.Text), int.Parse(txtDaysWorked.Text));
                    }
                    else
                    {
                        netSalary = decimal.Parse(txtNetSalary.Text);
                    }

                    cmd.Parameters.AddWithValue("@NetSalary", netSalary);
                    cmd.Parameters.AddWithValue("@DaysWorked", int.Parse(txtDaysWorked.Text));
                    cmd.Parameters.AddWithValue("@ProcessedDate", DateTime.Now);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ClearForm();
                        LoadPayrollRecords();
                        LoadPayrollSummary();
                        ShowMessage("Payroll record added successfully!", "success");

                        // FIXED: Use Bootstrap 5 syntax
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hideModal",
                            "var modal = bootstrap.Modal.getInstance(document.getElementById('addPayrollModal')); modal.hide();", true);
                    }
                }
            }
        }
        private void UpdatePayroll()
        {
            string query = @"UPDATE Payroll 
                   SET EmployeeID = @EmployeeID, Month = @Month, Year = @Year, 
                       BasicSalary = @BasicSalary, NetSalary = @NetSalary, DaysWorked = @DaysWorked, ProcessedDate = @ProcessedDate
                   WHERE PayrollID = @PayrollID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PayrollID", hdnPayrollID.Value);
                    cmd.Parameters.AddWithValue("@EmployeeID", ddlEmployee.SelectedValue);
                    cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                    cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                    cmd.Parameters.AddWithValue("@BasicSalary", decimal.Parse(txtBasicSalary.Text));

                    decimal netSalary;
                    if (string.IsNullOrEmpty(txtNetSalary.Text))
                    {
                        netSalary = CalculateNetSalary(decimal.Parse(txtBasicSalary.Text), int.Parse(txtDaysWorked.Text));
                    }
                    else
                    {
                        netSalary = decimal.Parse(txtNetSalary.Text);
                    }

                    cmd.Parameters.AddWithValue("@NetSalary", netSalary);
                    cmd.Parameters.AddWithValue("@DaysWorked", int.Parse(txtDaysWorked.Text));
                    cmd.Parameters.AddWithValue("@ProcessedDate", DateTime.Now);

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ClearForm();
                        LoadPayrollRecords();
                        LoadPayrollSummary();
                        ShowMessage("Payroll record updated successfully!", "success");

                        // FIXED: Use Bootstrap 5 syntax
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "hideModal",
                            "var modal = bootstrap.Modal.getInstance(document.getElementById('addPayrollModal')); modal.hide();", true);
                    }
                }
            }
        }
        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtBasicSalary.Text) && !string.IsNullOrEmpty(txtDaysWorked.Text))
                {
                    decimal basicSalary = decimal.Parse(txtBasicSalary.Text);
                    int daysWorked = int.Parse(txtDaysWorked.Text);

                    decimal netSalary = CalculateNetSalary(basicSalary, daysWorked);
                    txtNetSalary.Text = netSalary.ToString("F2");
                }
                else
                {
                    ShowMessage("Please enter basic salary and days worked to calculate.", "warning");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error calculating net salary: " + ex.Message, "danger");
            }
        }

        protected void gvPayroll_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPayroll.PageIndex = e.NewPageIndex;
            LoadPayrollRecords();
        }

        protected void gvPayroll_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int payrollId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewPayroll":
                    ViewPayrollDetails(payrollId);
                    break;
                case "EditPayroll":
                    EditPayroll(payrollId);
                    break;
                case "DeletePayroll":
                    DeletePayroll(payrollId);
                    break;
            }
        }

        private void ViewPayrollDetails(int payrollId)
        {
            // For now, just edit the record. In a full implementation, 
            // you might want a separate view-only modal
            EditPayroll(payrollId);
        }

        private void EditPayroll(int payrollId)
        {
            try
            {
                string query = @"SELECT * FROM Payroll WHERE PayrollID = @PayrollID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PayrollID", payrollId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hdnPayrollID.Value = reader["PayrollID"].ToString();
                                ddlEmployee.SelectedValue = reader["EmployeeID"].ToString();
                                ddlMonth.SelectedValue = reader["Month"].ToString();
                                ddlYear.SelectedValue = reader["Year"].ToString();
                                txtBasicSalary.Text = reader["BasicSalary"].ToString();
                                txtNetSalary.Text = reader["NetSalary"].ToString();
                                txtDaysWorked.Text = reader["DaysWorked"].ToString();

                                lblModalTitle.Text = "Edit Payroll Record";
                                btnSavePayroll.Text = "Update Payroll";

                                // FIXED: Use Bootstrap 5 syntax with timeout for reliability
                                string script = @"
                            setTimeout(function() {
                                var modalEl = document.getElementById('addPayrollModal');
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
                ShowMessage("Error loading payroll details: " + ex.Message, "danger");
            }
        }

        private void DeletePayroll(int payrollId)
        {
            try
            {
                string query = "DELETE FROM Payroll WHERE PayrollID = @PayrollID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PayrollID", payrollId);
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            LoadPayrollRecords();
                            LoadPayrollSummary();
                            ShowMessage("Payroll record deleted successfully!", "success");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting payroll record: " + ex.Message, "danger");
            }
        }





        // Helper Methods

        private int GetDaysWorkedForMonth(SqlConnection conn, int employeeId, int month, int year)
        {
            string query = @"SELECT COUNT(*) FROM Attendance 
                           WHERE EmployeeID = @EmployeeID 
                           AND MONTH(a.AttendanceDate) = @Month 
                           AND YEAR(a.AttendanceDate) = @Year 
                           AND Status IN ('Present', 'Late')";

            // Note: some SQL Servers require alias in WHERE if used; we'll use a direct query below
            query = @"SELECT COUNT(*) FROM Attendance 
                      WHERE EmployeeID = @EmployeeID 
                      AND MONTH(AttendanceDate) = @Month 
                      AND YEAR(AttendanceDate) = @Year 
                      AND Status IN ('Present', 'Late')";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                object result = cmd.ExecuteScalar();
                return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 22; // Default to 22 working days
            }
        }

        private decimal CalculateNetSalary(decimal basicSalary, int daysWorked)
        {
            // Simple calculation: (Basic Salary * Days Worked) / 22 working days
            // In a real system, you'd include allowances, deductions, taxes, etc.
            decimal workingDaysInMonth = 22m;
            return (basicSalary * daysWorked) / workingDaysInMonth;
        }

        private bool PayrollExists(int employeeId, int month, int year)
        {
            string query = @"SELECT COUNT(*) FROM Payroll 
                           WHERE EmployeeID = @EmployeeID AND Month = @Month AND Year = @Year";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private DataTable GetPayrollDataForExport(int? employeeId = null, int? month = null, int? year = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
        SELECT 
            e.FirstName + ' ' + e.LastName AS EmployeeName,
            DATENAME(MONTH, DATEFROMPARTS(p.Year, p.Month, 1)) AS MonthName,
            p.Year,
            p.BasicSalary,
            p.NetSalary,
            p.DaysWorked,
            p.ProcessedDate
        FROM Payroll p
        INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
        WHERE 1=1
    ");

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (employeeId.HasValue)
            {
                query.Append(" AND p.EmployeeID = @EmployeeID");
                parameters.Add(new SqlParameter("@EmployeeID", employeeId.Value));
            }

            if (month.HasValue)
            {
                query.Append(" AND p.Month = @Month");
                parameters.Add(new SqlParameter("@Month", month.Value));
            }

            if (year.HasValue)
            {
                query.Append(" AND p.Year = @Year");
                parameters.Add(new SqlParameter("@Year", year.Value));
            }

            query.Append(" ORDER BY p.Year DESC, p.Month DESC, e.FirstName");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                {
                    if (parameters.Count > 0)
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
            StringBuilder sb = new StringBuilder();

            // Add column headers
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append("\"" + dt.Columns[i].ColumnName + "\"");
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // Add data rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = Convert.ToString(row[i]).Replace("\"", "\"\"");
                    sb.Append("\"" + value + "\""); // wrap in quotes
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            // Send file to browser
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", $"attachment; filename={filename}");
            Response.Write(sb.ToString());
            Response.Flush();

            // Stop further HTML rendering
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }


        private bool ValidatePayrollForm()
        {
            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                ShowMessage("Please select an employee.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlMonth.SelectedValue))
            {
                ShowMessage("Please select a month.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlYear.SelectedValue))
            {
                ShowMessage("Please select a year.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtBasicSalary.Text))
            {
                ShowMessage("Please enter basic salary.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtDaysWorked.Text))
            {
                ShowMessage("Please enter days worked.", "danger");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            hdnPayrollID.Value = "";
            ddlEmployee.SelectedIndex = 0;
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
            ddlYear.SelectedValue = DateTime.Now.Year.ToString();
            txtBasicSalary.Text = "";
            txtNetSalary.Text = "";
            txtDaysWorked.Text = "";
            lblModalTitle.Text = "Process Payroll";
            btnSavePayroll.Text = "Save Payroll";
        }

        protected void FilterPayroll(object sender, EventArgs e)
        {
            LoadPayrollRecords();
        }


        private void ShowMessage(string message, string type = "info")
        {
            lblMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
            pnlMessage.Visible = true;
        }
    }
}



