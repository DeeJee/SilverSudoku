using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

/// <summary>
/// Summary description for ColorSwitcher
/// </summary>
public class ColorSwitcher
{
    private int[] _colors = { 0, 0, 0 };
    
    private int _updateIndex;
    public ColorSwitcher()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Color NextColor()
    {
        NextUpdate();
        IncrementColor();

        return Color.FromArgb(_colors[0], _colors[1], _colors[2]);
    }

    private void NextUpdate()
    {
        switch (_updateIndex)
        {
            case 0:
                _updateIndex = 1; 
                break;
            case 1:
                _updateIndex = 2;
                break;
            case 2:
                _updateIndex = 0;
                break;
        }
    }

    private void IncrementColor()
    {
        _colors[_updateIndex] += 10;
    }

}
