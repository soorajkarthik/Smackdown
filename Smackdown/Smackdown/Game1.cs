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
        Texture2D backgroundTexture;
        SoundEffect maintheme;
        SoundEffect battle1;
        SoundEffect battle2;

        enum GameState
        {
            MainMenu,
            LevelSelection,
            PlayerLoadScreen,
            Play,
            PauseMenu,
            GameOver,
            Controls,
            Settings
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player temp;
        Player[] players;

        GameState gameState;

        Map map;

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

            players = new Player[2];

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

            temp = new Player(new Vector2(100, 100), Content.Load<Texture2D>("sprites/players/blueknight"), PlayerIndex.One, map, Content.Load<Texture2D>("sprites/balls/dodgeball"));
            backgroundTexture = Content.Load<Texture2D>("tiles/background1");
            map.spriteSheet = this.Content.Load<Texture2D>("tiles/tileset");

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            temp.Update(gameTime);
            // TODO: Add your update logic here
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.DarkSlateBlue);

            
            map.Draw(spriteBatch);
            temp.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
