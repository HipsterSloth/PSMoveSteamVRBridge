#!/bin/bash

#Function to build driver
buildDriver(){
	local BUILD_TYPE=$1
	cmake --build ide --target INSTALL --config $BUILD_TYPE || return $?
    echo BUILD SUCCESS
    echo -e "\E[1;32mBUILD SUCCESS\E[;0m";
	exit 0
}

#Function to handle errors
function handleError() {
   error_msg="BUILD FAILED"
   echo -e "\E[1;31m$error_msg\E[;0m";
   exit 1;
}


# this will trap any errors or commands with non-zero exit status
# by calling function catch_errors()
trap handleError ERR;

#Main entry point
buildDriver
