<%@ Page Title="Employees" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="HRManagement.Pages.Employees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
        <h1 class="h3 mb-0 text-gray-800">Employee Management</h1>
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addEmployeeModal">
            <i class="fas fa-user-plus me-2"></i>Add New Employee
        </button>
    </div>

    <!-- Search -->
<div class="card shadow mb-4">
    <div class="card-header py-3">
        <h6 class="m-0 font-weight-bold text-primary">Search Employees</h6>
    </div>
    <div class="card-body">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label for="txtSearch" class="form-label">Search Employee</label>
                <asp:TextBox ID="txtSearch" runat="server" 
                                AutoPostBack="true" 
                                OnTextChanged="txtSearch_TextChanged" 
                                CssClass="form-control" 
                             placeholder="Search by name, email, department, role, date, etc." />
            </div>
            <div class="col-md-6 mb-3 d-flex align-items-end">
                <div>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary"
                                OnClick="btnSearch_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary ms-1"
                                OnClick="btnClear_Click" />
                </div>
            </div>
        </div>
    </div>
</div>


    <!-- Employee List -->
    <div class="card shadow mb-4">
        <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <h6 class="m-0 font-weight-bold text-primary">All Employees</h6>
            <div>
                <asp:Button ID="btnExportCSV" runat="server"
                        Text="Export CSV"
                        CssClass="btn btn-primary"
                        OnClick="btnExportCSV_Click"
                        CausesValidation="false"
                        formnovalidate="formnovalidate" />

            </div>
        </div>
        <div class="card-body">
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </asp:Panel>

            <div class="table-responsive">
                <asp:GridView ID="gvEmployees" runat="server" CssClass="table table-striped table-hover" 
                            AutoGenerateColumns="false" GridLines="None" AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvEmployees_PageIndexChanging" OnRowCommand="gvEmployees_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="PhoneNumber" HeaderText="Phone" />
                        <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                        <asp:BoundField DataField="RoleName" HeaderText="Role" />
                        <asp:BoundField DataField="Salary" HeaderText="Salary" DataFormatString="₱{0:N2}" />
                        <asp:BoundField DataField="DateJoined" HeaderText="Join Date" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:TemplateField HeaderText="Actions">
    <ItemTemplate>
        <asp:LinkButton ID="btnView" runat="server" CommandName="ViewProfile" 
            CommandArgument='<%# Eval("EmployeeID") %>' 
            CssClass="btn btn-info btn-sm me-1">
            <i class="fas fa-eye"></i>
        </asp:LinkButton>

        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteEmployee" 
            CommandArgument='<%# Eval("EmployeeID") %>' 
            CssClass="btn btn-danger btn-sm"
            OnClientClick="return confirm('Are you sure you want to delete this employee?');">
            <i class="fas fa-trash"></i>
        </asp:LinkButton>
    </ItemTemplate>
</asp:TemplateField>

                    </Columns>
                    <HeaderStyle CssClass="table-primary" />
                    <PagerStyle CssClass="pagination-ys" />
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-5">
                            <i class="fas fa-users fa-3x mb-3"></i>
                            <p class="mb-0">No employees found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add Employee Modal -->
    <div class="modal fade" id="addEmployeeModal" tabindex="-1" aria-labelledby="addEmployeeModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addEmployeeModalLabel">Add New Employee</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label for="txtFirstName" class="form-label">First Name *</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtLastName" class="form-label">Last Name *</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtEmail" class="form-label">Email *</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtPhone" class="form-label">Phone Number</label>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="ddlDepartment" class="form-label">Department *</label>
                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select" required="true">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="ddlRole" class="form-label">Role *</label>
                            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select" required="true">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtSalary" class="form-label">Salary</label>
                            <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtDateJoined" class="form-label">Date Joined *</label>
                            <asp:TextBox ID="txtDateJoined" runat="server" CssClass="form-control" TextMode="Date" required="true" />
                        </div>
                        <div class="col-12 mb-3">
                            <label for="txtAddress" class="form-label">Address</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSaveEmployee" runat="server" Text="Save Employee" CssClass="btn btn-primary"
                              OnClick="btnSaveEmployee_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>


