#!/bin/sh

echo "::debug::Current working directory: $(pwd)"

ls -al | awk '{print "::debug::" $0}'
