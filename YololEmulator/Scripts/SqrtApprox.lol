x=:a/79+9x*=-0.7*:a/9999+1.4x=(x+:a/x)/2:o=(x+:a/x)/2goto:done++

/--------//--------//--------//--------//--------//--------//--------/

x=:a/80+9x=(x+:a/x)/2x=(x+:a/x)/2:o=(x+:a/x)/2goto:done++

> 79631, Total error: .028.

/--------//--------//--------//--------//--------//--------//--------/

x=:a/80+9x=(x+:a/x)/2x=(x+:a/x)/2x=(x+:a/x)/2:o=(x+:a/x)/2goto:done++

> 79631, no error

:o=(x+:a/x)/2
:o=x-(x*x>:a)*0.001

/--------//--------//--------//--------//--------//--------//--------/

x=:a/100+15x=(x+:a/x)/2x=(x+:a/x)/2x=(x+:a/x)/2x=(x+:a/x)/2
:o=(x+:a/x)/2goto:done++

/--------//--------//--------//--------//--------//--------//--------/

a=:a x=a/99+15x=(x+a/x)/2x=(x+a/x)/2x=(x+a/x)/2x=(x+a/x)/2
:o=(x+a/x)/2goto:done++

x=(x+a/x)/2
x+=a/x x/=2
x=x/2+a/x/2
x=a/(2*x)+x/2
x=(a+x*x)/(2*x)

# Babylonian Method
    Begin with an arbitrary positive starting value x0 (the closer to the actual square root of S, the better).
    Let xn + 1 be the average of xn and S/xn (using the arithmetic mean to approximate the geometric mean).
    Repeat step 2 until the desired accuracy is achieved.

/--------//--------//--------//--------//--------//--------//--------/

x=50x=(x+:a/x)/2x=(x+:a/x)/2x=(x+:a/x)/2:o=(x+:a/x)/2goto:done++

# Total error: 0.953

/--------//--------//--------//--------//--------//--------//--------/

x=40a=(:a-x*x)/(2*x) b=x+a x=b-(a*a)/(2*b) 
a=(:a-x*x)/(2*x) b=x+a x=b-(a*a)/(2*b)
a=(:a-x*x)/(2*x) b=x+a x=b-(a*a)/(2*b)
a=(:a-x*x)/(2*x) b=x+a :o=b-(a*a)/(2*b) goto:done++

# Bakhshali method
    x(n+1) = x(n) + a(n) - a(n)^2 / (2*(x(n)+a(n))

# In theory converges faster than babylonian method. Doesn't seem to get past
1.972 total error in actual testing for some reason.

/--------//--------//--------//--------//--------//--------//--------/

a=:a l=3 r=100
m=(l+r)*0.5 c=m*m>=a l=l*c+m*(1-c) r=r*(1-c)+m*c goto2+(r-l<=0.001)
:o=l goto:done++

/--------//--------//--------//--------//--------//--------//--------/