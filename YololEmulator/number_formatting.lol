z=0 y=:a/31536000 y-=y%1 :a-=y*31536000 x/=y z=" year" x/=y>1 z+="s"
e=0 d=:a/86400 d-=d%1 :a-=d*86400 x/=d e=" day" x/=d>1 e+="s"
i=0 h=:a/3600 h-=h%1 :a-=h*3600 x/=h i=" hour" x/=h>1 i+="s"
n=0 m=:a/60 m-=m%1 :a-=m*60 x/=m n=" minute" x/=m>1 n+="s"
t=0 s=:a u=d+h+m+s v=y :o=y+z-"0" x/=s t=" second" x/=s>1 t+="s"
v+=d u-=d x/=d ifv>0thenifu>0then:o+=", "else:o+=" and "endend :o+=d+e
v+=h u-=h x/=h ifv>0thenifu>0then:o+=", "else:o+=" and "endend :o+=h+i
v+=m u-=m x/=m ifv>0thenifu>0then:o+=", "else:o+=" and "endend :o+=m+n
v+=s u-=s x/=s ifv>0thenifu>0then:o+=", "else:o+=" and "endend :o+=s+t
:o="x"+:o:o-="x, ":o-="x and ":o-="x"goto:done++

/--------//--------//--------//--------//--------//--------//--------/

if x!=0 then
	v+=s
	u-=s
	if v>0 then
		if u>0 then
			:o+=", "
		else
			:o+=" and "
		end
	end
	:o+=s+t
end

ifv>0thenifu>0then:o+=", "else:o+=" and "endend
ifv*u then:o+=", "endifv*(1-u)then:o+=" and "end
:o+=((v*u>0)+", "+(v*(1-u)>0)+" and ")-1


y : years
z : years string suffix
d : days
e : days string suffix
h : hours
i : hours string suffix
m : minutes
n : minutes string suffix
s : seconds
t : second string suffix
u : sum of y+d+h+m+s
v : placed string sections sum


abc__fg__jkl__opqr__uvwx__