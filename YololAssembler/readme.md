# YololAssembler

YololAssembler is an assembler for a language called `yasm` which compiles into Yolol. `Yasm` is designed to produce code that is **as compact** as the best hand-written Yolol and is always compatible with in-game Yolol.

To run YololEmulator:
 - Download the [latest release](https://github.com/martindevans/Yolol/releases)
 - Extract the zip to a folder
 - Open a terminal in the extracted folder
 - Run YololAssembler, this will list the parameters you should pass

For example, to run a file called `example.yasm` into `example.yolol`:

```
./YololEmulator.exe --input example.yasm --output example.yolol --watch
```

This will immediately compile `example.yasm` into `example.yolol`. The `watch` parameter causes the compiler to watch the input file - as soon as it is changed the compiler will immediately recompile the code. It is often useful to have the input file and the output file open side-by-side in your editor, so you can watch the Yolol output as you write your yasm code.

# YASM

YASM is a macro language - the compiler simply substitutes blocks of text in place of other blocks of text. This simple approach ensures that YASM is always compatible with the game (the Yolol is never parsed or validated) and that the language is very simple to understand (because everything is based on simple substitution).

There are example scripts [here](Scripts). These scripts have been written as competition entries for the `Referee` Yolol code golf challenges where every space and every tick is important for scoring so they are written to be as compact as possible!

## Comments

A Comment is started with the `##` character, all text after the comment marker will be completely removed from the compiled output. All of the whitespace before the comment marker will also be removed.

```yasm
a=1 ## All of this text is a comment
```

## Line Labels

YASM code is written line-by-line and there is no automatic rearranging of code from one line to another, this leaves exact control of code flow and timing in the hands of the programmer. All yasm code must be defined within a line label block, any code outside of a block is ignored. A new line is defined with the `@` sigil before the name of the line:

```yasm
@setup:             ## define a line called `setup`
    a=1             ## Put some code into this line

@loop:              ## define a line called `loop`
    a++             ## Put some code on this line
    if a>100 then
        goto exit   ## `exit` is a macro that is replaced with the correct line number
    end
    goto @          ## `@` is a macro that is replaced with the current line number

@exit:
    :done=1
```

A line label can be blank, this can be useful for lines which you never need to jump to (such as one off setup) or for lines which simply jump to themselves.

```yasm
@:
    a=1
    
@:
    a++
    goto @
```

## Defines

The most important construct of YASM is defining new macros, this is done with the `#define` keyword. A macro defines two parts, the name of the macro (which is what gets replaced) and the body of the macro (which is what replaces it).

```yasm
#define LongMacroName a     // Replaces "LongMacroName" with "a"

@line1:
    LongMacroName=1         // Compiles to "a=1"
```

More complex _function-like_ macros can define parameters which are used when substituting the body of the macro.

```yasm
#define functionname(arg1, arg2) arg1+=arg2     // Replaces "functionname(a, b)" with "arg1+=arg2"

@line1:
    functionname(x, 2)                          // Compiles to "x+=2"
```

function-like macros can have a body containing multiple lines of code by enclosing the body with braces:

```yasm
#define longfunc(arg1, arg2, arg3) {
    arg1+=arg2;
    arg1/=arg3
}

@line1:
    longfunc(a, b, c)                   // Compiles to "a+=b a/=c"
```

## Imports

A YASM program can import other YASM files, this allows libraries of re-usable code to be written and imported into multiple projects. There is a standard library of generic macros available [here](Scripts/lib.yasm) (lib.yasm). When a file is imported everything defined in the file is available for use in the file it was imported into.

```yasm
#import lib.yasm        // Import lib.yasm

@line1:
    continue_if(1==2)   // Use a macro defined in lib.yasm
```

## Spaces

Control over the layout of code is extremely important for compact Yolol programs. Spaces and tabs at the _start_ of the line are stripped to allow for indentation, but all (see comment section below for the one exception) other spaces and tabs are preserved so that the compiled Yolol can be spaced out as necessary.

```yasm
@test:
    a=1 
    ## The previous line ends with a space, but you can't see it!    
    b=2
```

Compiles to:

```yolol
a=1 b=2
```

While you _can_ use invisible spaces at the end of your lines of code it's not advisable! YASM replaces semicolons (`;`) with an empty space so that code can be properly spaced out.

```yasm
@test:
    a=1;
    b=2
```

```yolol
a=1 b=2
```

## Braces

Because control over spacing is extremely important for Yolol sometimes YASM code needs to be written without spaces, which looks ugly:

```
@line10:
    gotolinelabel6
```

Braces `{` and `}` are removed from the compiled output of the program. This allows you to separate blocks of code.

```
@line10:
    goto{linelabel6}
```

Braces are removed from the output _after_ macros are substituted, this can also be used to disambiguate macro uses. For example if you have three macros `foo`, `bar` and `foobar` defined, the following code is ambiguous:

```
#define foo 1
#define bar 2
#define foobar 3

@:
    x=foobar
```

This could match `foo` and `bar`, or it could match `foobar`. To fix this:

```
@:
    x={foo}{bar}
```