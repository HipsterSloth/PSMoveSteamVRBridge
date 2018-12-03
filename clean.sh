#!/bin/bash

#Function loads configuration properties and generates project files
main(){
	#Initialise
	PROJECT_ROOT=$PWD
	BUILD_PROPS_FILE=$PROJECT_ROOT/build.properties

	#Clean
	cleanProjectFolder || $?
	cleanBuildFolder || $?
	cleanDependencies || $?

	# Exit batch script successfully
    echo -e "\E[1;32mCLEAN SUCCESSFUL\E[;0m";
	exit 0
}

#Clean up the old PSMoveSteamVRBridge generated project folder
cleanProjectFolder(){
	if [ -d "$PROJECT_ROOT/generated" ]; then
		echo "Cleaning old generated folder..."
		rm -rf $PROJECT_ROOT/generated
	fi
}

#Clean up the old PSMoveSteamVRBridge target build folder
cleanBuildFolder(){
	if [ -d "$PROJECT_ROOT/build" ]; then
		echo "Cleaning old build folder..."
		rm -rf $PROJECT_ROOT/build
	fi
}

#Clean up the old PSMoveSteamVRBridge deps folder
cleanDependencies(){
	if [ -d "$PROJECT_ROOT/deps" ]; then
		echo "Cleaning old deps folder..."
		rm -rf $PROJECT_ROOT/deps
	fi
}

#Function to handle errors
function handleError() {
   error_msg="CLEAN FAILED"
   echo -e "\E[1;31m$error_msg\E[;0m";
   exit 1;
}

# this will trap any errors or commands with non-zero exit status
# by calling function catch_errors()
trap handleError ERR;

# entry point of script
main
