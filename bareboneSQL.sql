USE [master]
GO

/****** Object:  Database [wasteapp]    Script Date: 18/01/2023 09:21:13 ******/
CREATE DATABASE [wasteapp]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'wasteapp', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\wasteapp.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'wasteapp_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\wasteapp_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [wasteapp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [wasteapp] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [wasteapp] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [wasteapp] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [wasteapp] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [wasteapp] SET ARITHABORT OFF 
GO

ALTER DATABASE [wasteapp] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [wasteapp] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [wasteapp] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [wasteapp] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [wasteapp] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [wasteapp] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [wasteapp] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [wasteapp] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [wasteapp] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [wasteapp] SET  DISABLE_BROKER 
GO

ALTER DATABASE [wasteapp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [wasteapp] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [wasteapp] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [wasteapp] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [wasteapp] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [wasteapp] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [wasteapp] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [wasteapp] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [wasteapp] SET  MULTI_USER 
GO

ALTER DATABASE [wasteapp] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [wasteapp] SET DB_CHAINING OFF 
GO

ALTER DATABASE [wasteapp] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [wasteapp] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [wasteapp] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [wasteapp] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [wasteapp] SET QUERY_STORE = OFF
GO

ALTER DATABASE [wasteapp] SET  READ_WRITE 
GO
