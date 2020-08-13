f=",o00,an01,an10,an00,xo11,xo00,no01,no10,no11,nan11,e01,e10,ne00"
f+=",ne11,xno10,xno01,fals00,fals01,fals10,fals11"
s=","+(--:op)+:a+:b :o=f==(f-s) :done++ goto3

/--------//--------//--------//--------//--------//--------//--------/

t=",o01,o10,o11,an11,xo10,xo01,no00,nan00,nan01,nan10,e11,e00,ne01,"
t+="ne10,xno00,xno11,tru00,tru01,tru10,tru11"
s=","+(--:op)+:a+:b :o=t!=(t-s) :done++ goto3

/--------//--------//--------//--------//--------//--------//--------/

a=:a b=:b op=:op
if op=="or" then :o=(a or b) goto:done++ end
if op=="and" then :o=(a and b) goto:done++ end
if op=="xor" then :o=a or b and not(a and b) goto:done++ end
if op=="nor" then :o=not(a or b) goto:done++ end
if op=="nand" then :o=not(a and b) goto:done++ end
if op=="eq" then :o=a==b goto:done++ end
if op=="neq" then :o=a!=b goto:done++ end
if op=="xnor" then :o=not(a or b and not(a and b)) goto:done++ end
if op=="true" then :o=1 goto:done++ end
if op=="false" then :o=0 goto:done++ end
:o="unknown op " + op :done=1 goto12

/--------//--------//--------//--------//--------//--------//--------/

a b nor
0 0 1
0 1 0
1 0 0
1 1 0

a b nand
0 0 1
0 1 1
1 0 1
1 1 0

a b xnor
0 0 1
1 0 0
0 1 0
1 1 1