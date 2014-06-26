using UnityEngine;

public static class RendererExtensions {
	// <summary>
	// Check to see whether an object is visible by the camera
	// </summary>
	public static bool IsVisibleFrom(this Renderer my_renderer, Camera my_camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(my_camera);
		return GeometryUtility.TestPlanesAABB(planes, my_renderer.bounds);
	}
}
