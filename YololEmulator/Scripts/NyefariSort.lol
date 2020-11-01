i=--:i m="="n=0s=i---i+">"
j=i---i s=j+s x/=j>m j=i---i s=j+s x/=j>m j=i---i s=j+s goto2+(j<m)
e=d d=c c=b b=a a=s i-- s=i---i+">" j=i---i s=j+s goto2+(j<m)
k=0ifa>b thent=a a=b b=t k=1end ifb>c thent=b b=c c=t k=1end
ifc>d thent=c c=d d=t k=1end ifd>e thent=d d=e e=t k=1end goto6-k*2
:o=a+b+c+d+e goto++:done


//<Y><YQ><KGII><KCSU><ZLI>
//<CRUFK><QLOSV><RPM><SP><X>



/--------//--------//--------//--------//--------//--------//--------/

i=--:i m="="n=0s=i---i+">"
j=i---i s=j+s x/=j>m j=i---i s=j+s x/=j>m j=i---i s=j+s goto2+(j<m)
e=d d=c c=b b=a a=s i-- s=i---i+">" j=i---i s=j+s goto2+(j<m)
k=0ifa>b thent=a a=b b=t k=1end ifb>c thent=b b=c c=t k=1end
ifc>d thent=c c=d d=t k=1end ifd>e thent=d d=e e=t k=1end goto6-k*2
:o=a+b+c+d+e goto++:done

/--------//--------//--------//--------//--------//--------//--------/

i=--:i m="="n=0 s=i---i+">" :o=""
j=i---i s=j+s x/=j>m j=i---i s=j+s x/=j>m j=i---i s=j+s goto2+(j<m)
e=d d=c c=b b=a a=s i-- s=i---i+">" j=i---i s=j+s goto2+(j<m)
l=a<b m=a<c n=a<d o=a<e p=b<c q=b<d r=b<e s=c<d t=c<e u=d<e v=l+m+n+o
w=1-l+p+q+r x=2-m-p+s+t y=3-n-q-s+u z=4-o-r-t-u
:o+=a+(v==4):o-=a+0:o+=b+(w==4):o-=b+0:o+=c+(x==4):o-=c+0:o+=d+(y==4)
:o-=d+0:o+=e+(z==4):o-=e+0:o+=a+(v==3):o-=a+0:o+=b+(w==3):o-=b+0
:o+=c+(x==3):o-=c+0:o+=d+(y==3):o-=d+0:o+=e+(z==3):o-=e+0:o+=a+(v==2)
:o-=a+0:o+=b+(w==2):o-=b+0:o+=c+(x==2):o-=c+0:o+=d+(y==2):o-=d+0
:o+=e+(z==2):o-=e+0:o+=a+(v==1):o-=a+0:o+=b+(w==1):o-=b+0:o+=c+(x==1)
:o-=c+0:o+=d+(y==1):o-=d+0:o+=e+(z==1):o-=e+0:o+=a+(v==0):o-=a+0
:o+=b+(w==0):o-=b+0:o+=c+(x==0):o-=c+0:o+=d+(y==0):o-=d+0:o+=e+(z==0)
:o-=e+0:o-=1:o-=1:o-=1:o-=1:o-=1:done=1goto1

/--------//--------//--------//--------//--------//--------//--------/

i=:i s="" m="="
j=i---i s=j+s x/=j>m j=i---i s=j+s x/=j>m j=i---i s=j+s goto2+(j<m)
e=d d=c c=b b=a a=s s="" x/=i>0 goto2
k=0 if a>b then t=a a=b b=t k=1 end if b>c then t=b b=c c=t k=1 end
if c>d then t=c c=d d=t k=1 end if d>e then t=d d=e e=t k=1 end
:o=a+b+c+d+e :done=1-k goto1+k*3