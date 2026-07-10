-- File service database
CREATE USER file_user WITH PASSWORD 'Password@123';
CREATE DATABASE file_db OWNER file_user;

-- Student service database
CREATE USER student_user WITH PASSWORD 'Password@123';
CREATE DATABASE student_db OWNER student_user;

-- Central Organization service database
CREATE USER central_organization_user WITH PASSWORD 'Password@123';
CREATE DATABASE central_organization_db OWNER central_organization_user;

-- Notification service database
CREATE USER notification_user WITH PASSWORD 'Password@123';
CREATE DATABASE notification_db OWNER notification_user;