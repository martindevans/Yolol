i=:l+:r a=i---i b=i---i:i=b+(a-b)
a=i---i b=i---i:i=b+(a+(:i-a)-b)a=i---i b=i---i:i=b+(a+(:i-a)-b)goto2
:done=1goto1

^^^^ current best

/--------//--------//--------//--------//--------//--------//--------/

i=:l+:r:i=i---i c=i---i:i=c+(:i-c)goto20


















c=i---i:i=c+(:i-c)c=i---i:i=c+(:i-c)c=i---i:i=c+(:i-c):done=i<0goto20



/--------//--------//--------//--------//--------//--------//--------/

z=:l+:r a=z---z b=z---z c=z---z d=z---z e=z---z f=z---z g=z---z
h=z---z i=z---z j=z---z k=z---z l=z---z m=z---z n=z---z o=z---z 
p=z---z q=z---z r=z---z s=z---z t=z---z
a-=b a-=c a-=d a-=e a-=f a-=g a-=h a-=i a-=j a-=k a-=l a-=m a-=n a-=o
a-=p a-=q a-=r a-=s a-=t b-=c b-=d b-=e b-=f b-=g b-=h b-=i b-=j b-=k
b-=l b-=m b-=n b-=o b-=p b-=q b-=r b-=s b-=t c-=d c-=e c-=f c-=g c-=h
c-=i c-=j c-=k c-=l c-=m c-=n c-=o c-=p c-=q c-=r c-=s c-=t d-=e d-=f
d-=g d-=h d-=i d-=j d-=k d-=l d-=m d-=n d-=o d-=p d-=q d-=r d-=s d-=t
e-=f e-=g e-=h e-=i e-=j e-=k e-=l e-=m e-=n e-=o e-=p e-=q e-=r e-=s
e-=t f-=g f-=h f-=i f-=j f-=k f-=l f-=m f-=n f-=o f-=p f-=q f-=r f-=s
f-=t g-=h g-=i g-=j g-=k g-=l g-=m g-=n g-=o g-=p g-=q g-=r g-=s g-=t
h-=i h-=j h-=k h-=l h-=m h-=n h-=o h-=p h-=q h-=r h-=s h-=t i-=j i-=k
i-=l i-=m i-=n i-=o i-=p i-=q i-=r i-=s i-=t j-=k j-=l j-=m j-=n j-=o
j-=p j-=q j-=r j-=s j-=t k-=l k-=m k-=n k-=o k-=p k-=q k-=r k-=s k-=t
l-=m l-=n l-=o l-=p l-=q l-=r l-=s l-=t m-=n m-=o m-=p m-=q m-=r m-=s
m-=t n-=o n-=p n-=q n-=r n-=s n-=t o-=p o-=q o-=r o-=s o-=t p-=q p-=r
p-=s p-=t q-=r q-=s q-=t r-=s r-=t s-=t
:i=t+s+r+q+p+o+n+m+l+k+j+i+h+g+f+e+d+c+b+a :done=1 goto1


/--------//--------//--------//--------//--------//--------//--------/

:i=""
c=:r---:r:i=c+(:i-c)c=:r---:r:i=c+(:i-c)c=:r---:r:i=c+(:i-c) goto2
c=:l---:l:i=c+(:i-c)c=:l---:l:i=c+(:i-c):done=:l<0 goto3-2*(:l<0)

/--------//--------//--------//--------//--------//--------//--------/
i=:l+:r:i=""
c=i---i:i=c+(:i-c)c=i---i:i=c+(:i-c):done=i<0goto2-(i<0)

/--------//--------//--------//--------//--------//--------//--------/

i=:l+:r:i=""
x=i---i y=i---i z=i---i :i-=x x-=y x-=z y-=z :i-=y :i-=z :i=z+y+x+:i goto2
:done=1goto1

/--------//--------//--------//--------//--------//--------//--------/

i=:l+:r:i=""
c=i---i:i=c+(:i-c)c=i---i:i=c+(:i-c)c=i---i:i=c+(:i-c) goto2
:done=1goto1




