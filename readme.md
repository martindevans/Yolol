## Usage

Running the application requires that you have `dotnet` installed. Run the application with:

```
dotnet run -- --code path_to_my_code_file.lol
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
 - `A %= B`
 - `A ^ B`
 - `A % B`
 - `ABS A`
 - `A!`
 - `SQRT A`
 - `SIN A`
 - `COS A`
 - `TAN A`
 - `ARCSIN A`
 - `ARCCOS A`
 - `ARCTAN A`

## Help, Something Broke!

Open an issue. Even better fix the issue yourself and submit a pull request.