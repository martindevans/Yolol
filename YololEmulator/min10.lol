o=:a+(:a>:b)*(:b-:a)o+=(o>:c)*(:c-o)o+=(o>:d)*(:d-o)o=:e/(:e<o)
o+=(o>:f)*(:f-o)o+=(o>:g)*(:g-o)o+=(o>:h)*(:h-o)o=:i/(:i<o)
:o=o+(o>:j)*(:j-o)goto:done++



/--------//--------//--------//--------//--------//--------//--------/

:a-=(:b<:a)*(:a-:b)
:c-=(:d<:c)*(:c-:d)
:e-=(:f<:e)*(:e-:f)
:g-=(:h<:g)*(:g-:h)
:i-=(:j<:i)*(:i-:j)

:a-=(:c<:a)*(:a-:c)
:e-=(:g<:e)*(:e-:g)
:e-=(:i<:e)*(:e-:i)

:o=:a+(:a>:e)*(:e-:a)

goto:done++

/--------//--------//--------//--------//--------//--------//--------/

:o=:a+(:a>:b)*(:b-:a)
:o+=(:o>:c)*(:c-:o)
:o+=(:o>:d)*(:d-:o)
:o+=(:o>:e)*(:e-:o)
:o+=(:o>:f)*(:f-:o)
:o+=(:o>:g)*(:g-:o)
:o+=(:o>:h)*(:h-:o)
:o+=(:o>:i)*(:i-:o)
:o+=(:o>:j)*(:j-:o)
goto:done++

:o+=(:o>:j)*(:j-:o)
c=:o>:j c*=:j-:o :o+=c

/--------//--------//--------//--------//--------//--------//--------/

:o=:a if:a<:o then:o=:a end if:b<:o then:o=:b end
if:c<:o then:o=:c end if:d<:o then:o=:d end if:e<:o then:o=:e end
if:f<:o then:o=:f end if:g<:o then:o=:g end if:h<:o then:o=:h end
if:i<:o then:o=:i end if:j<:o then:o=:j endgoto:done++

/--------//--------//--------//--------//--------//--------//--------/

:o=4001
if :a<:o then :o=:a end
if :b<:o then :o=:b end
if :c<:o then :o=:c end
if :d<:o then :o=:d end
if :e<:o then :o=:e end
if :f<:o then :o=:f end
if :g<:o then :o=:g end
if :h<:o then :o=:h end
if :i<:o then :o=:i end
if :j<:o then :o=:j end
:done=1
goto 1