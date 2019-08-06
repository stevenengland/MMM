@echo off
REM This is only for local tests. 
REM This batch distributes the dll files to the MataTrader folders where they can be tested.
REM To use it on your machine, you have to create a file named .local.config.txt that declares the variables %MT4LibFolder% / %MT5LibFolder%
REM Example: 
REM		MT4LibFolder="C:\Users\user\AppData\Roaming\MetaTrader\MQL5\Libraries\"
REM		MT5LibFolder="C:\Users\user\AppData\Roaming\MetaTrader 5\MQL5\Libraries\"

for /f "delims=" %%x in (.local.config.txt) do (set "%%x")

xcopy /Y "%~dp0..\..\src\StEn.MMM\Mql.Telegram\bin\x64\Release\netstandard2.0\Mql_Telegram.dll" %MT4LibFolder%
xcopy /Y "%~dp0..\..\src\StEn.MMM\Mql.Telegram\bin\x64\Release\netstandard2.0\Mql_Telegram.dll" %MT5LibFolder%