# - try to find the PSMoveService SDK - currently designed for the version on GitHub.
#
# Cache Variables: (probably not for direct use in your scripts)
#  PSM_INCLUDE_DIR
#
# Non-cache variables you might use in your CMakeLists.txt:
#  PSM_FOUND
#  PSM_INCLUDE_DIR
#  PSM_BINARIES_DIR
#  PSM_LIBRARIES
#
# Requires these CMake modules:
#  FindPackageHandleStandardArgs (known included with CMake >=2.6.2)
#
# Adapted from the OpenVR cmake script which was adapter from the LibUSB cmake script
#
# Redistribution and use is allowed according to the terms of the BSD license.
# For details see the accompanying COPYING-CMAKE-SCRIPTS file.

message(STATUS "Finding PSMoveService, CMAKE_CURRENT_LIST_DIR=${CMAKE_CURRENT_LIST_DIR}/../../../deps/PSMoveService/src/PSMoveService")

IF (PSM_INCLUDE_DIR AND PSM_LIBRARIES AND PSM_BINARIES_DIR)
    # in cache already
    set(PSM_FOUND TRUE)

ELSE (PSM_INCLUDE_DIR AND PSM_LIBRARIES AND PSM_BINARIES_DIR)
    set(PSM_ROOT_DIR ${CMAKE_CURRENT_LIST_DIR}/../../../deps/PSMoveService/src/PSMoveService)

    # Find the include path
    find_path(PSM_INCLUDE_DIR
        NAMES
            ClientConstants.h ClientGeometry_CAPI.h ProtocolVersion.h PSMoveClient_CAPI.h PSMoveClient_export.h SharedConstants.h
        PATHS 
            ${PSM_ROOT_DIR}/include
            /usr/local/include)

    # Find the libraries to include
    set(PSM_LIB_SEARCH_PATH ${PSM_ROOT_DIR}/lib)
    IF(${CMAKE_SYSTEM_NAME} MATCHES "Windows")
        IF (${CMAKE_C_SIZEOF_DATA_PTR} EQUAL 8)
            list(APPEND PSM_LIB_SEARCH_PATH
                ${PSM_ROOT_DIR}/lib/win64)
        ELSE()
            list(APPEND PSM_LIB_SEARCH_PATH
                ${PSM_ROOT_DIR}/lib/win32)
        ENDIF()
    ELSEIF(${CMAKE_SYSTEM_NAME} MATCHES "Linux")
        IF (${CMAKE_C_SIZEOF_DATA_PTR} EQUAL 8)
            list(APPEND PSM_LIB_SEARCH_PATH
                ${PSM_ROOT_DIR}/lib/linux64
                /usr/local/lib)
        ELSE()
            list(APPEND PSM_LIB_SEARCH_PATH
                ${PSM_ROOT_DIR}/lib/linux32
                /usr/local/lib)  
        ENDIF()      
    ELSEIF(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
        list(APPEND PSM_LIB_SEARCH_PATH
            ${PSM_ROOT_DIR}/lib/osx32
            /usr/local/lib)
    ENDIF()
    
    FIND_LIBRARY(PSM_LIBRARY
            NAMES PSMoveClient_CAPI.dylib PSMoveClient_CAPI.so PSMoveClient_CAPI.lib PSMoveClient_CAPI
            PATHS ${PSM_LIB_SEARCH_PATH})
    SET(PSM_LIBRARIES ${PSM_LIBRARY})
    
    # Find the path to copy DLLs from
    set(PSM_BIN_SEARCH_PATH ${PSM_ROOT_DIR}/bin)
    IF(${CMAKE_SYSTEM_NAME} MATCHES "Windows")
        IF (${CMAKE_C_SIZEOF_DATA_PTR} EQUAL 8)
            list(APPEND PSM_BIN_SEARCH_PATH
                ${PSM_ROOT_DIR}/bin/win64)
        ELSE()
            list(APPEND PSM_BIN_SEARCH_PATH
                ${PSM_ROOT_DIR}/bin/win32)
        ENDIF()
    ELSEIF(${CMAKE_SYSTEM_NAME} MATCHES "Linux")
        IF (${CMAKE_C_SIZEOF_DATA_PTR} EQUAL 8)
            list(APPEND PSM_BIN_SEARCH_PATH
                ${PSM_ROOT_DIR}/bin/linux64
                /usr/local/bin)
        ELSE()
            list(APPEND PSM_BIN_SEARCH_PATH
                ${PSM_ROOT_DIR}/bin/linux32
                /usr/local/bin)  
        ENDIF()      
    ELSEIF(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
        list(APPEND PSM_BIN_SEARCH_PATH
            ${PSM_ROOT_DIR}/bin/osx32
            /usr/local/bin)
    ENDIF()

    find_path(PSM_BINARIES_DIR
        NAMES
            PSMoveClient_CAPI.so PSMoveClient_CAPI.dylib PSMoveClient_CAPI.dll
        PATHS 
            ${PSM_BIN_SEARCH_PATH})    

    # Register PSM_LIBRARIES, PSM_INCLUDE_DIR, PSM_BINARIES_DIR
    include(FindPackageHandleStandardArgs)
    FIND_PACKAGE_HANDLE_STANDARD_ARGS(PSM DEFAULT_MSG PSM_LIBRARIES PSM_INCLUDE_DIR PSM_BINARIES_DIR)

    MARK_AS_ADVANCED(PSM_INCLUDE_DIR PSM_LIBRARIES)           
     
ENDIF (PSM_INCLUDE_DIR AND PSM_LIBRARIES AND PSM_BINARIES_DIR)