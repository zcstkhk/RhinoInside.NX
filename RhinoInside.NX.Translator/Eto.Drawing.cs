using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.Convert.Eto.Drawing
{
  using global::Eto.Drawing;

  public static class ColorConverter
  {
    public static Color ToColor(this DB.Color c)
    {
      return c.IsValid ?
             Color.FromArgb(c.Red, c.Green, c.Blue, 0xFF) :
             Color.FromArgb(0, 0, 0, 0);
    }

    public static DB::Color ToColor(this Color c)
    {
      return c.ToArgb() == 0 ?
        DB::Color.InvalidColorValue :
        new DB::Color((byte) c.Rb, (byte) c.Gb, (byte) c.Bb);
    }
  }
}
