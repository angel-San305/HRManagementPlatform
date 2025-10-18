<%@ Page Title="Employee Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeProfile.aspx.cs" Inherits="HRManagement.Pages.EmployeeProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Back Button -->
    <!-- Back Button -->
<div class="mb-3">
    <asp:HyperLink ID="lnkBackToEmployees" runat="server" 
        NavigateUrl="~/Pages/Employees.aspx" 
        CssClass="btn btn-outline-secondary">
        <i class="fas fa-arrow-left me-2"></i>Back to Employees
    </asp:HyperLink>
</div>


    <!-- Employee Profile Header -->
    <div class="row">
        <div class="col-lg-4">
            <div class="card shadow mb-4">
                <div class="card-body text-center">
                    <div class="mb-3 position-relative">
                        <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle border" 
                                 Width="120" Height="120" ImageUrl="~/Images/default-avatar.png" />
                        <button type="button" class="btn btn-primary btn-sm position-absolute bottom-0 end-0 rounded-circle"
                                style="width: 35px; height: 35px;" data-bs-toggle="modal" data-bs-target="#uploadPhotoModal">
                            <i class="fas fa-camera"></i>
                        </button>
                    </div>
                    <h4 class="mb-1">
                        <asp:Label ID="lblEmployeeName" runat="server"></asp:Label>
                    </h4>
                    <p class="text-muted mb-2">
                        <asp:Label ID="lblEmployeeRole" runat="server"></asp:Label>
                    </p>
                    <p class="text-muted mb-3">
                        <asp:Label ID="lblEmployeeDepartment" runat="server"></asp:Label>
                    </p>
                    <div class="row text-center">
                        <div class="col-6">
                            <h6 class="text-primary">Salary</h6>
                            <p class="mb-0">
                                <asp:Label ID="lblSalary" runat="server"></asp:Label>
                            </p>
                        </div>
                        <div class="col-6">
                            <h6 class="text-primary">Years</h6>
                            <p class="mb-0">
                                <asp:Label ID="lblYearsOfService" runat="server"></asp:Label>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-8">
            <!-- Basic Information -->
            <div class="card shadow mb-4">
                <div class="card-header py-3 d-flex justify-content-between align-items-center">
                    <h6 class="m-0 font-weight-bold text-primary">Basic Information</h6>
                    <button type="button" class="btn btn-warning btn-sm" data-bs-toggle="modal" data-bs-target="#editEmployeeModal">
                        <i class="fas fa-edit me-1"></i>Edit
                    </button>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <strong>Email:</strong><br />
                            <asp:Label ID="lblEmail" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                        <div class="col-md-6 mb-3">
                            <strong>Phone:</strong><br />
                            <asp:Label ID="lblPhone" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                        <div class="col-md-6 mb-3">
                            <strong>Date Joined:</strong><br />
                            <asp:Label ID="lblDateJoined" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                        <div class="col-md-6 mb-3">
                            <strong>Employee ID:</strong><br />
                            <asp:Label ID="lblEmployeeID" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                        <div class="col-12">
                            <strong>Address:</strong><br />
                            <asp:Label ID="lblAddress" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Attendance Summary -->
    <div class="row">
        <div class="col-lg-6">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">Attendance Summary (Last 30 Days)</h6>
                </div>
                <div class="card-body">
                    <div class="row text-center">
                        <div class="col-4">
                            <div class="border-end">
                                <h4 class="text-success">
                                    <asp:Label ID="lblPresentDays" runat="server" Text="0"></asp:Label>
                                </h4>
                                <small class="text-muted">Present</small>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="border-end">
                                <h4 class="text-warning">
                                    <asp:Label ID="lblLateDays" runat="server" Text="0"></asp:Label>
                                </h4>
                                <small class="text-muted">Late</small>
                            </div>
                        </div>
                        <div class="col-4">
                            <h4 class="text-danger">
                                <asp:Label ID="lblAbsentDays" runat="server" Text="0"></asp:Label>
                            </h4>
                            <small class="text-muted">Absent</small>
                        </div>
                    </div>
                    <hr />
                    <div class="text-center">
                        <p class="mb-1"><strong>Average Hours per Day:</strong></p>
                        <h5 class="text-primary">
                            <asp:Label ID="lblAverageHours" runat="server" Text="0"></asp:Label> hours
                        </h5>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-6">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">Recent Attendance</h6>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvRecentAttendance" runat="server" CssClass="table table-sm" 
                                AutoGenerateColumns="false" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="AttendanceDate" HeaderText="Date" DataFormatString="{0:MMM dd}" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class="badge bg-<%# GetStatusColor(Eval("Status").ToString()) %>">
                                        <%# Eval("Status") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Hours" HeaderText="Hours" DataFormatString="{0:F1}" />
                        </Columns>
                        <HeaderStyle CssClass="table-primary" />
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-3">
                                <p>No attendance records found</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <!-- Payroll History -->
    <div class="card shadow mb-4">
        <div class="card-header py-3">
            <h6 class="m-0 font-weight-bold text-primary">Recent Payroll History</h6>
        </div>
        <div class="card-body">
            <div class="table-responsive">
                <asp:GridView ID="gvPayrollHistory" runat="server" CssClass="table table-striped" 
                            AutoGenerateColumns="false" GridLines="None">
                    <Columns>
                        <asp:TemplateField HeaderText="Period">
                            <ItemTemplate>
                                <%# Eval("Month") %>/<%# Eval("Year") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BasicSalary" HeaderText="Basic Salary" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="NetSalary" HeaderText="Net Salary" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DaysWorked" HeaderText="Days Worked" />
                        <asp:BoundField DataField="ProcessedDate" HeaderText="Processed Date" DataFormatString="{0:MMM dd, yyyy}" />
                    </Columns>
                    <HeaderStyle CssClass="table-primary" />
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-3">
                            <i class="fas fa-money-bill fa-2x mb-3"></i>
                            <p>No payroll records found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Edit Employee Modal -->
    <div class="modal fade" id="editEmployeeModal" tabindex="-1" aria-labelledby="editEmployeeModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="editEmployeeModalLabel">Edit Employee Information</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="pnlEditMessage" runat="server" Visible="false" CssClass="alert alert-info">
                        <asp:Label ID="lblEditMessage" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label for="txtEditFirstName" class="form-label">First Name *</label>
                            <asp:TextBox ID="txtEditFirstName" runat="server" CssClass="form-control" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtEditLastName" class="form-label">Last Name *</label>
                            <asp:TextBox ID="txtEditLastName" runat="server" CssClass="form-control" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtEditEmail" class="form-label">Email *</label>
                            <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" TextMode="Email" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtEditPhone" class="form-label">Phone Number</label>
                            <asp:TextBox ID="txtEditPhone" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="ddlEditDepartment" class="form-label">Department *</label>
                            <asp:DropDownList ID="ddlEditDepartment" runat="server" CssClass="form-select" required="true">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="ddlEditRole" class="form-label">Role *</label>
                            <asp:DropDownList ID="ddlEditRole" runat="server" CssClass="form-select" required="true">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtEditSalary" class="form-label">Salary</label>
                            <asp:TextBox ID="txtEditSalary" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtEditDateJoined" class="form-label">Date Joined *</label>
                            <asp:TextBox ID="txtEditDateJoined" runat="server" CssClass="form-control" TextMode="Date" required="true" />
                        </div>
                        <div class="col-12 mb-3">
                            <label for="txtEditAddress" class="form-label">Address</label>
                            <asp:TextBox ID="txtEditAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnUpdateEmployee" runat="server" Text="Update Employee" CssClass="btn btn-primary"
                              OnClick="btnUpdateEmployee_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Upload Photo Modal -->
    <div class="modal fade" id="uploadPhotoModal" tabindex="-1" aria-labelledby="uploadPhotoModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="uploadPhotoModalLabel">Update Profile Photo</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="text-center mb-3">
                        <asp:Image ID="imgPreview" runat="server" CssClass="rounded-circle border" 
                                 Width="100" Height="100" ImageUrl="~/Images/default-avatar.png" />
                    </div>
                    <div class="mb-3">
                        <label for="fileUpload" class="form-label">Choose Photo</label>
                        <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" 
                                      accept="image/*" onchange="previewImage(this)" />
                        <small class="text-muted">Supported formats: JPG, PNG, GIF (Max 2MB)</small>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnUploadPhoto" runat="server" Text="Upload Photo" CssClass="btn btn-primary"
                              OnClick="btnUploadPhoto_Click" />
                </div>
            </div>
        </div>
    </div>

    <script>
        function previewImage(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById('<%= imgPreview.ClientID %>').src = e.target.result;
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</asp:Content>
