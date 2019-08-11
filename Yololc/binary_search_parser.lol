char = :input
min=0 max=10 search=5 k10=10000
l1 = char >= search if l1 then min=search else max=search end search=min+((max-min)/2)/k10*k10
l2 = char >= search if l2 then min=search else max=search end search=min+((max-min)/2)/k10*k10
l3 = char >= search min=l3--*search-l3*min max=-l3++*search+l3*max search=min+((max-min)/2)/k10*k10
l4 = char >= search min=l4--*search-l4*min max=-l4++*search+l4*max search=min+k10*(max-min)/k20
:output=search goto 1