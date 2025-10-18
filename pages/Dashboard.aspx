<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="HRManagement.Pages.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Dashboard Statistics Cards -->
    <div class="row mb-4">
        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-primary shadow h-100 py-2" style="border-left: 4px solid #667eea;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                                Total Employees
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblTotalEmployees" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-users fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-success shadow h-100 py-2" style="border-left: 4px solid #28a745;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                                Departments
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblTotalDepartments" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-building fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-info shadow h-100 py-2" style="border-left: 4px solid #17a2b8;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-info text-uppercase mb-1">
                                Today's Attendance
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblTodayAttendance" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-calendar-check fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-warning shadow h-100 py-2" style="border-left: 4px solid #ffc107;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
                                Active Roles
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblTotalRoles" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-user-tag fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Activity and Quick Actions -->
    <div class="row">
        <!-- Recent Employees -->
        <div class="col-lg-6 mb-4">
            <div class="card shadow">
                <div class="card-header py-3 d-flex flex-row align-items-center justify-content-between">
                    <h6 class="m-0 font-weight-bold text-primary">Recent Employees</h6>
                    <a href="Employees.aspx" class="btn btn-primary btn-sm">View All</a>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvRecentEmployees" runat="server" CssClass="table table-striped" 
                                AutoGenerateColumns="false" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="FullName" HeaderText="Name" />
                            <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                            <asp:BoundField DataField="DateJoined" HeaderText="Join Date" DataFormatString="{0:MMM dd, yyyy}" />
                        </Columns>
                        <HeaderStyle CssClass="table-primary" />
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-3">
                                <i class="fas fa-users fa-3x mb-3"></i>
                                <p>No employees found</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- Quick Actions -->
        <div class="col-lg-6 mb-4">
            <div class="card shadow">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">Quick Actions</h6>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <a href="Employees.aspx" class="btn btn-outline-primary btn-block p-3">
                                <i class="fas fa-user-plus fa-2x mb-2"></i>
                                <br />Add Employee
                            </a>
                        </div>
                        <div class="col-md-6 mb-3">
                            <a href="Departments.aspx" class="btn btn-outline-success btn-block p-3">
                                <i class="fas fa-building fa-2x mb-2"></i>
                                <br />Manage Departments
                            </a>
                        </div>
                        <div class="col-md-6 mb-3">
                            <a href="Attendance.aspx" class="btn btn-outline-info btn-block p-3">
                                <i class="fas fa-calendar-check fa-2x mb-2"></i>
                                <br />Track Attendance
                            </a>
                        </div>
                        <div class="col-md-6 mb-3">
                            <a href="Payroll.aspx" class="btn btn-outline-warning btn-block p-3">
                                <i class="fas fa-money-bill fa-2x mb-2"></i>
                                <br />Process Payroll
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Welcome Message -->
    <div class="row">
        <div class="col-12">
            <div class="alert alert-info alert-dismissible fade show" role="alert">
                <i class="fas fa-info-circle me-2"></i>
                <strong>Welcome to HR Management System!</strong> 
                You are logged in as <strong><asp:Label ID="lblWelcomeUser" runat="server"></asp:Label></strong>
                with <strong><asp:Label ID="lblUserRole" runat="server"></asp:Label></strong> privileges.
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        </div>
    </div>
</asp:Content>