using System;

namespace ExpenseTrackerNewAPI.API.Documentation
{
    public static class ApiDocumentation
    {
        public static class GeneralInfo
        {
            public const string BaseUrl = "https://your-api-url.com/api";
            public const string Authentication = "JWT Bearer Token";
            public const string ContentType = "application/json";
        }

        public static class AuthEndpoints
        {
            public const string Register = "POST /api/auth/register";
            public const string Login = "POST /api/auth/login";
            public const string ResetPassword = "POST /api/auth/reset-password";
            public const string UpdateSalary = "PUT /api/auth/update-salary";
            public const string UpdateProfile = "PUT /api/auth/update-profile";
            public const string UpdatePreferences = "PUT /api/auth/update-preferences";
            public const string GetPreferences = "GET /api/auth/preferences";

            public static class RequestBodies
            {
                public static class Register
                {
                    public const string Example = @"
{
    ""username"": ""string"",
    ""email"": ""string"",
    ""password"": ""string"",
    ""monthlySalary"": ""decimal""
}";
                }

                public static class Login
                {
                    public const string Example = @"
{
    ""email"": ""string"",
    ""password"": ""string""
}";
                }

                public static class ResetPassword
                {
                    public const string Example = @"
{
    ""email"": ""string"",
    ""newPassword"": ""string""
}";
                }

                public static class UpdateSalary
                {
                    public const string Example = @"
{
    ""newSalary"": ""decimal""
}";
                }

                public static class UpdateProfile
                {
                    public const string Example = @"
{
    ""profilePicture"": ""string"",
    ""language"": ""string"",
    ""theme"": ""string""
}";
                }

                public static class UpdatePreferences
                {
                    public const string Example = @"
{
    ""itemsPerPage"": ""int"",
    ""defaultCurrency"": ""string"",
    ""notificationEnabled"": ""boolean""
}";
                }
            }
        }

        public static class CategoryEndpoints
        {
            public const string Create = "POST /api/category";
            public const string Update = "PUT /api/category/{id}";
            public const string Delete = "DELETE /api/category/{id}";
            public const string GetAll = "GET /api/category";

            public static class RequestBodies
            {
                public static class Create
                {
                    public const string Example = @"
{
    ""name"": ""string""
}";
                }

                public static class Update
                {
                    public const string Example = @"
{
    ""name"": ""string""
}";
                }
            }
        }

        public static class ExpenseEndpoints
        {
            public const string Create = "POST /api/expense";
            public const string Update = "PUT /api/expense/{id}";
            public const string Delete = "DELETE /api/expense/{id}";
            public const string GetAll = "GET /api/expense";
            public const string GetByCategory = "GET /api/expense/category/{categoryId}";
            public const string GetTotal = "GET /api/expense/total";
            public const string GetPaginated = "GET /api/expense/paginated";

            public static class RequestBodies
            {
                public static class Create
                {
                    public const string Example = @"
{
    ""name"": ""string"",
    ""amount"": ""decimal"",
    ""date"": ""datetime"",
    ""categoryId"": ""int"",
    ""description"": ""string""
}";
                }

                public static class Update
                {
                    public const string Example = @"
{
    ""name"": ""string"",
    ""amount"": ""decimal"",
    ""date"": ""datetime"",
    ""categoryId"": ""int"",
    ""description"": ""string""
}";
                }
            }

            public static class QueryParameters
            {
                public const string StartDate = "startDate: DateTime? (opsiyonel)";
                public const string EndDate = "endDate: DateTime? (opsiyonel)";
                public const string PageNumber = "pageNumber: int (varsayılan: 1)";
                public const string PageSize = "pageSize: int (varsayılan: 10)";
                public const string CategoryId = "categoryId: int? (opsiyonel)";
            }
        }

        public static class ReportEndpoints
        {
            public const string Monthly = "GET /api/report/monthly";
        }

        public static class StatusCodes
        {
            public const string Ok = "200 OK: İşlem başarılı";
            public const string BadRequest = "400 Bad Request: Geçersiz istek";
            public const string Unauthorized = "401 Unauthorized: Kimlik doğrulama hatası";
            public const string NotFound = "404 Not Found: Kaynak bulunamadı";
            public const string InternalServerError = "500 Internal Server Error: Sunucu hatası";
        }

        public static class FrontendExample
        {
            public const string Code = @"
// API servis örneği
import axios from 'axios';

const API_URL = 'https://your-api-url.com/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json'
    }
});

// Token'ı header'a ekle
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Auth servisi
export const authService = {
    login: async (email: string, password: string) => {
        const response = await api.post('/auth/login', { email, password });
        return response.data;
    },
    register: async (userData: {
        username: string;
        email: string;
        password: string;
        monthlySalary: number;
    }) => {
        const response = await api.post('/auth/register', userData);
        return response.data;
    }
};

// Expense servisi
export const expenseService = {
    getExpenses: async (startDate?: Date, endDate?: Date) => {
        const response = await api.get('/expense', {
            params: { startDate, endDate }
        });
        return response.data;
    },
    createExpense: async (expenseData: {
        name: string;
        amount: number;
        date: Date;
        categoryId: number;
        description: string;
    }) => {
        const response = await api.post('/expense', expenseData);
        return response.data;
    }
};";
        }
    }
} 