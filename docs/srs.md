# Software Requirements Specification (SRS)

## AcademicHub

### Version 1.0

---

# 1. Introduction

## 1.1 Purpose

AcademicHub is a distributed university management system designed for undergraduate education management.

The system supports student admission, academic management, faculty management, course enrollment, assessment management and graduation workflows.

The system is designed using Domain-Driven Design (DDD), CQRS, Event-Driven Architecture and Microservices Architecture principles.

---

# 1.2 Scope

The system shall provide:

* Online student registration
* Student admission management
* Student number generation
* Faculty and department management
* Academic structure management
* Curriculum management
* Semester management
* Course offering management
* Enrollment management
* Grade management
* Grade appeal management
* Graduation request management

The system is intended exclusively for undergraduate students.

---

# 2. User Roles

## 2.1 University President

Responsible for university-wide administration.

## 2.2 Education Manager

Responsible for educational operations and student admissions.

## 2.3 Department Manager

Responsible for department-level academic management.

## 2.4 Department Expert

Responsible for operational academic processes.

## 2.5 Professor

Responsible for teaching and grade submission.

## 2.6 Student

Responsible for enrollment, academic activities and graduation requests.

---

# 3. System Architecture

The system shall be implemented as a Microservices Architecture.

## Services

### Identity Service

Responsibilities:

* Authentication
* Authorization
* User Management
* Role Management
* Permission Management

### Student Service

Responsibilities:

* Admission Requests
* Student Lifecycle Management
* Student Number Generation
* Graduation Requests

### Faculty Service

Responsibilities:

* Faculty Management
* Department Management
* Professor Management

### Academic Service

Responsibilities:

* Major Management
* Curriculum Management
* Course Management
* Semester Management
* Academic Calendar Management
* Course Offering Management
* Enrollment Management

### Assessment Service

Responsibilities:

* Grade Management
* Transcript Management
* Grade Appeal Management

---

# 4. Functional Requirements

## FR-001 Student Registration

Students shall be able to submit online registration requests.

## FR-002 Admission Review

Education managers shall be able to review registration requests.

## FR-003 Admission Approval

Education managers shall be able to approve or reject admission requests.

## FR-004 Student Number Generation

The system shall generate a unique student number after admission approval.

## FR-005 Faculty Creation

Authorized users shall be able to create faculties and departments.

## FR-006 Professor Management

Authorized users shall be able to create and manage professor records.

## FR-007 Major Management

Authorized users shall be able to create majors.

## FR-008 Curriculum Management

Authorized users shall be able to define curricula for each major.

## FR-009 Course Management

Authorized users shall be able to create and manage courses.

## FR-010 Semester Management

Authorized users shall be able to create academic semesters.

## FR-011 Academic Calendar Management

Authorized users shall be able to define:

* Enrollment period
* Add/Drop period
* Examination period

for each semester.

## FR-012 Academic Rules Management

Authorized users shall be able to configure:

* Maximum enrollment units
* Minimum enrollment units
* Add/Drop limits

for each semester.

## FR-013 Course Offering

Authorized users shall be able to offer courses for a semester.

## FR-014 Enrollment Submission

Students shall be able to submit enrollment requests.

## FR-015 Enrollment Validation

The system shall validate:

* Enrollment period
* Course capacity
* Curriculum rules
* Prerequisite requirements
* Unit limits

before enrollment approval.

## FR-016 Enrollment Approval

Authorized users shall be able to approve enrollment requests.

## FR-017 Grade Submission

Professors shall be able to submit grades during the grade submission period.

## FR-018 Transcript Access

Students shall be able to view their academic transcript.

## FR-019 Grade Appeal

Students shall be able to submit grade appeals.

## FR-020 Appeal Review

Professors shall be able to review and resolve grade appeals.

## FR-021 Graduation Request

Students shall be able to submit graduation requests.

## FR-022 Graduation Validation

The system shall validate:

* Required credits completed
* Curriculum completion
* No unresolved academic issues
* No open grade appeals

before approving graduation.

---

# 5. Business Rules

## BR-001

A student cannot enroll outside the enrollment period.

## BR-002

A student cannot exceed the maximum allowed units.

## BR-003

Prerequisite courses must be completed before enrollment.

## BR-004

Only assigned professors may submit grades.

## BR-005

Grades may only be submitted during the active grading period.

## BR-006

Graduation requests require successful completion of all curriculum requirements.

---

# 6. Non-Functional Requirements

## NFR-001 Availability

Services shall be independently deployable.

## NFR-002 Scalability

Services shall support horizontal scaling.

## NFR-003 Observability

The system shall support:

* Distributed Tracing
* Centralized Logging
* Metrics Collection

## NFR-004 Security

Authentication shall be JWT-based.

Authorization shall be role-based.

## NFR-005 Reliability

Inter-service communication shall support eventual consistency through messaging.

---

# 7. Integration Requirements

## Messaging

RabbitMQ shall be used for asynchronous communication.

## Caching

Redis shall be used for distributed caching.

## Persistence

SQL Server shall be used as the primary data store.

## Gateway

YARP shall be used as API Gateway.

---

# 8. Architectural Patterns

* Domain-Driven Design (DDD)
* CQRS
* Vertical Slice Architecture
* Event-Driven Architecture
* Outbox Pattern
* Saga Pattern
* Clean Boundaries
