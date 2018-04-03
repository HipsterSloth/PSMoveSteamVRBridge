#pragma once
#include "PSMoveClient_CAPI.h"

namespace steamvrbridge {
	class IHandOrientationSolver
	{
	public:
		virtual PSMQuatf solveHandOrientation(const PSMPosef &hmdPose, const PSMVector3f &handLocation) = 0;
	};

	class CFacingHandOrientationSolver : public IHandOrientationSolver
	{
	public:
		CFacingHandOrientationSolver();
		PSMQuatf solveHandOrientation(const PSMPosef &hmdPose, const PSMVector3f &handLocation);
	};
}