<div align="center">  

# HospitalWeb

Web-application for a clinic based on ASP.NET CORE Framework.

Author: Ihor Nekhaienko

</div>  

## Description
Among the main features could be mentioned:
- Allows patients to make appointments with doctors and view the history of visits.
- Doctors can diagnose and print appointments in PDF format.
- Has filtering functionality, schedule manager and email notification system.
- Patients are added by direct registration, new admins and doctors only through the administration functionality.
- Browsing statistics with responsible charts.

### Project structure
The solution is currently consists of 7 projects:
- HospitalWeb.Domain - Core part of the application
- Hospital.WebApi - API to the Domain part based on ASP NET CORE WebApi containing CRUD functionality
- Hospital.WebApi.Services - Services for HospitalWeb.WebApi project
- Hospital.WebApi.Tests - Project with unit tests for HospitalWeb.WebApi project
- HospitalWeb.Tests.Services - Common services for testing
- HospitalWeb.Mvc - Client based on ASP NET CORE MVC 
- Hospital.Mvc.Services - Services for HospitalWeb.Mvc project

### API integrations & used technologies
The project includes:
- Internal authentication with ASP NET CORE Identity and cstom JWT tokens for the access to WebApi 
- External authentication with Google and Facebook
- 2FA based on Google/Microsoft Authenticator
- LiqPay integration in sandbox mode
- Google Calendar integration for appointment scheduling
- Zoom integration for creating meetings
- Hangfire background services
- Support chat, push notification system and rating functionality on SignalR
- PlotlyJS charts
- Microsoft Azure Email communication service integration 
- iText framework for building PDF-reports
- Unit tests using xUnit, Moq, Fluent Assertions and Bogus frameworks

### CI/CD pipeline
The CI/CD pipeline works as follows:
1) Solution restoration
2) Solution building
3) Running WebApi tests
4) Docker hub login
5) Docker-compose build
6) Docker-compose push to the hub
7) Pushed containers could be connected to the Azure app service or any other cloud

