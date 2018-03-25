cmake_minimum_required(VERSION 3.0)

# initialise variables
set(ROOT_DIR ${CMAKE_CURRENT_LIST_DIR}/..)
set(PROJECT_SRC_DIR ${ROOT_DIR}/src/main/cpp/driver)
set(OPENVR_COMPAT_HEADERS ${ROOT_DIR}/src/main/cpp/openvr-driver-compat)
set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} ${CMAKE_CURRENT_LIST_DIR}/main/cmake)
message(STATUS "CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH}")
message(STATUS "Running cmake in ${CMAKE_CURRENT_LIST_DIR}")
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


##############################################################################################
# Looks up the required include,library and binary files for each respective external project#
##############################################################################################

#OpenVR references
FIND_PACKAGE(OpenVR REQUIRED)
set(OPENVR_PLUGIN_INCL_DIRS)
set(OPENVR_PLUGIN_REQ_LIBS)
set(OPENVR_MONITOR_INCL_DIRS)
set(OPENVR_MONITOR_REQ_LIBS)
list(APPEND OPENVR_PLUGIN_INCL_DIRS ${OPENVR_INCLUDE_DIR})
list(APPEND OPENVR_PLUGIN_REQ_LIBS ${OPENVR_LIBRARIES})    
list(APPEND OPENVR_MONITOR_INCL_DIRS ${OPENVR_INCLUDE_DIR})
list(APPEND OPENVR_MONITOR_REQ_LIBS ${OPENVR_LIBRARIES})   
    
# PSMoveService references
FIND_PACKAGE(PSMoveService REQUIRED)
set(PSM_PLUGIN_INCL_DIRS)
set(PSM_PLUGIN_REQ_LIBS)
set(PSM_MONITOR_INCL_DIRS)
set(PSM_MONITOR_REQ_LIBS)
list(APPEND PSM_PLUGIN_INCL_DIRS ${PSM_INCLUDE_DIR})
list(APPEND PSM_PLUGIN_REQ_LIBS ${PSM_LIBRARIES})    
list(APPEND PSM_MONITOR_INCL_DIRS ${PSM_INCLUDE_DIR})
list(APPEND PSM_MONITOR_REQ_LIBS ${PSM_LIBRARIES})   

##############################################################################################
# This driver makes two build artifacts, the monitor app binary and the psmove driver library#
##############################################################################################

# Shared library: defines what is compiled, what output type it is and what libs/includes it needs
add_library(driver_psmove SHARED ${PROJECT_SRC_DIR}/driver_psmoveservice.cpp)
target_include_directories(driver_psmove PUBLIC ${OPENVR_PLUGIN_INCL_DIRS} ${PSM_PLUGIN_INCL_DIRS} ${OPENVR_COMPAT_HEADERS})
target_link_libraries(driver_psmove ${OPENVR_PLUGIN_REQ_LIBS} ${PSM_PLUGIN_REQ_LIBS})

# Monitor app: defines what is compiled, what output type it is and what libs/includes it needs
add_executable(monitor_psmove ${PROJECT_SRC_DIR}/monitor_psmoveservice.cpp)
target_include_directories(monitor_psmove PUBLIC ${OPENVR_MONITOR_INCL_DIRS} ${PSM_PLUGIN_INCL_DIRS})
target_link_libraries(monitor_psmove ${OPENVR_MONITOR_REQ_LIBS} ${PSM_PLUGIN_REQ_LIBS})

##############################################################################################
# Install: defines how the target build is made, i.e. what libraries, binaries, config files #
#          and models get packaged up with the final release.                                #
##############################################################################################
IF(${WIN32})
	install(TARGETS driver_psmove
		RUNTIME DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin
		LIBRARY DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib
		ARCHIVE DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib)
	install(FILES $<TARGET_PDB_FILE:driver_psmove> 
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin OPTIONAL)
	install(TARGETS monitor_psmove
		RUNTIME DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin
		LIBRARY DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib
		ARCHIVE DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib)    
	install(FILES $<TARGET_PDB_FILE:monitor_psmove> 
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin OPTIONAL)
	install(DIRECTORY ${OPENVR_BINARIES_DIR}/ 
			DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin
			FILES_MATCHING PATTERN "*.dll" PATTERN "*.pdb")     
	install(DIRECTORY ${CMAKE_CURRENT_LIST_DIR}/
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin
		FILES_MATCHING PATTERN "*${ARCH_LABEL}.bat" PATTERN "*.vrdrivermanifest")
	install(DIRECTORY ${CMAKE_CURRENT_LIST_DIR}/resources
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin
		FILES_MATCHING PATTERN "*.png" PATTERN "*.tga" PATTERN "*.json" PATTERN "*.obj" PATTERN "*.mtl")
	install(DIRECTORY ${CMAKE_CURRENT_LIST_DIR}/configuration
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin
		FILES_MATCHING PATTERN "*.vricons")
	install(DIRECTORY ${PSM_BIN_DIR}/assets
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin OPTIONAL)
	install(FILES ${PSM_BIN_DIR}/PSMoveClient_CAPI.dll
	   DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/bin OPTIONAL)
ELSE() #Linux/Darwin
	# TODO
ENDIF()
