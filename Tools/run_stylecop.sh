#!/bin/sh
IGNORED_DIRS=$(paste -s Tools/stylecop_blacklist)
exec stylecop Settings.StyleCop Assets/ ${IGNORED_DIRS}
