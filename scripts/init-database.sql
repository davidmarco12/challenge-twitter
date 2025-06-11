USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TWITTER_DEV')
BEGIN
    CREATE DATABASE TWITTER_DEV;
    PRINT 'Database TWITTER_DEV created successfully';
END
GO

-- Cambiar a la nueva base de datos
USE TWITTER_DEV;
GO

-- Crear schema User si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'User')
BEGIN
    EXEC('CREATE SCHEMA [User]');
    PRINT 'Schema User created successfully';
END
GO

PRINT 'Database initialization completed';
GO