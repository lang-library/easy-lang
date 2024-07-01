(load "~/.emacs.d/xprint.el")
(require 'eieio)

(defclass <rectangle> ()
  ((width  :initarg :width)
   (height :initarg :height))
  )

(cl-defmethod !area ((this <rectangle>))
  (with-slots
      (width height)
      this
    (* width height)))

(setq rect (make-instance <rectangle> :width 20 :height 10))

(xdump (!area rect))
