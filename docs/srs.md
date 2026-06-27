# Software Requirements Specification (SRS)

## University Management System (UMS)

### Version 1.0

---

# 1. Introduction

## 1.1 Purpose

UMS is a distributed university management system designed for undergraduate education management.

The system supports student admission, academic management, faculty management, course enrollment, assessment management and graduation workflows.

The system is designed using Domain-Driven Design (DDD), CQRS, Event-Driven Architecture and Microservices Architecture principles.

---

# 1.2 Scope

The system provides a comprehensive undergraduate academic management platform that supports the following business capabilities:

* Online student registration
* Student admission and approval
* Student number generation
* Student lifecycle management
* Faculty, department, and professor management
* Academic structure management (majors, curricula, and courses)
* Semester and academic calendar management
* Course offering management
* Course enrollment and approval
* Grade submission and transcript management
* Grade appeal management
* Graduation request management
* Emergency course withdrawal management

This system is designed exclusively for undergraduate education and administrative processes.

---

# 1.3 Out of Scope

The following features are intentionally excluded from the current version of the system:

* Management of master's and doctoral programs
* Tuition and financial management
* Dormitory management
* Library management
* Scholarship management
* Student disciplinary processes
* Special academic committee requests
* Student card issuance or replacement
* Medical course exemption requests
* Thesis and dissertation management
* Alumni management

---

# 2. User Roles

## 2.1 Super Administrator

Responsible for system-wide administration, including user management, role and permission management, system configuration, and operational maintenance.

---

## 2.2 University President

Responsible for university-level administration and strategic oversight. This role has full access to all business capabilities of the system.

---

## 2.3 Education Manager

Responsible for managing academic operations, including student admissions, enrollment approvals, academic calendar management, semester administration, and graduation approvals.

---

## 2.4 Department Manager

Responsible for managing department-level academic activities, including curriculum planning, course offerings, professor assignments, and departmental supervision.

---

## 2.5 Department Expert

Responsible for executing daily academic operations within a department, including processing student requests, maintaining academic records, and supporting administrative workflows.

---

## 2.6 Professor

Responsible for teaching assigned courses, submitting grades, reviewing grade appeals, and monitoring students enrolled in their courses.

---

## 2.7 Student

Responsible for online registration, course enrollment, viewing academic records, submitting grade appeals, and requesting graduation.

---

# 3. System Architecture

## 3.1 Architectural Overview

The system shall be implemented using a Microservices Architecture based on Domain-Driven Design (DDD).

Each microservice represents a single Bounded Context, owns its data, and is independently deployable. Services communicate synchronously through HTTP APIs when immediate consistency is required and asynchronously through domain events to achieve eventual consistency.

Each service shall maintain an independent database to ensure loose coupling and autonomous data ownership.

---

## 3.2 Bounded Contexts

The system is divided into the following bounded contexts:

- Identity
- Student
- Faculty
- Academic
- Assessment

Each bounded context encapsulates its own business rules, domain model, application logic, and persistence.

---

## 3.3 Service Responsibilities

### Identity Service

#### Purpose

Provides authentication, authorization, and identity management for all users.

#### Responsibilities

- Authentication
- Authorization
- User Management
- Role Management
- Permission Management
- JWT Token Issuance
- Refresh Token Management

#### Owns

- Users
- Roles
- Permissions
- Refresh Tokens

---

### Student Service

#### Purpose

Manages the complete lifecycle of a student from admission to graduation.

#### Responsibilities

- Online Registration
- Admission Requests
- Admission Approval
- Student Profile Management
- Student Number Generation
- Graduation Requests

#### Owns

- Admission Requests
- Students
- Graduation Requests

---

### Faculty Service

#### Purpose

Manages the university organizational structure and academic staff.

#### Responsibilities

- Faculty Management
- Department Management
- Professor Management
- Professor Assignment to Departments

#### Owns

- Faculties
- Departments
- Professors

---

### Academic Service

#### Purpose

Manages academic planning and course delivery.

#### Responsibilities

- Major Management
- Curriculum Management
- Course Management
- Semester Management
- Academic Calendar Management
- Course Offering Management
- Enrollment Management

#### Owns

- Majors
- Curricula
- Courses
- Semesters
- Academic Calendars
- Course Offerings
- Enrollments

---

### Assessment Service

#### Purpose

Manages student assessment and academic performance.

#### Responsibilities

- Grade Submission
- Transcript Generation
- Grade Appeal Management

#### Owns

- Grades
- Grade Appeals
- Transcripts

---

## 3.4 Service Communication

Services shall communicate using the following mechanisms:

- REST APIs for synchronous communication.
- RabbitMQ for asynchronous event-driven communication.

The Outbox Pattern shall be used to guarantee reliable event publishing.

Long-running business workflows spanning multiple services shall be coordinated using the Saga Pattern.

---

## 3.5 Data Ownership

Each microservice shall own its database.

Direct database access between services is prohibited.

All cross-service communication shall occur through APIs or published domain events.

---

## 3.6 Shared Infrastructure

The system shall use the following shared infrastructure components:

- API Gateway (YARP)
- RabbitMQ
- SQL Server
- Redis
- OpenTelemetry
- Jaeger
- Grafana
- Serilog

---

# 4. Functional Requirements

## 4.1 Student Admission

### FR-001 Online Student Registration

The system shall allow prospective students to submit online registration requests.

### FR-002 Admission Review

The system shall allow authorized users to review submitted admission requests.

### FR-003 Admission Decision

The system shall allow authorized users to approve or reject admission requests.

### FR-004 Student Number Generation

The system shall generate a unique student number for each approved student.

### FR-005 Student Account Activation

The system shall activate the student's academic profile after successful admission.

---

## 4.2 Faculty Management

### FR-006 Faculty Management

The system shall allow authorized users to create, update, activate, deactivate, and manage faculties.

### FR-007 Department Management

The system shall allow authorized users to create, update, activate, deactivate, and manage departments.

### FR-008 Professor Management

The system shall allow authorized users to create, update, activate, deactivate, and manage professor records.

### FR-009 Department Assignment

The system shall allow authorized users to assign professors to departments.

---

## 4.3 Academic Structure

### FR-010 Major Management

The system shall allow authorized users to create, update, activate, deactivate, and manage academic majors.

### FR-011 Curriculum Management

The system shall allow authorized users to define and maintain curricula for each academic major.

### FR-012 Course Management

The system shall allow authorized users to create, update, activate, deactivate, and manage academic courses.

### FR-013 Course Prerequisites

The system shall allow authorized users to define prerequisite courses.

---

## 4.4 Semester Management

### FR-014 Semester Management

The system shall allow authorized users to create, update, activate, close, and archive academic semesters.

### FR-015 Academic Calendar

The system shall allow authorized users to configure the academic calendar for each semester, including:

* Enrollment period
* Add/Drop period
* Grade submission period
* Final examination period

### FR-016 Academic Regulations

The system shall allow authorized users to configure academic regulations for each semester, including:

* Minimum enrollment units
* Maximum enrollment units
* Maximum add/drop units

---

## 4.5 Course Offering

### FR-017 Course Offering Management

The system shall allow authorized users to create and manage course offerings for each semester.

### FR-018 Professor Assignment

The system shall allow authorized users to assign professors to course offerings.

### FR-019 Course Capacity

The system shall allow authorized users to define the enrollment capacity for each course offering.

---

## 4.6 Enrollment

### FR-020 Enrollment Submission

The system shall allow students to submit enrollment requests during the enrollment period.

### FR-021 Enrollment Validation

The system shall validate enrollment requests according to:

* Enrollment period
* Course capacity
* Curriculum requirements
* Prerequisite requirements
* Unit limits

### FR-022 Enrollment Approval

The system shall allow authorized users to approve or reject enrollment requests.

### FR-023 Emergency Course Withdrawal

The system shall allow eligible students to submit emergency course withdrawal requests according to university regulations.

---

## 4.7 Assessment

### FR-024 Grade Submission

The system shall allow professors to submit grades during the grade submission period.

### FR-025 Grade Modification

The system shall allow professors to update grades before the grading period is closed.

### FR-026 Transcript

The system shall generate and maintain an academic transcript for each student.

### FR-027 Transcript Access

The system shall allow students to view their academic transcripts.

### FR-028 Grade Appeal Submission

The system shall allow students to submit grade appeal requests.

### FR-029 Grade Appeal Review

The system shall allow professors to review and resolve submitted grade appeals.

---

## 4.8 Graduation

### FR-030 Graduation Request

The system shall allow eligible students to submit graduation requests.

### FR-031 Graduation Validation

The system shall validate graduation requests by verifying:

* Completion of all required curriculum courses
* Completion of the required academic credits
* No pending grades
* No unresolved grade appeals

### FR-032 Graduation Approval

The system shall allow authorized users to approve or reject graduation requests.

---

## 4.9 Security

### FR-033 Authentication

The system shall authenticate users before granting access to protected resources.

### FR-034 Authorization

The system shall authorize access to system resources based on roles and permissions.

---

## 4.10 Audit

### FR-035 Audit Logging

The system shall record audit logs for all critical business operations, including admission decisions, enrollment approvals, grade modifications, and graduation approvals.

---

# 5. Business Rules

Business rules define the core business constraints and policies that govern the behavior of the university management system. These rules are independent of implementation details and must always be enforced by the domain model.

---

# 5.1 Admission Rules

### BR-001

A registration request shall be reviewed before a student account is created.

### BR-002

A student number shall only be generated after an admission request has been approved.

### BR-003

Each student shall have exactly one unique student number.

### BR-004

An approved admission request cannot be modified.

### BR-005

A rejected admission request cannot be approved without resubmission.

### BR-006

A person cannot have more than one active student profile.

---

# 5.2 Faculty and Academic Structure Rules

### BR-007

Each department shall belong to exactly one faculty.

### BR-008

Each academic major shall belong to exactly one department.

### BR-009

Each academic major shall have exactly one active curriculum.

### BR-010

Each course shall belong to exactly one department.

### BR-011

Each course shall have a fixed number of academic credits.

### BR-012

A course may define zero or more prerequisite courses.

### BR-013

A course cannot be its own prerequisite.

### BR-014

Circular prerequisite relationships shall not be permitted.

### BR-015

A professor may belong to one or more departments.

### BR-016

Each department shall have exactly one active department manager.

---

# 5.3 Semester Rules

### BR-017

Only one semester may be active for enrollment at any given time.

### BR-018

Academic calendar dates shall not overlap within the same semester.

### BR-019

The enrollment period shall end before the add/drop period begins.

### BR-020

The add/drop period shall end before the final examination period begins.

### BR-021

The grade submission period shall begin only after the examination period has ended.

---

# 5.4 Course Offering Rules

### BR-022

A course offering shall belong to exactly one semester.

### BR-023

A course offering shall reference exactly one academic course.

### BR-024

A course offering shall have exactly one assigned professor.

### BR-025

A course offering shall define a maximum enrollment capacity.

### BR-026

The enrollment capacity shall be greater than zero.

### BR-027

Students shall not enroll after the course offering reaches its capacity.

---

# 5.5 Enrollment Rules

### BR-028

Students may enroll only during the official enrollment period.

### BR-029

Students shall not exceed the maximum allowed academic units.

### BR-030

Students shall not enroll below the minimum required academic units unless permitted by university regulations.

### BR-031

All prerequisite courses shall be successfully completed before enrollment.

### BR-032

A student shall not enroll in the same course more than once unless course repetition is permitted.

### BR-033

A student shall not enroll in two course offerings whose schedules overlap.

### BR-034

Enrollment shall require approval by an authorized academic officer.

### BR-035

Emergency course withdrawal shall only be permitted during the configured withdrawal period.

---

# 5.6 Assessment Rules

### BR-036

Only the assigned professor may submit grades.

### BR-037

Grades shall only be submitted during the grade submission period.

### BR-038

Published grades shall become read-only after the grading period has closed.

### BR-039

Grade modifications after publication shall require an approved grade appeal.

### BR-040

A student may submit only one active appeal for a specific course.

### BR-041

Only the assigned professor may review grade appeals.

### BR-042

A resolved appeal cannot be reopened.

---

# 5.7 Graduation Rules

### BR-043

Students may request graduation only after completing all required curriculum courses.

### BR-044

Students shall complete the minimum required academic credits before graduation approval.

### BR-045

Students shall have no pending grades.

### BR-046

Students shall have no unresolved grade appeals.

### BR-047

Students shall have an active academic status.

### BR-048

Each graduation request may be approved or rejected only once.

---

# 5.8 Security Rules

### BR-049

All protected operations shall require authentication.

### BR-050

Authorization shall be based on assigned roles and permissions.

### BR-051

Users may only access resources permitted by their assigned permissions.

### BR-052

Disabled user accounts shall not be authenticated.

---

# 5.9 Audit Rules

### BR-053

The system shall record audit logs for all critical business operations.

### BR-054

Audit records shall be immutable.

### BR-055

Audit records shall include the acting user, operation, timestamp, and affected resource.

---

# 5.10 Consistency Rules

### BR-056

Each aggregate shall enforce its own business invariants.

### BR-057

Cross-service business consistency shall be maintained using eventual consistency.

### BR-058

Business events shall be published only after successful transaction completion.

### BR-059

Business events shall not be published more than once for the same transaction.

### BR-060

Critical business workflows spanning multiple bounded contexts shall be coordinated using the Saga pattern.

---

# 6. Non-Functional Requirements

The following non-functional requirements define the quality attributes of the system.

---

## 6.1 Availability

### NFR-001

Each microservice shall be independently deployable without requiring redeployment of other services.

### NFR-002

A failure in one microservice shall not prevent other services from continuing normal operation whenever possible.

---

## 6.2 Scalability

### NFR-003

Each microservice shall support independent horizontal scaling.

### NFR-004

Stateless services shall support running multiple instances simultaneously.

---

## 6.3 Performance

### NFR-005

The system shall support asynchronous processing for long-running business operations.

### NFR-006

Frequently accessed data shall be cached using Redis where appropriate.

---

## 6.4 Reliability

### NFR-007

Inter-service communication shall support eventual consistency through asynchronous messaging.

### NFR-008

Business events shall be published using the Outbox Pattern.

### NFR-009

Long-running distributed transactions shall be coordinated using the Saga Pattern.

### NFR-010

The system shall prevent duplicate processing of integration events.

---

## 6.5 Security

### NFR-011

Authentication shall be implemented using OAuth 2.0 and OpenID Connect (OIDC).

### NFR-012

Access tokens shall be issued as JWTs.

### NFR-013

Authorization shall be based on roles and permissions.

### NFR-014

Passwords shall never be stored in plain text.

### NFR-015

Sensitive data shall be transmitted only over HTTPS.

---

## 6.6 Observability

### NFR-016

The system shall provide centralized structured logging.

### NFR-017

The system shall provide distributed tracing across all microservices.

### NFR-018

The system shall expose application metrics for monitoring.

### NFR-019

Every request shall include a Correlation ID to support end-to-end tracing.

---

## 6.7 Maintainability

### NFR-020

Each microservice shall own its domain model and persistence layer.

### NFR-021

Each microservice shall be independently testable.

### NFR-022

Public APIs shall be documented using OpenAPI.

---

## 6.8 Data Management

### NFR-023

Each microservice shall own its own database.

### NFR-024

Direct database access between services shall not be permitted.

### NFR-025

Data consistency between services shall be maintained using domain events.

---

## 6.9 Interoperability

### NFR-026

Synchronous communication shall be performed using REST APIs.

### NFR-027

Asynchronous communication shall be performed using RabbitMQ.

---

## 6.10 Auditability

### NFR-028

Critical business operations shall be recorded in immutable audit logs.

### NFR-029

Audit records shall include the acting user, timestamp, operation, and affected resource.

---

# 7. Integration Requirements

This section defines how the system interacts internally across bounded contexts and externally with infrastructure components.

---

## 7.1 Service Communication

### IR-001

The system shall support synchronous communication between microservices for operations requiring immediate responses.

### IR-002

The system shall support asynchronous communication for business events and long-running workflows.

---

## 7.2 Event-Driven Integration

### IR-003

Business events shall be published whenever a significant domain event occurs.

Examples include:

- StudentAdmitted
- ProfessorCreated
- CourseOffered
- EnrollmentApproved
- GradeSubmitted
- GradeAppealSubmitted
- GraduationRequested
- GraduationApproved

### IR-004

Services shall subscribe only to the business events required by their bounded context.

---

## 7.3 Data Ownership

### IR-005

Each microservice shall exclusively own and manage its data.

### IR-006

Direct database access between microservices shall be prohibited.

### IR-007

Cross-service data access shall occur only through public APIs or published integration events.

---

## 7.4 Identity Integration

### IR-008

All business services shall rely on the Identity Service for authentication and authorization.

### IR-009

Authenticated user information shall be propagated using access tokens.

---

## 7.5 Reliability

### IR-010

Integration events shall be delivered reliably without data loss.

### IR-011

The system shall tolerate temporary communication failures between services.

### IR-012

Duplicate integration events shall be safely handled.

---

## 7.6 External Infrastructure

The system shall integrate with the following infrastructure services:

- API Gateway
- Message Broker
- Distributed Cache
- Relational Database
- Centralized Logging
- Distributed Tracing
- Metrics Collection

---

# 8. Architectural Patterns

This project intentionally demonstrates multiple architectural styles.

- Identity Service and Student Service are implemented using Clean Architecture to demonstrate layered architecture for complex and stable domains.

- Faculty Service, Academic Service, and Assessment Service are implemented using Vertical Slice Architecture to demonstrate feature-oriented organization and high cohesion.

Both approaches follow the same Domain-Driven Design principles and share a common architectural foundation including CQRS, Domain Events, Outbox Pattern, and Event-Driven Communication.

---

## AP-001 Domain-Driven Design (DDD)

The system shall be designed using Domain-Driven Design.

Business capabilities shall be modeled as bounded contexts.

Each bounded context shall own its domain model and business rules.

---

## AP-002 Microservices Architecture

Each bounded context shall be implemented as an independent microservice.

Each microservice shall be independently deployable, scalable, and maintainable.

---

## AP-003 Clean Architecture

Selected microservices shall be implemented using the principles of Clean Architecture.

Business rules and domain logic shall remain independent of infrastructure, presentation, and external frameworks.

Dependencies shall always point toward the domain layer.

---

## AP-003 Vertical Slice Architecture

Selected microservices shall organize application features using the Vertical Slice Architecture.

Each feature shall encapsulate its commands, queries, handlers, validation, endpoints, and feature-specific logic.

---

## AP-004 CQRS

Command and Query Responsibility Segregation (CQRS) shall be used to separate write operations from read operations.

Read and write models may evolve independently.

---

## AP-005 Event-Driven Architecture

Business events shall be used to communicate between bounded contexts.

Services shall remain loosely coupled through asynchronous messaging.

---

## AP-006 Outbox Pattern

The Outbox Pattern shall be used to guarantee reliable publication of integration events.

---

## AP-007 Saga Pattern

Distributed business workflows involving multiple bounded contexts shall be coordinated using the Saga Pattern.

---

## AP-008 Repository Pattern

Repositories shall abstract persistence concerns from the domain model.

---

## AP-009 Specification Pattern

Complex business queries may be implemented using the Specification Pattern where appropriate.

---

## AP-010 Result Pattern

Application operations shall return explicit success or failure results instead of using exceptions for business validation.

---

## AP-011 Domain Events

Business state changes within a bounded context shall be represented using domain events.

---

## AP-012 Integration Events

Communication between bounded contexts shall occur through integration events.

---

## AP-013 API Gateway Pattern

External clients shall access business services through a single API Gateway.

---

## AP-014 Database per Service

Each microservice shall own an independent database.

Shared databases between services shall not be permitted.

---

## AP-015 Eventual Consistency

Consistency across bounded contexts shall be achieved using asynchronous messaging and domain events rather than distributed transactions.