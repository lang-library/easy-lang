#! /usr/bin/env bash
set -uvx
set -e
#emacs -batch -l easy-lang.el a b "c d"
emacs -batch -l easy-lang.el -l test.el a b "c d"
