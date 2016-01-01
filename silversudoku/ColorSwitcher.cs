using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

/// <summary>
/// Summary description for ColorSwitcher
/// </summary>
public class ColorSwitcher
{
    private int index;
    public ColorSwitcher()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Color NextColor()
    {
        if (index < kleuren.Length - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        return kleuren[index];

    }

    private Color[] kleuren =
    {
        Color.FromArgb(255,240,248,255),
        Color.FromArgb(255,250,235,215),
        Color.FromArgb(255,0,255,255),
        Color.FromArgb(255,127,255,212),
        Color.FromArgb(255,240,255,255),
        Color.FromArgb(255,245,245,220),
        Color.FromArgb(255,255,228,196),
        Color.FromArgb(255,255,235,205),
        Color.FromArgb(255,0,0,255),
        Color.FromArgb(255,138,43,226),
        Color.FromArgb(255,165,42,42),
        Color.FromArgb(255,222,184,135),
        Color.FromArgb(255,95,158,160),
        Color.FromArgb(255,127,255,0),
        Color.FromArgb(255,210,105,30),
        Color.FromArgb(255,255,127,80),
        Color.FromArgb(255,100,149,237),
        Color.FromArgb(255,255,248,220),
        Color.FromArgb(255,220,20,60),
        Color.FromArgb(255,0,255,255),
        Color.FromArgb(255,0,0,139),
        Color.FromArgb(255,0,139,139),
        Color.FromArgb(255,184,134,11),
        Color.FromArgb(255,169,169,169),
        Color.FromArgb(255,0,100,0),
        Color.FromArgb(255,189,183,107),
        Color.FromArgb(255,139,0,139),
        Color.FromArgb(255,85,107,47),
        Color.FromArgb(255,255,140,0),
        Color.FromArgb(255,255,140,0),
        Color.FromArgb(255,153,50,204),
        Color.FromArgb(255,139,0,0),
        Color.FromArgb(255,233,150,122),
        Color.FromArgb(255,143,188,139),
        Color.FromArgb(255,72,61,139),
        Color.FromArgb(255,47,79,79)
    };
}
