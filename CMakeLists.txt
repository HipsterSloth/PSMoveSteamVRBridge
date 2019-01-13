cmake_minimum_required(VERSION 3.8)

project(PSMoveSteamVRBridge VERSION 0.1.0)

include(${CMAKE_CURRENT_SOURCE_DIR}/src/main/cmake/CMakeUtils.txt)
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

# Set project info
set(PSM_DRIVER_PROJECT_NAME "PSMoveSteamVRBridge")
set(TARGET_BUILD_DIR "build")
message(STATUS "Project driver version: ${DRIVER_VERSION}")
message(STATUS "Project name: ${PSM_DRIVER_PROJECT_NAME}")
message(STATUS "Target build directory: ${TARGET_BUILD_DIR}")

# Configure and build external projects
build_external_project(PSMoveService ${ROOT_DIR}/deps ${PSM_PACKAGE_URL} "")
build_external_project(OpenVR ${ROOT_DIR}/deps ${OPENVR_PACKAGE_URL} "")
  
# Configure the cmake for the driver
add_subdirectory(src)
