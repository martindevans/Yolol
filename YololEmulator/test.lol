:seed=123456787654321
m=2^28-57 a=31792125 c=12345 k=2^16 d=2^11 r=:seed%m
r=(a*r+c)%m :out=r/d%k*k r=(a*r+c)%m :out+=r/d%k :out-=:out%1
high+=:out>2^31 count++ goto 3