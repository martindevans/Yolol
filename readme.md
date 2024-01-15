[![Build status](https://github.com/martindevans/Yolol/workflows/Build/badge.svg?branch=master)](https://github.com/martindevans/Yolol/workflows/Build/badge.svg?branch=master)
[![Test status](https://github.com/martindevans/Yolol/workflows/Test/badge.svg?branch=master)](https://github.com/martindevans/Yolol/workflows/Test/badge.svg?branch=master)

This repository contains 4 projects:

#### YololEmulator

This is an emulator for Yolol. It allows you to input a file of Yolol code and execute it line by line. See detailed documentation [here](YololEmulator). Use an online version of the emulator [here](https://martindevans.github.io/YololBlazor/?state=AB5672CE4F4955B252CA48CDC9C9B78D512ACF2FCA4989518AC9030BE8EA2AA4E797E41BC5E4812843251DA5B0C49CD2D46225ABEA5A1DA580A2FCF4A2C45CE7FCD2BC92D422252B835A00))!

#### YololAssembler

This is an assembler for a higher level language (yasm) which compiles into Yolol. See detailed documentation [here](YololAssembler).

#### Yolol

This is a library for interacting with Yolol code from C#. It allows you to parse a string into an AST and execute the AST.

#### Yolol.Analysis

This is a library for analysing/optimising Yolol code. It provides easy to use methods for analysing and modifying the AST of a Yolol program.
