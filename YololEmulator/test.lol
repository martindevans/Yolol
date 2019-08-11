char = :input
min=0 max=10 search=5 k10=10000
l1 = char <  search min=l1*min+(1-l1)*search max=l1*search+(1-l1)*max search=min+((max-min)/2)/k10*k10
l2 = char <= search min=l2*min+(1-l2)*search max=l2*search+(1-l2)*max search=min+((max-min)/2)/k10*k10
l3 = char <= search min=l3*min+(1-l3)*search max=l3*search+(1-l3)*max search=min+((max-min)/2)/k10*k10
l4 = char=="1"or"6"==char

