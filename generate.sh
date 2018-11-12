#!/bin/bash
set -eE
###########################################################################
# Author: Stephen O'Hair
#                                                                     
# Purpose: Generates the openvr driver make project files for use on linux.
###########################################################################

# Initialise
PROJECT_ROOT=$PWD
BUILD_PROPS_FILE=$PROJECT_ROOT/build.properties

#Function loads configuration properties and generates project files
main(){
	# Load configuration properties
	loadBuildProperties || return $?

	# Generate the project files for PSMoveService
	generateProjectFiles || return $?

	# Exit batch script successfully
    echo -e "\E[1;32mBUILD SUCCESS\E[;0m";
	exit 0
}

#Function loads build properties into local variables
loadBuildProperties(){
    echo Loading properties from $BUILD_PROPS_FILE
    PSM_PACKAGE_URL=$(loadBuildProperty "psmoveservice.package.url" $BUILD_PROPS_FILE)
	echo PSM_PACKAGE_URL=$PSM_PACKAGE_URL

	OPENVR_PACKAGE_URL=$(loadBuildProperty "openvr.package.url" $BUILD_PROPS_FILE)
	echo OPENVR_PACKAGE_URL=$OPENVR_PACKAGE_URL

	DRIVER_VERSION=$(loadBuildProperty "driver.version" $BUILD_PROPS_FILE)
	echo DRIVER_VERSION=$DRIVER_VERSION

	BUILD_GENERATOR=$(loadBuildProperty "cmake.build.generator.linux" $BUILD_PROPS_FILE)
	echo BUILD_GENERATOR=$BUILD_GENERATOR

	BUILD_TYPE=$(loadBuildProperty "build.type" $BUILD_PROPS_FILE)
	echo BUILD_TYPE=$BUILD_TYPE

	echo Properties loaded successfully
}

#Fuction returns a configured build property value for the given key
loadBuildProperty(){
	local PROP_KEY=$1
	local FILE=$2
    grep "$PROP_KEY" $FILE | cut -d'=' -f2
}

#Function generates project files for the configured ide
generateProjectFiles(){
	if [ ! -d "$PROJECT_ROOT/generated" ]; then
		mkdir $PROJECT_ROOT/generated
	fi
	echo "Rebuilding PSMoveSteamVRBridge Project files..."
	echo "Running cmake in $PROJECT_ROOT"
	cd $PROJECT_ROOT/generated
	#TODO - This should use cmakes configure file instead	
	cmake .. -G "$BUILD_GENERATOR" -DDRIVER_VERSION="$DRIVER_VERSION" -DPSM_PACKAGE_URL="$PSM_PACKAGE_URL" -DOPENVR_PACKAGE_URL="$OPENVR_PACKAGE_URL"
}

#Function to handle errors
function handleError() {
   error_msg="GENERATE FAILED"
   echo -e "\E[1;31m$error_msg\E[;0m";
   exit 1;
}


# this will trap any errors or commands with non-zero exit status
# by calling function catch_errors()
trap handleError ERR;

# entry point of script
main
