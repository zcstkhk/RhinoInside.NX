using System;
using NXOpen.UF;
using NXOpen;
using System.Globalization;

namespace RhinoInside.NX.Extensions
{
    public static class ListingWindow
    {
        static UFSession _theUfSession;

        static ListingWindow()
        {
            _theUfSession = UFSession.GetUFSession();
        }

        /// <summary>
        /// Write an array of doubles to the Info window (with a newline added)
        /// </summary>
        /// <param name="myDoubleArray">The array of doubles to write</param>
        /// <param name="paddedWidth">The total width of each number field (padding added on the right). Optional. Default = 10</param>
        /// <remarks>
        /// The double is converted to a string in either fixed-point or scientific
        /// (exponential) form, whichever is more compact, and with the decimal point
        /// represented by a period symbol. If you need more control
        /// over the conversion process, please use the standard .NET ToString function
        /// with an appropriate format specifier.
        /// </remarks>
        public static void ListingWindowWriteLine(this double[] myDoubleArray, int paddedWidth = 10)
        {
            int num = myDoubleArray.Length;
            for (int i = 0; i < num; i++)
            {
                string text = myDoubleArray[i].ToString(new NumberFormatInfo
                {
                    NumberDecimalSeparator = "."
                });
                if (i != num - 1)
                {
                    ListingWindowWrite(text.PadRight(paddedWidth));
                }
                else
                {
                    ListingWindowWrite(text);
                }
            }
            ListingWindowWrite("\n");
        }

        /// <summary>
        /// Write an array of doubles to the Info window (with a newline added)
        /// </summary>
        /// <param name="myDoubleArray">The array of doubles to be written</param>
        /// <param name="format">The format specifier to be used when writing each double</param>
        /// <remarks>
        /// You can use any format specifier that is allowed in the standard .NET string.Format function.
        /// </remarks>
        // Token: 0x06000063 RID: 99 RVA: 0x000034C0 File Offset: 0x000016C0
        public static void ListingWindowWriteLine(this double[] myDoubleArray, string format)
        {
            int num = myDoubleArray.Length;
            for (int i = 0; i < num; i++)
            {
                string mystring = string.Format(format, myDoubleArray[i]);
                ListingWindowWrite(mystring);
            }
            ListingWindowWrite("\n");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myDoubleArray"></param>
        /// <param name="paddedWidth"></param>
        public static void ListingWindowWriteLine(this double[,] myDoubleArray, int paddedWidth = 10)
        {
            int length = myDoubleArray.GetLength(0);
            int length2 = myDoubleArray.GetLength(1);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    string text = myDoubleArray[i, j].ToString(new NumberFormatInfo
                    {
                        NumberDecimalSeparator = "."
                    });

                    ListingWindowWrite(text.PadRight(paddedWidth));
                }
                ListingWindowWrite("\n");
            }
        }

        /// <summary>
        /// Writes a string to the Info window (with no newline added)
        /// </summary>
        /// <param name="myString">The string to write</param>
        public static void ListingWindowWrite(this string myString)
        {
            _theUfSession.Ui.OpenListingWindow();
            _theUfSession.Ui.WriteListingWindow(myString);
        }

        /// <summary>
        /// Writes a string to the Info window (with a newline added)
        /// </summary>
        /// <param name="mystring">The string to write</param>
        public static void ListingWindowWriteLine(this string mystring)
        {
            ListingWindowWrite(mystring + "\n");
        }

        /// <summary>
        /// Write an array of strings to the Info window (with a newline added)
        /// </summary>
        /// <param name="myStringArray">The array of strings to write</param>
        /// <param name="paddedWidth">The total width of each number field (padding added on the right). Optional. Default = 20</param>
        public static void ListingWindowWriteLine(this string[] myStringArray, int paddedWidth = 20)
        {
            int num = myStringArray.Length;
            for (int i = 0; i < num; i++)
            {
                string text = myStringArray[i];
                if (i != num - 1)
                {
                    ListingWindowWrite(text.PadRight(paddedWidth));
                }
                else
                {
                    ListingWindowWrite(text);
                }
            }
            ListingWindowWrite("\n");
        }

        /// <summary>
        /// Write an array of integers to the Info window (with a newline added)
        /// </summary>
        /// <param name="myIntArray">The array of integers to write</param>
        /// <param name="paddedWidth">The total width of each number field (padding added on the right). Optional. Default = 7</param>
        public static void ListingWindowWriteLine(this int[] myIntArray, int paddedWidth = 7)
        {
            int num = myIntArray.Length;
            for (int i = 0; i < num; i++)
            {
                string text = myIntArray[i].ToString();
                if (i != num - 1)
                {
                    ListingWindowWrite(text.PadRight(paddedWidth));
                }
                else
                {
                    ListingWindowWrite(text);
                }
            }
            ListingWindowWrite("\n");
        }

        public static void ListingWindowWriteLine(this int[,] myIntArray, int paddedWidth = 7)
        {
            int length = myIntArray.GetLength(0);
            int length2 = myIntArray.GetLength(1);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    string text = myIntArray[i, j].ToString();
                    ListingWindowWrite(text.PadRight(paddedWidth));
                }
                ListingWindowWrite("\n");
            }
        }

        /// <summary>
        /// Writes a double to the Info window (with no newline added)
        /// </summary>
        /// <param name="mydouble">The double to write</param>
        /// <remarks>
        /// The double is converted to a string in either fixed-point or scientific
        /// (exponential) form, whichever is more compact, and with the decimal point
        /// represented by a period symbol. If you need more control
        /// over the conversion process, please use the standard .NET ToString function
        /// with an appropriate format specifier.
        /// </remarks>
        public static void ListingWindowWrite(this double mydouble)
        {
            ListingWindowWrite(mydouble.ToString(new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            }));
        }

        public static void ListingWindowWriteLine(this Point3d point)
        {
            ListingWindowWrite(point.ToString());
            ListingWindowWrite("\n");
        }

        public static void ListingWindowWriteLine(this Vector3d vector)
        {
            ListingWindowWrite(vector.ToString());
            ListingWindowWrite("\n");
        }

        /// <summary>
        /// Writes a double to the Info window (with a newline added)
        /// </summary>
        /// <param name="mydouble">The double to write</param>
        /// <remarks>
        /// The double is converted to a string in either fixed-point or scientific
        /// (exponential) form, whichever is more compact, and with the decimal point
        /// represented by a period symbol. If you need more control
        /// over the conversion process, please use the standard .NET ToString function
        /// with an appropriate format specifier.
        /// </remarks>
        public static void ListingWindowWriteLine(this double mydouble)
        {
            ListingWindowWrite(mydouble.ToString(new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            }) + "\n");
        }

        /// <summary>Closes the Information window</summary>
        /// <remarks>
        /// Closing the Info window will also clear its contents.
        /// </remarks>
        public static void Close()
        {
            _theUfSession.Ui.CloseListingWindow();
        }

        /// <summary>Clears the Info Window</summary>
        public static void Clear()
        {
            _theUfSession.Ui.ExitListingWindow();
            _theUfSession.Ui.OpenListingWindow();
        }
    }
}