a=3 goto:(2+3)


b=""c=""v="21 "i=:i:o=""a=--i+b x/=i>0a+=--i x/=i>0a+=--i x/=i>0a+=--i
r="32 "x/=i>0a+=--i x/=i>0a+=--i x/=i>0a+=--i x/=i>0a+=--i goto2
s="31 "u=2^:i-1 if:i%2==0then x=13y=10z=17goto17end x=10y=17z=13goto13
:o--goto:done++





t=0d=b---b t=1e=c---c t=2ifd<e thenc+=e+d:o+="23 "else b+=d+e:o+=r end
u--ift>1thengoto4+(u>0)*(z-4)end:o+=r b+=c---c gotoz

t=0d=a---a t=1e=c---c t=2ifd<e thenc+=e+d:o+="13 "else a+=d+e:o+=s end
u--ift>1thengoto4+(u>0)*(y-4)end w/=t<1:o+=s a+=c---c gotoy
:o+="13 "c+=d goto4+(u>0)*(y-4)

t=0d=a---a t=1e=b---b t=2ifd<e thenb+=e+d:o+="12 "else a+=d+e:o+=v end
u--ift<2thenift<1then:o+=v a+=b---b else:o+="12 "b+=d endendgotox

/--------//--------//--------//--------//--------//--------//--------/







// r: "32 "
// s: "31 "
// t: indicates success of part 1 of swap
// u: countdown to done (starts at 2^i-1)
// v: "21 "
// w: uninitialised
// x: where to go after AB
// y: where to go after AC
// z: where to go after BC

// d: character from left pin
// e: character from right pin

// Line10: BC/CB
// Line13: AC/CA
// Line17: AB/BA





