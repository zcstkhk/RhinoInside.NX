using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace RhinoInside.NX.GH
{
    public class RhinoInsideNXGHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "RhinoInsideNXGH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("2b3a325c-5d77-43cf-be56-cad61bd07b49");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
