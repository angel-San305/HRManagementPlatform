DROP TABLE Users;
DROP TABLE Departments;
DROP TABLE Employees;
DROP TABLE Performance;
DROP TABLE Training;
DROP TABLE Attendance
DROP TABLE Payroll

-- HR Employee Management Platform - Simplified Database Schema
-- Execute this script in SQL Server Management Studio

-- Create Database
CREATE DATABASE HRManagementDB;
GO

USE HRManagementDB;
GO

-- Create Tables with simplified structure

-- 1. Users Table (for login)
CREATE TABLE Users (
    UserID int IDENTITY(1,1) PRIMARY KEY,
    Username nvarchar(50) UNIQUE NOT NULL,
    Email nvarchar(100) UNIQUE NOT NULL,
    Password nvarchar(255) NOT NULL,
    Role nvarchar(20) CHECK (Role IN ('Admin', 'HR')) NOT NULL,
    IsActive bit DEFAULT 1,
    CreatedDate datetime DEFAULT GETDATE()
);

SELECT * FROM Users
DELETE FROM Users
WHERE UserID = 4;




-- 2. Departments Table
CREATE TABLE Departments (
    DepartmentID int IDENTITY(1,1) PRIMARY KEY,
    DepartmentName nvarchar(100) NOT NULL,
    Description nvarchar(500),
    IsActive bit DEFAULT 1
);

-- 3. Roles Table
CREATE TABLE Roles (
    RoleID int IDENTITY(1,1) PRIMARY KEY,
    RoleName nvarchar(100) NOT NULL,
    Description nvarchar(500),
    IsActive bit DEFAULT 1
);

-- 4. Employees Table
CREATE TABLE Employees (
    EmployeeID int IDENTITY(1,1) PRIMARY KEY,
    FirstName nvarchar(50) NOT NULL,
    LastName nvarchar(50) NOT NULL,
    Email nvarchar(100) UNIQUE NOT NULL,
    PhoneNumber nvarchar(20),
    DepartmentID int FOREIGN KEY REFERENCES Departments(DepartmentID),
    RoleID int FOREIGN KEY REFERENCES Roles(RoleID),
    Salary decimal(10,2),
    DateJoined date NOT NULL,
    IsActive bit DEFAULT 1,
    Address nvarchar(255)
);

ALTER TABLE Employees ADD Photo nvarchar(255) NULL;
ALTER TABLE Employees 
ALTER COLUMN Salary DECIMAL(18,2);

SELECT * FROM Employees
DELETE FROM Employees
WHERE EmployeeID in (11, 12, 13,14,15,16,17,18,19,20,21);


-- 5. Attendance Table (simplified)
CREATE TABLE Attendance (
    AttendanceID int IDENTITY(1,1) PRIMARY KEY,
    EmployeeID int FOREIGN KEY REFERENCES Employees(EmployeeID),
    AttendanceDate date NOT NULL,
    Status nvarchar(20) CHECK (Status IN ('Present', 'Absent', 'Late')) DEFAULT 'Present',
    Hours decimal(4,2) DEFAULT 8,
    Notes nvarchar(255)
);

-- 6. Payroll Table (simplified)
CREATE TABLE Payroll (
    PayrollID int IDENTITY(1,1) PRIMARY KEY,
    EmployeeID int FOREIGN KEY REFERENCES Employees(EmployeeID),
    Month int NOT NULL,
    Year int NOT NULL,
    BasicSalary decimal(10,2),
    NetSalary decimal(10,2),
    DaysWorked int,
    ProcessedDate datetime DEFAULT GETDATE()
);

SELECT * FROM Payroll;
DELETE FROM Payroll
WHERE Daysworked = 8;

SELECT COUNT(*) FROM Payroll WHERE EmployeeID = @EmployeeID AND Month = @Month AND Year = @Year



-- Insert Sample Data

-- Users
INSERT INTO Users (Username, Email, Password, Role) VALUES
('admin', 'admin@company.com', 'admin123', 'Admin'),
('hr1', 'hr1@company.com', 'hr123', 'HR');

-- Departments
INSERT INTO Departments (DepartmentName, Description) VALUES
('IT', 'Information Technology Department'),
('HR', 'Human Resources Department'),
('Finance', 'Finance Department'),
('Marketing', 'Marketing Department');

-- Roles
INSERT INTO Roles (RoleName, Description) VALUES
('Manager', 'Department Manager'),
('Developer', 'Software Developer'),
('Analyst', 'Business Analyst'),
('Coordinator', 'Department Coordinator');



-- Sample Attendance (last 10 days)
DECLARE @Date date = DATEADD(day, -10, GETDATE());
WHILE @Date <= GETDATE()
BEGIN
    IF DATEPART(WEEKDAY, @Date) NOT IN (1, 7) -- Skip weekends
    BEGIN
        INSERT INTO Attendance (EmployeeID, AttendanceDate, Status, Hours)
        SELECT EmployeeID, @Date, 'Present', 8 FROM Employees WHERE IsActive = 1;
    END
    SET @Date = DATEADD(day, 1, @Date);
END

SELECT p.PayrollID, p.EmployeeID, e.FirstName, e.LastName, p.Month, p.Year, p.NetSalary, p.ProcessedDate
FROM Payroll p
INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
ORDER BY p.ProcessedDate DESC;

PRINT 'Database created successfully!';
PRINT 'Login: admin/admin123 or hr1/hr123';

-- Step 1: Add a link between Users and Employees tables
ALTER TABLE Employees
ADD UserID INT NULL;

ALTER TABLE Employees
ADD CONSTRAINT FK_Employees_Users 
FOREIGN KEY (UserID) REFERENCES Users(UserID);

-- Step 2: Create an index for better performance
CREATE INDEX IX_Employees_UserID ON Employees(UserID);

-- Step 3: Ensure your Users table has the Role column (if not already)
-- ALTER TABLE Users ADD Role NVARCHAR(50) DEFAULT 'Employee';

ALTER TABLE Users
DROP CONSTRAINT CK__Users__Role__04E4BC85;

ALTER TABLE Users
ADD CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'HR', 'Employee'));


SELECT * FROM Users;