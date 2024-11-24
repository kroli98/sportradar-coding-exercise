# SportradarCodingExercise

## Overview

A sports event calendar web application built with ASP.NET Core 8 and Angular 16.1.8. The application displays sports events with filtering capabilities and detailed event information. The data is stored in a SQL Server database.

## Setup Instructions

### Requirements

- Visual Studio 2022 
- .NET 8.0 SDK
- Microsoft SQL Server
- Node.js and npm
- Angular CLI

### Installation Steps

1. Clone the repository
2. Update connection string in `appsettings.json` to point to your SQL Server instance
3. The database will be automatically created on first run and seeded if you uncomment `await dbInitializer.SeedAsync();` line in Program.cs
4. You can easily run backend and frontend with start button from Visual Studio by opening `SportradarCodingExercise.sln` file
5. Access the application:
   Frontend: http://localhost:4200/ 
   Backend: https://localhost:7264/

## Development Assumptions and Decisions

1. Database Design

  Event Structure:
    Each event has exactly 2 participants (home and away teams)
    Based on provided JSON file structure
  
  
  Data Types:
    Using varchar instead of nvarchar
    Assumption: Database doesn't require storage of characters outside ASCII/Latin
    Benefit: Smaller database size and faster operations
  
  Address Management:
  
  Separate table for Addresses
    Reason: Multiple venues can share one address (e.g., sport complexes)
    Benefit: No duplicate address data storage
  
  
  Winner Determination:
  
  Simple winning logic based on higher score if event is 'Completed'
  
  Sport References:
  
  SportId maintained in both Event and Team tables
    Event table: For optimized sport filtering
    Team table: For ensuring unique abbreviations within sports
  
  Competition Structure:
  
  A competition might not have associated season, so it can be nullable
  

2. Development Approach

Version Control:

Single branch development (solo project)

Data Validation:

Skipped detailed validation for event-related tables
  Pre-initialized data considered unchangeable
  Focus on event data validation

Technology Choices:

Backend: ASP.NET Core 8 with ADO.NET
Database: Microsoft SQL Server
Frontend: Angular with ng-bootstrap
