y=:a*100 x=:b*100 pi=3.141
if x > 0 then d=atan(y / x) goto 10 end
if x < 0 and y >= 0 then d=atan(y / x) + 180 goto 10 end
if x < 0 and y < 0 then d=atan(y / x) - 180 goto 10 end
if x == 0 and y > 0 then :o=pi/2 :done=1 goto1 end
if x == 0 and y < 0 then :o=-pi/2 :done=1 goto1 end
:o=9223372036854775.807 :done=1 goto 1


o=(d*pi*2)/36 s=o+"" s=s---s o/=10
:done=1 goto 1I've go