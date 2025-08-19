package src.helpers;

class MathUtils {
	private static final FLOAT_TOLERANCE:Float = 0.1;

	public static inline function IsNearlyEqual(a:Float, b:Float) {
		return Math.abs(a - b) <= FLOAT_TOLERANCE;
	}

	public static inline function IsNearlyZero(a:Float) {
		return Math.abs(a) <= FLOAT_TOLERANCE;
	}

	public static inline function IsNearlyEqualTol(a:Float, b:Float, tolerance:Float) {
		return Math.abs(a - b) <= tolerance;
	}

	public static inline function To360Angle(angle:Float):Float {
		while (angle < 0) {
			angle += 360.0;
		}

		while (angle >= 360.0) {
			angle -= 360.0;
		}

		return angle;
	}
}
