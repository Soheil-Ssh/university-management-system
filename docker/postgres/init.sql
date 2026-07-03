-- File service database
CREATE USER file_user WITH PASSWORD 'Password@123';
CREATE DATABASE file_db OWNER file_user;

-- Student service database
CREATE USER student_user WITH PASSWORD 'Password@123';
CREATE DATABASE student_db OWNER student_user;