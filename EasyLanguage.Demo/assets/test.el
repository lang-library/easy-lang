#! /usr/bin/env elang
(program
 (define x 11)
 (define x (add 11 1))
 (define y 22)
 (Echo (add x y))
 )

(program
 (define sum (add 11 (add 22 33 44)))
 ([. Console WriteLine] sum)
 (Echo '"hello")
 @ Echo("as-is@@") @
 (define map {a: (add 11 22) b: @33+44@})
 @ Echo(map) @
 #sum
 @ undefined @
 )
