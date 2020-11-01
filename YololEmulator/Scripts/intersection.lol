l=:l r=:r a=l---l g=l---l h=l---l j=l---l k=l---l x/=r-a==r a--
m=l---l p=l---l q=l---l s=l---l y=r-q z=r-k x/=r-g!=r a=g+a
ifr-h!=r thena=h+(a-h)end ifr-j!=r thena=j+(a-j)end x/=z!=r a=k+(a-k)
ifr-m!=r thena=m+(a-m)end ifr-p!=r thena=p+(a-p)end x/=y!=r a=q+(a-q)
ifr-s!=r thena=s+(a-s)end ifr-l!=r thena=l+(a-l) end :i=a goto++:done

// a g
// h j k(z)
// m p q(y)
// s l
/--------//--------//--------//--------//--------//--------//--------/



a=""l=:l r=:r f=l---l g=l---l h=l---l j=l---l k=l---l m=l---l p=l---l
q=l---l s=l---l y=r-q z=r-k ifr-f!=r thena=f end x/=r-g!=r a=g+a
ifr-h!=r thena=h+(a-h)end ifr-j!=r thena=j+(a-j)end x/=z!=r a=k+(a-k)
ifr-m!=r thena=m+(a-m)end ifr-p!=r thena=p+(a-p)end x/=y!=r a=q+(a-q)
ifr-s!=r thena=s+(a-s)end ifr-l!=r thena=l+(a-l) end :i=a goto++:done

// f g
// h j k(z)
// m p q(y)
// s l
/--------//--------//--------//--------//--------//--------//--------/

a=""l=:l r=:r f=l---l g=l---l h=l---l j=l---l k=l---l m=l---l p=l---l
q=l---l s=l---l ifr-f!=r thena=f end x/=r-g!=r a=g+(a-g)
 y=r-l!=r ifr-h!=r thena=h+(a-h)end x/=r-j!=r a=j+(a-j)
ifr-k!=r thena=k+(a-k)end ifr-m!=r thena=m+(a-m)end x/=z a=p+(a-p)
ifr-q!=r thena=q+(a-q)end ifr-s!=r thena=s+(a-s)end x/=y a=l+(a-l)
:i=a goto++:done

// f g
// h j
// k m p
// q s t
/--------//--------//--------//--------//--------//--------//--------/

x=:l a=x---x b=x---x c=x---x d=x---x e=x---x f=x---x g=x---x h=x---x 
i=x---x j=x a-=b a-=c a-=d a-=e a-=f a-=g a-=h a-=i a-=j b-=c b-=d
b-=e b-=f b-=g b-=h b-=i b-=j c-=d c-=e c-=f c-=g c-=h c-=i c-=j d-=e
d-=f d-=g d-=h d-=i d-=j e-=f e-=g e-=h e-=i e-=j f-=g f-=h f-=i f-=j
g-=h g-=i g-=j h-=i h-=j i-=j x=a+b+c+d+e+f+g+h+i+j m="" r=:r
k=x---x ifr-k!=r thenm+=k end l=x---x z/=r-l!=r m+=l goto6
:i=m d=x<0:done=d goto6-d*5

a-=b a-=c a-=d a-=e a-=f a-=g a-=h a-=i a-=j
b-=c b-=d b-=e b-=f b-=g b-=h b-=i b-=j
c-=d c-=e c-=f c-=g c-=h c-=i c-=j
d-=e d-=f d-=g d-=h d-=i d-=j
e-=f e-=g e-=h e-=i e-=j
f-=g f-=h f-=i f-=j
g-=h g-=i g-=j
h-=i h-=j
i-=j

l=:l f=l---l g=l---l h=l---l j=l---l k=l---l m=l---l p=l---l q=l---l
f-=g f-=h f-=j f-=k f-=m f-=p f-=q t=l---l f-=t g-=t h-=t j-=t m-=t
g-=h g-=j g-=k g-=m g-=p g-=q s=l---l f-=s g-=s j-=t s-=t k-=t k-=t
h-=j h-=k h-=m h-=p h-=q h-=s j-=k j-=m j-=p j-=q j-=s j-=t k-=t k-=s
k-=m k-=p k-=q k-=s m-=p m-=q m-=s m-=t p-=q p-=s p-=t q-=s q-=t
/--------//--------//--------//--------//--------//--------//--------/

a=""l=:l r=:r f=l---l g=l---l h=l---l j=l---l k=l---l m=l---l p=l---l
q=l---l s=l---l ifr-f!=r thena=f end x/=r-g!=r a=g+(a-g)
z=r-p!=r y=r-l!=r ifr-h!=r thena=h+(a-h)end x/=r-j!=r a=j+(a-j)
ifr-k!=r thena=k+(a-k)end ifr-m!=r thena=m+(a-m)end x/=z a=p+(a-p)
ifr-q!=r thena=q+(a-q)end ifr-s!=r thena=s+(a-s)end x/=y a=l+(a-l)
:i=a goto++:done

// f g
// h j
// k m p(z)
// q s t(y)
/--------//--------//--------//--------//--------//--------//--------/

x=:l a=x---x b=x---x c=x---x d=x---x e=x---x f=x---x g=x---x h=x---x 
i=x---x j=x y=:r a-=b a-=c a-=d a-=e a-=f a-=g a-=h a-=i a-=j b-=c 
b-=d b-=e b-=f b-=g b-=h b-=i b-=j c-=d c-=e c-=f c-=g c-=h c-=i c-=j
d-=e d-=f d-=g d-=h d-=i d-=j e-=f e-=g e-=h e-=i e-=j f-=g f-=h f-=i
f-=j g-=h g-=i g-=j h-=i h-=j i-=j z="" ify-j!=y then z+=j end
ify-i!=y then z+=i end ify-h!=y then z+=h end ify-g!=y then z+=g end
ify-f!=y then z+=f end ify-e!=y then z+=e end ify-d!=y then z+=d end
ify-c!=y then z+=c end ify-b!=y then z+=b end ify-a!=y then z+=a end
:done=1 :i=z goto1
/--------//--------//--------//--------//--------//--------//--------/

a="" l=:l r=:r f=l---l g=l---l h=l---l j=l---l k=l---l m=l---l p=l---l
q=l---l s=l---l if(r-f!=r)thena=f end x/=(r-g!=r)a=g+(a-g)
t=l---l if(r-h!=r)thena=h+(a-h)end x/=(r-j!=r)a=j+(a-j)
if(r-k!=r)thena=k+(a-k)end x/=(r-m!=r)a=m+(a-m)
if(r-p!=r)thena=p+(a-p)end x/=(r-q!=r)a=q+(a-q)
if(r-s!=r)thena=s+(a-s)end if(r-t!=r)thena=t+(a-t)end:done=1:i=a goto1
/--------//--------//--------//--------//--------//--------//--------/

a="" l=:l r=:r
x=l---l y=l---l if(r-x!=r)thena=x+(a-x) end z/=(r-y!=r)a=y+(a-y)goto2
:i=a d=l<0:done=d goto2-d
/--------//--------//--------//--------//--------//--------//--------/

:i="" l=:l r=:r
c=l---l d=l<0:done=d if(r-c!=r)then:i=c+(:i-c) endgoto2-d
/--------//--------//--------//--------//--------//--------//--------/

:i="" l=:l r=:r
c=l---l d=l<0:done=d if(r-c!=r)*(l-c==l)then:i=c+:i endgoto2-d
/--------//--------//--------//--------//--------//--------//--------/




