<%@ Page Title="Departments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Departments.aspx.cs" Inherits="HRManagement.Pages.Departments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
        <h1 class="h3 mb-0 text-gray-800">Department Management</h1>
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addDepartmentModal">
            <i class="fas fa-plus me-2"></i>Add New Department
        </button>
    </div>

    <!-- Department List -->
    <div class="row">
        <div class="col-lg-8">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">All Departments</h6>
                </div>
                <div class="card-body">
                    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </asp:Panel>

                    <div class="table-responsive">
                        <asp:GridView ID="gvDepartments" runat="server" CssClass="table table-striped table-hover" 
                                    AutoGenerateColumns="false" GridLines="None" OnRowCommand="gvDepartments_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="Department Name">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDepartmentName" runat="server" 
                                                      CommandName="ViewEmployees" 
                                                      CommandArgument='<%# Eval("DepartmentID") %>'
                                                      CssClass="text-primary fw-bold">
                                            <%# Eval("DepartmentName") %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:BoundField DataField="EmployeeCount" HeaderText="Employees" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditDepartment" 
                                                      CommandArgument='<%# Eval("DepartmentID") %>' 
                                                      CssClass="btn btn-warning btn-sm me-1" 
                                                      ToolTip="Edit Department">
                                            <i class="fas fa-edit"></i> Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteDepartment" 
                                                      CommandArgument='<%# Eval("DepartmentID") %>' 
                                                      CssClass="btn btn-danger btn-sm"
                                                      ToolTip="Delete Department"
                                                      OnClientClick="return confirm('Are you sure you want to delete this department? This action cannot be undone.');">
                                            <i class="fas fa-trash"></i> Delete
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="table-primary" />
                            <EmptyDataTemplate>
                                <div class="text-center text-muted py-5">
                                    <i class="fas fa-building fa-3x mb-3"></i>
                                    <p class="mb-0">No departments found</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <!-- Department Employees Panel -->
        <div class="col-lg-4">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">
                        <asp:Label ID="lblDepartmentTitle" runat="server" Text="Department Employees"></asp:Label>
                    </h6>
                </div>
                <div class="card-body">
                    <asp:Panel ID="pnlNoSelection" runat="server" Visible="true" CssClass="text-center text-muted py-4">
                        <i class="fas fa-mouse-pointer fa-2x mb-3"></i>
                        <p>Click on a department name to view employees</p>
                    </asp:Panel>

                    <asp:Panel ID="pnlDepartmentEmployees" runat="server" Visible="false">
                        <div class="list-group list-group-flush">
                            <asp:Repeater ID="rptrDepartmentEmployees" runat="server" OnItemCommand="rptrDepartmentEmployees_ItemCommand">
                                <ItemTemplate>
                                    <div class="list-group-item list-group-item-action d-flex justify-content-between align-items-center">
                                        <div>
                                            <asp:LinkButton ID="lnkEmployeeName" runat="server" 
                                                          CommandName="ViewProfile" 
                                                          CommandArgument='<%# Eval("EmployeeID") %>'
                                                          CssClass="fw-bold text-decoration-none">
                                                <%# Eval("FullName") %>
                                            </asp:LinkButton>
                                            <br />
                                            <small class="text-muted"><%# Eval("RoleName") %></small>
                                        </div>
                                        <span class="badge bg-primary rounded-pill">
                                            <%# Eval("DateJoined", "{0:MMM yyyy}") %>
                                        </span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <!-- Add Department Modal -->
    <div class="modal fade" id="addDepartmentModal" tabindex="-1" aria-labelledby="addDepartmentModalLabel" aria-hidden="true" data-bs-backdrop="static">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addDepartmentModalLabel">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Add New Department"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnDepartmentID" runat="server" />
                    
                    <!-- Debug info (remove in production) -->
                    <div id="debugInfo" style="background: #f8f9fa; padding: 10px; margin-bottom: 10px; border-radius: 5px; font-size: 12px;">
                        Department ID: <span id="currentDeptId">None</span><br/>
                        Mode: <span id="currentMode">Add</span>
                    </div>
                    
                    <div class="mb-3">
                        <label for="txtDepartmentName" class="form-label">Department Name *</label>
                        <asp:TextBox ID="txtDepartmentName" runat="server" CssClass="form-control" required="true" />
                    </div>
                    <div class="mb-3">
                        <label for="txtDescription" class="form-label">Description</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                   TextMode="MultiLine" Rows="4" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSaveDepartment" runat="server" Text="Save Department" CssClass="btn btn-primary"
                              OnClick="btnSaveDepartment_Click" UseSubmitBehavior="true" />
                </div>
            </div>
        </div>
    </div>

    <script>
        // Update debug info when modal is shown (optional)
        document.getElementById('addDepartmentModal').addEventListener('shown.bs.modal', function () {
            var deptId = document.getElementById('<%= hdnDepartmentID.ClientID %>').value;
        var mode = deptId ? 'Edit' : 'Add';
        document.getElementById('currentDeptId').textContent = deptId || 'None';
        document.getElementById('currentMode').textContent = mode;
        console.log('Modal shown - Department ID:', deptId, 'Mode:', mode);
    });

        // Clear form when modal is hidden
        document.getElementById('addDepartmentModal').addEventListener('hidden.bs.modal', function () {
            clearDepartmentForm();
        });

        function clearDepartmentForm() {
            document.getElementById('<%= txtDepartmentName.ClientID %>').value = '';
        document.getElementById('<%= txtDescription.ClientID %>').value = '';
        document.getElementById('<%= hdnDepartmentID.ClientID %>').value = '';
        document.getElementById('<%= lblModalTitle.ClientID %>').innerHTML = 'Add New Department';
        document.getElementById('<%= btnSaveDepartment.ClientID %>').innerHTML = 'Save Department';

            // Update debug info
            document.getElementById('currentDeptId').textContent = 'None';
            document.getElementById('currentMode').textContent = 'Add';
        }

        // Optional: Show modal from C# EditDepartment() using Bootstrap 5 API
        function showDepartmentModal() {
            var modalEl = document.getElementById('addDepartmentModal');
            var modal = new bootstrap.Modal(modalEl);
            modal.show();
        }
    </script>

</asp:Content>