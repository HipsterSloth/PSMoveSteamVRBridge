cmake_minimum_required(VERSION 3.2)
project(PSMoveSteamVRBridge VERSION 0.1.0)
include(ExternalProject)

set(ROOT_DIR ${CMAKE_CURRENT_LIST_DIR})

LIST(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
IF (NOT MSVC)
    set(CMAKE_CXX_FLAGS "-std=c++11")
    IF (NOT CMAKE_COMPILER_IS_GNUCXX)
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -stdlib=libc++")
    ENDIF()
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-switch")
ENDIF()

# Disable GLM warnings in Darwin
IF(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
    #warning: 'register' storage class specifier is deprecated and incompatible with C++1z
    #warning: operator '<<' has lower precedence than '-'; '-' will be evaluated first
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-deprecated-register -Wno-shift-op-parentheses")
ENDIF()

set_property(GLOBAL PROPERTY USE_FOLDERS ON)

# Shared architecture label used for install folder locations
if (${CMAKE_C_SIZEOF_DATA_PTR} EQUAL 8)
    if(${CMAKE_SYSTEM_NAME} MATCHES "Windows")
        set(ARCH_LABEL "Win64")
    elseif(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
        set(ARCH_LABEL "OSX64")
    else()
        set(ARCH_LABEL "Linux64")
    endif()
else()
    if(${CMAKE_SYSTEM_NAME} MATCHES "Windows")
        set(ARCH_LABEL "Win32")
    elseif(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
        set(ARCH_LABEL "OSX32")
    else()
        set(ARCH_LABEL "Linux32")
    endif()
endif()

#------------------------
# Project Version
#------------------------

# Retrieve version from header file
set(PSM_DRIVER_VERSION_HEADER_FILE "${ROOT_DIR}/src/openvr_plugin/driver_version.h")

file(STRINGS ${PSM_DRIVER_VERSION_HEADER_FILE} PSM_DRIVER_VERSION_PARTS
  REGEX "^#define[ \t]+PSM_DRIVER_VERSION_(MAJOR|MINOR|HOTFIX)[ \t]+[0-9]+$")
  
# Verify version parts
string(REGEX MATCH "PSM_DRIVER_VERSION_MAJOR[ \t]+[0-9]+" PSM_DRIVER_VERSION_MAJOR  ${PSM_DRIVER_VERSION_PARTS})
string(REGEX MATCH "PSM_DRIVER_VERSION_MINOR[ \t]+[0-9]+" PSM_DRIVER_VERSION_MINOR  ${PSM_DRIVER_VERSION_PARTS})
string(REGEX MATCH "PSM_DRIVER_VERSION_HOTFIX[ \t]+[0-9]+" PSM_DRIVER_VERSION_HOTFIX  ${PSM_DRIVER_VERSION_PARTS})

if(NOT PSM_DRIVER_VERSION_MAJOR 
    OR NOT PSM_DRIVER_VERSION_MINOR 
    OR NOT PSM_DRIVER_VERSION_HOTFIX)
  message(FATAL_ERROR "Unable to retrieve project version from ${PSM_DRIVER_VERSION_HEADER_FILE}")
endif()

# Extract version numbers
string(REGEX REPLACE "PSM_DRIVER_VERSION_MAJOR[ \t]+([0-9]+)" "\\1" PSM_DRIVER_VERSION_MAJOR  ${PSM_DRIVER_VERSION_MAJOR})
string(REGEX REPLACE "PSM_DRIVER_VERSION_MINOR[ \t]+([0-9]+)" "\\1" PSM_DRIVER_VERSION_MINOR  ${PSM_DRIVER_VERSION_MINOR})
string(REGEX REPLACE "PSM_DRIVER_VERSION_HOTFIX[ \t]+([0-9]+)" "\\1" PSM_DRIVER_VERSION_HOTFIX  ${PSM_DRIVER_VERSION_HOTFIX})

set(PSM_DRIVER_VERSION_STRING "${PSM_DRIVER_VERSION_MAJOR}.${PSM_DRIVER_VERSION_MINOR}.${PSM_DRIVER_VERSION_HOTFIX}")

message(STATUS "Project version: ${PSM_DRIVER_VERSION_STRING}")

set(PSM_DRIVER_PROJECT_NAME "PSMoveSteamVRBridge_${PSM_DRIVER_VERSION_STRING}")

# PSMoveService Build

# Make sure psmoveservice build URL has been specified
IF(NOT PSMOVESERVICE_BUILD_URL)
    message(FATAL_ERROR "Please set PSMoveService build URL using -DPSMOVESERVICE_BUILD_URL=/path/to/psmoveservice/build.zip")
ELSE()
    message(STATUS "PSMoveService Build URL: ${PSMOVESERVICE_BUILD_URL}")
ENDIF()

ExternalProject_Add(
    PSMoveService
    PREFIX ${ROOT_DIR}/deps/PSMoveService
    URL ${PSMOVESERVICE_BUILD_URL}
    # Disable install step
    INSTALL_COMMAND ""
    # Disable the configure step
    CONFIGURE_COMMAND ""
    # Disable the build step
    BUILD_COMMAND ""    
    # Wrap download, configure and build steps in a script to log output
    LOG_DOWNLOAD ON
    LOG_CONFIGURE ON
    LOG_BUILD ON
    INSTALL_DIR ${ROOT_DIR}/${PSM_DRIVER_PROJECT_NAME}/${ARCH_LABEL}/)
    
# Get the PSMoveClient_CAPI include and lib paths
ExternalProject_Get_Property(PSMoveService source_dir)
set(PSMOVESERVICE_BIN_DIR ${source_dir}/bin)
set(PSMOVECLIENT_INCLUDE_DIR ${source_dir}/include)
set(PSMOVECLIENT_LIB_DIR ${source_dir}/lib)
if(${CMAKE_SYSTEM_NAME} MATCHES "Windows")
  set(PSMOVECLIENT_LIB_FILE "PSMoveClient_CAPI.lib")
else()
  set(PSMOVECLIENT_LIB_FILE "libPSMoveClient_CAPI.a")
endif()
    
# Step into the subdirectories
add_subdirectory(src)
