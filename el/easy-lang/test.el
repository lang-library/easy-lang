(xdump (+ 11 22))

(xdump command-line-args)
(xdump command-line-args-left)

(defun test-func ()
  (xprint "test-func() started")
  (xdump command-line-args-left)
  )


(class! <rectangle2> ()
        (:width  1.0)
        (height 1.0)
        :temp
        )

(cl-defmethod !area ((this <rectangle2>))
  (with-slots
      (width height)
      this
    (* width height)))
(setq rect (make-instance <rectangle2> :width 30 :height 20))
(xdump (slot-value rect :width))
(xdump (slot-value rect :height))
(xdump (!area rect))
(set-slot-value rect :height 10)
(xdump (!area rect))
