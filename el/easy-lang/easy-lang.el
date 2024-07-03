(provide 'easy-lang)

(load "~/.emacs.d/xprint.el")
(require 'eieio)

(xdump command-line-args)
(xdump command-line-args-left)

;;(find-file "test.el")
;;(goto-char (point-min))
;;(setq sxp (read (current-buffer)))
;;(xdump sxp)

(defun prop ($x $key)
  (if (eieio-object-p $x)
      (slot-value $x $key)
    (gethash $key $x)))

(gv-define-setter prop ($store $seq $n)
  `(if (eieio-object-p ,$seq)
      (set-slot-value ,$seq ,$n ,$store)
    (puthash ,$n ,$store ,$seq)))

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

(defmacro ! ($exp $slots)
  (setf $slots (!::to-list $slots))
  ;;`(,$exp ,$slots)
  (catch 'return
    (cl-do*
        (($i 0 (1+ $i)))
        ((>= $i (length $slots)) $exp)
      (let (($slot (elt $slots $i)))
        (if (not (string-match "^\\^" (symbol-name $slot)))
            (setf $exp `(slot-value ,$exp ,$slot))
            (let (($args (nthcdr (1+ $i) $slots)))
              (xdump $args)
              (throw 'return `(,$slot ,$exp ,@$args))
              )
            )
        )
      )
    )
  )

(defun !::to-list ($x)
  (cond
   ((listp $x) $x)
   ((vectorp $x)
    (cl-do*
        (($i 0 (1+ $i))
         ($result nil))
        ((>= $i (length $x)) (nreverse $result))
      (let* (($sym (aref $x $i)))
        (push $sym $result)
        )
      )
    )
   ( t (list $x) )
   )
  )

(gv-define-setter ! ($store $seq $n)
  (setf $n (!::to-list $n))
  ;;(xdump $store)
  ;;(xdump $seq)
  ;;(xdump $n)
  (let* (($rev (reverse $n)))
    ;;(xdump $rev)
    (setf $seq (macroexpand-1 `(! ,$seq ,(reverse (cdr $rev)))))
    ;;(xdump $seq)
    `(set-slot-value ,$seq ,(car $rev) ,$store)
    )
  )

(progn
  (xclear)
  (xpand (! x [:width2]))
  (xpand (! x :width3))
  (xpand (! rect [^area]))

  (!class <rectangle> ()
          width
          height)
  (cl-defmethod ^area ((this <rectangle>))
    (with-slots
        (width height)
        this
      (* width height)))
  (setq rect (make-instance <rectangle> :width 20 :height 10))
  (xpand-1 (setf (! rect [:x :y :width]) 777))
  (xpand-1 (setf (! rect [:width]) 777))
  (setf (prop rect :width) 777)
  (setf (slot-value rect :height) 100)
  (xdump (! rect :width))
  (xdump (! rect [:height]))
  (xdump (^area rect))
  (xdump (! rect [^area]))
  (xdump (object-of-class-p rect <rectangle>))
  (xdump (eieio-object-p rect))
  (setq h (make-hash-table :test #'equal))
  (setf (prop h :abc) "abc")
  (xdump (prop h :abc))
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
