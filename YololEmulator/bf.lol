program= "+." program_rev="" OUTPUT=""																								// BrainFuck program to execute
if program == "" then goto 12 end instr = (program - --program)																		// Convert Program to backwards BoolFuck (1 bit brainfuck, with the string reversed): http://samuelhughes.com/boof/
goto (instr==">")*4+(instr=="<")*5+(instr=="]")*6+(instr=="[")*7+(instr==".")*8+(instr==",")*9+(instr=="+")*10+(instr=="-")*11		// Jump table for BrainFuck instructions
program_rev += ">>>>>>>>>" goto 2																									// >
program_rev += "<<<<<<<<<" goto 2																									// <
program_rev += "]<+[<]>>>>>>>>>]<[<]+>[+<<<<<<<<+>>>>>>>>>" goto 2																	// ]
program_rev += "]<+[<+]>[<<<<<<<<+[>>>>>>>>>]<[<]+>[+<<<<<<<<+>>>>>>>>>" goto 2														// [
program_rev += "<<<<<<<<;>;>;>;>;>;>;>;>" goto 2																					// .
program_rev += "<<<<<<<<,>,>,>,>,>,>,>,>" goto 2																					// ,
program_rev += "<<<<<<<<<]+[>>>>>>>>>]<+[<+]>[>" goto 2																				// +
program_rev += "<<<<<<<<<]+[>>>>>>>>>]<[<]+>[+<<<<<<<<+>>>>>>>>>" goto 2															// -
left="" head="0" right="" lcount=0 rcount=0 past_program=""																			// Set up memory (values are "1" or "0" on the tape. Default value is "0")
if program_rev == "" then goto 26 end instr = (program_rev - --program_rev) past_program += instr									// fill instr with the instruction to execute
goto 15
goto (instr=="+")*16+(instr==",")*17+(instr==";")*18+(instr=="<")*19+(instr==">")*20+(instr=="[")*21+(instr=="]")*23				// Jump table for decoded instructions
if head=="0" then head="1" else head="0" end goto 13																				// `+` Flip the value of the bit under the pointer.
if :input then head=1 else head=0 end goto 13																						// `,` Read bit of input
OUTPUT+=head goto 13																												// `;` Write bit of output
right+=head rcount++ t=left if lcount > 0 then head=t-(--left) lcount-- else head="0" end goto 13									// `<` Move pointer left
left+=head lcount++ t=right if rcount > 0 then head=t-(--right) rcount-- else head="0" end goto 13									// `>` Move pointer right
if head != "0" then goto 13 end bcount=1										 													// `[` If the value under the pointer is zero, jumps forward, just past the matching ] character.
instr = (program_rev - --program_rev) past_program += instr bcount+=instr=="[" bcount-=instr=="]" if bcount == 0 then goto 13 else goto 22 end
bcount=1 past_program-- program_rev+=instr																									// `]` Jumps back to the matching [ character.
instr=past_program - --past_program program_rev+=instr bcount+=instr=="]" bcount-=instr=="[" if bcount == 0 then goto 13 else goto 24 end

PROGRAM_ENDED=1 goto 26 // end of program