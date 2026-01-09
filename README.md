# PluggKompis API

Backend API for the PluggKompis homework help coordination platform.

**Project Type:** School project - Advanced Object-Oriented Programming  
**Institution:** NBI Handelsakademin, Gothenburg  
**Team:** 2 developers  
**Timeline:** 3 weeks

---

## 📋 About PluggKompis

PluggKompis is a platform that connects students and parents with free homework help (läxhjälp) offered at libraries, youth centers (fritidsgårdar), and study associations throughout Sweden.

**The Problem:**  
Finding homework help is scattered across different websites, Facebook groups, and physical flyers. Parents and students struggle to discover what's available nearby, see schedules, or book spots in advance.

**Our Solution:**  
A centralized platform where parents/students can browse, filter, and book homework help sessions, while venues can coordinate their volunteers and schedules efficiently.

---

## 🏗 Architecture Overview

This project uses **Clean Architecture** with clear separation of concerns:
```
API → Application → Domain
API → Infrastructure (through DI)
Infrastructure ← Application (Abstractions only)
Domain has zero external dependencies
```

---

## 🚀 Features

### Core Functionality
- **Authentication & Authorization** - JWT-based auth with role-based access (Parent, Student, Volunteer, Coordinator)
- **Venue Management** - CRUD operations for homework help locations
- **TimeSlot Management** - Recurring and one-time session scheduling
- **Booking System** - Parents book for children, students book for themselves
- **Volunteer Management** - Application, approval, and shift assignment workflow
- **Child Management** - Parents can register multiple children

### VG (Advanced) Features
- **PDF Export** - Volunteers can export their hours as PDF for CSN/university (QuestPDF)
- **Coordinator Dashboard** - Real-time stats, subject coverage heatmap, alerts for unfilled shifts
- **Automated Reminders** - Azure Function sends email reminders 24h before sessions (SendGrid)
- **Attendance Tracking** - Coordinators mark volunteer attendance and add notes

### Technical Features
- Clean Architecture structure with clear boundaries
- CQRS with MediatR
- Entity Framework Core with SQL Server
- Repository Pattern
- Global Exception Handling Middleware
- Dependency Injection per layer
- Swagger/OpenAPI documentation
- BCrypt password hashing
- Role-based authorization policies

---

## 📁 Project Structure
```
PluggKompis/
├─ API/
│  ├─ Controllers/
│  │  ├─ AuthController.cs
│  │  ├─ VenuesController.cs
│  │  ├─ BookingsController.cs
│  │  ├─ VolunteersController.cs
│  │  ├─ CoordinatorController.cs
│  │  └─ ChildrenController.cs
│  ├─ Middleware/
│  │  └─ ExceptionHandlingMiddleware.cs
│  ├─ Extensions/
│  │  └─ ClaimsPrincipalExtensions.cs
│  ├─ appsettings.json
│  ├─ appsettings.Development.json
│  └─ Program.cs
│
├─ Application/
│  ├─ Services/
│  │  ├─ Auth/
│  │  │  └─ AuthService.cs
│  │  ├─ Venues/
│  │  │  └─ VenueService.cs
│  │  ├─ Bookings/
│  │  │  └─ BookingService.cs
│  │  ├─ Volunteers/
│  │  │  └─ VolunteerService.cs
│  │  ├─ Reports/
│  │  │  └─ VolunteerReportService.cs
│  │  └─ Dashboard/
│  │     └─ CoordinatorDashboardService.cs
│  ├─ DTOs/
│  │  ├─ Auth/
│  │  ├─ Venues/
│  │  ├─ Bookings/
│  │  └─ Volunteers/
│  ├─ Interfaces/
│  │  ├─ IAuthService.cs
│  │  ├─ IVenueRepository.cs
│  │  ├─ IBookingRepository.cs
│  │  └─ ... (other repository interfaces)
│  ├─ Mappings/
│  │  └─ AutoMapperProfile.cs
│  └─ DependencyInjection.cs
│
├─ Domain/
│  ├─ Entities/
│  │  ├─ User.cs
│  │  ├─ Venue.cs
│  │  ├─ TimeSlot.cs
│  │  ├─ Subject.cs
│  │  ├─ Booking.cs
│  │  ├─ Child.cs
│  │  ├─ VolunteerProfile.cs
│  │  ├─ VolunteerShift.cs
│  │  ├─ VolunteerSubject.cs
│  │  └─ TimeSlotSubject.cs
│  ├─ Enums/
│  │  ├─ UserRole.cs
│  │  ├─ BookingStatus.cs
│  │  ├─ ShiftStatus.cs
│  │  └─ ... (other enums)
│  └─ Common/
│     ├─ Result.cs
│     └─ PagedResult.cs
│
├─ Infrastructure/
│  ├─ Data/
│  │  ├─ PluggKompisDbContext.cs
│  │  └─ Migrations/
│  ├─ Repositories/
│  │  ├─ UserRepository.cs
│  │  ├─ VenueRepository.cs
│  │  ├─ BookingRepository.cs
│  │  ├─ VolunteerRepository.cs
│  │  └─ ... (other repositories)
│  ├─ Auth/
│  │  ├─ JwtTokenGenerator.cs
│  │  └─ JwtSettings.cs
│  ├─ Email/
│  │  └─ EmailService.cs (SendGrid)
│  └─ DependencyInjection.cs
│
└─ Tests/
   ├─ UnitTests/
   └─ IntegrationTests/
```

---

## 🎯 Domain Model

### Core Entities

**User** - System users with roles (Parent, Student, Volunteer, Coordinator)  
**Venue** - Physical locations offering homework help (libraries, youth centers)  
**TimeSlot** - When homework help is available (recurring or one-time)  
**Subject** - Academic subjects (Math, Swedish, English, etc.)  
**Booking** - Student/child booking for a specific timeslot  
**Child** - Children registered by parents (under 16 years old)  
**VolunteerProfile** - Extended volunteer information (bio, experience, subjects)  
**VolunteerShift** - Volunteer assignment to a specific timeslot  

### Relationships

- User (Parent) → many Children
- User (Coordinator) → one Venue
- Venue → many TimeSlots
- TimeSlot ↔ many Subjects (via TimeSlotSubject)
- TimeSlot → many Bookings
- TimeSlot → many VolunteerShifts
- Volunteer ↔ many Subjects (via VolunteerSubject with confidence levels)
- Child → many Bookings

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server Express (local development)
- Azure account (for production deployment)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/PluggKompis/pluggkompis-api.git
cd pluggkompis-api
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Update connection string**

Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PluggKompis;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

4. **Run database migrations**
```bash
dotnet ef migrations add InitialCreate -p Infrastructure -s API
dotnet ef database update -p Infrastructure -s API
```

5. **Run the API**
```bash
dotnet run --project API
```

API will be available at `https://localhost:5001`  
Swagger documentation at `https://localhost:5001/swagger`

---

## 📜 Available Scripts
```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run API
dotnet run --project API

# Run tests
dotnet test

# Check code formatting
dotnet format --verify-no-changes

# Fix code formatting
dotnet format

# Create new migration
dotnet ef migrations add MigrationName -p Infrastructure -s API

# Update database
dotnet ef database update -p Infrastructure -s API

# Rollback migration
dotnet ef database update PreviousMigrationName -p Infrastructure -s API
```

---

## 🔐 Authentication & Authorization

### JWT Configuration

Configure JWT settings in `appsettings.json`:
```json
{
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-characters-long!",
    "Issuer": "PluggKompis",
    "Audience": "PluggKompisUsers",
    "ExpiryInHours": 24
  }
}
```

### User Roles

- **Parent** - Can register children and book sessions
- **Student (16+)** - Can book sessions for themselves
- **Volunteer** - Can apply to venues, sign up for shifts, export hours
- **Coordinator** - Can manage their venue, approve volunteers, track attendance

### Example: Protected Endpoint
```csharp
[Authorize(Roles = "Coordinator")]
[HttpPost("venues")]
public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequest request)
{
    var userId = User.GetUserId(); // Extension method
    // ...
}
```

---

## 📦 NuGet Packages

| Package | Purpose |
|---------|---------|
| **Entity Framework Core** |
| `Microsoft.EntityFrameworkCore` | ORM |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server provider |
| `Microsoft.EntityFrameworkCore.Design` | Migrations & scaffolding |
| `Microsoft.EntityFrameworkCore.Tools` | EF CLI tools |
| **Authentication** |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT authentication |
| `BCrypt.Net-Next` | Password hashing |
| **Validation & Mapping** |
| `FluentValidation` | Request validation |
| `AutoMapper` | Object mapping |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | DI integration |
| **Documentation** |
| `Swashbuckle.AspNetCore` | Swagger/OpenAPI |
| **VG Features** |
| `QuestPDF` | PDF generation for volunteer hours |
| `SendGrid` | Email service for reminders |
| **Testing** |
| `xUnit` | Unit testing framework |
| `Moq` | Mocking framework |
| `FluentAssertions` | Test assertions |

---

## 🌐 API Endpoints

### Authentication
```
POST   /api/auth/register     - Register new user
POST   /api/auth/login        - Login and get JWT token
```

### Venues
```
GET    /api/venues                    - List all venues (with filters)
GET    /api/venues/{id}               - Get venue details
POST   /api/venues                    - Create venue (Coordinator only)
PUT    /api/venues/{id}               - Update venue (Coordinator only)
DELETE /api/venues/{id}               - Delete venue (Coordinator only)
GET    /api/venues/{id}/timeslots     - Get venue's schedule
GET    /api/venues/{id}/volunteers    - Get venue's volunteers
```

### TimeSlots
```
GET    /api/timeslots                 - List time slots (with filters)
GET    /api/timeslots/{id}            - Get time slot details
POST   /api/timeslots                 - Create time slot (Coordinator only)
PUT    /api/timeslots/{id}            - Update time slot
PUT    /api/timeslots/{id}/cancel     - Cancel time slot
```

### Bookings
```
GET    /api/bookings                  - List user's bookings
GET    /api/bookings/{id}             - Get booking details
POST   /api/bookings                  - Create booking
DELETE /api/bookings/{id}             - Cancel booking
```

### Volunteers
```
POST   /api/volunteers/apply                      - Apply to be volunteer
GET    /api/volunteers/{id}/profile               - Get volunteer profile
PUT    /api/volunteers/{id}/profile               - Update volunteer profile
GET    /api/volunteers/{id}/shifts                - Get volunteer's shifts
POST   /api/volunteers/shifts                     - Sign up for shift
DELETE /api/volunteers/shifts/{id}                - Cancel shift
GET    /api/volunteers/{id}/reports/hours.pdf     - Export hours as PDF (VG)
```

### Coordinator
```
GET    /api/coordinator/dashboard              - Get dashboard data (VG)
GET    /api/coordinator/applications           - Get pending volunteer applications
PUT    /api/coordinator/applications/{id}/approve  - Approve volunteer
PUT    /api/coordinator/applications/{id}/decline  - Decline volunteer
PUT    /api/coordinator/shifts/{id}/attendance     - Mark attendance
```

### Children
```
GET    /api/children              - List parent's children
POST   /api/children              - Register child
PUT    /api/children/{id}         - Update child info
DELETE /api/children/{id}         - Remove child
```

### Subjects
```
GET    /api/subjects              - List all subjects
```

---

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Project
```bash
dotnet test Tests/UnitTests
dotnet test Tests/IntegrationTests
```

### Test Coverage
Integration tests verify:
- Booking creation and validation
- Venue filtering by subject
- TimeSlot cancellation with notifications
- Volunteer application workflow
- PDF export functionality

---

## 🚀 Deployment

### Local Development
Uses SQL Server Express with connection string in `appsettings.Development.json`

### Production (Azure)
Deployed to Azure App Service with:
- Azure SQL Database
- Azure Functions (for automated reminders)
- Application Insights (monitoring)

**Live API:** https://pluggkompis-api.azurewebsites.net/api  
**Swagger:** https://pluggkompis-api.azurewebsites.net/swagger

### CI/CD Pipeline
GitHub Actions automatically:
- Runs tests on every PR to `development` or `main`
- Checks code formatting
- Deploys to Azure when merged to `main`

---

## 🔧 Development Guidelines

### Code Style
- Follow Clean Architecture principles
- Keep controllers thin - delegate to services
- Use dependency injection
- Implement interfaces in Application, concrete classes in Infrastructure
- Keep Domain pure (no external dependencies)
- Use meaningful variable names
- Add XML comments for public APIs

### Adding New Features
1. Define entity in `Domain/Entities`
2. Create repository interface in `Application/Interfaces`
3. Implement repository in `Infrastructure/Repositories`
4. Create service in `Application/Services`
5. Create DTOs in `Application/DTOs`
6. Add controller in `API/Controllers`
7. Write tests

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName -p Infrastructure -s API

# Review migration file in Infrastructure/Data/Migrations

# Apply migration
dotnet ef database update -p Infrastructure -s API
```

---

## 📊 Database Schema

See [ER Diagram](docs/er-diagram.png) for complete database structure.

**Key Tables:**
- Users - System users with roles
- Venues - Homework help locations
- TimeSlots - Session schedules
- Subjects - Academic subjects
- Bookings - Student bookings
- Children - Registered children
- VolunteerProfiles - Volunteer information
- VolunteerShifts - Shift assignments
- VolunteerSubjects - Volunteer competencies (join table)
- TimeSlotSubjects - Session subjects (join table)

---

## 🤝 Contributing

This is a school project with limited contributors. For team members:

1. Create feature branch from `development`
2. Follow naming: `feature/feature-name` or `fix/bug-name`
3. Write tests for new functionality
4. Ensure CI passes
5. Create PR to `development`
6. Get 1 approval before merging
7. Merge to `main` when ready for production

---

## 📝 License

This project is created as a school project for NBI Handelsakademin.

---

## 🔗 Related Repositories

- **Frontend:** [pluggkompis-client](https://github.com/PluggKompis/pluggkompis-client)
- **Project Board:** [PluggKompis Development](https://github.com/orgs/PluggKompis/projects/1)

---

## 👥 Team

- **[Gabby](https://github.com/GabbyFerm)** - Full-stack developer
- **[Mohanad](https://github.com/mohald-3)** - Full-stack developer

**Course:** Advanced Object-Oriented Programming  
**Institution:** NBI Handelsakademin, Gothenburg  
**Instructors:** Nemanja

---

## 📞 Support

For questions or issues:
- Create an issue in this repository
- Contact team members via project communication channels

---

**Built with ❤️ as part of our .NET System Development education**
