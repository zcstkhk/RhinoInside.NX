using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhinoInside.NX.Extensions
{
    /// <summary>
    ///    Outline is a generic object that provides a bounding box/bounding outline. It supports
    ///    operations to scale and transform. It also supports intersections and contains operations.
    /// </summary>
    /// <since>
    ///    2011
    /// </since>
    public class Outline : IDisposable
    {
       public Point3d MinimumPoint;
       public Point3d MaximumPoint;
        private bool disposedValue;

        public Outline(Point3d min, Point3d max)
        {
            MinimumPoint = min;
            MaximumPoint = max;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Outline()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
