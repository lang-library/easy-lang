(xdump (+ 11 22))

(xdump command-line-args)
(xdump command-line-args-left)

(defun test-func ()
  (xprint "test-func() started")
  (xdump command-line-args-left)
  )

(xpand
 (!class <rectangle> ()
         (:width  1.0)
         (:height 1.0)
         :temp
         ))
