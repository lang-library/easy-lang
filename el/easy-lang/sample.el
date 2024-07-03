;;(load "~/.emacs.d/xprint.el")
;;(require 'eieio)
(!class <rectangle> ()
  ((:width  1.0)
   (:height 1.0)
   :temp)
  )
(!method !area ((this <rectangle>))
  (with-slots
      (width height)
      this
    (return (* width height))))
(setq rect (!new <rectangle> :width 20 :height 10))
(xdump [! rect :width])
(xdump [! rect :height])
(xdump [! rect !area])

[! ary [0]]
[! ht [:key]]
