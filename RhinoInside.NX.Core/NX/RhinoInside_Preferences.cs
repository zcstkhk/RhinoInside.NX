using System;
using NXOpen;
using NXOpen.BlockStyler;
using static NXOpen.Extensions.Globals;
using System.Linq;
using NXOpen.Extensions;

namespace RhinoInside.NX.Core
{
    public class RhinoInsidePreferences
    {
        //class members
        private string theDlxFileName;
        private NXOpen.BlockStyler.BlockDialog theDialog;
        private NXOpen.BlockStyler.Group groupSolidExchange;// Block type: Group
        private NXOpen.BlockStyler.Enumeration enumSTEPExchangeMode;// Block type: Enumeration

        private static string PreferenceFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RhinoInside Preferences.config");

        //------------------------------------------------------------------------------
        //Constructor for NX Styler class
        //------------------------------------------------------------------------------
        public RhinoInsidePreferences()
        {
            try
            {
                theDlxFileName = "RhinoInside_Preferences.dlx";
                theDialog = TheUI.CreateDialog(theDlxFileName);
                theDialog.AddOkHandler(new NXOpen.BlockStyler.BlockDialog.Ok(ok_cb));
                theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
                theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
                theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                throw ex;
            }
        }

        public NXOpen.UIStyler.DialogResponse Show()
        {
            try
            {
                theDialog.Show();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Method Name: Dispose
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            if (theDialog != null)
            {
                theDialog.Dispose();
                theDialog = null;
            }
        }

        public void initialize_cb()
        {
            try
            {
                groupSolidExchange = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("groupSolidExchange");
                enumSTEPExchangeMode = (NXOpen.BlockStyler.Enumeration)theDialog.TopBlock.FindBlock("enumSTEPExchangeMode");
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        public void dialogShown_cb()
        {
            try
            {
                if (System.IO.File.Exists(PreferenceFileName))
                {
                    var preferenceLines = System.IO.File.ReadAllLines(PreferenceFileName);

                    enumSTEPExchangeMode.ValueAsString = preferenceLines.First(obj => obj.Contains("STEPMode=")).Split('=')[1];
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        public int update_cb(NXOpen.BlockStyler.UIBlock block)
        {
            try
            {
                if (block == enumSTEPExchangeMode)
                {
                    "Please Reboot NX after close this dialog to prevent errors.".ShowInNXMessageBox(NXMessageBox.DialogType.Warning);
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        public int ok_cb()
        {
            int errorCode = 0;
            try
            {
                using (var stream = System.IO.File.CreateText(PreferenceFileName))
                {
                    stream.WriteLine("STEPMode=" + enumSTEPExchangeMode.ValueAsString);
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                errorCode = 1;
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return errorCode;
        }

        //------------------------------------------------------------------------------
        //Function Name: GetBlockProperties
        //Returns the propertylist of the specified BlockID
        //------------------------------------------------------------------------------
        public PropertyList GetBlockProperties(string blockID)
        {
            PropertyList plist = null;
            try
            {
                plist = theDialog.GetBlockProperties(blockID);
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                TheUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return plist;
        }

    }
}