CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Logs" (
    "Id" uuid NOT NULL,
    "EntityName" text NOT NULL,
    "Operation" text NOT NULL,
    "Data" text NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Logs" PRIMARY KEY ("Id")
);

CREATE TABLE "Subjects" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Icon" character varying(100),
    CONSTRAINT "PK_Subjects" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PasswordHash" text NOT NULL,
    "RefreshToken" text,
    "RefreshTokenExpiresAt" timestamp with time zone,
    "Role" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Children" (
    "Id" uuid NOT NULL,
    "ParentId" uuid NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "BirthYear" integer NOT NULL,
    "SchoolGrade" character varying(50) NOT NULL,
    CONSTRAINT "PK_Children" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Children_Users_ParentId" FOREIGN KEY ("ParentId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Venues" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Address" character varying(300) NOT NULL,
    "City" character varying(100) NOT NULL,
    "PostalCode" character varying(20) NOT NULL,
    "Description" character varying(2000) NOT NULL,
    "ContactEmail" character varying(256) NOT NULL,
    "ContactPhone" character varying(50) NOT NULL,
    "Latitude" double precision,
    "Longitude" double precision,
    "CoordinatorId" uuid NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Venues" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Venues_Users_CoordinatorId" FOREIGN KEY ("CoordinatorId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "VolunteerSubjects" (
    "VolunteerId" uuid NOT NULL,
    "SubjectId" uuid NOT NULL,
    "ConfidenceLevel" integer NOT NULL,
    CONSTRAINT "PK_VolunteerSubjects" PRIMARY KEY ("VolunteerId", "SubjectId"),
    CONSTRAINT "FK_VolunteerSubjects_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES "Subjects" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_VolunteerSubjects_Users_VolunteerId" FOREIGN KEY ("VolunteerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TimeSlots" (
    "Id" uuid NOT NULL,
    "VenueId" uuid NOT NULL,
    "DayOfWeek" integer NOT NULL,
    "StartTime" interval NOT NULL,
    "EndTime" interval NOT NULL,
    "MaxStudents" integer NOT NULL,
    "IsRecurring" boolean NOT NULL,
    "SpecificDate" date,
    "RecurringStartDate" date,
    "RecurringEndDate" date,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_TimeSlots" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TimeSlots_Venues_VenueId" FOREIGN KEY ("VenueId") REFERENCES "Venues" ("Id") ON DELETE CASCADE
);

CREATE TABLE "VolunteerApplications" (
    "Id" uuid NOT NULL,
    "VolunteerId" uuid NOT NULL,
    "VenueId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "AppliedAt" timestamp with time zone NOT NULL,
    "ReviewedByCoordinatorId" uuid,
    "ReviewedAt" timestamp with time zone,
    "DecisionNote" character varying(1000),
    CONSTRAINT "PK_VolunteerApplications" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_VolunteerApplications_Users_ReviewedByCoordinatorId" FOREIGN KEY ("ReviewedByCoordinatorId") REFERENCES "Users" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_VolunteerApplications_Users_VolunteerId" FOREIGN KEY ("VolunteerId") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_VolunteerApplications_Venues_VenueId" FOREIGN KEY ("VenueId") REFERENCES "Venues" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "VolunteerProfiles" (
    "VolunteerId" uuid NOT NULL,
    "Bio" character varying(2000) NOT NULL,
    "Experience" character varying(2000) NOT NULL,
    "MaxHoursPerWeek" integer,
    "PreferredVenueId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "VenueId" uuid,
    CONSTRAINT "PK_VolunteerProfiles" PRIMARY KEY ("VolunteerId"),
    CONSTRAINT "FK_VolunteerProfiles_Users_VolunteerId" FOREIGN KEY ("VolunteerId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_VolunteerProfiles_Venues_PreferredVenueId" FOREIGN KEY ("PreferredVenueId") REFERENCES "Venues" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_VolunteerProfiles_Venues_VenueId" FOREIGN KEY ("VenueId") REFERENCES "Venues" ("Id")
);

CREATE TABLE "Bookings" (
    "Id" uuid NOT NULL,
    "TimeSlotId" uuid NOT NULL,
    "StudentId" uuid,
    "ChildId" uuid,
    "BookedByUserId" uuid NOT NULL,
    "BookingDate" timestamp with time zone NOT NULL,
    "BookedAt" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "Notes" character varying(2000),
    "CancelledAt" timestamp with time zone,
    CONSTRAINT "PK_Bookings" PRIMARY KEY ("Id"),
    CONSTRAINT "CK_Booking_StudentOrChild" CHECK ((("StudentId" IS NOT NULL AND "ChildId" IS NULL) OR ("StudentId" IS NULL AND "ChildId" IS NOT NULL))),
    CONSTRAINT "FK_Bookings_Children_ChildId" FOREIGN KEY ("ChildId") REFERENCES "Children" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Bookings_TimeSlots_TimeSlotId" FOREIGN KEY ("TimeSlotId") REFERENCES "TimeSlots" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Bookings_Users_BookedByUserId" FOREIGN KEY ("BookedByUserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Bookings_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "TimeSlotSubjects" (
    "TimeSlotId" uuid NOT NULL,
    "SubjectId" uuid NOT NULL,
    CONSTRAINT "PK_TimeSlotSubjects" PRIMARY KEY ("TimeSlotId", "SubjectId"),
    CONSTRAINT "FK_TimeSlotSubjects_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES "Subjects" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TimeSlotSubjects_TimeSlots_TimeSlotId" FOREIGN KEY ("TimeSlotId") REFERENCES "TimeSlots" ("Id") ON DELETE CASCADE
);

CREATE TABLE "VolunteerShifts" (
    "Id" uuid NOT NULL,
    "TimeSlotId" uuid NOT NULL,
    "VolunteerId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "IsAttended" boolean NOT NULL,
    "Notes" character varying(2000),
    "OccurrenceStartUtc" timestamp with time zone NOT NULL,
    "OccurrenceEndUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_VolunteerShifts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_VolunteerShifts_TimeSlots_TimeSlotId" FOREIGN KEY ("TimeSlotId") REFERENCES "TimeSlots" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_VolunteerShifts_Users_VolunteerId" FOREIGN KEY ("VolunteerId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Bookings_BookedByUserId" ON "Bookings" ("BookedByUserId");

CREATE INDEX "IX_Bookings_ChildId" ON "Bookings" ("ChildId");

CREATE INDEX "IX_Bookings_Status" ON "Bookings" ("Status");

CREATE INDEX "IX_Bookings_StudentId" ON "Bookings" ("StudentId");

CREATE INDEX "IX_Bookings_TimeSlotId" ON "Bookings" ("TimeSlotId");

CREATE INDEX "IX_Bookings_TimeSlotId_BookingDate" ON "Bookings" ("TimeSlotId", "BookingDate");

CREATE INDEX "IX_Bookings_TimeSlotId_ChildId" ON "Bookings" ("TimeSlotId", "ChildId");

CREATE INDEX "IX_Bookings_TimeSlotId_StudentId" ON "Bookings" ("TimeSlotId", "StudentId");

CREATE INDEX "IX_Children_ParentId" ON "Children" ("ParentId");

CREATE INDEX "IX_TimeSlots_Status" ON "TimeSlots" ("Status");

CREATE INDEX "IX_TimeSlots_VenueId" ON "TimeSlots" ("VenueId");

CREATE INDEX "IX_TimeSlots_VenueId_DayOfWeek" ON "TimeSlots" ("VenueId", "DayOfWeek");

CREATE INDEX "IX_TimeSlots_VenueId_SpecificDate" ON "TimeSlots" ("VenueId", "SpecificDate");

CREATE INDEX "IX_TimeSlotSubjects_SubjectId" ON "TimeSlotSubjects" ("SubjectId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE INDEX "IX_Venues_City" ON "Venues" ("City");

CREATE INDEX "IX_Venues_CoordinatorId" ON "Venues" ("CoordinatorId");

CREATE INDEX "IX_VolunteerApplications_AppliedAt" ON "VolunteerApplications" ("AppliedAt");

CREATE INDEX "IX_VolunteerApplications_ReviewedByCoordinatorId" ON "VolunteerApplications" ("ReviewedByCoordinatorId");

CREATE INDEX "IX_VolunteerApplications_VenueId_Status" ON "VolunteerApplications" ("VenueId", "Status");

CREATE INDEX "IX_VolunteerApplications_VolunteerId_Status" ON "VolunteerApplications" ("VolunteerId", "Status");

CREATE INDEX "IX_VolunteerProfiles_PreferredVenueId" ON "VolunteerProfiles" ("PreferredVenueId");

CREATE INDEX "IX_VolunteerProfiles_VenueId" ON "VolunteerProfiles" ("VenueId");

CREATE INDEX "IX_VolunteerShifts_Status" ON "VolunteerShifts" ("Status");

CREATE INDEX "IX_VolunteerShifts_TimeSlotId" ON "VolunteerShifts" ("TimeSlotId");

CREATE UNIQUE INDEX "IX_VolunteerShifts_TimeSlotId_VolunteerId" ON "VolunteerShifts" ("TimeSlotId", "VolunteerId");

CREATE INDEX "IX_VolunteerShifts_VolunteerId" ON "VolunteerShifts" ("VolunteerId");

CREATE INDEX "IX_VolunteerShifts_VolunteerId_OccurrenceStartUtc" ON "VolunteerShifts" ("VolunteerId", "OccurrenceStartUtc");

CREATE INDEX "IX_VolunteerSubjects_SubjectId" ON "VolunteerSubjects" ("SubjectId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260506110732_InitialCreate', '8.0.15');

COMMIT;

