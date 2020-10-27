using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace RhinoInside.NX.Extensions.NX
{
    public static class Matrix4x4Ex
    {
        ///    Creates a transform that represents a translation via the specified vector.
        public static Matrix4x4 CreateTranslation(Vector3d vector)
        {
            var result = Identity;
            result.Xt = vector.X;
            result.Yt = vector.Y;
            result.Zt = vector.Z;

            return result;
        }

        public static Matrix4x4 Identity => new Matrix4x4()
        {
            Rxx = 1.0,
            Rxy = 0.0,
            Rxz = 0.0,
            Ryx = 0.0,
            Ryy = 1.0,
            Ryz = 0.0,
            Rzx = 0.0,
            Rzy = 0.0,
            Rzz = 1.0,
            Xt = 0.0,
            Yt = 0.0,
            Zt = 0.0,
            Ss = 1.0,

            Sx = 0.0,
            Sy = 0.0,
            Sz = 0.0,
        };

        public static void SetRotationAxisX(this Matrix4x4 matrix, Vector3d vector)
        {
            matrix.Rxx = vector.X;
            matrix.Rxy = vector.Y;
            matrix.Rxz = vector.Z;
        }

        public static void SetRotationAxisY(this Matrix4x4 matrix, Vector3d vector)
        {
            matrix.Ryx = vector.X;
            matrix.Ryy = vector.Y;
            matrix.Ryz = vector.Z;
        }

        public static void SetRotationAxisZ(this Matrix4x4 matrix, Vector3d vector)
        {
            matrix.Rzx = vector.X;
            matrix.Rzy = vector.Y;
            matrix.Rzz = vector.Z;
        }

        public static void SetTranslation(this Matrix4x4 matrix, Vector3d vector)
        {
            matrix.Xt = vector.X;
            matrix.Yt = vector.Y;
            matrix.Zt = vector.Z;
        }
    }
}
