using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Game_Files.Managers.UniverseGenerator;

namespace Game_Files.Managers
{
    public class UniverseGenerator
    {
        public struct starStats
        {
            public bool exists;
            public char starType;  //O, B, A, F, G, K, L, M, T  - Determines heat and colour
            public int starSize;   //0, 1, 2, 3, 4, 5, 6, 7     - Determines magnitude, should be roman numerals
            //planetStats[] planets;                            - Will make planets once gal gen is working
        }

        //2D array for the galaxy that holds a star system in every index-able postion
        //Array index used that is x and y coordinates for the star system in galaxy
        //      eg: Star system at (14, 153) will be Array[14, 153]
        starStats[,] galaxyMap;

        private UInt32 genSeed = 0;
        private int chance = 0;
        private char[] starTypes = { 'O', 'B', 'A', 'F', 'G', 'K', 'L', 'M', 'T' };

        //SETTERS

        //GETTERS

        //GENERATION
        public void galaxyGeneration(int galaxyWidth, int galaxyHeight)
        {
            int tempStarType = 0;
            galaxyMap = new starStats[galaxyWidth, galaxyHeight];

            //Loop through all positions in the galaxy and randomly decide to place a star system there
            //Random chance will be greater the closer the index is to gal center (width/2, height/2)
            for (int x_pos = 0; x_pos < galaxyWidth; x_pos++)
            {
                for (int y_pos = 0; y_pos < galaxyHeight; y_pos++)
                {
                    //Initialize the random seed using the position of the star in the galaxy
                    genSeed = (UInt32)((x_pos & 0xFFFF) << 16 | (y_pos & 0xFFFF));
                    //Get the absolute value of the distance from center, greater value means less chance of star
                    chance = (int)Math.Abs(Math.Pow(x_pos - (galaxyWidth/2), 2) + (Math.Pow(y_pos - (galaxyHeight/2), 2)));
                    //If random number is greater than the chance, generate star, because greates chance is at 0: gal center
                    //Use of 1000000 is due to greatest distance from center being 1,213,200 without use of square-root for distance equation
                    //-2000000 added to lower bound to make universe less dense overall. Using 0 lower bound caused completely filled center
                    if(rndInt(-2000000, 1000000) > chance)
                    {
                        //Make the star
                        galaxyMap[x_pos, y_pos].exists = true;
                        tempStarType = rndInt(0, 8);                                    //9 Different star types, need to select one at random
                        galaxyMap[x_pos, y_pos].starType = starTypes[tempStarType];     //Assign the randomly chosen type using the helper array Types
                        galaxyMap[x_pos, y_pos].starSize = rndInt(0, 7);                //Randomly assign a size from 0 to 7
                    }
                    else
                    {
                        //No star exists here
                        galaxyMap[x_pos, y_pos].exists = false;
                    }
                }
            }
        }

        public void DrawGalaxyMap(SpriteBatch spriteBatch, int width, int height, Texture2D starTexture)
        {
            spriteBatch.Begin();
            for(int x_pos = 0; x_pos < width; x_pos++)
            {
                for(int y_pos = 0; y_pos < height; y_pos++)
                {
                    //Draw the dang thing, pixel by pixel!
                    if (galaxyMap[x_pos, y_pos].exists)
                    {
                        spriteBatch.Draw(starTexture, new Vector2(x_pos, y_pos), Color.White);
                    }
                }
            }
            spriteBatch.End();
        }

        public void LoadGalaxyData(string filePath)
        {

        }

        public void SaveGalaxyData(string filePath)
        {

        }

        double rndDouble(double min, double max)
        {
            return ((double)rnd() / (double)(0x7FFFFFFF)) * (max - min) + min;
        }

        int rndInt(int min, int max)
        {
            return (int)(rnd() % (max - min)) + min;
        }

        // Modified from this for 64-bit systems:
        // https://lemire.me/blog/2019/03/19/the-fastest-conventional-random-number-generator-that-can-pass-big-crush/
        // Now I found the link again - Also, check out his blog, it's a fantastic resource!
        UInt32 rnd()
        {
            genSeed += 0xe120fc15;
            UInt64 tmp;
            tmp = (UInt64)genSeed * 0x4a39b70d;
            UInt32 m1 = (UInt32)((tmp >> 32) ^ tmp);
            tmp = (UInt64)m1 * 0x12fad5c9;
            UInt32 m2 = (UInt32)((tmp >> 32) ^ tmp);
            return m2;
        }
    }


    //EVERYTHING BELOW HERE NEEDS TO BE REVIEWED OR SCRAPPED
    public class StarData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public int Size { get; set; }
        public List<PlanetData> Planets { get; set; }
    }

    public class PlanetData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
        public int DistanceFromStar { get; set; }
        public int OrbitalSpeed { get; set; }
    }

    public class GalaxyData
    {
        //Dictionary / hashmap that stores all galaxy map star data
        public Dictionary<int, StarData> Stars { get; set; } = new Dictionary<int, StarData>();
        //Name of the galaxy
        public String Name { get; set; }

        //Load the galaxy data from a save file in proper XML format
        public void LoadGalaxyData(string filePath)
        {
            XDocument xDoc = XDocument.Load(filePath);

            int x, y, z = 0;

            //Get the star data
            foreach (XElement starElement in xDoc.Descendants("star"))
            {
                StarData star = new StarData();

                star.Id = int.Parse(starElement.Attribute("id").Value);
                star.Name = starElement.Attribute("name").Value;
                x = int.Parse(starElement.Element("Position").Attribute("X").Value);
                y = int.Parse(starElement.Element("Position").Attribute("Y").Value);
                z = int.Parse(starElement.Element("Position").Attribute("Z").Value);
                star.Position = new Vector3(x, y, z);
                star.Size = int.Parse(starElement.Attribute("Radius").Value);

                //Get the planet data
                foreach (XElement planetElement in starElement.Elements())
                {
                    PlanetData planet = new PlanetData();

                    planet.Name = planetElement.Attribute("name").Value;
                    planet.Type = planetElement.Attribute("type").Value;
                    planet.Size = int.Parse(planetElement.Attribute("size").Value);
                    planet.DistanceFromStar = int.Parse(planetElement.Attribute("distance").Value);
                    planet.OrbitalSpeed = int.Parse(planetElement.Attribute("orbitalspeed").Value);

                    star.Planets.Add(planet);
                }

                Stars.Add(star.Id, star);
            }
        }

        //Save the galaxy data in an XML file
        public void SaveGalaxyData(string filePath)
        {
            XDocument xDoc = new XDocument();

            //Create root element
            XElement root = new XElement("GalaxyData");
            xDoc.Add(root);

            //Loop through all stars and create elements for them
            foreach (StarData star in Stars.Values)
            {
                //Make an element for the star with all associated attributes - new attributes need to be added here to be saved!
                XElement starElement = new XElement("Star",
                    new XAttribute("id", star.Id),
                    new XAttribute("name", star.Name),
                    new XElement("position",
                        new XAttribute("X", star.Position.X),
                        new XAttribute("Y", star.Position.Y),
                        new XAttribute("Z", star.Position.Z)));

                //Loop through all planets in the star and create elements for them
                foreach (PlanetData planet in star.Planets)
                {
                    //Make an element for the planet with all associated attributes - new attributes need to be added here to be saved!
                    XElement planetElement = new XElement("Planet",
                        new XAttribute("name", planet.Name),
                        new XAttribute("type", planet.Type),
                        new XAttribute("size", planet.Size),
                        new XAttribute("distance", planet.DistanceFromStar),
                        new XAttribute("orbitalspeed", planet.OrbitalSpeed));

                    //Add the new planet element to the star element
                    starElement.Add(planetElement);
                }

                //Add the new star element to the root
                root.Add(starElement);
            }

            //Save this shit!
            xDoc.Save(filePath);
        }

        //Function to display the galaxy data that has been generated or loaded from XML
        public void DisplayGalaxyMap(SpriteBatch spriteBatch, int width, int height, int galacticScaleFactor, Vector2 mapOffset, Texture2D starTexture)
        {
            float x, y, z = 0;

            var projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            var view = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);

            var world = Matrix.Identity;

            var transform = world * view * projection;

            // Set the basic drawing parameters
            spriteBatch.Begin(transformMatrix: transform);

            // Set the color and size of the stars
            Color starColor = Color.White;

            // Draw each star as a point on the map
            foreach (var star in Stars)
            {
                //Debug.WriteLine("Drawing a star!");
                // Calculate the 2D position of the star on the map
                //x = (star.Value.Position.X / galacticScaleFactor) + mapOffset.X;
                //y = (star.Value.Position.Y / galacticScaleFactor) + mapOffset.Y;
                //z = star.Value.Position.Z / galacticScaleFactor + mapOffset.Z;
                x = star.Value.Position.X;
                y = star.Value.Position.Y;

                // Draw the star on the map
                spriteBatch.Draw(starTexture, new Vector2(x, y), Color.White);
            }

            spriteBatch.End();
        }
    }

}
