c=:i---:i i=c>1i+=c>3i+=c>5i+=c>7i*=2i+=c>i x/=c>=0o+=i*10^p++goto1
:o=o:done=c>0o*=(c<0)/10^pp=0goto1


c=:i---:i
i+=c>1 i+=c>3 i+=c>5 i+=c>7 i*=2 i+=c>i
x/=c>=0
o+=i*10^p++
goto1





:o=o
:done=c>0
o*=(c<0)/10^p
p=0
goto1





