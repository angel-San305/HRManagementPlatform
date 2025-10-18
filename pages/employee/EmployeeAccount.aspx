<%@ Page Title="Account Settings" Language="C#" MasterPageFile="~/EmployeeSite.Master" AutoEventWireup="true" CodeBehind="EmployeeAccount.aspx.cs" Inherits="HRManagement.Pages.Employee.EmployeeAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-lg-8 mx-auto">
            <!-- Account Information Card -->
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">
                        <i class="fas fa-user-cog me-2"></i>Account Settings
                    </h6>
                </div>
                <div class="card-body">
                    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="mb-4">
                        <h6 class="text-muted mb-3">Current Account Information</h6>
                        <div class="row">
                            <div class="col-md-6 mb-2">
                                <strong>Username:</strong>
                                <asp:Label ID="lblCurrentUsername" runat="server" CssClass="text-muted ms-2"></asp:Label>
                            </div>
                            <div class="col-md-6 mb-2">
                                <strong>Email:</strong>
                                <asp:Label ID="lblCurrentEmail" runat="server" CssClass="text-muted ms-2"></asp:Label>
                            </div>
                        </div>
                    </div>

                    <hr />

                    <!-- Change Username Section -->
                    <div class="mb-4">
                        <h6 class="mb-3">Change Username</h6>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtNewUsername" class="form-label">New Username *</label>
                                <asp:TextBox ID="txtNewUsername" runat="server" CssClass="form-control" 
                                           placeholder="Enter new username" />
                                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                                    ControlToValidate="txtNewUsername"
                                    ErrorMessage="Username is required"
                                    CssClass="text-danger"
                                    Display="Dynamic"
                                    ValidationGroup="UpdateAccount" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtConfirmUsername" class="form-label">Confirm Username *</label>
                                <asp:TextBox ID="txtConfirmUsername" runat="server" CssClass="form-control" 
                                           placeholder="Confirm new username" />
                                <asp:CompareValidator ID="cvUsername" runat="server"
                                    ControlToValidate="txtConfirmUsername"
                                    ControlToCompare="txtNewUsername"
                                    ErrorMessage="Usernames do not match"
                                    CssClass="text-danger"
                                    Display="Dynamic"
                                    ValidationGroup="UpdateAccount" />
                            </div>
                        </div>
                    </div>

                    <hr />

                    <!-- Change Password Section -->
                    <div class="mb-4">
                        <h6 class="mb-3">Change Password</h6>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <label for="txtCurrentPassword" class="form-label">Current Password *</label>
                                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" 
                                           TextMode="Password" placeholder="Enter current password" />
                                <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server" 
                                    ControlToValidate="txtCurrentPassword"
                                    ErrorMessage="Current password is required"
                                    CssClass="text-danger"
                                    Display="Dynamic"
                                    ValidationGroup="UpdateAccount" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtNewPassword" class="form-label">New Password *</label>
                                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" 
                                           TextMode="Password" placeholder="Enter new password" />
                                <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" 
                                    ControlToValidate="txtNewPassword"
                                    ErrorMessage="New password is required"
                                    CssClass="text-danger"
                                    Display="Dynamic"
                                    ValidationGroup="UpdateAccount" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtConfirmPassword" class="form-label">Confirm Password *</label>
                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" 
                                           TextMode="Password" placeholder="Confirm new password" />
                                <asp:CompareValidator ID="cvPassword" runat="server"
                                    ControlToValidate="txtConfirmPassword"
                                    ControlToCompare="txtNewPassword"
                                    ErrorMessage="Passwords do not match"
                                    CssClass="text-danger"
                                    Display="Dynamic"
                                    ValidationGroup="UpdateAccount" />
                            </div>
                        </div>
                    </div>

                    <div class="d-flex justify-content-between align-items-center">
                        <small class="text-muted">
                            <i class="fas fa-info-circle me-1"></i>
                            Leave password fields empty if you don't want to change it
                        </small>
                        <asp:Button ID="btnUpdateAccount" runat="server" Text="Update Account" 
                                  CssClass="btn btn-primary" 
                                  OnClick="btnUpdateAccount_Click"
                                  ValidationGroup="UpdateAccount" />
                    </div>
                </div>
            </div>

            <!-- Security Notice -->
            <div class="card shadow border-warning">
                <div class="card-body">
                    <h6 class="text-warning mb-2">
                        <i class="fas fa-exclamation-triangle me-2"></i>Security Notice
                    </h6>
                    <p class="text-muted mb-0 small">
                        Keep your account credentials secure. Never share your password with anyone. 
                        If you suspect unauthorized access, change your password immediately and contact HR.
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>