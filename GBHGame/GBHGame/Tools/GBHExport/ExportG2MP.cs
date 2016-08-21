using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    public static class ExportG2MP
    {
        public static void ExportMain(string[] args)
        {
            Log.Initialize(LogLevel.All);
            Log.AddListener(new ConsoleLogListener());

            ConVar.Initialize();
            FileSystem.Initialize();

            var filename = args[1];
            var baseDir = Path.GetDirectoryName(filename);

            var mmp = new MMPFile(filename);

            // convert the style
            var styFile = mmp.GetValue("MapFiles", "STYFile");
            var styBaseName = Path.GetFileNameWithoutExtension(styFile);
            styFile = Path.Combine(baseDir, styFile);

            ConvertStyle(styFile, styBaseName);

            // get gmp file stuff
            var gmpFile = mmp.GetValue("MapFiles", "GMPFile");
            var gmpBaseName = Path.GetFileNameWithoutExtension(gmpFile);
            gmpFile = Path.Combine(baseDir, gmpFile);

            // load the original map
            var gmp = new GBHMap();
            gmp.Load(gmpFile);

            // make a map structure for the new map
            var g2mp = new GBH2Map();
            g2mp.Map = gmp.Map;
            g2mp.Lights = gmp.Lights;

            // collect the sprites from the used .sty
            var materialList = new List<string>();
            materialList.Add("default");

            var materialMapping = new Dictionary<ushort, ushort>();
            materialMapping.Add(0, 0);

            var idx = 0;

            foreach (var block in g2mp.Map.Block)
            {
                Func<ushort, ushort> mapMaterial = inIndex =>
                {
                    ushort mappedMaterial;

                    if (!materialMapping.TryGetValue(inIndex, out mappedMaterial))
                    {
                        mappedMaterial = (ushort)materialList.Count;
                        materialList.Add($"gbh/{styBaseName}/{inIndex}");
                        materialMapping.Add(inIndex, mappedMaterial);
                    }

                    return mappedMaterial;
                };

                block.Top.Sprite = mapMaterial(block.Top.Sprite);
                block.Bottom.Sprite = mapMaterial(block.Bottom.Sprite);
                block.Left.Sprite = mapMaterial(block.Left.Sprite);
                block.Right.Sprite = mapMaterial(block.Right.Sprite);
                block.Lid.Sprite = mapMaterial(block.Lid.Sprite);

                idx++;
            }

            g2mp.Materials = materialList.ToArray();

            g2mp.Save($"Data/Maps/{gmpBaseName}.g2mp");
        }

        private static void ConvertStyle(string fileName, string baseName)
        {
            StyleManager.Load(fileName);

            var outMaterialFile = "Data/Styles/" + baseName + ".material";
            var outBitmap = "Data/Styles/" + baseName + ".png";

            Vector2 uv;
            var bitmap = StyleManager.GetBitmap();
            bitmap.Save(outBitmap, System.Drawing.Imaging.ImageFormat.Png);

            // create a material file
            using (var writer = new StreamWriter(outMaterialFile))
            {
                for (int i = 0; i < 992; i++)
                {
                    StyleManager.GetTileTextureBordered(i, out uv);

                    writer.WriteLine(string.Format("gbh/{0}/{1}", baseName, i));
                    writer.WriteLine("{");
                    writer.WriteLine(string.Format("\ttexture Styles/{0}.png", baseName));
                    writer.WriteLine(string.Format("\tuv {0} {1} {2} {3}", uv.X, uv.Y, uv.X + 0.03125f, uv.Y + (0.03125f / 2)));
                    writer.WriteLine("}");
                    writer.WriteLine();
                }
            }
        }
    }
}
