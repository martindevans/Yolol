[![Build Status](https://travis-ci.org/martindevans/Yolol.svg?branch=master)](https://travis-ci.org/martindevans/Yolol)

This repository contains 4 projects:

#### YololEmulator

This is a commandline emulator for Yolol. It allows you to input a file of Yolol code and execute it line by line.

#### Yololc

This is an optimiser for Yolol. It allows you to input a file of Yolol and it will output a file of better (i.e. shorter) Yolol.

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

## Multi Device networks

It is possible to run multiple instances of the program at once, all affecting the same external variables.

First start the host:

```
dotnet run -- --input path_to_my_host_code_file.lol --host port_number
```

Now start as many clients as you wish:

```
dotnet run -- --input path_to_my_client_code_file.lol --client "http://localhost:port"
```

Make sure to use the correct url for the clients to connect (same port number).

You can also view the complete state of the device network by visiting `http://localhost:port` in your browser, this will dump the value of all external variables.

## Missing Language Features

These language features are currently not implemented:
 - `A!`
 
 Additionally while function calls (e.g. `ABS A`) do exist, they require brackets around the argument (i.e. `ABS(A)`)

## Help, Something Broke!

Open an issue. Even better fix the issue yourself and submit a pull request.
