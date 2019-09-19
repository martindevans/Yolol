i=:read_string_to_parse_to_number o=0 n=1 goto(i=="")*18+(i!="")*2		// Input, setup, check for empty string
a=i---i goto(a=="1")*2+(a=="2")*3+(a=="3")*4+(a=="4")*5+(a=="5")*6+4	// Consume 1 character of input, jump to characters [12345], fall through due to runtime error is string is empty
:result=o goto 20														// SUCCESS, result is stored in `o`
goto(a=="6")*6+(a=="7")*7+(a=="8")*8+(a=="9")*9+(a=="0")*10+5			// Jump to characters [67890], jump to next line for unknown char
goto 19																	// FAILURE, unknown char (stored in `a`)
o+=1*n n*=10 goto 2
o+=2*n n*=10 goto 2
o+=3*n n*=10 goto 2
o+=4*n n*=10 goto 2
o+=5*n n*=10 goto 2
o+=6*n n*=10 goto 2
o+=7*n n*=10 goto 2
o+=8*n n*=10 goto 2
o+=9*n n*=10 goto 2
o+=0*n n+=10 goto 2


failed="true" empty_input="true" goto 18								// Infinite loop for demo purposes
failed="true" unknown_char=a goto 19									// Infinite loop for demo purposes
failed="false" goto 20													// Infinite loop for demo purposes