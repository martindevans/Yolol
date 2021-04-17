## YololEmulator

YololEmulator is an interpreter and debugger for Yolol code.

To run YololEmulator:
 - Download the [latest release](https://github.com/martindevans/Yolol/releases)
 - Extract the zip to a folder
 - Open a terminal in the extracted folder
 - Run YololEmulator, this will list the parameters you should pass

For example, to run a file called `example.yolol`:

```
./YololEmulator.exe --input example.yolol
```

The emulator will run the Yolol file one line at a time. Each time it will display the line of code and the state of the program:

```
[03] s = s-("B"+i)+("B"+i)
State:
 | s = "1B2"
 | i = 2

Press F5 to continue
```

The first line shows the line number, and the line of Yolol code. After this it shows a list of variables and their current value. Finally after the line is done it will pause until you press `F5` to proceed to the next line.

YololEmulator reads the code from the file every time a line is run, which means that you can edit a program without needing to restart the emulator.

If an external variable is read execution will pause and ask you to enter the value for the variable by hand, this allows you to mimic the functions of other devices on the network. When an external variable is written the new value will be printed.