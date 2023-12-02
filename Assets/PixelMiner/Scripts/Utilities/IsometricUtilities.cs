using UnityEngine;

namespace PixelMiner.Utilities
{
    public static class IsometricUtilities
    {

        /// <summary>
        /// Applies an isometric transformation to a <see cref="Vector3"/> using the provided Euler angles.
        /// </summary>
        /// <param name="input">The input <see cref="Vector3"/> to transform isometrically.</param>
        /// <param name="euler">The Euler angles specifying the rotation for the isometric transformation.</param>
        /// <returns>A new <see cref="Vector3"/> representing the result of the isometric transformation.</returns>
        public static Vector3 Iso(this Vector3 input, Vector3 euler) => Matrix4x4.Rotate(Quaternion.Euler(euler)).MultiplyPoint3x4(input); 
    }
}

