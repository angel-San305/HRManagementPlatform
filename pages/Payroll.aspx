<%@ Page Title="Payroll" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payroll.aspx.cs" Inherits="HRManagement.Pages.Payroll" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
        <h1 class="h3 mb-0 text-gray-800">Payroll Management</h1>
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addPayrollModal">
            <i class="fas fa-plus me-2"></i>Process Payroll
        </button>
    </div>

    <!-- Filter Panel -->
    <div class="card shadow mb-4">
        <div class="card-header py-3">
            <h6 class="m-0 font-weight-bold text-primary">Filter Payroll Records</h6>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-3 mb-3">
                    <label for="ddlFilterEmployee" class="form-label">Employee</label>
                    <asp:DropDownList ID="ddlFilterEmployee" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="FilterPayroll">
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 mb-3">
                    <label for="ddlFilterMonth" class="form-label">Month</label>
                    <asp:DropDownList ID="ddlFilterMonth" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="FilterPayroll">
                        <asp:ListItem Value="">All Months</asp:ListItem>
                        <asp:ListItem Value="1">January</asp:ListItem>
                        <asp:ListItem Value="2">February</asp:ListItem>
                        <asp:ListItem Value="3">March</asp:ListItem>
                        <asp:ListItem Value="4">April</asp:ListItem>
                        <asp:ListItem Value="5">May</asp:ListItem>
                        <asp:ListItem Value="6">June</asp:ListItem>
                        <asp:ListItem Value="7">July</asp:ListItem>
                        <asp:ListItem Value="8">August</asp:ListItem>
                        <asp:ListItem Value="9">September</asp:ListItem>
                        <asp:ListItem Value="10">October</asp:ListItem>
                        <asp:ListItem Value="11">November</asp:ListItem>
                        <asp:ListItem Value="12">December</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 mb-3">
                    <label for="ddlFilterYear" class="form-label">Year</label>
                    <asp:DropDownList ID="ddlFilterYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="FilterPayroll">
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 mb-3">
                    <label class="form-label">&nbsp;</label>
                    <div>
                        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary"
                                  OnClick="btnFilter_Click" />
                        <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn btn-outline-secondary ms-1"
                                  OnClick="btnClearFilter_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Payroll Summary -->
    <div class="row mb-4">
        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-primary shadow h-100 py-2" style="border-left: 4px solid #667eea;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                                Total Payroll (This Month)
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblTotalPayroll" runat="server" Text="₱0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-peso-sign fa-2x text-gray-300"></i>
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
                                Processed Records
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblProcessedRecords" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-check-circle fa-2x text-gray-300"></i>
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
                                Average Salary
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblAverageSalary" runat="server" Text="$0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-chart-line fa-2x text-gray-300"></i>
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
                                Employees Paid
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblEmployeesPaid" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-users fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    
<!-- Quick Actions -->
<div class="row mb-4">
    <div class="col-12">
        <div class="card shadow">
            <div class="card-header py-3">
                <h6 class="m-0 font-weight-bold text-primary">Quick Actions</h6>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <div class="d-grid">
                            <input type="button" value="Process Current Month Payroll" 
                                   class="btn btn-success btn-lg" 
                                   onclick="__doPostBack('btnProcessCurrentMonth', '')" />
                        </div>
                    </div>
                    <div class="col-md-6 mb-3">
                        <div class="d-grid">
                            <input type="button" value="Export Payroll Report" 
                                   class="btn btn-warning btn-lg" 
                                   onclick="__doPostBack('btnExportPayroll', '')" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

    <!-- Payroll Records -->
    <div class="card shadow mb-4">
        <div class="card-header py-3">
            <h6 class="m-0 font-weight-bold text-primary">Payroll Records</h6>
        </div>
        <div class="card-body">
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </asp:Panel>

            <div class="table-responsive">
                <asp:GridView ID="gvPayroll" runat="server" CssClass="table table-striped table-hover" 
                            AutoGenerateColumns="false" GridLines="None" AllowPaging="true" PageSize="15"
                            OnPageIndexChanging="gvPayroll_PageIndexChanging" OnRowCommand="gvPayroll_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                        <asp:TemplateField HeaderText="Period">
                            <ItemTemplate>
                                <%# Eval("Month") %>/<%# Eval("Year") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BasicSalary" HeaderText="Basic Salary" DataFormatString="₱{0:N2}" />
                        <asp:BoundField DataField="NetSalary" HeaderText="Net Salary" DataFormatString="₱{0:N2}" />
                        <asp:BoundField DataField="DaysWorked" HeaderText="Days Worked" />
                        <asp:BoundField DataField="ProcessedDate" HeaderText="Processed Date" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
         
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditPayroll" 
                                              CommandArgument='<%# Eval("PayrollID") %>' 
                                              CssClass="btn btn-warning btn-sm me-1">
                                    <i class="fas fa-edit"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeletePayroll" 
                                              CommandArgument='<%# Eval("PayrollID") %>' 
                                              CssClass="btn btn-danger btn-sm"
                                              OnClientClick="return confirm('Are you sure you want to delete this payroll record?');">
                                    <i class="fas fa-trash"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="table-primary" />
                    <PagerStyle CssClass="pagination-ys" />
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-5">
                            <i class="fas fa-money-bill fa-3x mb-3"></i>
                            <p class="mb-0">No payroll records found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add/Edit Payroll Modal -->
    <div class="modal fade" id="addPayrollModal" tabindex="-1" aria-labelledby="addPayrollModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addPayrollModalLabel">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Process Payroll"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnPayrollID" runat="server" />
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label for="ddlEmployee" class="form-label">Employee *</label>
                            <asp:DropDownList 
                                    ID="ddlEmployee" 
                                    runat="server" 
                                    CssClass="form-control" 
                                    onchange="onEmployeeChange()">
                                </asp:DropDownList>


                        </div>
                        <div class="col-md-3 mb-3">
                            <label for="ddlMonth" class="form-label">Month *</label>
                            <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-select" required="true">
                                <asp:ListItem Value="">Select Month</asp:ListItem>
                                <asp:ListItem Value="1">January</asp:ListItem>
                                <asp:ListItem Value="2">February</asp:ListItem>
                                <asp:ListItem Value="3">March</asp:ListItem>
                                <asp:ListItem Value="4">April</asp:ListItem>
                                <asp:ListItem Value="5">May</asp:ListItem>
                                <asp:ListItem Value="6">June</asp:ListItem>
                                <asp:ListItem Value="7">July</asp:ListItem>
                                <asp:ListItem Value="8">August</asp:ListItem>
                                <asp:ListItem Value="9">September</asp:ListItem>
                                <asp:ListItem Value="10">October</asp:ListItem>
                                <asp:ListItem Value="11">November</asp:ListItem>
                                <asp:ListItem Value="12">December</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3 mb-3">
                            <label for="ddlYear" class="form-label">Year *</label>
                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select" required="true">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtBasicSalary" class="form-label">Basic Salary *</label>
                            <asp:TextBox ID="txtBasicSalary" runat="server" CssClass="form-control" 
                                       TextMode="Number" step="0.01" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtDaysWorked" class="form-label">Days Worked *</label>
                            <asp:TextBox ID="txtDaysWorked" runat="server" CssClass="form-control" 
                                       TextMode="Number" min="0" max="31" required="true" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtNetSalary" class="form-label">Net Salary</label>
                            <asp:TextBox ID="txtNetSalary" runat="server" CssClass="form-control" 
                                       TextMode="Number" step="0.01" />
                            <small class="text-muted">Leave blank to auto-calculate based on days worked</small>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Auto Calculate</label>
                            <div>
                                <asp:Button ID="btnCalculate" runat="server" Text="Calculate Net Salary" 
                                          CssClass="btn btn-outline-primary" OnClick="btnCalculate_Click" 
                                          CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSavePayroll" runat="server" Text="Save Payroll" CssClass="btn btn-primary"
                              OnClick="btnSavePayroll_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // FIXED: Reset form to Add mode when Add Payroll button is clicked
        document.addEventListener('DOMContentLoaded', function () {
            // Get the Add Payroll button
            var addButton = document.querySelector('[data-bs-target="#addPayrollModal"]');

            if (addButton) {
                addButton.addEventListener('click', function () {
                    // Reset form to Add mode
                    resetPayrollForm();
                });
            }

            // Clear form when modal is hidden
            var modal = document.getElementById('addPayrollModal');
            if (modal) {
                modal.addEventListener('hidden.bs.modal', function () {
                    resetPayrollForm();
                });
            }
        });

        function resetPayrollForm() {
            // Clear all form fields
            document.getElementById('<%= hdnPayrollID.ClientID %>').value = '';
            document.getElementById('<%= ddlEmployee.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= ddlMonth.ClientID %>').value = '<%= DateTime.Now.Month %>';
            document.getElementById('<%= ddlYear.ClientID %>').value = '<%= DateTime.Now.Year %>';
            document.getElementById('<%= txtBasicSalary.ClientID %>').value = '';
            document.getElementById('<%= txtNetSalary.ClientID %>').value = '';
            document.getElementById('<%= txtDaysWorked.ClientID %>').value = '';

            // Reset modal title and button text
            document.getElementById('<%= lblModalTitle.ClientID %>').innerHTML = 'Process Payroll';
            document.getElementById('<%= btnSavePayroll.ClientID %>').innerHTML = 'Save Payroll';

            console.log('Payroll form reset to Add mode');
        }

        function onEmployeeChange() {
            var ddl = document.getElementById('<%= ddlEmployee.ClientID %>');
            var employeeId = ddl.value;
            var month = document.getElementById('<%= ddlMonth.ClientID %>').value;
            var year = document.getElementById('<%= ddlYear.ClientID %>').value;

    if (!employeeId || employeeId === "0") return; // prevent crash

    $.ajax({
        type: "POST",
        url: "Payroll.aspx/GetEmployeeDetails",
        data: JSON.stringify({ 
            employeeId: parseInt(employeeId), 
            month: parseInt(month), 
            year: parseInt(year) 
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var data = response.d;
            $('#<%= txtBasicSalary.ClientID %>').val(data.Salary.toFixed(2));
            $('#<%= txtDaysWorked.ClientID %>').val(data.DaysWorked);
            $('#<%= txtNetSalary.ClientID %>').val(data.NetSalary.toFixed(2));
        },
        error: function (xhr, status, error) {
            alert("Error: " + xhr.responseText);
        }
    });
        }



        // Add this for debugging
        document.addEventListener('DOMContentLoaded', function () {
            console.log('Page loaded, checking buttons...');

            // Find buttons by their generated IDs
            var processBtn = document.querySelector('[id*="btnProcessCurrentMonth"]');
            var exportBtn = document.querySelector('[id*="btnExportPayroll"]');

            console.log('Process button found:', processBtn);
            console.log('Export button found:', exportBtn);

            if (processBtn) {
                processBtn.addEventListener('click', function (e) {
                    console.log('Process Current Month button clicked!');
                    console.log('Event:', e);
                });
            }

            if (exportBtn) {
                exportBtn.addEventListener('click', function (e) {
                    console.log('Export Payroll button clicked!');
                    console.log('Event:', e);
                });
            }
        });


    </script>

</asp:Content>

