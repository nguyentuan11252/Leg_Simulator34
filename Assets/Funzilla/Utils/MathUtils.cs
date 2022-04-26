
namespace Funzilla
{
	class MathUtils
	{
		public static float Clamp360(float angle)
		{
			while (angle < 0)
			{
				angle += 360f;
			}
			while (angle > 360)
			{
				angle -= 360f;
			}
			return angle;
		}
	}
}