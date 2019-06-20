s = :BITSET
i = :SET_THIS_BIT
s = s-("B"+i)+("B"+i)
i = :SET_THIS_BIT
s = s-("B"+i)+("B"+i)
i = :CHECK_THIS_BIT
:SET=(s-("B"+i))~=s