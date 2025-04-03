@echo off
start cmd /c "dotnet add package Serilog"
start cmd /c "dotnet add package Serilog.Sinks.Console"
start cmd /c "dotnet add package Serilog.Sinks.File"
start cmd /c "dotnet add package Serilog.Settings.Configuration"
start cmd /c "dotnet add package Npgsql"
start cmd /c "dotnet add package BCrypt.Net-Next"
echo Установка пакетов запущена в параллельных терминалах!
