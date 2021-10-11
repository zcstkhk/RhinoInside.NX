using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.UserDefinedObjects;
using static NXOpen.Extensions.Globals;
using NXOpen.Extensions;
using NXOpen;

namespace RhinoInside.NX.Core
{
    public class GH_UserDefinedObject
    {
        static UserDefinedClass GHFeatureClass;

        static GH_UserDefinedObject()
        {
            try
            {
                Console.WriteLine("注册");

                // Define your custom UDO class 
                GHFeatureClass = TheSession.UserDefinedClassManager.CreateUserDefinedObjectClass
                              ("GHFeature", "Object Created From Grasshopper");
                // Setup properties on the custom UDO class 
                GHFeatureClass.AllowQueryClassFromName = UserDefinedClass.AllowQueryClass.On;
                // Register callbacks for the UDO class 
                //            GHFeature.AddDisplayHandler(new UserDefinedClass.DisplayCallback(Program.myDisplayCB));
                //            GHFeature.AddAttentionPointHandler(new UserDefinedClass.DisplayCallback(Program.myDisplayCB));
                //            GHFeature.AddFitHandler(new UserDefinedClass.DisplayCallback
                //(Program.myDisplayCB));
                //            GHFeature.AddSelectionHandler(new
                //UserDefinedClass.DisplayCallback(Program.myDisplayCB));
                //            GHFeature.AddEditHandler(new UserDefinedClass.GenericCallback(Program.myEditCB));
                //            GHFeature.AddInformationHandler(new
                //UserDefinedClass.GenericCallback(Program.myInfoCB));
                // Add this class to the list of object types available for selection in NX. 
                // If you skip this step you won't be able to select UDO's of this class, 
                // even though you registered a selection callback. 
                TheUI.SelectionManager.SetSelectionStatusOfUserDefinedClass(GHFeatureClass, true);
            }
            catch (NXOpen.NXException ex)
            {
                // ---- Enter your exception handling code here ----- 
                TheUI.NXMessageBox.Show("Caught exception", NXMessageBox.DialogType.Error, ex.Message);
                throw ex;
            }
        }

        public static UserDefinedObject CreateGHFeatureObject()
        {
            var ghFeatureObj = WorkPart.UserDefinedObjectManager.CreateUserDefinedObject(GHFeatureClass);

            ghFeatureObj.Color = 36;

            View myView = null;
            Point3d myCursor;
            myCursor.X = 0;
            myCursor.Y = 0;
            myCursor.Z = 0;
            // ask the user to select an origin for this UDO 
            Selection.DialogResponse myResponse =
    TheUI.SelectionManager.SelectScreenPosition("Select Origin of C# UDO", out myView, out myCursor);

            double[] myUDOdoubles = new double[3];
            myUDOdoubles[0] = myCursor.X;
            myUDOdoubles[1] = myCursor.Y;
            myUDOdoubles[2] = myCursor.Z;
            ghFeatureObj.SetDoubles(myUDOdoubles);

            TheUfSession.Disp.AddItemToDisplay(ghFeatureObj.Tag);

            NXOpen.Features.UserDefinedObjectFeatureBuilder fb = WorkPart.Features.CreateUserDefinedObjectFeatureBuilder(null);
            fb.UserDefinedObject = ghFeatureObj;
            fb.Commit();
            fb.Destroy();

            return ghFeatureObj;
        }
    }
}
