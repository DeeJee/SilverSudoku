using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SilverSudoku;
using System.Drawing;
using System.Diagnostics;


public partial class _Default : System.Web.UI.Page
{

    private Color[] colors ={
        Color.AliceBlue,
        Color.AntiqueWhite,
        Color.Aqua,
        Color.Aquamarine,
        Color.Azure,
        Color.Beige,
        Color.Bisque,
        Color.Black,
        Color.BlanchedAlmond,
        Color.Blue,
        Color.BlueViolet,
        Color.Brown,
        Color.BurlyWood,
        Color.CadetBlue,
        Color.Chartreuse,
        Color.Chocolate,
        Color.Coral,
        Color.CornflowerBlue,
        Color.Cornsilk,
        Color.Crimson,
        Color.Cyan,
        Color.DarkBlue,
        Color.DarkCyan,
    Color.DarkGoldenrod,
    Color.DarkGray,
    Color.DarkGreen,
    Color.DarkKhaki,
    Color.DarkMagenta,
    Color.DarkOliveGreen,
    Color.DarkOrange,Color.DarkOrange,Color.DarkOrchid,Color.DarkRed,Color.DarkSalmon,Color.DarkSeaGreen,Color.DarkSlateBlue,Color.DarkSlateGray
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            
                            };


    protected void Page_Load(object sender, EventArgs e)
    {
        //RecentlyOpenedFilesHandler handler = new RecentlyOpenedFilesHandler();
        //handler.StoreRecentlyOpenedFile("een", FileLocation.FileSystem);
        //handler.StoreRecentlyOpenedFile("twee", FileLocation.IsolatedStorage);
        //handler.StoreRecentlyOpenedFile("drie", FileLocation.FileSystem);
        //handler.StoreRecentlyOpenedFile("vier", FileLocation.IsolatedStorage);
        //handler.StoreRecentlyOpenedFile("vijf", FileLocation.IsolatedStorage);
        //handler.StoreRecentlyOpenedFile("zes", FileLocation.IsolatedStorage);


        //RecentFiles files = handler.GetRecentlyOpenedFiles();
        //foreach (RecentFile fil in files)
        //{
        
        //}

        foreach (Color kl in colors)
        {
            Debug.WriteLine("Color.FromArgb(" + kl.A.ToString() + "," + kl.R.ToString() + "," + kl.G.ToString() + "," + kl.B.ToString() + "),");
        }




    }
}
