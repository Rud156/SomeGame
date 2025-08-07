package src.helpers;

class MathUtils {
	private static final FLOAT_TOLERANCE:Float = 0.1;

	public static inline function clampf(a:Float, min:Float, max:Float) {
		if (a < min) {
			return min;
		} else if (a > max) {
			return max;
		} else {
			return a;
		}
	}

	public static inline function IsNearlyEqual(a:Float, b:Float) {
		return Math.abs(a - b) <= FLOAT_TOLERANCE;
	}

	public static inline function IsNearlyZero(a:Float) {
		return Math.abs(a) <= FLOAT_TOLERANCE;
	}

	public static inline function IsNearlyEqualTol(a:Float, b:Float, tolerance:Float) {
		return Math.abs(a - b) <= tolerance;
	}
}
