left = "" head=" " right="" lcount=0 rcount=0 op=0													// These values should all be stored on a memory chip for persistence of the tape
op = :op goto 2 + op * 2

:value = head :op=-1																				// op=1 Read, set :op to -1 to indicate completion
if :op != 0 then goto 5 end	goto 2																	// Block until op flag is cleared
head = :value :op=-1																				// op=2 Write, set :op to -1 to indicate completion
if :op != 0 then goto 7 end goto 2																	// Block until op flag is cleared
left+=head lcount++ t=right if rcount > 0 then head=t-(--right) rcount-- else head=" " end :op=-1	// op=3 Move right, set :op to -1 to indicate completion
if :op != 0 then goto 9 end goto 2																	// Block until op flag is cleared
right+=head rcount++ t=left if lcount > 0 then head=t-(--left) lcount-- else head=" " end :op=-1	// op=4 Move left, set :op to -1 to indicate completion
if :op != 0 then goto 11 end goto 2																	// Block until op flag is cleared