(provide 'easy-lang)

(load "~/.emacs.d/xprint.el")
(require 'eieio)

(xdump command-line-args)
(xdump command-line-args-left)

;;(find-file "test.el")
;;(goto-char (point-min))
;;(setq sxp (read (current-buffer)))
;;(xdump sxp)

(defmacro class! ($class $super &rest $spec-list)
  (if (not (listp $spec-list))
      (error "$spec list is not list")
    (let ( $spec-list2 )
      (dolist ($spec $spec-list)
        (setq $spec-list2 (nconc $spec-list2 (list (class!::spec $spec))))
        )
      (let (($form `(defclass ,$class ,$super ,$spec-list2)))
        ;;(xdump $form)
        $form
        )
      )
    )
  )

(defun class!::spec ($spec)
  (let* (($sym-name (class!::symbol-name $spec))
         ($ini-form (class!::init-form $spec))
         )
    `( ,(intern $sym-name)
       :initarg ,(intern (concat ":" $sym-name))
       :initform ,$ini-form )
    )
  )

(defun class!::symbol-name ($spec)
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

(defun class!::init-form ($spec)
  ;;(xprint "class!::init-form() called")
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
        (if (not (string-match "^!" (symbol-name $slot)))
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

(progn
  (xclear)
  (xpand (! x (:width1)))
  (xpand (! x [:width2]))
  (xpand (! x :width3))
  (xpand (! rect [!area]))

  (class! <rectangle> ()
          width
          height)
  (cl-defmethod !area ((this <rectangle>))
    (with-slots
        (width height)
        this
      (* width height)))
  (setq rect (make-instance <rectangle> :width 20 :height 10))
  (xdump (! rect :width))
  (xdump (! rect [:height]))
  (xdump (!area rect))
  (xdump (! rect [!area]))
  )
