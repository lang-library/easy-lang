;;(load "~/.emacs.d/xprint.el")
;;(require 'eieio)
(!class <rectangle> ()
        (:width  1.0)
        (:height 1.0)
        :temp
        )
(!method ^area ((this <rectangle>))
  (with-slots
      (width height)
      this
    (return (* width height))))
(setq rect (!new <rectangle> :width 20 :height 10))
(xdump (! rect :width))
(xdump (! rect :height))
(xdump (! rect ^area))

(!get ary (elt ! 0) :width)
(!get ht (gethash ! :key))
(!get x :prop (elt ! 0) (hashget ! "key") (^method-a a ! c) :prop2)
(!put $height x :prop (elt ! 0) ^set-height)
