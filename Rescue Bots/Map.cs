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
        public string[,] MapStringObjects { get; set; }
        public string Tileset { get; set; }
        public List<int[]> Tiles { get; set; }
        public List<string> LayersNames { get; set; }
        public Bitmap MapBackgroundBitmap { get; set; }
        public System.Windows.Controls.Image MapBackground { get; set; }
        public List<GameObject> Targets { get; set; }
        public List<Tractor> Tractors { get; set; }

        /// <summary>
        /// Создание карты из файла
        /// </summary>
        /// <param name="path"></param>
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
            Tractors = new List<Tractor>();
            Targets = new List<GameObject>();

            // парсинг xml-файла карта
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

            LoadMap();
            MapBackground = ImageDrawer.BitmapToControl(MapBackgroundBitmap);
        }

        /// <summary>
        /// Отрисовка карты на Bitmap с использованием тайлсета в формате png
        /// </summary>
        public void LoadMap()
        {
            Bitmap bmp = new Bitmap(TileWidth * MapWidth, TileHeight * MapHeight);
            Graphics g = Graphics.FromImage(bmp);

            Bitmap tileset = (Bitmap)System.Drawing.Image.FromFile("tiles 4.png");
            System.Drawing.Imaging.PixelFormat format = tileset.PixelFormat;

            int iTilesetMax = tileset.Width / TileWidth;

            int indexLayer = 0;

            foreach (int[] layer in Tiles)
            {
                int iMap = 0;
                int jMap = 0;

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

                        if (LayersNames[indexLayer] == "Background" || LayersNames[indexLayer] == "Back")
                        {
                            g.DrawImage(tile, rectangleInsert);
                            MapString[iMap, jMap] = "1";
                        }
                        else if (LayersNames[indexLayer] == "Blocks" || LayersNames[indexLayer] == "Block")
                        {
                            g.DrawImage(tile, rectangleInsert);
                            MapString[iMap, jMap] = "B";
                            GameObject o = Targets.Find(a => a.X == iMap && a.Y == jMap);
                            if (o != null) Targets.Remove(o);
                        }
                        else if (LayersNames[indexLayer] == "Targets" || LayersNames[indexLayer] == "Target")
                        {
                            if (MapString[iMap, jMap] != "B")
                            {
                                Targets.Add(new GameObject(tile, iMap, jMap));
                                MapString[iMap, jMap] = "0";
                            }
                        }
                        else if (LayersNames[indexLayer] == "Tractors" || LayersNames[indexLayer] == "Tractor")
                        {
                            Tractors.Add(new Tractor(Tractors.Count + 1, tile, iMap, jMap, "Трактор"));
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
            MapBackgroundBitmap = bmp;
            g.Dispose();
        }
    }
}
