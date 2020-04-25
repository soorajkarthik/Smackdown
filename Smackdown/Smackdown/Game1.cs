using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Smackdown
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState
        {
            MainMenu,
            LevelSelection,
            Play,
            PauseMenu,
            GameOver,
            Controls,
            Settings
        }

        private GameState gameState;
        private GamePadState oldp1gps;
        private Player temp;
        private int numPlayers;
        private Map map;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont largeFont;
        private SpriteFont medFont;
        private SpriteFont smallFont;

        private Texture2D emptyTex;
        private Texture2D mainMenuTex;
        private Texture2D backgroundTexture;

        private SoundEffect maintheme;
        private SoundEffect battle1;
        private SoundEffect battle2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;

            this.graphics.PreferredBackBufferWidth = 48 * 30;
            this.graphics.PreferredBackBufferHeight = 48 * 20;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameState = GameState.MainMenu;

            map = new Map(30, 20);
            numPlayers = 0; 
            map.loadMap(@"Content/maps/testmap2.txt");
            
            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            largeFont = Content.Load<SpriteFont>("fonts/SpriteFont1");
            medFont = Content.Load<SpriteFont>("fonts/SpriteFont2");
            smallFont = Content.Load<SpriteFont>("fonts/SpriteFont3");

            backgroundTexture = Content.Load<Texture2D>("tiles/background1");
            map.spriteSheet = Content.Load<Texture2D>("tiles/tileset");
            emptyTex = Content.Load<Texture2D>("tiles/empty");
            mainMenuTex = Content.Load<Texture2D>("tiles/empty"); //CHANGE THIS

            maintheme = Content.Load<SoundEffect>("music/Smackdown Main Theme");
            battle2 = Content.Load<SoundEffect>("music/Smackdown_Battle_Theme_02");


            temp = new Player(new Vector2(100, 100), Content.Load<Texture2D>("sprites/players/blueknight"), PlayerIndex.One, map, Content.Load<Texture2D>("sprites/balls/dodgeball"));


            //TEMP MUSIC
            //battle2.Play();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Allows the game to exit
            GamePadState p1gps = GamePad.GetState(PlayerIndex.One);
            if (p1gps.IsButtonDown(Buttons.LeftShoulder) && p1gps.IsButtonDown(Buttons.RightShoulder))
                this.Exit();
            
            switch(gameState)
            {
                case GameState.MainMenu:
                    if (p1gps.IsButtonDown(Buttons.LeftThumbstickRight) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickRight)) 
                        numPlayers++;
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickLeft))
                        numPlayers+=3;
                    if(p1gps.IsButtonDown(Buttons.Start))
                    {
                        gameState = GameState.Play;
                    }
                    numPlayers %= 4;
                    break;

                case GameState.Play:
                    temp.Update(gameTime);
                    for (int i = 0; i < numPlayers; i++)
                        if (GamePad.GetState((PlayerIndex)i).IsButtonDown(Buttons.Back))
                            gameState = GameState.PauseMenu;
                    break;

                case GameState.PauseMenu:
                    for(int i = 0; i < numPlayers; i++)
                        if (GamePad.GetState((PlayerIndex)i).IsButtonDown(Buttons.Start))
                            gameState = GameState.Play;
                    break;

                default:
                    break;
            }

            // TODO: Add your update logic here

            oldp1gps = p1gps;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(emptyTex, new Rectangle(numPlayers * 360, 300, 360, 360), Color.Black*.5f);
                    break;

                case GameState.Play:
                    spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.DarkSlateBlue);
                    map.Draw(spriteBatch);
                    temp.Draw(spriteBatch);
                    break;

                case GameState.PauseMenu:
                    spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.DarkSlateBlue);
                    map.Draw(spriteBatch);
                    temp.Draw(spriteBatch);
                    spriteBatch.Draw(emptyTex, GraphicsDevice.Viewport.Bounds, Color.Black * 0.65f);
                    spriteBatch.DrawString(largeFont, "Paused", new Vector2(535, 400), Color.White);
                    spriteBatch.DrawString(medFont, "Press start to resume", new Vector2(420, 540), Color.White);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
