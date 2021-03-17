using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using System.Xml;

namespace Rescue_Bots
{
   public class Map
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public string[,] MapString { get; set; }
        public List<int[]> Tiles { get; set; }
        public List<string> LayersNames { get; set; }
        public string Tileset { get; set; }
        public Bitmap MapBitmap { get; set; }

        public System.Windows.Controls.Image MapLayout {get;set;}
        public List<Object> Robots { get; set; }
        public List<Object> Humans { get; set; }


        // public Bitmap PlayingField { get; set; }


        public Map(string path)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            TileWidth = Convert.ToInt32(xRoot.GetAttributeNode("tilewidth").Value);
            TileHeight = Convert.ToInt32(xRoot.GetAttributeNode("tileheight").Value);
            MapWidth = Convert.ToInt32(xRoot.GetAttributeNode("width").Value);
            MapHeight = Convert.ToInt32(xRoot.GetAttributeNode("height").Value);

            MapString = new string[MapWidth, MapHeight];
            Tiles = new List<int[]>(4);
            LayersNames = new List<string>(4);
            Robots = new List<Object>();
            Humans = new List<Object>();

            int countTilesets = 0;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "tileset")
                {
                    countTilesets++;
                    Tileset = xnode.Attributes.GetNamedItem("source").Value;
                }
                // получаем атрибут name
                if (xnode.Name == "layer")
                {
                    LayersNames.Add(xnode.Attributes.GetNamedItem("name").Value);
                    XmlNode data = xnode.FirstChild;
                    int[] tiles = new int[MapHeight * MapWidth];
                    int index = 0;
                    foreach (XmlNode childnode in data.ChildNodes)
                    {
                        if (childnode.Attributes.Count > 0)
                        {
                            tiles[index] = Convert.ToInt32(childnode.Attributes.GetNamedItem("gid").Value);
                        }
                        else
                        {
                            tiles[index] = 0;
                        }

                        index++;
                    }
                    Tiles.Add(tiles);
                }
                if (xnode.Name == "objectgroup")
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        // name //x //y
                    }
                }
            }
            
            MapBitmap = LoadMap();
            if (MainWindow.Drawer == null) throw new Exception("ImageDrawer is null");
            MapLayout = MainWindow.Drawer.BitmapToControl(MapBitmap);
        }

        public Bitmap LoadMap()
        {
            Bitmap bmp = new Bitmap(TileWidth * MapWidth, TileHeight * MapHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Bitmap tileset = (Bitmap)System.Drawing.Image.FromFile("tiles 1.png");
                System.Drawing.Imaging.PixelFormat format = tileset.PixelFormat;

                int iTilesetMax = tileset.Width / TileWidth;

                int indexLayer = 0;

                foreach (int[] layer in Tiles)
                {
                   int iMap = 0;
                    int jMap = 0;

                    if (LayersNames[indexLayer] == "Humans" || LayersNames[indexLayer] == "Robots")
                    {
                        foreach (int t in layer)
                        {
                            int num = t;
                            if (num != 0)
                            {
                                num--;

                                int jTileset = num / iTilesetMax;
                                int iTileset = num % iTilesetMax;

                                System.Drawing.Rectangle rectangleCut = new System.Drawing.Rectangle(iTileset * TileWidth, jTileset * TileHeight, TileWidth, TileHeight);
                                Bitmap tile = tileset.Clone(rectangleCut, format);

                                if (LayersNames[indexLayer] == "Robots")
                                {
                                    Robots.Add(new Object(tile, iMap, jMap, Robots.Count+1));
                                    MapString[iMap, jMap] = "1";
                                }
                                else
                                {
                                    Humans.Add(new Object(tile, iMap, jMap));
                                    MapString[iMap, jMap] = "X";
                                }
                            }

                            iMap++;
                            if (iMap == MapWidth)
                            {
                                iMap = 0;
                                jMap++;
                            }
                        }
                    }
                    

                    foreach (int t in layer)
                    {
                        int num = t;
                        if (num != 0)
                        {
                            num--;

                            int jTileset = num / iTilesetMax;
                            int iTileset = num % iTilesetMax;

                            System.Drawing.Rectangle rectangleCut = new System.Drawing.Rectangle(iTileset * TileWidth, jTileset * TileHeight, TileWidth, TileHeight);
                            Bitmap tile = tileset.Clone(rectangleCut, format);

                            System.Drawing.Rectangle rectangleInsert = new System.Drawing.Rectangle(iMap * TileWidth, jMap * TileHeight, TileWidth, TileHeight);
                            g.DrawImage(tile, rectangleInsert);

                            if (LayersNames[indexLayer] == "Ground" || LayersNames[indexLayer] == "Grass")
                            {
                                MapString[iMap, jMap] = "0";
                            }
                            else if (LayersNames[indexLayer] == "Holes")
                            {
                                MapString[iMap, jMap] = "H";
                            }
                            else if (LayersNames[indexLayer] == "Rocks")
                            {
                                MapString[iMap, jMap] = "R";
                            }

                        }

                        iMap++;
                        if (iMap == MapWidth)
                        {
                            iMap = 0;
                            jMap++;
                        }
                    }

                    indexLayer++;
                }
                //g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red), 0, 0, bmp.Width, 0);
                //g.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Red), 0, 0, 0, bmp.Height);
            }

            return bmp;
        }
    }
}
