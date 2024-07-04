(provide 'easy-lang)

(load "~/.emacs.d/xprint.el")
(require 'eieio)

(xdump command-line-args)
(xdump command-line-args-left)

;;(find-file "test.el")
;;(goto-char (point-min))
;;(setq sxp (read (current-buffer)))
;;(xdump sxp)

(defun getprop ($x $key)
  (cond
   ((eieio-object-p $x)
    (slot-value $x $key))
   ((hash-table-p $x)
    (gethash $key $x))
   (t (elt $x $key))))

(defun putprop ($x $key $store)
  (cond
   ((eieio-object-p $x)
    (set-slot-value $x $key $store))
   ((hash-table-p $x)
    (puthash $key $store $x))
   (t (setf (elt $x $key) $store))))

(gv-define-setter getprop ($store $seq $n)
  `(putprop ,$seq ,$n ,$store))

(defmacro ! ($x &rest $specs)
  (setf $specs (!::specs $specs))
  (dolist ($spec $specs)
    (setf $x (!::replace $spec $x))
    )
  $x
  )

(defun !::replace ($spec $x)
  (let ( $result )
    (dolist ($e $spec (nreverse $result))
      (if (eq $e '!)
          (push $x $result)
        (push $e $result)
        )
      )
    )
  )

(defun !::specs ($specs)
  (let* ( $result )
    (dolist ($spec $specs (nreverse $result))
      ;;(xdump $spec 1)
      (setf $spec (!::spec $spec))
      ;;(xdump $spec 2)
      (push $spec $result)
      )
    )
  )

(defun !::spec ($spec)
  (cond
   ((and (listp $spec) (memq '! $spec))
    $spec)
   ((and (symbolp $spec) (string-match "^\\^" (symbol-name $spec)))
    `(,$spec !))
   (t
    `(getprop ! ,$spec))
   )
  )

(defmacro !class ($class $super &rest $spec-list)
  (if (not (listp $spec-list))
      (error "$spec list is not list")
    (let ( $spec-list2 )
      (dolist ($spec $spec-list)
        (setq $spec-list2 (nconc $spec-list2 (list (!class::spec $spec))))
        )
      (let (($form `(defclass ,$class ,$super ,$spec-list2)))
        ;;(xdump $form)
        $form
        )
      )
    )
  )

(defun !class::spec ($spec)
  (let* (($sym-name (!class::symbol-name $spec))
         ($ini-form (!class::init-form $spec))
         )
    `( ,(intern $sym-name)
       :initarg ,(intern (concat ":" $sym-name))
       :initform ,$ini-form )
    )
  )

(defun !class::symbol-name ($spec)
  (let* (($sym $spec))
    (if (listp $sym)
        (setq $sym (nth 0 $sym)))
    (let* (($sym-name (symbol-name $sym)))
      (if (string-match "^:" $sym-name)
          (setf $sym-name (substring $sym-name 1)))
      $sym-name
      )
    )
  )

(defun !class::init-form ($spec)
  ;;(xprint "!class::init-form() called")
  (if (symbolp $spec)
      nil
    (nth 1 $spec)))

(progn
  (xclear)
  (!class <rectangle> ()
          width
          height)
  (cl-defmethod ^area ((this <rectangle>))
    (with-slots
        (width height)
        this
      (* width height)))
  (setq rect (make-instance <rectangle> :width 20 :height 10))
  (setf (getprop rect :width) 777)
  (! rect (putprop ! :width 888))
  (setf (slot-value rect :height) 100)
  (xdump (! rect :width))
  (xdump (! rect :height))
  (xdump (! rect ^area))
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

(ert-deftest pp-test-quote ()
  "Tests the rendering of `quote' symbols in `pp-to-string'."
  (should (equal (pp-to-string '(quote quote)) "'quote"))
  (should (equal (pp-to-string '((quote a) (quote b))) "('a 'b)\n"))
  (should (equal (pp-to-string '('a 'b)) "('a 'b)\n")))

(ert-delete-all-tests)

(ert-deftest my-test-01 ()
  "my-test01 is calculation test."
  (should (equal (+ 11 22) 34)))

(xdump (ert-test-passed-p (ert-run-test (ert-get-test 'my-test-01))))

;;(ert t)
