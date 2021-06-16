# YASM quick reference

## Lines
```
@line-label:
	## code goes here. "##" is a comment.
```
- `@:` unlabeled lines also work, if you never need to jump to them!
- `@` alone is replaced with the current line. `goto@`

## Spacing

- Spaces / tabs in front of a line are ignored.
- Newlines are ignored. Empty or comment lines are ignored.
- ; is replaced with a space: meant to replace spaces at the end of a line.
- Spaces before a comment are ignored. *(**`a=1  ##`** won't add a space)*
- {} are removed after macro replacement. `goto{line} == gotoline`

## Defines

Defines let you create macros.
```
#define LongMacroName a
## LongMacroName=1    => a=1

#define functionname(arg1, arg2) arg1+=arg2
## functionname(x, 2) => x+=2

#define longfunc(arg1, arg2, arg3) {
    arg1+=arg2;
    arg1/=arg3
}
## longfunc(a, b, c)  => a+=b a/=c
```

## Imports

Imports let you take the **#define**s of another file and use them.

This doesn't just copy paste *everything*: YOLOL lines are ignored.

```
#import lib.yasm

@:
	continue_if(1==2)
```