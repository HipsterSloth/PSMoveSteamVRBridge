cmake_minimum_required(VERSION 3.0)

# initialise variables
set(ROOT_DIR ${CMAKE_CURRENT_LIST_DIR}/..)
set(PROJECT_SRC_DIR ${ROOT_DIR}/src/main/cpp/driver)
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
# Looks up the required include, library and binary files for each respective external project#
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
add_library(driver_psmove SHARED ${PROJECT_SRC_DIR}/driver.cpp 
								 ${PROJECT_SRC_DIR}/constants.h
								 ${PROJECT_SRC_DIR}/controller.h
								 ${PROJECT_SRC_DIR}/controller.cpp
								 ${PROJECT_SRC_DIR}/driver.h
								 ${PROJECT_SRC_DIR}/driver.cpp
								 ${PROJECT_SRC_DIR}/facing_handsolver.h
								 ${PROJECT_SRC_DIR}/facing_handsolver.cpp
								 ${PROJECT_SRC_DIR}/logger.h
								 ${PROJECT_SRC_DIR}/logger.cpp
								 ${PROJECT_SRC_DIR}/ps_ds4_controller.h
								 ${PROJECT_SRC_DIR}/ps_ds4_controller.cpp
								 ${PROJECT_SRC_DIR}/ps_move_controller.h
								 ${PROJECT_SRC_DIR}/ps_move_controller.cpp
								 ${PROJECT_SRC_DIR}/ps_navi_controller.h
								 ${PROJECT_SRC_DIR}/ps_navi_controller.cpp
								 ${PROJECT_SRC_DIR}/server_driver.h
								 ${PROJECT_SRC_DIR}/server_driver.cpp
								 ${PROJECT_SRC_DIR}/settings_util.h
								 ${PROJECT_SRC_DIR}/settings_util.cpp
								 ${PROJECT_SRC_DIR}/trackable_device.h
								 ${PROJECT_SRC_DIR}/trackable_device.cpp
								 ${PROJECT_SRC_DIR}/tracker.h
								 ${PROJECT_SRC_DIR}/tracker.cpp
								 ${PROJECT_SRC_DIR}/utils.h
								 ${PROJECT_SRC_DIR}/utils.cpp
								 ${PROJECT_SRC_DIR}/virtual_controller.h
								 ${PROJECT_SRC_DIR}/virtual_controller.cpp
								 ${PROJECT_SRC_DIR}/watchdog.h
								 ${PROJECT_SRC_DIR}/watchdog.cpp
								 )
target_include_directories(driver_psmove PUBLIC ${OPENVR_PLUGIN_INCL_DIRS} ${PSM_PLUGIN_INCL_DIRS} )
target_link_libraries(driver_psmove ${OPENVR_PLUGIN_REQ_LIBS} ${PSM_PLUGIN_REQ_LIBS})

# Monitor app: defines what is compiled, what output type it is and what libs/includes it needs
add_executable(monitor_psmove ${PROJECT_SRC_DIR}/monitor.cpp)
target_include_directories(monitor_psmove PUBLIC ${OPENVR_MONITOR_INCL_DIRS} ${PSM_PLUGIN_INCL_DIRS})
target_link_libraries(monitor_psmove ${OPENVR_MONITOR_REQ_LIBS} ${PSM_PLUGIN_REQ_LIBS})

##############################################################################################
# Install: defines how the target build is made, i.e. what libraries, binaries, config files #
#          and models get packaged up with the final release.                                #
##############################################################################################
IF(${WIN32})
	# Install required system libraries
	IF(${BUILD_TYPE} MATCHES "Debug" OR ${BUILD_TYPE} MATCHES "debug")
		message("Including debug dependencies")
		set(CMAKE_INSTALL_DEBUG_LIBRARIES TRUE)
		set(CMAKE_INSTALL_SYSTEM_RUNTIME_DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME})
		INCLUDE(InstallRequiredSystemLibraries)
	ENDIF()
	

	# Copy driver library
	install(
		TARGETS driver_psmove
		RUNTIME DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}
		LIBRARY DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib
		ARCHIVE DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib
	)
	
	# Copy Program DataBase(*.pdb) file for the driver's psmove binary (only required for debugging)
	install(FILES $<TARGET_PDB_FILE:driver_psmove> 
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME} OPTIONAL)
	
	# Copy monitor binary
	install(
		TARGETS monitor_psmove
		RUNTIME DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}
		LIBRARY DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib
		ARCHIVE DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/lib
	)    
	
	# Copy Program DataBase(*.pdb) file for the driver's monitor binary (only required for debugging)
	install(
		FILES $<TARGET_PDB_FILE:monitor_psmove> 
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME} 
		OPTIONAL
	)
	
	# Copy OpenVR library
	install(
		DIRECTORY ${OPENVR_BINARIES_DIR}/ 
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}
		FILES_MATCHING PATTERN "*.dll" PATTERN "*.pdb"
	)
	
	# Copy PSMoveClient library
	install(
		FILES ${PSM_BINARIES_DIR}/PSMoveClient_CAPI.dll
	    DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME} 
		OPTIONAL
	)
	
	# Copy the PSMoveService assets folder
	install(
		DIRECTORY ${PSM_BINARIES_DIR}/assets
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME} 
		OPTIONAL
	)
	
	# Copy the post-installation scripts the user will use to install the driver on their PC
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/scripts/installer
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}
		FILES_MATCHING PATTERN "*${ARCH_LABEL}.bat"
	)
	
	#  Copy the driver icon resources
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/icons
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}/resources
		FILES_MATCHING PATTERN "*.png" PATTERN "*.svg"
	)

	#  Copy the driver input controller profile resources
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/input
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}/resources
		FILES_MATCHING PATTERN "*.json" 
	)

	#  Copy the driver localization resources
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/localization
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}/resources
		FILES_MATCHING PATTERN "*.json" 
	)
	
	#  Copy the driver overlay resources
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/overlays
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}/resources
		FILES_MATCHING PATTERN "*.png" 
	)
	
	#  Copy the driver rendermodel resources
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/rendermodels
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}/resources
		FILES_MATCHING PATTERN "*.png" PATTERN "*.tga" PATTERN "*.json" PATTERN "*.obj" PATTERN "*.mtl"
	)
	
	# Copy driver vr manifest
	install(
		FILES ${ROOT_DIR}/src/main/resources/configuration/driver.vrdrivermanifest
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}
	)
	
	# Copy the driver configuration folder
	install(
		DIRECTORY ${ROOT_DIR}/src/main/resources/configuration
		DESTINATION ${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}
		FILES_MATCHING PATTERN "*.vricons" PATTERN
	)
	
ELSE() #Linux/Darwin
	# TODO
ENDIF()

#add_custom_target(create_zip COMMAND
 #   ${CMAKE_COMMAND} -E tar "cfv" "${PSM_DRIVER_PROJECT_NAME}.zip" --format=zip 
	#   "${ROOT_DIR}/${TARGET_BUILD_DIR}/${ARCH_LABEL}/${PSM_DRIVER_PROJECT_NAME}")
