a=:a x=a/99+1-(a>999)*a/230y=x*x*x x*=(y+2*a)/(2*y+a)y=x*x*x
x*=(y+2*a)/(2*y+a)x=(a/x/x+x+x)/3:o=(a/x/x+x+x)/3goto:done++

y=x*x*x x*=(y+2*a)/(2*y+a)
y=x*x*x x*=y+2*a x/=2*y+a 

x=n*(a/(x*x)+2*x)
x=(a/x/x+x+x)/3

# 109,142 points. 10,000 ticks. Total error: 0.065. Worst Error: 0.001.

/--------//--------//--------//--------//--------//--------//--------/

a=:a x=a/10
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
x=2*x+a/x/x x/=3
:o=x goto:done++

x=(2*x+a/(x*x))/3
x=2*x+a/(x*x)x/=3

a=:a x=a/10
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
x=(2/3)*x+(1/3)*a/(x*x)
:o=x goto:done++

# https://www.mathpath.org/Algor/cuberoot/cube.root.babylon.htm
# 103,730 points. 60,000 ticks. Total error: 1550.263. Worst Error: 1.637.

/--------//--------//--------//--------//--------//--------//--------/

k=1000 d=2000 t=10 n=0.333 p=0.005 i=0.001 h=0.5 y=3.24z=3.019
a=:a x=a*0.008+1.06-(a>d)*a*p x=n*(a/(x*x)+2*x)-3l=(x-y)*t r=(x+z)*t
m=(l+r)*h c=(m*m*m)/k>=a l=l*c+m*(1-c)r+=c*(m-r)goto3+(r-l<=i)
:o=l/10 :done=1 goto2



# 107,491 points. 94,775 ticks. No Error.
 - one off setup
 - read input, guesstimate cube root, one round of newtons, setup binary search range
 - binary search until perfect
 - output, correcting for precision boost

/--------//--------//--------//--------//--------//--------//--------/

a=:a x=a*0.008+1.06-(a>2000)*a*0.005x=0.33*(a/(x*x)+2*x)-3l=x-3.3r=x+4
m=(l+r)*0.5 c=m*m*m>=a l=l*c+m*(1-c) r=r*(1-c)+m*c goto2+(r-l<=0.001)
:o=l goto:done++

# 107,825 points. 74,413 tcks. Total error: 0.058. Worst Error: 0.001.

/--------//--------//--------//--------//--------//--------//--------/

a=:a x=a/50
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
y=x*x*x x=x*(y+2*a)/(2*y+a)
:o=x goto:done++

# 104,841 points. 60,000 ticks. Total error: 37.749. Worst Error: 0.021.

/--------//--------//--------//--------//--------//--------//--------/
# 107,405 points. 98,711 ticks. No Error.

a=:a l=6 r=216 k=1000 h=0.5 i=0.001
m=(l+r)*h c=(m*m*m)/k>=a l=l*c+m*(1-c)r+=c*(m-r)goto2+(r-l<=i)
:o=l/10 goto:done++


l=l*c+m*(1-c)
l+=(1-c)*(m-l)

r=r*(1-c)+m*c
r+=c*(m-r)

/--------//--------//--------//--------//--------//--------//--------/
# 107,726 points. 82,183 ticks. Total error: 0.426. Worst error: 0.001

a=:a l=0.5 r=22
m=(l+r)*0.5 c=m*m*m>=a l=l*c+m*(1-c) r=r*(1-c)+m*c goto2+(r-l<=0.001)
:o=l goto:done++

/--------//--------//--------//--------//--------//--------//--------/