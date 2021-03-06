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
            Controls,
            Settings,
            PlayerSelection,
            MapSelection,
            Play,
            PauseMenu,
            GameOver
        }

        private GameState gameState;
        private GamePadState oldp1gps;
        private List<Player> players;
        private Player winner;
        private int highlightIndex;
        private Map map;

        private Texture2D tileset1;
        private Texture2D tileset2;
        private Texture2D tileset3;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont largeFont;
        private SpriteFont medFont;
        private SpriteFont medFont2;
        private SpriteFont smallFont;

        private Texture2D emptyTex;
        private Texture2D mainMenuTex;
        private Texture2D playerSelectionTex;
        private Texture2D mapSelectionTex;
        private Texture2D controlScreenTex;
        private Texture2D settingsScreenTex;
        private Texture2D backgroundTex;

        private Song mainTheme;
        private Song[] battleThemes;
        private SoundEffect pause, unpause;

        private int currentSong; //for battle themes only
        private long songTimeLeft; //for battle themes only
        private long unpauseTimeLeft;

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
            highlightIndex = 0;

            map = new Map(30, 20);

            MediaPlayer.IsRepeating = true;
            songTimeLeft = unpauseTimeLeft = currentSong = 0;

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
            medFont2 = Content.Load<SpriteFont>("fonts/SpriteFont3");
            smallFont = Content.Load<SpriteFont>("fonts/SpriteFont4");

            backgroundTex = Content.Load<Texture2D>("tiles/background1");
            tileset1 = Content.Load<Texture2D>("tiles/tileset1");
            tileset2 = Content.Load<Texture2D>("tiles/tileset2");
            tileset3 = Content.Load<Texture2D>("tiles/tileset3");
            emptyTex = Content.Load<Texture2D>("menus/empty");
            mainMenuTex = Content.Load<Texture2D>("menus/mainMenu");
            controlScreenTex = Content.Load<Texture2D>("menus/controls");
            settingsScreenTex = Content.Load<Texture2D>("menus/settings");
            playerSelectionTex = Content.Load<Texture2D>("menus/playerSelection");
            mapSelectionTex = Content.Load<Texture2D>("menus/mapSelection");

            //music and sound effects
            mainTheme = Content.Load<Song>("music/Smackdown Main Theme");
            battleThemes = new Song[]
            {
                Content.Load<Song>("music/Smackdown_Battle_Theme_01"),
                Content.Load<Song>("music/Smackdown_Battle_Theme_02")
            };

            pause = Content.Load<SoundEffect>("music/PauseButton");
            unpause = Content.Load<SoundEffect>("music/UnpauseButton");

            MediaPlayer.Play(mainTheme);
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

            switch (gameState)
            {
                case GameState.MainMenu:
                    if (p1gps.IsButtonDown(Buttons.A) && !oldp1gps.IsButtonDown(Buttons.A))
                    {
                        switch (highlightIndex)
                        {
                            case 0:
                                gameState = GameState.PlayerSelection;
                                break;
                            case 1:
                                gameState = GameState.Controls;
                                break;
                            case 2:
                                highlightIndex = MediaPlayer.IsMuted ? 1 : 0;
                                gameState = GameState.Settings;
                                break;
                            case 3:
                                this.Exit();
                                break;
                        }
                    }

                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickDown) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickDown))
                        highlightIndex++;
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickUp) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickUp))
                        highlightIndex += 3;

                    highlightIndex %= 4;
                    break;

                case GameState.Controls:
                    if (p1gps.IsButtonDown(Buttons.B))
                        gameState = GameState.MainMenu;
                    break;

                case GameState.Settings:


                    if (Math.Abs(p1gps.ThumbSticks.Left.X) >= 0.75 && Math.Abs(oldp1gps.ThumbSticks.Left.X) <= 0.65)
                        highlightIndex++;
                    highlightIndex %= 2;

                    MediaPlayer.IsMuted = highlightIndex == 1;

                    if (p1gps.IsButtonDown(Buttons.B))
                    {
                        highlightIndex = 2;
                        gameState = GameState.MainMenu;

                    }

                    break;

                case GameState.PlayerSelection:

                    if (p1gps.IsButtonDown(Buttons.A) && !oldp1gps.IsButtonDown(Buttons.A))
                    {
                        spawnPlayers(highlightIndex + 2); //accounting for the fact that we're using index for highlighting box   
                        highlightIndex = 0;
                        gameState = GameState.MapSelection;
                    }
                    else if (p1gps.IsButtonDown(Buttons.B) && !oldp1gps.IsButtonDown(Buttons.B))
                    {
                        highlightIndex = 0;
                        gameState = GameState.MainMenu;
                    }
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickRight) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickRight))
                        highlightIndex++;
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickLeft))
                        highlightIndex += 2;

                    highlightIndex %= 3;
                    break;

                case GameState.MapSelection:
                    if (p1gps.IsButtonDown(Buttons.A) && !oldp1gps.IsButtonDown(Buttons.A))
                    {
                        try
                        {
                            map.loadMap(@"Content/maps/map" + (highlightIndex + 1) + ".txt");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Error Loading Map: Map " + (highlightIndex + 1) + " not found.");
                            map.loadMap(@"Content/maps/map1.txt");
                        }

                        try
                        {
                            map.spriteSheet = this.Content.Load<Texture2D>("tiles/tileset" + (highlightIndex + 1));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Error Loading Tiles: Specified Tile sheet for map " + (highlightIndex + 1) + " not found.");
                            map.spriteSheet = this.Content.Load<Texture2D>("tiles/tileset1");
                        }


                        gameState = GameState.Play;
                        highlightIndex = 0;

                        songTimeLeft = battleThemes[currentSong].Duration.Ticks;
                        MediaPlayer.Play(battleThemes[currentSong]);

                    }
                    else if (p1gps.IsButtonDown(Buttons.B) && !oldp1gps.IsButtonDown(Buttons.B))
                    {
                        highlightIndex = 0;
                        gameState = GameState.PlayerSelection;
                    }
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickRight) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickRight))
                        highlightIndex++;
                    else if (p1gps.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldp1gps.IsButtonDown(Buttons.LeftThumbstickLeft))
                        highlightIndex += 2;

                    highlightIndex %= 3;
                    break;

                case GameState.Play:

                    if (unpauseTimeLeft > 0)
                    {
                        unpauseTimeLeft--;
                        if (unpauseTimeLeft == 90)
                        {
                            MediaPlayer.Resume();
                        }
                    }
                    else
                    {
                        songTimeLeft--;
                        if (songTimeLeft == 0)
                        {
                            currentSong++;
                            currentSong %= 2;
                            songTimeLeft = battleThemes[currentSong].Duration.Ticks;
                            MediaPlayer.Play(battleThemes[currentSong]);
                        }
                    }



                    List<Dodgeball> currBalls = new List<Dodgeball>();
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player player = players[i];

                        if (players.Count == 1)
                        {
                            gameState = GameState.GameOver;
                            winner = player;
                            winner.OnGameOver();
                            MediaPlayer.Stop();
                            MediaPlayer.Play(mainTheme);
                            break;
                        }

                        else if (player.DeadAnimationEnded)
                        {
                            players.Remove(player);
                            i--;
                            continue;
                        }

                        player.Update(gameTime);
                        currBalls.AddRange(player.activeBalls);
                    }

                    currBalls.ForEach(ball =>
                    {
                        foreach (Player player in players)
                        {
                            if (ball.owner != player.playerIndex && ball.bounds.Intersects(player.bounds) && ball.bounds.GetIntersectionDepth(player.bounds).Length() >= 5f)
                            {
                                player.isAlive = false;
                                ball.velocity.X /= 50;
                                break;
                            }
                        }
                    });



                    for (int i = 0; i < players.Count; i++)
                    {
                        if (GamePad.GetState((PlayerIndex)i).IsButtonDown(Buttons.Start))
                        {
                            gameState = GameState.PauseMenu;

                            MediaPlayer.Pause();
                            pause.Play();

                        }


                        for (int k = 0; k < players.Count; k++)
                        {
                            for (int j = 0; j < players[k].activeBalls.Count; j++)
                            {
                                if (players[i].gps.IsButtonDown(Buttons.B) && players[i].oldgps.IsButtonDown(Buttons.B) && getDistance(players[k].activeBalls[j].position, players[i].position) < 40)
                                {
                                    players[i].balls++;
                                    players[k].activeBalls.RemoveAt(j);
                                }
                            }

                        }
                    }


                    break;

                case GameState.PauseMenu:
                    for (int i = 0; i < players.Count; i++)
                    {
                        GamePadState gps = GamePad.GetState((PlayerIndex)i);

                        if (gps.IsButtonDown(Buttons.Back))
                        {
                            gameState = GameState.Play;

                            unpause.Play();
                            unpauseTimeLeft = unpause.Duration.Seconds * 60;

                        }
                        else if (gps.IsButtonDown(Buttons.LeftShoulder) && gps.IsButtonDown(Buttons.RightShoulder))
                        {
                            gameState = GameState.MainMenu;

                            MediaPlayer.Play(mainTheme);

                        }
                    }

                    break;

                case GameState.GameOver:
                    winner.Update(gameTime);

                    for (int i = 0; i < 4; i++)
                    {
                        GamePadState temp = GamePad.GetState((PlayerIndex)i);
                        if (temp.IsButtonDown(Buttons.LeftShoulder) && temp.IsButtonDown(Buttons.RightShoulder))
                            gameState = GameState.MainMenu;
                    }

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
                for (int i = 0; i < numPlayers; i++)
                {
                    String[] input = reader.ReadLine().Split(' ');
                    Vector2 pos = new Vector2(Convert.ToInt32(input[0]), Convert.ToInt32(input[1]));
                    players.Add(new Player(pos, Content.Load<Texture2D>(input[2]), (PlayerIndex)i, map, Content.Load<Texture2D>(input[3])));
                }
            }
        }

        private int getDistance(Vector2 one, Vector2 two)
        {
            return Math.Abs((int)Math.Sqrt(Math.Pow(two.X - one.X, 2) + Math.Pow(two.Y - one.Y, 2)));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTex, GraphicsDevice.Viewport.Bounds, Color.DarkSlateBlue);

            switch (gameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(mainMenuTex, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.DrawString(medFont, "Play", new Vector2(320, 235), Color.Black);
                    spriteBatch.DrawString(medFont, "Controls", new Vector2(253, 385), Color.Black);
                    spriteBatch.DrawString(medFont, "Settings", new Vector2(253, 535), Color.Black);
                    spriteBatch.DrawString(medFont, "Quit", new Vector2(320, 685), Color.Black);
                    spriteBatch.Draw(emptyTex, new Rectangle(200, (150 * highlightIndex) + 180, 360, 150), Color.DarkBlue * .35f);
                    break;

                case GameState.Controls:
                    spriteBatch.Draw(controlScreenTex, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.DrawString(medFont2, "Controls", new Vector2(530, 100), Color.SlateGray);
                    break;

                case GameState.Settings:
                    spriteBatch.Draw(settingsScreenTex, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.DrawString(medFont2, "Settings", new Vector2(530, 100), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "Music On", new Vector2(430, 345), Color.Black);
                    spriteBatch.DrawString(medFont, "Music Off", new Vector2(770, 345), Color.Black);
                    spriteBatch.Draw(emptyTex, new Rectangle((highlightIndex * 360) + 360, 290, 360, 150), Color.DarkBlue * .35f);
                    break;

                case GameState.PlayerSelection:
                    spriteBatch.Draw(playerSelectionTex, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.Draw(emptyTex, new Rectangle(highlightIndex * 480, 300, 480, 360), Color.DarkBlue * .35f);
                    spriteBatch.DrawString(medFont, "How Many Fighters?", new Vector2(465, 100), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "A to confirm, B to return to Main Menu", new Vector2(210, 800), Color.SlateGray);
                    break;
                case GameState.MapSelection:
                    spriteBatch.Draw(mapSelectionTex, GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.Draw(emptyTex, new Rectangle(highlightIndex * 480, 300, 480, 360), Color.DarkBlue * .35f);
                    spriteBatch.DrawString(medFont, "What Map Would You Like to Play?", new Vector2(300, 100), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "A to confirm, B to return to Player Selection", new Vector2(130, 800), Color.SlateGray);
                    break;
                case GameState.Play:
                    map.Draw(spriteBatch);
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].isAlive)
                        {
                            String ballStr = "";
                            for (int j = 0; j < players[i].balls; j++)
                            {
                                ballStr += "1 ";
                            }
                            spriteBatch.DrawString(smallFont, "" + ballStr + "", new Vector2(players[i].position.X - 10, players[i].position.Y - 50), Color.Yellow);
                        }
                    }
                    players.ForEach(player => player.Draw(spriteBatch));

                    break;

                case GameState.PauseMenu:
                    map.Draw(spriteBatch);
                    players.ForEach(player => player.Draw(spriteBatch));

                    spriteBatch.Draw(emptyTex, GraphicsDevice.Viewport.Bounds, Color.Black * 0.65f);
                    spriteBatch.DrawString(largeFont, "Paused", new Vector2(535, 300), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "Press back to resume", new Vector2(435, 440), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "Press LB and RB to quit", new Vector2(420, 520), Color.SlateGray);
                    break;

                case GameState.GameOver:
                    winner.Draw(spriteBatch);
                    spriteBatch.DrawString(medFont2, String.Format("Player {0} wins!", ((int)winner.playerIndex) + 1), new Vector2(445, 250), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "Press LB and RB to", new Vector2(460, 650), Color.SlateGray);
                    spriteBatch.DrawString(medFont, "return to main menu", new Vector2(440, 715), Color.SlateGray);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
