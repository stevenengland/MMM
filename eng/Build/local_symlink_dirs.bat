@echo off
REM This is only for local tests. 
REM This batch creates symbolic links that point to the MT4/5 MQL folder. It's purpose is that you can distribute files build with VS to the folder and they will also be versioned.
REM To use it on your machine, you have to create a file named .local.config.txt that declares the variables %MT4MQLFolder% / %MT5MQLFolder%
REM Example: 
REM		MT4MQLFolder="C:\Users\user\AppData\Roaming\MetaTrader\MQL5\"
REM		MT5MQLFolder="C:\Users\user\AppData\Roaming\MetaTrader 5\MQL5\"

for /f "delims=" %%x in (.local.config.txt) do (set "%%x")

mklink /J "D:\coding\MMM\doc\module_usage\implementation\metatrader5\MQL5" %MT5MQLFolder%