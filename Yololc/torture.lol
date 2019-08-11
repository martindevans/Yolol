x = (sin(2646.0496 * ++s) * 43758.4530) % 1 y=(sin(2646.0496 * ++s) * 43758.4530) % 1
all++ if (x ^ 2 + y ^ 2) < 1 then inside += 1 end pi = 4 * (inside / all) if all == 10 then :pi = pi goto 3 else goto 1 end
f1 = 0 f2 = 1 fibcount=15
tmp = f2 f2 = f1 + f2 f1 = tmp if (fibcount-- > 0) then :fib50 = f1 + f2 goto 5 else goto 4 end
factorial_count = 15 factorial_accumulator = 1
if factorial_count-- > 1 then factorial_accumulator *= factorial_count goto 6 end :factorial_result = factorial_accumulator goto 7
if (3 + 4 * 5 + 6)!=29 then :sanity_check="failed" else :sanity_check="success" end