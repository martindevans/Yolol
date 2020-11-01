i=:i h=i---i r=""s=0e=""
a=i---i ifa>h then s=1 r=a+r else r=h+r h=a end goto2+(i<0)
e+=h i=r:o=e+i:done=1-s h=0if i>0then h=i---i end r=""goto1+(s-->0)



/--------//--------//--------//--------//--------//--------//--------/


i=:i z=i---i y=""x=""
c=i---i d=c if c>z then d=z z=c end x+=d goto2+(i<0)
y=z+y z="" i=x :o=y :done=x<0 x="" goto 2-:done

// i: input
// z: max
// w: min
// y: output accumulator
// x: unsorted characters
// c: character

/--------//--------//--------//--------//--------//--------//--------/


i=:i z="[" y="" x=""
c=i---i if c<z then x=z+x-"[" z=c else x+=c end goto2
y+=z z="[" i=x :o=y :done=x<0 x="" goto 2-:done


/--------//--------//--------//--------//--------//--------//--------/


i=:i a=""b=""cc=""d=""e=""f=""g=""h=""ii=""j=""k=""l=""m=""n=""o=""
p=""q=""r=""s=""t=""u=""v=""w=""x=""y=""z=""
c=i---i goto5+(c>"C")+(c>"F")+(c>"I")+(c>"L")+(c>"O")+(c>"R")+(c>"U")
:o=a+b+cc+d+e+f+g+h+ii+j+k+l+m+n+o+p+q+r+s+t+u+v+w+x+y+z:done=1 goto1
a+=c b+=c cc+=c a-="B"a-="C"b-="A"b-="C"cc-="A"cc-="B"goto3
d+=c e+=c f+=c d-="E"d-="F"e-="D"e-="F"f-="D"f-="E"goto3
g+=c h+=c ii+=c g-="H"g-="I"h-="G"h-="I"ii-="G"ii-="H"goto3
j+=c k+=c l+=c j-="K"j-="L"k-="J"k-="L"l-="J"l-="K"goto3
m+=c n+=c o+=c m-="N"m-="O"n-="M"n-="O"o-="M"o-="N"goto3
p+=c q+=c r+=c p-="Q"p-="R"q-="P"q-="R"r-="P"r-="Q"goto3
s+=c t+=c u+=c s-="T"s-="U"t-="S"t-="U"u-="S"u-="T"goto3
ifc=="V"thenv+=c end ifc=="W"thenw+=c end ifc=="X"thenx+=c end
ifc=="Y"theny+=c end ifc=="Z"thenz+=c end goto3


/--------//--------//--------//--------//--------//--------//--------/

i=:i h=""r=""s=0e=""
a=i---i if(h>0)*(a>h)then t=a a=h h=t s=1end r=h+r h=a goto2+(i<0)
e+=h i=r:o=e+i:done=1-s h=""r=""goto1+(s-->0)

/--------//--------//--------//--------//--------//--------//--------/


i=:i h=""r=""s=0e=""
a=i---i if(h!="")*(a>h)then t=a a=h h=t s=1end r=h+r h=a goto2
e+=h i=r:o=e+i:done=1-s h=""r=""goto1+(s-->0)

/--------//--------//--------//--------//--------//--------//--------/


i=:i h="" r="" s=0
a=i---i if(h!="")*(a>h)then t=a a=h h=t s=1 end r=h+r h=a goto2
i=h+r :o=i :done=1-s h="" r="" goto 1+(s-->0)

/--------//--------//--------//--------//--------//--------//--------/


i=:i h="" r="" s=0
a=i---i if h!="" and a>h then t=a a=h h=t s=1 end r=h+r h=a goto 2
i=h+r h="" r="" q=s s=0 goto 4-q*2
:o=i :done=1 goto 1

/--------//--------//--------//--------//--------//--------//--------/





