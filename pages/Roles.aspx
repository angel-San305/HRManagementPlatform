<%@ Page Title="Roles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="HRManagement.Pages.Roles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
        <h1 class="h3 mb-0 text-gray-800">Role Management</h1>
        <button type="button" class="btn btn-primary" 
        data-bs-toggle="modal" 
        data-bs-target="#addRoleModal"
        onclick="resetRoleForm();">
    <i class="fas fa-plus me-2"></i>Add New Role
</button>
    </div>

    <!-- Role List -->
    <div class="card shadow mb-4">
        <div class="card-header py-3">
            <h6 class="m-0 font-weight-bold text-primary">All Roles</h6>
        </div>
        <div class="card-body">
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </asp:Panel>

            <div class="table-responsive">
                <asp:GridView ID="gvRoles" runat="server" CssClass="table table-striped table-hover" 
                            AutoGenerateColumns="false" GridLines="None" OnRowCommand="gvRoles_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="RoleName" HeaderText="Role Name" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="EmployeeCount" HeaderText="Employees" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditRole" 
                                              CommandArgument='<%# Eval("RoleID") %>' 
                                              CssClass="btn btn-warning btn-sm me-1">
                                    <i class="fas fa-edit"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteRole" 
                                              CommandArgument='<%# Eval("RoleID") %>' 
                                              CssClass="btn btn-danger btn-sm"
                                              OnClientClick="return confirm('Are you sure you want to delete this role?');">
                                    <i class="fas fa-trash"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="table-primary" />
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-5">
                            <i class="fas fa-user-tag fa-3x mb-3"></i>
                            <p class="mb-0">No roles found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add Role Modal -->
    <div class="modal fade" id="addRoleModal" tabindex="-1" aria-labelledby="addRoleModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addRoleModalLabel">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Role"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnRoleID" runat="server" />
                    <div class="mb-3">
                        <label for="txtRoleName" class="form-label">Role Name *</label>
                        <asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" required="true" />
                    </div>
                    <div class="mb-3">
                        <label for="txtDescription" class="form-label">Description</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                   TextMode="MultiLine" Rows="4" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSaveRole" runat="server" Text="Save Role" CssClass="btn btn-primary"
                              OnClick="btnSaveRole_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
    function resetRoleForm() {
        // Clear hidden field and textboxes
        document.getElementById('<%= hdnRoleID.ClientID %>').value = "";
        document.getElementById('<%= txtRoleName.ClientID %>').value = "";
        document.getElementById('<%= txtDescription.ClientID %>').value = "";
        document.getElementById('<%= lblModalTitle.ClientID %>').innerText = "Add New Role";
        document.getElementById('<%= btnSaveRole.ClientID %>').value = "Save Role";
    }
    </script>

</asp:Content>