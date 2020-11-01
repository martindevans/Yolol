a=:a e=3.934*a^0.228-a^0.319e+=(a<50)*(2.3*a^0.2-e)
b=e-0.5c=e+0.5b*=b>0i=0j=0.001t=(b+c)/2q=2^t>a c+=q--*(t-c)b-=q*(t-b)
t=(b+c)/2q=2^t>a c+=q--*(t-c)b-=q*(t-b)goto3+(i++>9)
w=2^b x=2^(b+j)y=abs(a-w)z=abs(a-x):o=(y<=z)*b+(y>z)*(b+j)goto:done++



e=3.934*a^0.228-a^0.319e+=(a<50)*(2.3*a^0.2-a)



2.3*:a^0.2	// Worst case 0.5 when :a<50


// Worst:0.949 Total:46
:o=3.934*:a^0.228-:a^0.319     // Worst case 0.1 when :a>50

// Worst:1.077 Total:214
:o=3.201*:a^0.177+sqrt(:a)*-0.029

// Worst:1.305 Total:315
:o=3.598*:a^0.143

/--------//--------//--------//--------//--------//--------//--------/

a=:a e=3.934*a^0.228-a^0.319 b=e-1c=e+1b*=b>0i=0j=0.001
t=(b+c)/2q=2^t>a c+=q--*(t-c)b-=q*(t-b)goto2+(i++>9)
w=2^b x=2^(b+j)y=abs(a-w)z=abs(a-x):o=(y<=z)*b+(y>z)*(b+j)goto:done++

/--------//--------//--------//--------//--------//--------//--------/

a=:a e=3.598*:a^0.143b=e-2c=e+2b*=b>0i=0j=0.001
t=(b+c)/2q=2^t>a c+=q--*(t-c)b-=q*(t-b)goto2+(i++>10)
w=2^b x=2^(b+j)y=abs(a-w)z=abs(a-x):o=(y<=z)*b+(y>z)*(b+j)goto:done++

/--------//--------//--------//--------//--------//--------//--------/


a=:a b=0 c=14 i=0 j=0.001 t=(b+c)/2 q=2^t if q>a then c=t else b=t end
t=(b+c)/2 q=2^t if q>a then c=t else b=t end goto2+(i++>11)
w=2^b x=2^(b+j)y=abs(a-w)z=abs(a-x):o=(y<=z)*b+(y>z)*(b+j)goto:done++


/--------//--------//--------//--------//--------//--------//--------/

a=:a e=3.598*:a^0.143 b=e-2 c=e+2 b*=b>0i=0
t=(b+c)/2q=2^t>a c+=q--*(t-c)b-=q*(t-b) goto2+(i++>10)
:o=b goto:done++



t=(b+c)/2q=2^t if q>a then c=t else b=t end
t=(b+c)/2q=2^t>a c+=q*(t-c)b+=(1-q)*(t-b)
t=(b+c)/2q=2^t>a c+=q--*(t-c)b-=q*(t-b)

b+=(1-q)*(t-b)
b+=t-b-q*(t-b)
b-=(q-1)*(t-b)

/--------//--------//--------//--------//--------//--------//--------/

:o=3.598*:a^0.143 goto:done++

// Total error: 315.896.

/--------//--------//--------//--------//--------//--------//--------/

a=:a b=0 c=14 i=0
t=(b+c)/2 q=2^t if q>a then c=t else b=t end goto2+(i++>12)
:o=b goto:done++