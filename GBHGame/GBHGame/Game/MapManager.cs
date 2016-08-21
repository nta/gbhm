using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace GBH
{
    public static class MapManager
    {
        public static MapFileBase CurrentMap { get; set; }

        public static void Load(string mapname)
        {
            if (mapname.ToLowerInvariant() == CurrentMap?.Name)
            {
                return;
            }

            ConVar.SetValue("mapname", mapname);

            string filename = $"Maps/{mapname}.g2mp";

            if (FileSystem.FileExists(filename))
            {
                CurrentMap = new GBH2Map();
                CurrentMap.Load(filename);
            }
            else
            {
                filename = $"Maps/{mapname}.gmp";

                if (FileSystem.FileExists(filename))
                {
                    CurrentMap = new GBHMap();
                    CurrentMap.Load(filename);
                }
                else
                {
                    Log.Write(LogLevel.Error, $"No such map {mapname}");
                    return;
                }
            }

            CellManager.InitializeFromMap();
        }
    }
}
