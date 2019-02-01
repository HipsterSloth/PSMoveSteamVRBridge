using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSMoveService;

namespace SystemTrayApp
{
    public class MathUtility
    {
        public static float k_real_max = float.MaxValue;
        public static float k_real_min = float.MinValue;

        public static float k_positional_epsilon = 0.001f;
        public static float k_normal_epsilon = 0.0001f;
        public static float k_real_epsilon = float.Epsilon;

        public static double k_real64_positional_epsilon = 0.001;
        public static double k_real64_normal_epsilon = 0.0001;
        public static double k_real64_epsilon = double.Epsilon;

        public static float k_real_pi = (float)Math.PI;
        public static float k_real_two_pi = 2.0f * k_real_pi; // 360 degrees
        public static float k_real_half_pi = 0.5f * k_real_pi; // 90 degrees
        public static float k_real_quarter_pi = 0.25f * k_real_pi; // 45 degrees

        public static float k_degrees_to_radians = k_real_pi / 180.0f;
        public static float k_radians_to_degreees = 180.0f / k_real_pi;

        public static double k_real64_pi = Math.PI;
        public static double k_real64_two_pi = 2.0 * k_real64_pi; // 360 degrees
        public static double k_real64_half_pi = 0.5*k_real64_pi; // 90 degrees
        public static double k_real64_quarter_pi = 0.25*k_real64_pi; // 45 degrees

        public static double k_real64_degrees_to_radians = k_real64_pi / 180.0;
        public static double k_real64_radians_to_degreees = 180.0 / k_real64_pi;

        public static bool is_nearly_equal(float a, float b, float epsilon)
        {
            return (Math.Abs(a - b) <= epsilon);
        }

        public static bool is_nearly_zero(float x)
        {
            return is_nearly_equal(x, 0.0f, k_real_epsilon);
        }

        public static bool is_double_nearly_equal(double a, double b, double epsilon)
        {
            return (Math.Abs(a - b) <= epsilon);
        }

        public static bool is_double_nearly_zero(double x)
        {
            return is_double_nearly_equal(x, 0.0, k_real64_epsilon);
        }

        public static float safe_divide_with_default(float numerator, float denomenator, float default_result)
        {
            return is_nearly_zero(denomenator) ? default_result : (numerator / denomenator);
        }

        public static double safe_divide_with_default(double numerator, double denomenator, double default_result)
        {
            return is_double_nearly_zero(denomenator) ? default_result : (numerator / denomenator);
        }

        public static float safe_sqrt_with_default(float square, float default_result)
        {
            return (square > k_real_epsilon) ? (float)Math.Sqrt(square) : default_result;
        }

        public static double safe_sqrt_with_default(double square, double default_result)
        {
            return (square > k_real64_epsilon) ? (float)Math.Sqrt(square) : default_result;
        }

        public static float clampf(float x, float lo, float hi)
        {
            return Math.Min(Math.Max(x, lo), hi);
        }

        public static float clampf01(float x)
        {
            return clampf(x, 0.0f, 1.0f);
        }

        public static float lerpf(float a, float b, float u)
        {
            return a * (1.0f - u) + b * u;
        }

        public static float lerp_clampf(float a, float b, float u)
        {
            return clampf(lerpf(a, b, u), a, b);
        }

        public static float degrees_to_radians(float x)
        {
            return ((x * k_real_pi) / 180.0f);
        }

        public static float radians_to_degrees(float x)
        {
            return ((x * 180.0f) / k_real_pi);
        }

        public static float wrap_radians(float angle)
        {
            return (angle + k_real_two_pi) % k_real_two_pi;
        }

        public static float wrap_degrees(float angle)
        {
            return (angle + 360.0f) % 360.0f;
        }

        public static float cosf(float radians)
        {
            return (float)Math.Cos((double)radians);
        }

        public static float sinf(float radians)
        {
            return (float)Math.Sin((double)radians);
        }

        public static float atan2f(float y, float x)
        {
            return (float)Math.Atan2((double)y, (double)x);
        }

        public static float asinf(float x)
        {
            return (float)Math.Asin((float)x);
        }

        public static float wrap_range(float value, float range_min, float range_max)
        {
            Debug.Assert(range_max > range_min);
            float range = range_max - range_min;

            return range_min + ((value - range_min + range) % range);
        }

        public static double wrap_ranged(double value, double range_min, double range_max)
        {
            Debug.Assert(range_max > range_min);
            double range = range_max - range_min;

            return range_min + ((value - range_min + range) % range);
        }

        public static float wrap_lerpf(float a, float b, float u, float range_min, float range_max)
        {
            Debug.Assert(range_max > range_min);
            float range = range_max - range_min;
            float wrapped_a = a;
            float wrapped_b = b;

            if (Math.Abs(a - b) >= (range / 2.0f)) {
                if (a > b)
                    wrapped_a = wrap_range(a, range_min, range_max) - range;
                else
                    wrapped_b = wrap_range(b, range_min, range_max) - range;
            }

            return wrap_range(lerpf(wrapped_a, wrapped_b, u), range_min, range_max);
        }

        public static float ExtractYaw(PSMQuatf q)
        {
            return atan2f(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
        }

        public static float ExtractPitch(PSMQuatf q)
        {
            return atan2f(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);
        }

        public static float ExtractRoll(PSMQuatf q)
        {
            return asinf(2 * q.x * q.y + 2 * q.z * q.w);
        }
    }
}
