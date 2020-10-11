t=:b b=:a%:b a=t t=b b=a%b a=t t=b b=a%b a=t t=b b=a%b a=t
t=b b=a%b a=t t=b b=a%b a=t t=b b=a%b a=t t=b b=a%b a=t goto2+(b<1)
:o=a<2goto:done++



t=b b=a%b a=t



a(0) b(0)
b(1) = a(0) % b(0)  a(1) = b(0)
b(2) = b(0) % b(1)  a(2) = b(1)

a
b
c=a%b
d=b%c
a=c
b=d

if a>b then a%=b else b%=a end
c=a>b a+=(a%b-a)*c b+=(b%a-b)*c




/--------//--------//--------//--------//--------//--------//--------/


public static int GetGCDByModulus(int value1, int value2)
{
    while (value1 != 0 && value2 != 0)
    {
        if (value1 > value2)
            value1 %= value2;
        else
            value2 %= value1;
    }
    return Math.Max(value1, value2);
}

public static bool Coprime(int value1, int value2)
{
    return GetGCDByModulus(value1, value2) == 1;
}

while b ≠ 0
    t := b
    b := a mod b
    a := t
return a

t=b b=a%b a=t
