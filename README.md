# HR Management Platform
A web-based HR management system that automates employee profiles, department and role management, attendance tracking, payroll processing, and dashboard reporting. Administrators can efficiently manage employee data while employees can access their personal dashboard, including the ability to update their username and password.


## Features
- Role-based access (Admin vs Employee)
- Employee management (Add, Edit, Delete, View)
- Department management (Add, Edit, Delete, View)
- Role management (Add, Edit, Delete, View)
- Attendance tracking:
  - Admin: mark employees as Present, Absent, or Late
  - Employee: mark themselves as Present, Absent, or Late
- Payroll management:
  - Automatically calculates net salary based on attendance (Present, Absent, Late) each month
  - Includes salary, deductions, and payroll summaries on the dashboard
- Dashboard for Admin and Employees to view attendance and payroll summaries
- Employees can change their username and password
- Search and filter functionalities
- Responsive UI

## Usage

- **Administrator**: Manage employees, departments, roles; mark attendance; view payroll and attendance summaries; run payroll.  
- **Employee**: View personal dashboard, attendance status, payroll history, update username/password, and mark their own attendance (Present, Absent, or Late).  
- Navigate using the menu: Employees, Departments, Roles, Attendance, Payroll.  
- Use search and filter tools for easier data management.
  
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



Contributing

1. Fork the repository
2. Create a branch: git checkout -b feature/YourFeature
3. Make your changes
4. Commit: git commit -m "Add feature"
5. Push: git push origin feature/YourFeature
6. Open a Pull Request for review



## License

This project is provided **as-is** for learning or internal use.  
You may modify or use it for personal projects but do not redistribute without permission. 






