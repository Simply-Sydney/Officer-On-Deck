using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game_Files.Managers;

namespace Game_Files
{
    public class Game1 : Game
    {
        //private BaseGameState _currentGameState;

        Texture2D starTexture;
        
        //Necessary datastructures for use with Monogame
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Resolution that the game screen was made around, to be rescaled from these values
        private const int DESIGNED_RESOLUTION_WIDTH = 1920;
        private const int DESIGNED_RESOLUTION_HEIGHT = 1080;

        //Aspect ratio made by the designed width and height
        private const float DESIGNED_RESOLUTION_ASPECT_RATIO = DESIGNED_RESOLUTION_WIDTH / (float)DESIGNED_RESOLUTION_HEIGHT;

        //Complate galaxy data structure
        private UniverseGenerator universeData = new UniverseGenerator();
        private int galViewWidth = 1920;
        private int galViewHeight = 1080;

        private bool _drawGalaxyMap = false;
        private int galacticScaleFactor = 1;
        private Vector2 mapOffset;

        //Mouse mouse-state for mouse related stating
        MouseState mouse;

        //Save file present in filesystem
        private bool SavePresent = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;

            starTexture = new Texture2D(GraphicsDevice, 1, 1);
            starTexture.SetData(new Color[] { Color.White });

            //Expand the playable window to the designed height and width in pixels
            _graphics.PreferredBackBufferWidth = DESIGNED_RESOLUTION_WIDTH;
            _graphics.PreferredBackBufferHeight = DESIGNED_RESOLUTION_HEIGHT;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            if (SavePresent)
            {
                universeData.LoadGalaxyData("galaxy.xml");
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                _drawGalaxyMap = !_drawGalaxyMap;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                //galaxyData.generateGalaxy(GalaxyData.galShapes.Elliptical, GalaxyData.galSize.Small);
                universeData.galaxyGeneration(DESIGNED_RESOLUTION_WIDTH, DESIGNED_RESOLUTION_HEIGHT);
            }

            mouse = Mouse.GetState();

            if (mouse.ScrollWheelValue < 0)
            {
                //Zoom out
            }
            else if (mouse.ScrollWheelValue > 0)
            {
                //Zoom in
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (_drawGalaxyMap)
            {
                GraphicsDevice.Clear(Color.Black);
                universeData.DrawGalaxyMap(_spriteBatch, galViewWidth, galViewHeight, starTexture);
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}