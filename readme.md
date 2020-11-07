[![Build status](https://github.com/martindevans/Yolol/workflows/Build/badge.svg?branch=master)](https://github.com/martindevans/Yolol/workflows/Build/badge.svg?branch=master)
[![Test status](https://github.com/martindevans/Yolol/workflows/Test/badge.svg?branch=master)](https://github.com/martindevans/Yolol/workflows/Test/badge.svg?branch=master)

This repository contains 4 projects:

#### YololEmulator

This is a commandline emulator for Yolol. It allows you to input a file of Yolol code and execute it line by line.

#### YololAssembler

This is a commandline assembler for a higher level language (yasm) which compiles into Yolol.

#### Yolol

This is a library for interacting with Yolol code from C#. It allows you to parse a string into an AST and execute the AST.

#### Yolol.Analysis

This ia a library for analysing/optimising Yolol code. It provides easy to use methods for analysing and modifying the AST of a Yolol program.

## YololEmulator Usage

Running the application requires that you have `dotnet` installed. Run the application with:

```
dotnet run -- --input path_to_my_code_file.lol
```

The emulator runs the yolol file one line at a time:

```
[03] s = s-("B"+i)+("B"+i)
State:
 | s = "1B2"
 | i = 2

Press F5 to continue
```

The first line shows the line number, and the line of Yolol code. If this line of code is invalid execution will pause, allowing you to modify the file before retrying the line. After this it shows the machine state - a list of variables and their value. Finally after the line is done it will pause until you press `F5` to proceed to the next line.

If an external variable is read execution will pause and ask you to enter the value for the variable by hand, this allows you to mimic the functions of other devices on the network. When an external variable is written the new value will be printed.

## Missing Language Features

These language features are currently not implemented:
 - Factorial: `A!`

## Help, Something Broke!

Open an issue. Even better fix the issue yourself and submit a pull request.
