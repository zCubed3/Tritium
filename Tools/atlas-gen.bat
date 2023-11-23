@echo off
set /p "font=Font Path: "
.\msdf-atlas-gen.exe -font %font% -charset tritium-charset.txt -type mtsdf -format png -imageout Roboto.png -json Roboto.json -errorcorrection auto-full -yorigin top