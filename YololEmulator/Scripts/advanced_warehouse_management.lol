r=:r i=:i:o=""t=0n=0j=0ifi<0thengoto4end a=","i-=a i-=a i-=a i-=a i-=a
c=i---i n=5*(c>4)n+=2*(c>n+1)n+=c>n n+=c>n c=i---i x/=c<=9n+=10c=i---i
j=0w+=c n--ifn>1thenw+=c+c n-=2endgoto4-(n>0)-(n<1andi>0)*2
c=r---r n=5*(c>4)n+=2*(c>n+1)n+=c>n n+=c>n c=r---r x/=c<=9n+=10c=r---r
x/=n--*(w-c!=w)t++w-=c x/=n--*(w-c!=w)t++w-=c goto5+(n<2)
ifn>0thent+=w-c!=w w-=c end :o=c+t+","+:o:o-=c+0+"," t=0r--goto4
:o-=a goto:done++


// Attempted to chain another conditional jump to go straight to the output line if request=="". Insufficient space on line 3.
goto4-(n>0)-(n<1andi>0)*2
goto4-(n>0)-(n<1andi>0)*2+(n<1andi<1andr<0)*3
goto(n<1)*(3*(i<1)*(r<0)-2*(i>0))+4-(n>0)

a=n>0
b=1-a
c=i>0
d=1-c
e=r<0
g = 4 - a - (1-a)*c*2 + (1-a)*(1-c)*e*3


// Attempted to replace accumulation on line 3. This is worse because it only saves 3 characters but
// only adds one item instead of 2! Also leaves `n` in a potentially negative state, which would break things
// later on (request parsing relies on n=0 at the start).
ifn>1thenw+=c+c n-=2end
w+=(c+n-->0)-(c+0)-1


// Replace if with string subtraction in output building
ift>0then:o=c+t+","+:o end
:o=(c+t+","+:o)-(c+0+",")
:o=c+t+","+:o:o-=c+0+","


// Break out of checking warehouse loop line line 5 when there is just one thing remaining to be found
// Find that thing at the start of the next line instead
if n>0 then t+=w-c!=w w-=c end


// Fit two iterations of adding to the warehouse on a single line
j=0 w+=c
if --n>0 then goto3 end
if i>0 then i-- goto2 end

goto4-(--n>0)-(n<1andi>0)*2
if n>0then w+=c n-- end


// Replace general number parser with special case parser for values < 20
c=i---i d=5*(c>4)/(c<=9)d+=2*(c>d+1)d+=c>d d+=c>d n+=d*10^j++ goto2
c=i---i n=5*(c>4)n+=2*(c>n+1)n+=c>n n+=c>n c=i---i x/=c<=9n+=10c=i---i

/--------//--------//--------//--------//--------//--------//--------/

r=:r i=:i :o="" n=0j=0 ifi<0thengoto4end
c=i---i d=5*(c>4)/(c<=9)d+=2*(c>d+1)d+=c>d d+=c>d n+=d*10^j++ goto2
w+=c w-=0 if --n>0 then goto3 end n=0j=0 if i>0 then i-- goto2 end
t=0c=r---r d=5*(c>4)/(c<=9)d+=2*(c>d+1)d+=c>d d+=c>d n+=d*10^j++ goto4
x/=n--*(w-c!=w)t++w-=c x/=n--*(w-c!=w)t++w-=c goto5+(n<1)
if t>0then :o=c+t+","+:o end t=0n=0j=0 if r>0then r-- goto4 end
if:o>0then:o-- end goto:done++

/--------//--------//--------//--------//--------//--------//--------/





_b__efgh_jklm_opq_s_uv_xyz

a : ","
c : used in parsing (line 2/4)
d : used in parsing (line 2/4)
i : input to be parsed
n : parser output (line 2/4)
r : request
t : accumulator for how many of a request have been found (line 5)
w : contents of warehouse (single letter per item)

