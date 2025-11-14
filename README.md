# HR Management Platform
A web-based HR management system that automates employee profiles, department and role management, attendance tracking, payroll processing, and dashboard reporting. Administrators can efficiently manage employee data while employees can access their personal dashboard, including the ability to update their username and password.

Features
## Features
- Role-based access (Admin vs Employee)
- Employee management (Add, Edit, Delete, View)
- Department management (Add, Edit, Delete, View)
- Role management (Add, Edit, Delete, View)
- Attendance tracking (Check-in, Check-out, Logs)
- Payroll management (Salary, Deductions, Reports)
- Dashboard for Admin and Employees
- Employees can change their username and password
- Search, filter, and reporting functionalities
- Responsive UI


## Technologies Used
- ASP.NET Web Forms / C#
- SQL Server / T-SQL
- HTML, CSS, JavaScript
- Master Pages (`Site.Master`, `EmployeeSite.Master`) for consistent layout
- Web.config for configuration



## Getting Started

### Prerequisites
- Visual Studio (compatible version)
- .NET Framework (version used)
- SQL Server (local or remote)
- Clone this repository to your machine

### Installation Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/angel-San305/HRManagementPlatform.git
2. Open the solution in Visual Studio.
3. Update the database connection string in Web.config to point to your SQL Server.
4. Run the SQL script HRManagementSystem.sql (in the root) to create tables and initial data.
5. Build and run the project (F5) to launch it.
6. Log in as Administrator (default credentials if provided) and start using.


Usage
– Administrator: Manage employees, departments, roles; track attendance; run payroll; view reports.
– Employee: View personal dashboard, attendance status, payroll history, and update username/password.
– Navigate using the menu: Employees, Departments, Roles, Attendance, Payroll, Reports.
– Use search, filters, and reporting tools for easier data management.


Contributing

1. Fork the repository
2. Create a branch: git checkout -b feature/YourFeature
3. Make your changes
4. Commit: git commit -m "Add feature"
5. Push: git push origin feature/YourFeature
6. Open a Pull Request for review










