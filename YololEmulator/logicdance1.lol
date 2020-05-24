c=:a+:b:o=((:op<"b"<c)+(:op>"p")*c)%3>0goto++:done

c=:a+:b:o=((:op<"b"<c)+(:op>"p")*c)%3>0
:o=((:a+:b)*((:op>"p")+1)-(:op<"o"))%4>0
c=:a+:b:o=(c+c*(:op>"p")-(:op<"o"))%4>0
c=:a+:b-(:op<"o"):o=(c+c*(:op>"p"))%4>0
c=:a+:b-(:op<"o")c+=c*(:op>"p"):o=c%4>0
x="or"c=:a+:b c-=:op<x c+=c*(:op>x):o=c%4>0
x="or"c=:a+:b:o=c-(c<2)*(:op<x)-(c>1)*(:op>x)*2>0
x="or"o=:op c=:a+:b:o=c-(c<2)*(o<x)-(c>1)*(o>x)*2>0
x="or"o=:op c=:a+:b:o=(c>0)*(1-(c<2)*(o<x)-(c>1)*(o>x))
x="or"o=:op c=:a+:b:o=(c>1)*(o<x)+(c>0)*(o==x)+(c==1)*(o>x)

		0 1 2
and		0 0 1
or      0 1 1
xor     0 1 0

/--------//--------//--------//--------//--------//--------//--------/

x="or"o=:op c=:a+:b:o=(c>1)*(o<x)+(c>0)*(o==x)+(c==1)*(o>x)goto++:done


and(d) = c>1
or(e)  = c>0
xor( ) = e-d c==1

		0 1 2
and		0 0 1     o<x
or      0 1 1     o==x
xor     0 1 0     o>x

/--------//--------//--------//--------//--------//--------//--------/

a="a"b="e"d="ne"e="no"f="or"g="x"c="n"+a
o=:op x=:a y=:b k=x+y:o=k>1 l=o<b:done=l goto2+(1-l)*(o>a)+(o>d)
h=x==y i=k<2j=k==1:o=(o<c)*h+(o>c)*j:done=1goto2
h=k<1i=k>0j=k==1:o=(o<f)*h+(o==f)*i+(o>f)*j:done=1goto2

/--------//--------//--------//--------//--------//--------//--------/

a="and"b="eq"d="neq"e="nor"f="or"g="xor"c="n"+a
o=:op x=:a y=:b k=x+y goto3+(o>a)+(o>d)
:o=k==2:done=1goto2
h=x==y i=k<2j=k==1:o=(o<c)*h+(o==c)*i+(o>c)*j:done=1goto2
h=k<1i=k>0j=k==1:o=(o<f)*h+(o==f)*i+(o>f)*j:done=1goto2

/--------//--------//--------//--------//--------//--------//--------/

a="and"b="eq"c="nand"d="neq"e="nor"f="or"g="xor"
o=:op x=:a y=:b j=x+y goto3+(o>a)+(o>c)+(o>e)
:o=j==2:done=1goto2
h=x==y i=j<2 :o=(o<c)*h+(o>b)*i:done=1goto2
h=x!=y i=j<1 :o=(o<e)*h+(o>d)*i:done=1goto2
h=j>0 i=j==1 :o=(o<g)*h+(o>f)*i:done=1goto2

/--------//--------//--------//--------//--------//--------//--------/

a="and"b="eq"c="nand"d="neq"e="nor"f="or"g="xor"
o=:op x=:a y=:b goto3+(o>a)+(o>b)+(o>c)+(o>d)+(o>e)+(o>f)
:o=(x and y):done=1goto2
:o=x==y:done=1goto2
:o=not(x and y):done=1goto2
:o=x!=y:done=1goto2
:o=not(x or y):done=1goto2
:o=x or y:done=1goto2
:o=(x+y)==1:done=1goto2

/--------//--------//--------//--------//--------//--------//--------/


		00 01 10 11
and		0  0  0  1
eq      1  0  0  1
nand    1  1  1  0
neq     0  1  1  0 <<
nor     1  0  0  0
or      0  1  1  1
xor     0  1  1  0 <<


o=:op goto1+(o>"neq")+(o>"eq")+(o>"nor")*4+(o>" ")+(o>" ")


and		1
eq		2
nand	3
neq		4
nor		5
or		6
xor		6