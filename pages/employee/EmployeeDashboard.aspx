<%@ Page Title="My Dashboard" Language="C#" MasterPageFile="~/EmployeeSite.Master" AutoEventWireup="true" CodeBehind="EmployeeDashboard.aspx.cs" Inherits="HRManagement.Pages.Employee.EmployeeDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-4">Welcome, <asp:Label ID="lblEmployeeName" runat="server"></asp:Label>!</h2>

    <!-- Quick Stats -->
    <div class="row mb-4">
        <div class="col-md-3">
            <div class="card">
                <div class="card-body text-center">
                    <h6 class="text-muted">This Month Attendance</h6>
                    <h3><asp:Label ID="lblAttendanceDays" runat="server" Text="0"></asp:Label></h3>
                    <small>Days Present</small>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card">
                <div class="card-body text-center">
                    <h6 class="text-muted">Last Payroll</h6>
                    <h3><asp:Label ID="lblLastPayroll" runat="server" Text="₱0"></asp:Label></h3>
                    <small>Net Salary</small>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card">
                <div class="card-body text-center">
                    <h6 class="text-muted">Department</h6>
                    <h3><asp:Label ID="lblDepartment" runat="server" Text="-"></asp:Label></h3>
                </div>
            </div>
        </div>
        <div class="col-md-3">
            <div class="card">
                <div class="card-body text-center">
                    <h6 class="text-muted">Years of Service</h6>
                    <h3><asp:Label ID="lblYearsOfService" runat="server" Text="0"></asp:Label></h3>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Activity -->
    <div class="row">
        <div class="col-md-6">
            <div class="card">
                <div class="card-header">
                    <h6>Recent Attendance</h6>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvRecentAttendance" runat="server" CssClass="table table-sm" 
                                AutoGenerateColumns="false" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="AttendanceDate" HeaderText="Date" DataFormatString="{0:MMM dd, yyyy}" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:BoundField DataField="Hours" HeaderText="Hours" DataFormatString="{0:F1}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
        <div class="col-md-6">
            <div class="card">
                <div class="card-header">
                    <h6>Recent Payroll</h6>
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvRecentPayroll" runat="server" CssClass="table table-sm" 
                                AutoGenerateColumns="false" GridLines="None">
                        <Columns>
                            <asp:TemplateField HeaderText="Period">
                                <ItemTemplate>
                                    <%# Eval("Month") %>/<%# Eval("Year") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="NetSalary" HeaderText="Net Salary" DataFormatString="₱{0:N2}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>