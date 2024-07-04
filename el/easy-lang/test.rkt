#! /usr/bin/env racket
#lang racket

(define xyz 777)
(println 'hello)

(define <rectangle>
  (class object%
    (init :width :height) (super-new)
    (println :width)
    (println :height)
    (define &width :width)
    (define &height :height)
    (define/public (^area)
      (* &width &height))
    )
  )

(define $rect
  (new <rectangle>
       [:width 30]
       [:height 20]
       )
  )

(define $area (send $rect ^area))
(println $area)
(println (send $rect &width))
