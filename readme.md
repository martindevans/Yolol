[![Build status](https://github.com/martindevans/Yolol/workflows/Build/badge.svg?branch=master)](https://github.com/martindevans/Yolol/workflows/Build/badge.svg?branch=master)
[![Test status](https://github.com/martindevans/Yolol/workflows/Test/badge.svg?branch=master)](https://github.com/martindevans/Yolol/workflows/Test/badge.svg?branch=master)

This repository contains 4 projects:

#### YololEmulator

This is an emulator for Yolol. It allows you to input a file of Yolol code and execute it line by line. See detailed documentation [here](YololEmulator).

#### YololAssembler

This is an assembler for a higher level language (yasm) which compiles into Yolol. See detailed documentation [here](YololAssembler).

#### Yolol

This is a library for interacting with Yolol code from C#. It allows you to parse a string into an AST and execute the AST.

#### Yolol.Analysis

This is a library for analysing/optimising Yolol code. It provides easy to use methods for analysing and modifying the AST of a Yolol program.
