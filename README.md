<div align="center">  

# HospitalWeb

Web-application for a clinic based on ASP.NET MVC Framework.

Author: Ihor Nekhaienko

</div>  

## Description
Among the main features could be mentioned:
- Allows patients to make appointments with doctors and view the history of visits.
- Doctors can diagnose and print appointments in PDF format.
- Has filtering functionality, schedule manager and email notification system.
- Patients are added by direct registration, new admins and doctors only through the administration functionality.

### Project structure
The solution is currently consists of 4 projects:
- HospitalWeb.
- HospitalWeb.DAL - data access layer with DB context, entities and repositories.
- Hospital.BLL - services project.
- Hospital.Filters - contains models for filtration and pagination.

### Database
There are 9 tables:
- AppUsers - based on IdentityUser.
- Admins.
- Doctors.
- Patients.
- Addresses - patient addresses.
- Localities - towns and cities.
- Records - history of appointments.
- Schedules - for interaction with doctors shifts and planning appointments.
- Specialties - doctors specialties.
