(provide 'easy-lang)

(load "~/.emacs.d/xprint.el")

(setq loop-count 5)
(dotimes (i loop-count)
  (xdump (1+ i))
  )
(setq misc-list '(11 22 abc "def ghi"))
(dolist (x misc-list)
  (xdump x)
  )

(defvar *easy-language-dummy* 777)

(xprint "easy-lang-main(1)")

(defun easy-lang-main ()
  (xprint "easy-lang-main")
  )
  
(easy-lang-main)

(xdump command-line-args)
(xdump command-line-args-left)

(find-file "test.el")
(goto-char (point-min))
(setq sxp (read (current-buffer)))
(xdump sxp)

(defmacro !class ($class $super &rest $spec-list)
  (xdump $class)
  (xdump $super)
  (xdump $spec-list)
  (if (not (listp $spec-list))
      (error "$spec list is not list")
    (dolist ($spec $spec-list)
      ;;(xdump $spec)
      (xdump (!class::spec $spec))
      )
    )
  nil
  )

(defun !class::spec ($spec)
  (xdump $spec)
  (let* (($sym-name (!class::symbol-name $spec))
         ($ini-form (!class::init-form $spec))
         )
    (xdump $sym-name)
    (xdump $ini-form)
    )
  nil
  )

(defun !class::symbol-name ($spec)
  (xprint "!class::symbol-name() called")
  (let* (($sym $spec))
    (if (listp $sym)
        (setq $sym (nth 0 $sym)))
    (let* (($sym-name (symbol-name $sym)))
      (if (string-match "^:" $sym-name)
          (setf $sym-name (substring $sym-name 1)))
      (xdump $sym-name 1)
      $sym-name
      )
    )
  )

(defun !class::init-form ($spec)
  (xprint "!class::init-form() called")
  (if (symbolp $spec)
      nil
    (nth 1 $spec)))
