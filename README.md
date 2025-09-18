# MonitorAndStart

MonitorAndStart is a custom Windows task scheduler built with WPF and .NET 8. It allows you to define, schedule, and manage jobs and workflows, supporting a variety of job types (scripts, services, files, etc.) and advanced scheduling options. The application uses a MySQL backend for persistent storage and leverages Dapper for data access.

## Features

- **Job Scheduling:** Create and manage jobs that can run scripts, start/stop services, or execute files.
- **Workflows:** Group jobs into workflows with custom intervals (minutes, hours, days, weeks).
- **MySQL Storage:** All jobs, workflows, and settings are stored in a MySQL database.
- **WPF UI:** Modern, user-friendly interface for managing tasks.
- **Logging:** Built-in logging for job execution and application events.
- **Update Support:** Checks for updates via a remote XML file.

## Requirements

- .NET 8 SDK
- MySQL Server (tested with MariaDB)
- Visual Studio 2022 (recommended)
- [Dapper](https://github.com/DapperLib/Dapper) and [Dapper.Contrib](https://github.com/DapperLib/Dapper) NuGet packages

## Getting Started

1. **Clone the repository:** `git clone https://github.com/miroppb/MonitorAndStart.git`

2. **Configure MySQL:**
- Create a new database (default: `monitorandstart`).
- Use the provided table schema below to create the necessary tables.

3. **Set up connection strings:**
- The connection string is managed in `Secrets.cs`. Update the credentials as needed.

4. **Build and run:**
- Open the solution in Visual Studio 2022.
- Restore NuGet packages.
- Build and run the project.

## MySQL Table Schema
```
CREATE TABLE IF NOT EXISTS `jobs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `enabled` int(1) NOT NULL DEFAULT 1,
  `pcname` text DEFAULT NULL,
  `type` int(5) NOT NULL DEFAULT 0,
  `name` text DEFAULT NULL,
  `json` longtext DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1;

CREATE TABLE IF NOT EXISTS `logs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pcname` tinytext NOT NULL,
  `datetime` datetime NOT NULL,
  `log` text NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=1;

CREATE TABLE IF NOT EXISTS `settings` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `pcname` text NOT NULL,
  `notificationengine` text NOT NULL,
  `apichannel` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1;

CREATE TABLE IF NOT EXISTS `workflows` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `pcname` text DEFAULT NULL,
  `name` text DEFAULT NULL,
  `jobIDs` text DEFAULT NULL,
  `intervalinminutes` int(10) unsigned DEFAULT NULL,
  `selectedinterval` int(5) unsigned DEFAULT NULL,
  `nexttimetorun` datetime DEFAULT NULL,
  `lastrun` datetime DEFAULT NULL,
  `runonstart` int(1) DEFAULT NULL,
  `enabled` int(1) DEFAULT NULL,
  `notify` int(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1;
```


## Project Structure

- `MonitorAndStart.v2\Models\` — Core data models (Job, Workflow, Script, etc.)
- `MonitorAndStart.v2\Data\` — Data access layer (Dapper-based)
- `MonitorAndStart.v2\ViewModel\` — ViewModels for MVVM pattern
- `MonitorAndStart.v2\Helpers\` — Utility and helper classes
- `MonitorAndStart.v2\Secrets.cs` — MySQL connection configuration

## Security

## License

This project is licensed under the MIT License.
[MIT](https://choosealicense.com/licenses/mit/)