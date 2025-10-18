using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;

namespace HRManagement.Pages
{
    public partial class EmployeeProfile : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HRConnectionString"].ConnectionString;
        private int employeeId;

        // ✅ Force currency formatting to Philippine Peso for the whole page
        protected override void InitializeCulture()
        {
            CultureInfo philippineCulture = new CultureInfo("en-PH");
            Thread.CurrentThread.CurrentCulture = philippineCulture;
            Thread.CurrentThread.CurrentUICulture = philippineCulture;
            base.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Check authentication - allow both Admin/HR and Employees
            if (Session["Username"] == null)
            {
                Response.Redirect("~/Pages/Users/Login.aspx");
                return;
            }

            // ✅ If Employee logs in → only see their own profile
            if (Session["UserRole"]?.ToString() == "Employee")
            {
                if (Session["EmployeeID"] == null)
                {
                    Response.Redirect("~/Pages/Users/Login.aspx");
                    return;
                }
                employeeId = Convert.ToInt32(Session["EmployeeID"]);

                if (!IsPostBack)
                {
                    // Hide edit/upload for employees
                    btnUpdateEmployee.Visible = false;
                    btnUploadPhoto.Visible = false;
                    lnkBackToEmployees.Visible = false; // hide "Back to Employees" button

                    // Remove Edit button in card header if it exists
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "hideEditBtn",
                        "document.querySelector('[data-bs-target=\"#editEmployeeModal\"]')?.remove();", true);
                }
            }
            else
            {
                // ✅ If Admin/HR logs in → can pass ?id=EmployeeID to view/edit profile
                if (!int.TryParse(Request.QueryString["id"], out employeeId))
                {
                    Response.Redirect("~/Pages/Employees.aspx");
                    return;
                }
            }

            if (!IsPostBack)
            {
                LoadEmployeeProfile();
                LoadDropDownsForEdit();
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



        /// <summary>
        /// Load complete employee profile with all details
        /// </summary>
        private void LoadEmployeeProfile()
        {
            try
            {
                LoadBasicInfo();
                LoadAttendanceSummary();
                LoadRecentAttendance();
                LoadPayrollHistory();
            }
            catch (Exception ex)
            {
                ShowEditMessage("Error loading employee profile: " + ex.Message, "danger");
            }
        }

        private void LoadBasicInfo()
        {
            string query = @"SELECT e.EmployeeID, e.FirstName, e.LastName,
                           e.FirstName + ' ' + e.LastName AS FullName,
                           e.Email, e.PhoneNumber, e.Salary, e.DateJoined, e.Address,
                           d.DepartmentName, d.DepartmentID, r.RoleName, r.RoleID,
                           DATEDIFF(YEAR, e.DateJoined, GETDATE()) AS YearsOfService,
                           e.Photo
                           FROM Employees e
                           LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                           LEFT JOIN Roles r ON e.RoleID = r.RoleID
                           WHERE e.EmployeeID = @EmployeeID AND e.IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblEmployeeName.Text = reader["FullName"]?.ToString() ?? "";
                        lblEmployeeRole.Text = reader["RoleName"]?.ToString() ?? "";
                        lblEmployeeDepartment.Text = reader["DepartmentName"]?.ToString() ?? "";

                        // ✅ Format salary in Philippine Peso
                        if (reader["Salary"] != DBNull.Value && decimal.TryParse(reader["Salary"].ToString(), out decimal salary))
                        {
                            lblSalary.Text = FormatToPeso(salary);
                        }
                        else
                        {
                            lblSalary.Text = "N/A";
                        }

                        lblYearsOfService.Text = reader["YearsOfService"]?.ToString() ?? "0";
                        lblEmployeeID.Text = reader["EmployeeID"]?.ToString() ?? "";
                        lblEmail.Text = reader["Email"]?.ToString() ?? "";
                        lblPhone.Text = reader["PhoneNumber"]?.ToString() ?? "N/A";

                        if (reader["DateJoined"] != DBNull.Value && DateTime.TryParse(reader["DateJoined"].ToString(), out DateTime dj))
                        {
                            lblDateJoined.Text = dj.ToString("MMMM dd, yyyy");
                        }
                        else
                        {
                            lblDateJoined.Text = "N/A";
                        }

                        lblAddress.Text = reader["Address"]?.ToString() ?? "N/A";

                        // Show photo if available
                        if (reader["Photo"] != DBNull.Value && !string.IsNullOrEmpty(reader["Photo"].ToString()))
                        {
                            string photoPath = reader["Photo"].ToString();
                            imgProfile.ImageUrl = photoPath;
                            imgPreview.ImageUrl = photoPath;
                        }
                        else
                        {
                            imgProfile.ImageUrl = "~/Images/default-avatar.png";
                            imgPreview.ImageUrl = "~/Images/default-avatar.png";
                        }

                        PopulateEditForm(reader);
                    }
                    else
                    {
                        Response.Redirect("~/pages/Employees.aspx");
                    }
                }
            }
        }

        /// <summary>
        /// Format decimal amount to Philippine Peso currency
        /// </summary>
        private string FormatToPeso(decimal amount)
        {
            CultureInfo philippineCulture = new CultureInfo("en-PH");
            return amount.ToString("C", philippineCulture);
        }

        private void LoadAttendanceSummary()
        {
            string query = @"SELECT 
                           COUNT(*) AS TotalDays,
                           SUM(CASE WHEN Status = 'Present' THEN 1 ELSE 0 END) AS PresentDays,
                           SUM(CASE WHEN Status = 'Absent' THEN 1 ELSE 0 END) AS AbsentDays,
                           SUM(CASE WHEN Status = 'Late' THEN 1 ELSE 0 END) AS LateDays,
                           AVG(CASE WHEN Status IN ('Present', 'Late') THEN Hours ELSE NULL END) AS AverageHours
                           FROM Attendance 
                           WHERE EmployeeID = @EmployeeID 
                           AND AttendanceDate >= DATEADD(DAY, -30, GETDATE())";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblPresentDays.Text = reader["PresentDays"]?.ToString() ?? "0";
                        lblAbsentDays.Text = reader["AbsentDays"]?.ToString() ?? "0";
                        lblLateDays.Text = reader["LateDays"]?.ToString() ?? "0";

                        if (reader["AverageHours"] != DBNull.Value && decimal.TryParse(reader["AverageHours"].ToString(), out decimal avgH))
                        {
                            lblAverageHours.Text = avgH.ToString("F1");
                        }
                        else
                        {
                            lblAverageHours.Text = "0.0";
                        }
                    }
                }
            }
        }

        private void LoadRecentAttendance()
        {
            string query = @"SELECT TOP 10 AttendanceDate, Status, Hours
                           FROM Attendance 
                           WHERE EmployeeID = @EmployeeID
                           ORDER BY AttendanceDate DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvRecentAttendance.DataSource = dt;
                gvRecentAttendance.DataBind();
            }
        }

        private void LoadPayrollHistory()
        {
            try
            {
                string query = @"
                    SELECT TOP 6
                        p.PayrollID,
                        CAST(p.Month AS VARCHAR(2)) + '/' + CAST(p.Year AS VARCHAR(4)) AS PayrollPeriod,
                        p.Month,
                        p.Year,
                        p.BasicSalary,
                        p.NetSalary,
                        p.DaysWorked,
                        p.ProcessedDate
                    FROM Payroll p
                    WHERE p.EmployeeID = @EmployeeID
                    ORDER BY p.Year DESC, p.Month DESC, p.ProcessedDate DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@EmployeeID", employeeId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // ✅ GridView will now auto-format BasicSalary & NetSalary in ₱ due to InitializeCulture()
                    gvPayrollHistory.DataSource = dt;
                    gvPayrollHistory.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage("Error loading payroll history: " + ex.Message, "danger");
            }
        }

        private void LoadDropDownsForEdit()
        {
            try
            {
                string deptQuery = "SELECT DepartmentID, DepartmentName FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(deptQuery, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ddlEditDepartment.DataSource = dt;
                    ddlEditDepartment.DataTextField = "DepartmentName";
                    ddlEditDepartment.DataValueField = "DepartmentID";
                    ddlEditDepartment.DataBind();
                }

                string roleQuery = "SELECT RoleID, RoleName FROM Roles WHERE IsActive = 1 ORDER BY RoleName";
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(roleQuery, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ddlEditRole.DataSource = dt;
                    ddlEditRole.DataTextField = "RoleName";
                    ddlEditRole.DataValueField = "RoleID";
                    ddlEditRole.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage("Error loading dropdown data: " + ex.Message, "danger");
            }
        }

        private void PopulateEditForm(SqlDataReader reader)
        {
            txtEditFirstName.Text = reader["FirstName"]?.ToString() ?? "";
            txtEditLastName.Text = reader["LastName"]?.ToString() ?? "";
            txtEditEmail.Text = reader["Email"]?.ToString() ?? "";
            txtEditPhone.Text = reader["PhoneNumber"]?.ToString() ?? "";
            txtEditSalary.Text = reader["Salary"] != DBNull.Value ? reader["Salary"].ToString() : "";
            txtEditDateJoined.Text = reader["DateJoined"] != DBNull.Value ? Convert.ToDateTime(reader["DateJoined"]).ToString("yyyy-MM-dd") : "";
            txtEditAddress.Text = reader["Address"]?.ToString() ?? "";

            ViewState["DepartmentID"] = reader["DepartmentID"]?.ToString();
            ViewState["RoleID"] = reader["RoleID"]?.ToString();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ViewState["DepartmentID"] != null && ddlEditDepartment.Items.FindByValue(ViewState["DepartmentID"].ToString()) != null)
            {
                ddlEditDepartment.SelectedValue = ViewState["DepartmentID"].ToString();
            }
            if (ViewState["RoleID"] != null && ddlEditRole.Items.FindByValue(ViewState["RoleID"].ToString()) != null)
            {
                ddlEditRole.SelectedValue = ViewState["RoleID"].ToString();
            }
        }

        protected void btnUpdateEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateEditForm())
                {
                    string query = @"UPDATE Employees 
                                   SET FirstName = @FirstName, LastName = @LastName, Email = @Email,
                                       PhoneNumber = @Phone, DepartmentID = @DepartmentID, RoleID = @RoleID,
                                       Salary = @Salary, DateJoined = @DateJoined, Address = @Address
                                   WHERE EmployeeID = @EmployeeID";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.Parameters.AddWithValue("@FirstName", txtEditFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtEditLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEditEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(txtEditPhone.Text) ? DBNull.Value : (object)txtEditPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@DepartmentID", ddlEditDepartment.SelectedValue);
                        cmd.Parameters.AddWithValue("@RoleID", ddlEditRole.SelectedValue);
                        cmd.Parameters.AddWithValue("@Salary", string.IsNullOrEmpty(txtEditSalary.Text) ? DBNull.Value : (object)decimal.Parse(txtEditSalary.Text));
                        cmd.Parameters.AddWithValue("@DateJoined", DateTime.Parse(txtEditDateJoined.Text));
                        cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(txtEditAddress.Text) ? DBNull.Value : (object)txtEditAddress.Text.Trim());

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            LoadEmployeeProfile();
                            ShowEditMessage("Employee information updated successfully!", "success");

                            ScriptManager.RegisterStartupScript(this, GetType(), "hideEditModal",
                                "var m = bootstrap.Modal.getOrCreateInstance(document.getElementById('editEmployeeModal')); m.hide();", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage("Error updating employee: " + ex.Message, "danger");
            }
        }

        private bool ValidateEditForm()
        {
            if (string.IsNullOrEmpty(txtEditFirstName.Text.Trim()))
            {
                ShowEditMessage("First name is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtEditLastName.Text.Trim()))
            {
                ShowEditMessage("Last name is required.", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(txtEditEmail.Text.Trim()))
            {
                ShowEditMessage("Email is required.", "danger");
                return false;
            }

            if (IsEmailExistsForOther(txtEditEmail.Text.Trim()))
            {
                ShowEditMessage("Email already exists for another employee.", "danger");
                return false;
            }

            return true;
        }

        private bool IsEmailExistsForOther(string email)
        {
            string query = "SELECT COUNT(*) FROM Employees WHERE Email = @Email AND EmployeeID != @EmployeeID AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        protected void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUpload.HasFile)
                {
                    string ext = Path.GetExtension(fileUpload.FileName).ToLower();
                    string[] allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (Array.IndexOf(allowed, ext) < 0)
                    {
                        ShowEditMessage("Only JPG/PNG/GIF images are allowed.", "warning");
                        return;
                    }

                    if (fileUpload.PostedFile.ContentLength > 2 * 1024 * 1024)
                    {
                        ShowEditMessage("File size must be 2MB or less.", "warning");
                        return;
                    }

                    string fileName = $"{employeeId}_{DateTime.Now.Ticks}{ext}";
                    string folder = Server.MapPath("~/Uploads/Employees/");
                    Directory.CreateDirectory(folder);

                    string savePath = Path.Combine(folder, fileName);
                    fileUpload.SaveAs(savePath);

                    string virtualPath = "~/Uploads/Employees/" + fileName;

                    string query = "UPDATE Employees SET Photo = @Photo WHERE EmployeeID = @EmployeeID";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Photo", virtualPath);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowEditMessage("Photo uploaded successfully!", "success");

                    imgProfile.ImageUrl = virtualPath;
                    imgPreview.ImageUrl = virtualPath;

                    ScriptManager.RegisterStartupScript(this, GetType(), "hideUploadModal",
                        "var m = bootstrap.Modal.getOrCreateInstance(document.getElementById('uploadPhotoModal')); m.hide();", true);
                }
                else
                {
                    ShowEditMessage("Please select a photo to upload.", "warning");
                }
            }
            catch (Exception ex)
            {
                ShowEditMessage("Error uploading photo: " + ex.Message, "danger");
            }
        }

        protected string GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "present": return "success";
                case "late": return "warning";
                case "absent": return "danger";
                default: return "secondary";
            }
        }

        private void ShowEditMessage(string message, string type = "info")
        {
            lblEditMessage.Text = message;
            pnlEditMessage.CssClass = $"alert alert-{type}";
            pnlEditMessage.Visible = true;
        }
    }
}
