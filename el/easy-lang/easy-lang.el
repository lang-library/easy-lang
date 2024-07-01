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

(provide 'easy-lang)
