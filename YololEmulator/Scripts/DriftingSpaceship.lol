x=-4.601 y=-6.397 z=11.336 s=20
:px+=x:py+=y:pz+=z s-- :done=1 q/=s goto2
x=:dx y=:dy z=:dz
x=(:dx+x)/2 y=(:dy+y)/2 z=(:dz+z)/2 :px+=x:py+=y:pz+=z :done=1 goto4

/--------//--------//--------//--------//--------//--------//--------/

ux=:dx ox=1
k=ox/(ox+1) ux+=k*(:dx-ux) ox-=k*ox px+=ux :done=1
goto2-((((:dx-ux)^2+(:dy-uy)^2+(:dz-uz)^2)^0.5)>1)

/--------//--------//--------//--------//--------//--------//--------/

ux=:dx ox=1 uy=:dy oy=1 uz=:dz oz=1 
k=ox/(ox+1) ux+=k*(:dx-ux) ox-=k*ox :px+=ux
k=oy/(oy+1) uy+=k*(:dy-uy) oy-=k*oy :py+=uy
k=oz/(oz+1) uz+=k*(:dz-uz) oz-=k*oz :pz+=uz :done=1
goto2-((((:dx-ux)^2+(:dy-uy)^2+(:dz-uz)^2)^0.5)>1)


// https://www.lesswrong.com/posts/nM4bm6tkbZZaScLPp/kalman-filter-for-bayesians
// Prior:       N(μ0,σ0)
// Observation: N(μ1,σ1)
// Posterior:
//  μ′ = μ0 + k*(μ1 − μ0)
//  σ′ = σ0 − k*σ0
//  k  = σ0 / (σ0+σ1)

/--------//--------//--------//--------//--------//--------//--------/

x=:dx y=:dy z=:dz u=:px v=:py w=:pz i=1
x=(:dx+x)/2y=(:dy+y)/2z=(:dz+z)/2 :px=u+x*i:py=v+y*i:pz=w+z*i++:done=1
goto2-((((:dx-x)^2+(:dy-y)^2+(:dz-z)^2)^0.5)>1)

/--------//--------//--------//--------//--------//--------//--------/

x=-4.601 y=-6.397 z=11.336
x=(:dx+x)/2 y=(:dy+y)/2 z=(:dz+z)/2 :px+=x:py+=y:pz+=z :done=1 goto2

x=(:dx+x)/2
x/=2x+=:dx/2

/--------//--------//--------//--------//--------//--------//--------/

:px+=:dx:py+=:dy:pz+=:dz goto:done++

/--------//--------//--------//--------//--------//--------//--------/

uvw = position at direction change
xyz = best estimate of current velocity
i   = counter since last velocity change