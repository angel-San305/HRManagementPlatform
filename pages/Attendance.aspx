<%@ Page Title="Attendance" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="HRManagement.Pages.Attendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page Header -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
        <h1 class="h3 mb-0 text-gray-800">Attendance Management</h1>
        <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addAttendanceModal">
                <i class="fas fa-plus me-2"></i>Mark Attendance
            </button>

    </div>

    <!-- Filter Panel -->
    <div class="card shadow mb-4">
        <div class="card-header py-3">
            <h6 class="m-0 font-weight-bold text-primary">Filter Attendance</h6>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-3 mb-3">
                    <label for="ddlFilterEmployee" class="form-label">Employee</label>
                    <asp:DropDownList ID="ddlFilterEmployee" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlFilterEmployee_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>
                <div class="col-md-3 mb-3">
                    <label for="txtFromDate" class="form-label">From Date</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"
    AutoPostBack="true" OnTextChanged="txtFromDate_TextChanged" />
                </div>
                <div class="col-md-3 mb-3">
                    <label for="txtToDate" class="form-label">To Date</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"
    AutoPostBack="true" OnTextChanged="txtToDate_TextChanged" />
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

    <!-- Attendance Summary -->
    <div class="row mb-4">
        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-success shadow h-100 py-2" style="border-left: 4px solid #28a745;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                                Present Today
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblPresentToday" runat="server" Text="0"></asp:Label>
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
            <div class="card border-left-warning shadow h-100 py-2" style="border-left: 4px solid #ffc107;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
                                Late Today
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblLateToday" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-clock fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6 mb-4">
            <div class="card border-left-danger shadow h-100 py-2" style="border-left: 4px solid #dc3545;">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-danger text-uppercase mb-1">
                                Absent Today
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblAbsentToday" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-times-circle fa-2x text-gray-300"></i>
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
                                Total Records
                            </div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">
                                <asp:Label ID="lblTotalRecords" runat="server" Text="0"></asp:Label>
                            </div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-list fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Attendance Records -->
    <div class="card shadow mb-4">
        <div class="card-header py-3 d-flex justify-content-between align-items-center">
            <h6 class="m-0 font-weight-bold text-primary">Attendance Records</h6>
            <div>
                <asp:Button ID="btnExportAttendance" runat="server" Text="Export CSV"
                        CssClass="btn btn-success btn-sm" 
                        OnClick="btnExportAttendance_Click" 
                        CausesValidation="false" UseSubmitBehavior="false" />

            </div>
        </div>
        <div class="card-body">
            <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </asp:Panel>

            <div class="table-responsive">
                <asp:GridView ID="gvAttendance" runat="server" CssClass="table table-striped table-hover" 
                            AutoGenerateColumns="false" GridLines="None" AllowPaging="true" PageSize="15"
                            OnPageIndexChanging="gvAttendance_PageIndexChanging" OnRowCommand="gvAttendance_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                        <asp:BoundField DataField="AttendanceDate" HeaderText="Date" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class="badge bg-<%# GetStatusColor(Eval("Status").ToString()) %>">
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Hours" HeaderText="Hours" DataFormatString="{0:F1}" />
                        <asp:BoundField DataField="Notes" HeaderText="Notes" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditAttendance" 
                                              CommandArgument='<%# Eval("AttendanceID") %>' 
                                              CssClass="btn btn-warning btn-sm me-1">
                                    <i class="fas fa-edit"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteAttendance" 
                                              CommandArgument='<%# Eval("AttendanceID") %>' 
                                              CssClass="btn btn-danger btn-sm"
                                              OnClientClick="return confirm('Are you sure you want to delete this attendance record?');">
                                    <i class="fas fa-trash"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="table-primary" />
                    <PagerStyle CssClass="pagination-ys" />
                    <EmptyDataTemplate>
                        <div class="text-center text-muted py-5">
                            <i class="fas fa-calendar-times fa-3x mb-3"></i>
                            <p class="mb-0">No attendance records found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Add/Edit Attendance Modal -->
    <div class="modal fade" id="addAttendanceModal" tabindex="-1" aria-labelledby="addAttendanceModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addAttendanceModalLabel">
                        <asp:Label ID="lblModalTitle" runat="server" Text="Mark Attendance"></asp:Label>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnAttendanceID" runat="server" />
                    <div class="mb-3">
                        <label for="ddlEmployee" class="form-label">Employee *</label>
                        <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select" required="true">
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label for="txtAttendanceDate" class="form-label">Date *</label>
                        <asp:TextBox ID="txtAttendanceDate" runat="server" CssClass="form-control" TextMode="Date" required="true" />
                    </div>
                    <div class="mb-3">
                        <label for="ddlStatus" class="form-label">Status *</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select" required="true">
                            <asp:ListItem Value="">Select Status</asp:ListItem>
                            <asp:ListItem Value="Present">Present</asp:ListItem>
                            <asp:ListItem Value="Late">Late</asp:ListItem>
                            <asp:ListItem Value="Absent">Absent</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label for="txtHours" class="form-label">Hours Worked</label>
                        <asp:TextBox ID="txtHours" runat="server" CssClass="form-control" TextMode="Number" 
                                   step="0.5" min="0" max="24" placeholder="8.0" />
                    </div>
                    <div class="mb-3">
                        <label for="txtNotes" class="form-label">Notes</label>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" 
                                   TextMode="MultiLine" Rows="3" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSaveAttendance" runat="server" Text="Save Attendance" CssClass="btn btn-primary"
                              OnClick="btnSaveAttendance_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Reset form to Add mode when Mark Attendance button is clicked
        document.addEventListener('DOMContentLoaded', function () {
            // Get the Mark Attendance button
            var addButton = document.querySelector('[data-bs-target="#addAttendanceModal"]');

            if (addButton) {
                addButton.addEventListener('click', function () {
                    // Reset form to Add mode
                    resetAttendanceForm();
                });
            }

            // Clear form when modal is hidden
            var modal = document.getElementById('addAttendanceModal');
            if (modal) {
                modal.addEventListener('hidden.bs.modal', function () {
                    resetAttendanceForm();
                });
            }
        });

        function resetAttendanceForm() {
            // Clear all form fields
            document.getElementById('<%= hdnAttendanceID.ClientID %>').value = '';
    document.getElementById('<%= ddlEmployee.ClientID %>').selectedIndex = 0;
    document.getElementById('<%= txtAttendanceDate.ClientID %>').value = '<%= DateTime.Today.ToString("yyyy-MM-dd") %>';
    document.getElementById('<%= ddlStatus.ClientID %>').selectedIndex = 0;
    document.getElementById('<%= txtHours.ClientID %>').value = '';
    document.getElementById('<%= txtNotes.ClientID %>').value = '';
    
    // Reset modal title and button text
    document.getElementById('<%= lblModalTitle.ClientID %>').innerHTML = 'Mark Attendance';
    document.getElementById('<%= btnSaveAttendance.ClientID %>').innerHTML = 'Save Attendance';

            console.log('Attendance form reset to Add mode');
        }
    </script>


</asp:Content>