(provide 'easy-lang)

(load "~/.emacs.d/xprint.el")
(load "~/.emacs.d/oop.el")
(load "~/.emacs.d/getprop")
;;(require 'eieio)
(require 'ert)

(xdump command-line-args)
(xdump command-line-args-left)

(ert-deftest my-test-00 ()
  ;;(xclear)
  (!class <rectangle> ()
          width
          height)
  (!method ^area ((this <rectangle>))
    (with-slots
        (width height)
        this
      (* width height)))
  (setq rect (make-instance <rectangle> :width 20 :height 10))
  (xdump (! rect ^area))
  (should (equal (! rect ^area) 200))
  (setf (getprop rect :width) 777)
  (! rect (putprop ! :height 20))
  (xdump (! rect :width))
  (should (equal (! rect :width) 777))
  (xdump (! rect :height))
  (should (equal (! rect :height) 20))
  (xdump (! rect ^area))
  (should (equal (! rect ^area) 15540))
  (xdump (object-of-class-p rect <rectangle>))
  (xdump (eieio-object-p rect))
  (setq h (make-hash-table :test #'equal))
  (setf (getprop h :abc) "abc")
  (xpand-1 (! h :abc))
  (xdump (! h :abc))
  (setf ary [111 222 333])
  (xdump ary)
  (xdump (getprop ary 1))
  (setf (getprop ary 1) 777)
  (xdump ary)
  (xdump-json ary)
  (xdump-json nil)
  (xdump-json h)
  (xdump-json (list "a" "b" "c"))
  (xpand-1 (! x :width (elt ! 0) ^method-1 (^method-2 a ! b)))
  )

;;(ert-delete-all-tests)

(ert-deftest my-test-01 ()
  "my-test01 is calculation test."
  (should (equal (+ 11 22) 33)))

;;(xdump (ert-test-passed-p (ert-run-test (ert-get-test 'my-test-01))))

(ert t)
