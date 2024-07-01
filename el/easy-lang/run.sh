#! /usr/bin/env bash
set -uvx
set -e
emacs -batch -l easy-lang.el a b "c d"
