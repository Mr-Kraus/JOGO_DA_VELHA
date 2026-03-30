@echo off
setlocal enabledelayedexpansion

echo ================================
echo   SETUP DEV - GCC + .NET
echo ================================
echo.

:: -------- CONFIG --------
set MSYS_PATH=C:\msys64
set MINGW_BIN=%MSYS_PATH%\mingw64\bin
set LOG=setup_log.txt

echo Iniciando log > %LOG%

:: -------- FUNÇÃO: ADD PATH SEM DUPLICAR --------
set "CURRENT_PATH=%PATH%"
echo %CURRENT_PATH% | find /I "%MINGW_BIN%" >nul
if %errorlevel% neq 0 (
    echo Adicionando GCC ao PATH...
    setx PATH "%PATH%;%MINGW_BIN%" >> %LOG%
) else (
    echo PATH ja contem GCC.
)

:: -------- GCC --------
echo.
echo Verificando GCC...
gcc --version >nul 2>&1

if %errorlevel% neq 0 (
    echo GCC nao encontrado. Instalando MSYS2...

    winget install -e --id MSYS2.MSYS2 >> %LOG%

    echo Atualizando MSYS2...
    %MSYS_PATH%\usr\bin\bash -lc "pacman -Syu --noconfirm" >> %LOG%
    %MSYS_PATH%\usr\bin\bash -lc "pacman -Su --noconfirm" >> %LOG%

    echo Instalando GCC...
    %MSYS_PATH%\usr\bin\bash -lc "pacman -S --noconfirm mingw-w64-x86_64-gcc" >> %LOG%

    echo GCC instalado.
) else (
    echo GCC ja instalado.
)

:: -------- .NET SDK --------
echo.
echo Verificando .NET SDK...
dotnet --version >nul 2>&1

if %errorlevel% neq 0 (
    echo Instalando .NET SDK...
    winget install Microsoft.DotNet.SDK.8 >> %LOG%
) else (
    echo .NET SDK ja instalado.
)

:: -------- .NET Desktop Runtime --------
echo.
echo Verificando Desktop Runtime...
dotnet --list-runtimes | findstr WindowsDesktop >nul

if %errorlevel% neq 0 (
    echo Instalando Desktop Runtime...
    winget install Microsoft.DotNet.DesktopRuntime.8 >> %LOG%
) else (
    echo Desktop Runtime ja instalado.
)

:: -------- FINAL --------
echo.
echo ================================
echo   FINALIZADO
echo ================================
echo.

echo IMPORTANTE:
echo - Reinicie o CMD para atualizar o PATH
echo - Log salvo em %LOG%

pause