x=:a:o=""y=:b a=""b=""u=0d=0c=0g=0a+=x---x a+=x---x goto2+(:t=="T")
b+=y---y b+=y---y b+=y---y b+=y---y b+=y---y b+=y---y b+=y---y goto2
a+=x---x a+=x---x a+=x---x a+=x---x a+=x---x a+=x---x a+=x---x goto3
goto 6+((:t>"D")+(:t>"K")+(:t>"SR"))*4

p=c c=a---a d=b---b>0 r=p<c u=r*d+(1-r)*u:o+=u :done=a<0goto6-:done*9
goto:done++


c=a---a>0 d=b---b>0 u=(c+d==2)*(1-u)+(c+d==1)*c+(c+d==0)*u:o+=u goto10
goto:done++


c=a---a>0u=(b---b>0<1)*(c+u-c*u):o+=u:done=a<0goto14-:done*99
goto:done++


c=a---a u=c>g!=u:o+=u d=a---a u=d>c!=u:o+=u e=a---a u=e>d!=u:o+=u
f=a---a u=f>e!=u:o+=u g=a---a u=g>f!=u:o+=u goto18
goto:done++




/--------//--------//--------//--------//--------//--------//--------/
a=:a:o=""b=:b c=0u=0e=0z="x0"y="x1"j=4+((:t>"D")+(:t>"K")+(:t>"SR"))*2
e=c c=a<1<1r=e<c a="x"+a a-=z a-=y d=b<1<1b="x"+b b-=z b-=y gotoj+c

:o+=u :done=a<0 goto2-:done                      D(00)(01)
u=r*d+(1-r)*u :o+=u :done=a<0 goto2-:done		 D(10)(11)
u=(1-d)*u :o+=u :done=a<0 goto2-:done           JK(00)(01)
u=(1-d)+d*(1-u) :o+=u :done=a<0 goto2-:done     JK(10)(11)
u=(1-d)*u :o+=u :done=a<0 goto2-:done           SR(00)(01)
u=1-d :o+=u :done=a<0 goto2-:done               SR(10)(11)
:o+=u :done=a<0 goto2-:done                      T(00)(01)
u=r*(1-u)+(1-r)*u :o+=u :done=a<0 goto2-:done    T(10)(11)




# D(00)(01) T(00)(01)
:o+=u

# JK(00)(01) SR(00)(01)
u=(1-d)*u :o+=u

# D(10)(11)
u=r*d+(1-r)*u :o+=u

# JK(10)(11)
u=(1-d)+d*(1-u) :o+=u

# SR(10)(11)
u=1-d :o+=u

# T(10)(11)
u=r*(1-u)+(1-r)*u :o+=u

/--------//--------//--------//--------//--------//--------//--------/
a=:a:o=""b=:b c=0u=0e=0z="x0"y="x1"j=4+((:t>"D")+(:t>"K")+(:t>"SR"))*2
e=c c=a<1<1r=e<c a="x"+a a-=z a-=y d=b<1<1b="x"+b b-=z b-=y gotoj+c

:o+=u :done=a<0 goto2-:done
u=r*d+(1-r)*u :o+=u :done=a<0 goto2-:done
u=(1-d)*u :o+=u :done=a<0 goto2-:done
u=(1-d)+d*(1-u) :o+=u :done=a<0 goto2-:done
u=(1-d)*u :o+=u :done=a<0 goto2-:done
u=1-d :o+=u :done=a<0 goto2-:done
:o+=u :done=a<0 goto2-:done
u=r*(1-u)+(1-r)*u :o+=u :done=a<0 goto2-:done

/--------//--------//--------//--------//--------//--------//--------/
a=:a:o=""b=:b c=0u=0e=0z="x0"y="x1"j=4+((:t>"D")+(:t>"K")+(:t>"SR"))*4
e=c c=a<1<1 a="x"+a a-=z a-=y d=b<1<1 b="x"+b b-=z b-=y gotoj+c+d*2

:o+=u :done=a<0 goto2-:done
u+=(e<c)*(d-u) :o+=u :done=a<0 goto2-:done
:o+=u :done=a<0 goto2-:done
u+=(e<c)*(d-u) :o+=u :done=a<0 goto2-:done
:o+=u :done=a<0 goto2-:done
u=1 :o+=u :done=a<0 goto2-:done
u=0 :o+=u :done=a<0 goto2-:done
u=1-u :o+=u :done=a<0 goto2-:done
:o+=u :done=a<0 goto2-:done
u=1 :o+=u :done=a<0 goto2-:done
u=0 :o+=u :done=a<0 goto2-:done
u=0 :o+=u :done=a<0 goto2-:done
u+=(e<c)*(1-u*2) :o+=u :done=a<0 goto2-:done
u+=(e<c)*(1-u*2) :o+=u :done=a<0 goto2-:done
u+=(e<c)*(1-u*2) :o+=u :done=a<0 goto2-:done
u+=(e<c)*(1-u*2) :o+=u :done=a<0 goto2-:done
/--------//--------//--------//--------//--------//--------//--------/

a=input
b=input
u=current_state
e=prev_a
z="x0"
y="x1"
c=single value from a
d=single value from b
r=indicates rising edge

# D (Rising Edge)
A  B O
~R 0 SAME
~R 1 SAME
R  0 0
R  1 1

# JK
A B O
0 0 SAME
1 0 1
0 1 0
1 1 TOGGLE

# SR
A B O
0 0 SAME
1 0 1
0 1 0
1 1 0

# T (Rising Edge)
A  O
~R SAME
R  TOGGLE