CREATE DATABASE IF NOT EXISTS rentify_db;
DROP USER IF EXISTS 'alex'@'localhost';
CREATE USER 'alex'@'localhost' IDENTIFIED BY 'Vichet@077';
GRANT ALL PRIVILEGES ON rentify_db.* TO 'alex'@'localhost';
FLUSH PRIVILEGES;
