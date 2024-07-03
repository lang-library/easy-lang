#! /usr/bin/env bash
set -uvx
set -e
#emacs -batch -l easy-lang.el a b "c d"
emacs -batch -l boot.el -l easy-lang.el -l test.el -f test-func a b "c d"
