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
using System.IO;

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
        private List<Player> players;
        private int numPlayerSelectionIndex;
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
            numPlayerSelectionIndex = 0;

            map = new Map(30, 20);
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
            emptyTex = Content.Load<Texture2D>("menus/empty");
            mainMenuTex = Content.Load<Texture2D>("menus/playerSelection");

            maintheme = Content.Load<SoundEffect>("music/Smackdown Main Theme");
            battle2 = Content.Load<SoundEffect>("music/Smackdown_Battle_Theme_02");

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
                        numPlayerSelectionIndex++;
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickLeft))
                        numPlayerSelectionIndex += 2;
                    numPlayerSelectionIndex %= 3;
                    if (p1gps.IsButtonDown(Buttons.Start))
                    {
                        spawnPlayers(numPlayerSelectionIndex + 2); //accounting for the fact that we're using index for highlighting box                                            
                        gameState = GameState.Play;
                    }
                    break;

                case GameState.Play:
                    players.ForEach(player => player.Update(gameTime));
                    for (int i = 0; i < numPlayerSelectionIndex; i++)
                        if (GamePad.GetState((PlayerIndex)i).IsButtonDown(Buttons.Back))
                            gameState = GameState.PauseMenu;
                    break;

                case GameState.PauseMenu:
                    for(int i = 0; i < numPlayerSelectionIndex; i++)
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

        private void spawnPlayers(int numPlayers)
        {
            players = new List<Player>();
            using (StreamReader reader = new StreamReader(@"Content/maps/playerSpawnInfo.txt"))
            {
                for(int i = 0; i < numPlayers; i++)
                {
                    String[] input = reader.ReadLine().Split(' ');
                    Vector2 pos = new Vector2(Convert.ToInt32(input[0]), Convert.ToInt32(input[1]));
                    players.Add(new Player(pos, Content.Load<Texture2D>(input[2]), (PlayerIndex)i, map, Content.Load<Texture2D>(input[3])));
                }
            }
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
                    spriteBatch.Draw(mainMenuTex, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.Draw(emptyTex, new Rectangle(numPlayerSelectionIndex * 480, 300, 480, 360), Color.SteelBlue*.25f);
                    spriteBatch.DrawString(medFont, "How Many Fighters?", new Vector2(465, 100), Color.Black);
                    spriteBatch.DrawString(medFont, "Press start to confirm", new Vector2(400, 800), Color.Black);
                    break;

                case GameState.Play:
                    spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.DarkSlateBlue);
                    map.Draw(spriteBatch);
                    players.ForEach(player => player.Draw(spriteBatch));
                    break;

                case GameState.PauseMenu:
                    spriteBatch.Draw(backgroundTexture, GraphicsDevice.Viewport.Bounds, Color.DarkSlateBlue);
                    map.Draw(spriteBatch);
                    players.ForEach(player => player.Draw(spriteBatch));
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
