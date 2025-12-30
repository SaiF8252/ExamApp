# Inventory, Sales & User Management System

A full-featured ASP.NET MVC & Web API–based web application designed to manage users, products, inventory, invoices, and authentication following real-world business workflows.

# Project Overview

This project is built to simulate a real-world inventory and sales management system, where users can securely log in, manage products and stock, generate invoices, and maintain relational data using modern .NET technologies.

# Key Features
🔐 Authentication & Authorization

User Login, Registration & Logout

Secure authentication using Forms Authentication

Access control using [Authorize] and [AllowAnonymous]

CSRF protection with Anti-Forgery Token

# User Management

Admin-level User CRUD operations

Secure model binding to prevent overposting

Server-side validation

# Product & Inventory Management

Product Create, Read, Update, Delete

Image upload with size validation and unique naming

Strip-wise to unit-wise stock calculation

Dynamic stock add & reduce logic

Duplicate product prevention

Exception handling

# Invoice & Sales Management

Invoice Master–Details implementation

Strip & unit quantity-based sales handling

Automatic stock reduction on sales

Invoice search, filter, sorting

Pagination using PagedList

Partial View for invoice details (AJAX-ready)

# Web API Features

RESTful APIs for Product & Category

Eager loading using Entity Framework Include()

Secure API endpoints

Image upload API with validation

# Technologies Used

ASP.NET MVC

ASP.NET Web API

C#

Entity Framework

SQL Server

LINQ

Bootstrap

HTML & CSS
