a=:a :o=a b=:b e="" d=a-b==a :done=d goto2-d
e=a---a+e x/=e==e-b e=a---a+e x/=e==e-b e=a---a+e goto2+(a<0)/(e==e-b)
if e!=e-b then e=:c+e-b end :o=e d=a=="" :done=d goto2-d

/--------//--------//--------//--------//--------//--------//--------/

a=:a :o=a b=:b c=:c e="" d=a-b==a :done=d goto2-d
e=a---a+e if e!=e-b then e=c+e-b end goto2
:o=e goto:done++

/--------//--------//--------//--------//--------//--------//--------/

a=:a :o=a b=:b c=:c e="" d=a-b==a :done=d goto2-d
e=a---a+e f=e-b if e!=f then e=c+f end d=a=="" :o=e :done=d goto2-d

/--------//--------//--------//--------//--------//--------//--------/

a = haystack
b = needle

d = done indicator
e = accumulator
f = d - b
g = ""

__c____hijklmnopqrstuvwxyz