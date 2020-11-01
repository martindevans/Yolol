a="*" b="" d="." p="123" x="1"y="2"z="3" s="L" i="" j=0 o="" g=15
c=:b---:b b=c if c==d or c==b then o+=d goto2 end i+=c j+=(c>"/")*(c<":") goto g
:o=o goto:done++
if c!=n then g=15 o+="B" goto2 else o+=d n=m---m end goto2
g=6 m=0 n="" goto2
o+=d n+=c m++ if m==3 then g=7 end goto2
g=15 if c!="#" then o+="B" goto2 end p=n z=n---n y=n---n x=n i="" j=0
o+="P" goto2






w/=c==a w/=s!="A" g=4 o+=d m=a+z+y n=x goto2
w/=i-p!=i w/=s=="L" s="U" o+=s i="" j=0 goto2
w/=i-p!=i w/=s=="U" s="L" o+=s i="" j=0 goto2
w/=i-p!=i w/=s=="A" s="C" o+=s i="" j=0 goto2
w/=j==9w/=s!="A" s="A" o+=s i="" j=0 goto2
o+="." goto2


/--------//--------//--------//--------//--------//--------//--------/

a : "*"
b : previous character of input
c : latest character of input
d : "."
g : line to goto after receiving a single character of input
i : unprocessed input so far
j : length of i
m : pinchange pin characters remaining to search for after current is matched / length of new pin accumulator
n : next expected pin change character / accumulator of new pin
o : output accumulator
p : current pin as a string
s : state (string)
w : 0
x : pin char 0 (string)
y : pin char 1 (string)
z : pin char 2 (string)

1) initialise
2) read characters, jump to `g` when character is not ".", fall through when input is empty
3) submit
4) check that pinchange character was the next character of pin, fall through when all characters of pin have been matched
5) fallthrough to here indicates that `*PIN*` has been detected in the input. set `g` to goto 6 for new characters
6) record 3 characters of input as the new pin into `n`
7) complete pinchange if input is "#"

15) start pin change sequence
16) unlock
17) lock
18) clear alarm
19) alarm due to lots of inputs
20) no particular state, go back to 2

 - sequence does not include pin for bong/ping