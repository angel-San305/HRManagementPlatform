<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HRManagement.Pages.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HR Management - Login</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet" />
    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 20px;
            box-sizing: border-box;
        }
        .login-container {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
            backdrop-filter: blur(15px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            display: flex;
            overflow: hidden;
            max-width: 900px;
            width: 90%;
        }
        .login-left {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 3rem;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
            flex: 1;
            min-height: 500px;
        }
        .login-right {
            padding: 3rem;
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: center;
        }
        .login-header {
            /* Remove since we're restructuring */
        }
        .login-body {
            /* Remove since we're restructuring */
        }
        .btn-login {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            border-radius: 25px;
            padding: 15px 35px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
            width: 100%;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
        }
        .btn-login:hover {
            transform: translateY(-3px);
            box-shadow: 0 8px 25px rgba(102, 126, 234, 0.4);
            background: linear-gradient(135deg, #5a67d8 0%, #6b46c1 100%);
        }
        .btn-login:active {
            transform: translateY(-1px);
        }
        .form-control {
            border-radius: 12px;
            border: 2px solid #e3f2fd;
            padding: 15px 18px;
            margin-bottom: 1.5rem;
            font-size: 1rem;
            transition: all 0.3s ease;
        }
        .form-control:focus {
            border-color: #667eea;
            box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
            transform: translateY(-1px);
        }
        .form-label {
            font-weight: 600;
            color: #495057;
            margin-bottom: 0.8rem;
            font-size: 1rem;
        }
        .demo-info {
            background: linear-gradient(135deg, #f3f4f6 0%, #f8f9ff 100%);
            border: 1px solid #d1d5db;
            border-radius: 10px;
            padding: 1rem;
            margin-bottom: 1rem;
            text-align: center;
        }
        .demo-toggle {
            color: #667eea;
            cursor: pointer;
            font-weight: 600;
            text-decoration: none;
            font-size: 0.9rem;
        }
        .demo-toggle:hover {
            color: #5a67d8;
            text-decoration: underline;
        }
        .demo-credentials {
            margin-top: 0.5rem;
            font-size: 0.85rem;
            color: #666;
            line-height: 1.4;
        }
        /* Form layout - side by side for wider container */
        .form-row {
            display: flex;
            gap: 1.5rem;
        }
        .form-row .form-group {
            flex: 1;
        }
        @media (max-width: 768px) {
            .login-container {
                flex-direction: column;
                max-width: 90%;
            }
            .login-left {
                min-height: 200px;
                padding: 2rem;
            }
            .login-right {
                padding: 2rem;
            }
        }
        @media (max-width: 576px) {
            body {
                padding: 15px;
            }
            .login-container {
                max-width: 100%;
            }
            .login-left {
                padding: 1.5rem;
            }
            .login-right {
                padding: 1.5rem;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <!-- Left Side - HR Management Header -->
            <div class="login-left">
                <i class="fas fa-users fa-4x mb-4"></i>
                <h1 class="mb-3">HR Management</h1>
                <p class="mt-3 opacity-75">IntelliHR: A Modern Employee Management Platform for Smarter Workforce Administration</p>
            </div>
            
            <!-- Right Side - Login Form -->
            <div class="login-right">
                <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-danger mb-4">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </asp:Panel>


                <div class="mb-3">
                    <label for="txtUsername" class="form-label">
                        <i class="fas fa-user me-2"></i>Username
                    </label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                               placeholder="Enter your username" required="true"></asp:TextBox>
                </div>

                <div class="mb-4">
                    <label for="txtPassword" class="form-label">
                        <i class="fas fa-lock me-2"></i>Password
                    </label>
                    <div class="position-relative">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" 
                                   CssClass="form-control pe-5" placeholder="Enter your password" required="true"></asp:TextBox>
                        <button type="button" class="btn position-absolute top-50 end-0 translate-middle-y me-2" 
                                style="border: none; background: none; color: #6c757d; z-index: 10;" onclick="togglePassword()">
                            <i class="fas fa-eye" id="toggleIcon"></i>
                        </button>
                    </div>
                </div>

                <div class="d-grid mb-4">
                    <asp:Button ID="btnLogin" runat="server" Text="Sign In" 
                              CssClass="btn btn-primary btn-login" OnClick="btnLogin_Click" />
                </div>

                
            </div>
        </div>
    </form>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/js/bootstrap.bundle.min.js"></script>
    
    <script>
        function togglePassword() {
            const passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            const toggleIcon = document.getElementById('toggleIcon');

            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                toggleIcon.classList.remove('fa-eye');
                toggleIcon.classList.add('fa-eye-slash');
            } else {
                passwordField.type = 'password';
                toggleIcon.classList.remove('fa-eye-slash');
                toggleIcon.classList.add('fa-eye');
            }
        }

    </script>
</body>
</html>