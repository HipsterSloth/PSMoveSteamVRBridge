#include "facing_handsolver.h"
#include "PSMoveClient_CAPI.h"

/*
 IK Model - Adapted from https://github.com/LastFreeUsername/qufIK/blob/master/cFABRIK.cpp
 */

namespace steamvrbridge {

	// right-handed system
	// +y is up
	// +x is to the right
	// -z is going away from you
	// Distance unit is  meters
	CFacingHandOrientationSolver::CFacingHandOrientationSolver()
	{
	}

	PSMQuatf CFacingHandOrientationSolver::solveHandOrientation(const PSMPosef &hmdPose, const PSMVector3f &handLocation)
	{
		// Use the orientation of the HMD as the hand orientation
		return hmdPose.Orientation;
	}

	// TODO - Doesn't work yet
#if 0
	class CRadialHandOrientationSolver : public IHandOrientationSolver
	{
	public:
		// right-handed system
		// +y is up
		// +x is to the right
		// -z is going away from you
		// Distance unit is  meters
		CRadialHandOrientationSolver(vr::ETrackedControllerRole hand, float neckLength, float halfShoulderLength)
			: m_hand(hand)
			, m_neckLength(neckLength)
			, m_halfShoulderLength(halfShoulderLength)
		{
		}

		PSMQuatf solveHandOrientation(const PSMPosef &hmdPose, const PSMVector3f &handLocation) override
		{
			// Assume the left/right shoulder is always pointing perpendicular to HMD forward.
			// This isn't always true, but it's generally the more comfortable default pose.
			const PSMPosef shoulderPose = solveWorldShoulderPose(&hmdPose);

			// Compute the world space locations of the elbow and hand
			const PSMVector3f shoulderToHand = PSM_Vector3fSubtract(&handLocation, &shoulderPose.Position);

			// Create ortho-normal basis vectors (forward, up, and right) for the hand
			// from the shoulder position and hand position
			const PSMVector3f handForward = PSM_Vector3fNormalizeWithDefault(&shoulderToHand, k_psm_float_vector3_k);
			const PSMVector3f up = *k_psm_float_vector3_j;
			PSMVector3f handRight = PSM_Vector3fCross(&handForward, &up);
			handRight = PSM_Vector3fNormalizeWithDefault(&handRight, k_psm_float_vector3_i);
			const PSMVector3f handUp = PSM_Vector3fCross(&handRight, &handForward);

			// Convert basis vectors into a 3x3 matrix
			const PSMVector3f negatedHandForward = PSM_Vector3fScale(&handForward, -1.f);
			const PSMMatrix3f handMat = PSM_Matrix3fCreate(&handRight, &handUp, &negatedHandForward);

			// Convert the hand orientation into a quaternion
			PSMQuatf handOrientation = psmMatrix3fToPSMQuatf(handMat);

			return handOrientation;
		}

	protected:
		PSMPosef solveWorldShoulderPose(const PSMPosef *hmdPose)
		{
			PSMVector3f localShoulderOffset = {
				(m_hand == vr::ETrackedControllerRole::TrackedControllerRole_RightHand) ? m_halfShoulderLength : -m_halfShoulderLength,
				-m_neckLength,
				0.f };
			PSMPosef localShoulderPose = PSM_PosefCreate(&localShoulderOffset, k_psm_quaternion_identity);
			PSMPosef worldShoulderPose = PSM_PosefConcat(&localShoulderPose, hmdPose);

			return worldShoulderPose;
		}

	private:
		vr::ETrackedControllerRole m_hand;
		float m_neckLength;
		float m_halfShoulderLength;
	};
#endif
}