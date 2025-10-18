<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/EmployeeSite.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="HRManagement.Pages.Employee.MyProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-lg-4">
            <div class="card shadow mb-4">
                <div class="card-body text-center">
                    <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle border mb-3" 
                             Width="120" Height="120" ImageUrl="~/Images/default-avatar.png" />
                    <h4 class="mb-1"><asp:Label ID="lblEmployeeName" runat="server"></asp:Label></h4>
                    <p class="text-muted mb-2"><asp:Label ID="lblEmployeeRole" runat="server"></asp:Label></p>
                    <p class="text-muted mb-3"><asp:Label ID="lblEmployeeDepartment" runat="server"></asp:Label></p>
                    <div class="row text-center">
                        <div class="col-6">
                            <h6 class="text-primary">Salary</h6>
                            <p class="mb-0"><asp:Label ID="lblSalary" runat="server"></asp:Label></p>
                        </div>
                        <div class="col-6">
                            <h6 class="text-primary">Years</h6>
                            <p class="mb-0"><asp:Label ID="lblYearsOfService" runat="server"></asp:Label></p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-8">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">Basic Information</h6>
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
</asp:Content>