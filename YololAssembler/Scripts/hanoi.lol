#define const32      r
#define const31      s
#define const21      v

// indicates success of part 1 of swap
#define swap_success t

// countdown to done (starts at 2^i-1)
#define countdown    u
#define undefined    w

// Where to goto after processing a pair of pegs
#define destAB       x
#define destAC       y
#define destBC       z

// char taken from left/right pin
#define l_pin_char   d
#define r_pin_char   e

#import lib.lol

@line1:
    b=""
    c=""
    const21="21 "
    i=:i
    :o=""
    a=--i+b
    err_if_not(i>0)
    a+=--i
    err_if_not(i>0)
    a+=--i
    err_if_not(i>0)
    a+=--i

@line2:
    const32="32 "
    err_if_not(i>0)
    a+=--i
    err_if_not(i>0)
    a+=--i
    err_if_not(i>0)
    a+=--i
    err_if_not(i>0)
    a+=--i
    goto2

@line3:
    const31="31 "
    countdown=2^:i-1
    if:i%2==0then
        destAB=13
        destAC=10
        destBC=17
        goto17
    end
    destAB=10 destAC=17 destBC=13 goto13

@line4:
    :o-- goto:done++

@line5:

@line6:

@line7:

@line8:

@line9:

@line10:
    swap_success=0
    l_pin_char=pop_char(b)
    swap_success=1
    r_pin_char=pop_char(c)
    swap_success=2
    if l_pin_char<r_pin_char then
        c+=r_pin_char+l_pin_char :o+="23 "
    else
        b+=l_pin_char+r_pin_char
        :o+=const32
    end

@line11:
    countdown--
    if swap_success>1 then
        goto4 + (u>0)*(destBC-4)
    end
    :o+=const32
    b+=pop_char(c)
    goto destBC

@line12:

@line13:
    swap_success=0
    l_pin_char=pop_char(a)
    swap_success=1
    r_pin_char=pop_char(c)
    swap_success=2
    if l_pin_char<e then
        c+=r_pin_char+l_pin_char
        :o+="13 "
    else
        a+=l_pin_char+r_pin_char
        :o+=const31
    end

@line14:
    countdown--
    if swap_success>1 then
        goto4+(u>0)*(destAC-4)
    end
    undefined/=swap_success<1
    :o+=const31
    a+=pop_char(c)
    goto destAC

@line15:
    :o+="13 "
    c+=l_pin_char
    goto4+(u>0)*(destAC-4)

@line16:

@line17:
    swap_success=0
    l_pin_char=pop_char(a)
    swap_success=1
    r_pin_char=pop_char(b)
    swap_success=2
    if l_pin_char<r_pin_char then
        b+=r_pin_char+l_pin_char
        :o+="12 "
    else
        a+=l_pin_char+r_pin_char
        :o+=const21
    end

@line18:
    countdown--
    if swap_success<2 then
        if swap_success<1 then
            :o+=const21
            a+=pop_char(b)
        else
            :o+="12 "
            b+=l_pin_char
        end
    end
    goto destAB