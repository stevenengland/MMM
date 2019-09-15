@echo off
REM This is only for local tests. 
REM This batch distributes the dll files to the MataTrader folders where they can be tested.
REM To use it on your machine, you have to execute the symlink batch file first

xcopy /Y "%~dp0..\..\src\StEn.MMM\Mql.Telegram\bin\x64\Release\netstandard2.0\Mql_Telegram.dll" "%~dp0..\..\doc\module_usage\implementation\metatrader5\MQL5\Libraries\"
xcopy /Y "%~dp0..\..\src\StEn.MMM\Mql.Mail\bin\x64\Release\netstandard2.0\Mql_Mail.dll" "%~dp0..\..\doc\module_usage\implementation\metatrader5\MQL5\Libraries\"