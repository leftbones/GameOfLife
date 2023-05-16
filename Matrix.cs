using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace GOL;

class Matrix {
    public Vector2i WindowSize { get; private set; }
    public Vector2i Size { get; private set; }
    public int[,] Cells { get; private set; }
    public int[,] Buffer { get; private set; }

    public Vector2i Center { get { return Size / 2; } }

    public int Ticks { get; private set; } = 0;
    public int TickTimer { get; private set; } = 0;
    public int TickRate { get; private set; }

    public Color LiveColor = new Color(255, 255, 255, 255);     // Color of a living cell
    public Color DeadColor = new Color(0, 0, 0, 255);           // Color of a dead cell

    public Image BufferImage;
    public Texture2D BufferTexture;

    private int SimulationSpeed = 0;            // Time (in "ticks") between generations, actual simulation speed is limited by hardware

    private int Scale = 2;                      // Cell to screen pixel ratio, higher numbers greatly increase speed

    private bool Active = false;                // Simulation pause/unpause (this is the default state, set to `true` to start unpaused)

    private string RebirthRule = "36";          // Rules for a cell to be reborn (valid number of living neighbors)
    private string SurviveRule = "23";          // Rules for a cell to die (valid number of living neighbors)

    private bool Fullscreen = false;            // Run the simulation in full screen (actual resolution is set in Program.cs)

    private bool Fade = true;                   // Cells should fade out when dying instead of changing instantly
    private int FadeAmount = 1;                 // How quickly cells should fade out when dead

    private bool SquareStart = false;           // Simulation starts with a small square of random cells rather than entirely random cells
    private int SquareSize = 100;               // The size of the random square

    private int StartRatio = 25;                // The % chance of a cell being alive during the random starting generation (in the square or full matrix)

    private List<Vector2i> Directions = new List<Vector2i>() {
        new Vector2i(-1, 0),    // Left
        new Vector2i(-1, -1),   // Up + Left
        new Vector2i(0, -1),    // Up
        new Vector2i(1, -1),    // Up + Right
        new Vector2i(1, 0),     // Right
        new Vector2i(1, 1),     // Down + Right
        new Vector2i(0, 1),     // Down
        new Vector2i(-1, 1)     // Down + Left
    };

    public Matrix(Vector2i size) {
        WindowSize = size;
        Size = WindowSize / Scale;
        Cells = new int[Size.X, Size.Y];
        Buffer = new int[Size.X, Size.Y];

        TickRate = SimulationSpeed;

        if (Fullscreen)
            Raylib.ToggleFullscreen();

        // Generate Matrix
        Generate();

        // Create Buffer Image + Texture
        BufferImage = GenImageColor(Size.X, Size.Y, DeadColor);
        BufferTexture = LoadTextureFromImage(BufferImage);
    }

    public void Generate() {
        // Erase Cells + Buffer
        Cells = new int[Size.X, Size.Y];
        Buffer = new int[Size.X, Size.Y];

        // Random Square Start
        if (SquareStart) {
            for (int x = Center.X - (SquareSize / 2); x < Center.X + (SquareSize / 2); x++) {
                for (int y = Center.Y - (SquareSize / 2); y < Center.Y + (SquareSize / 2); y++) {
                    Cells[x, y] = RNG.Chance(StartRatio) ? 255 : 0;
                    Buffer[x, y] = 1;
                }
            }
        }

        // Random Full Start
        else {
            for (int x = 0; x < Size.X; x++) {
                for (int y = 0; y < Size.Y; y++) {
                    Cells[x, y] = RNG.Chance(StartRatio) ? 255 : 0; 
                    Buffer[x, y] = Cells[x, y];
                }
            }
        }
    }

    public bool InBounds(Vector2i pos) {
        return pos.X > 0 && pos.X < Size.X && pos.Y > 0 && pos.Y < Size.Y;
    }

    public int Get(Vector2i pos) {
        int DX = pos.X;
        int DY = pos.Y;
        if (pos.X < 0) DX = Size.X - 1;
        if (pos.Y < 0) DY = Size.Y - 1;
        if (pos.X > Size.X - 1) DX = 0;
        if (pos.Y > Size.Y - 1) DY = 0;
        return Cells[DX, DY];
    }

    public void Flip(Vector2i pos) {
        int c = Buffer[pos.X, pos.Y];
        Buffer[pos.X, pos.Y] = c < 255 ? 255 : 254;
    }

    public Tuple<int, int> Count(Vector2i pos) {
        int Live = 0;
        int Dead = 0;
        foreach (Vector2i Dir in Directions) {
            if (Get(pos + Dir) == 255) Live++;
            else Dead++;
        }
        return new Tuple<int, int>(Live, Dead);
    }

    public void Input() {
        // Pause/Unpause
        if (IsKeyPressed(KeyboardKey.KEY_SPACE))
            Active = !Active;

        // Reset
        if (IsKeyPressed(KeyboardKey.KEY_R))
            Generate();

        // Double Speed
        TickRate = IsKeyDown(KeyboardKey.KEY_F) ? 0 : SimulationSpeed;
    }

    public void Update() {
        Input();

        if (!Active)
            return;

        TickTimer--;
        if (TickTimer <= 0) {
            TickTimer = TickRate;
            for (int x = 0; x < Size.X; x++) {
                for (int y = 0; y < Size.Y; y++) {
                    var Pos = new Vector2i(x, y);
                    int Cell = Get(Pos);

                    Tuple<int, int> NC = Count(Pos);
                    int Live = NC.Item1;
                    int Dead = NC.Item2;

                    if (Cell == 255) {
                        if (!SurviveRule.Contains(Live.ToString())) Flip(Pos);
                    } else {
                        if (RebirthRule.Contains(Live.ToString())) Flip(Pos);
                        else if (Cell > 0) Buffer[x, y] = Math.Max(Buffer[x, y] - FadeAmount, 0);
                    }
                }
            }

            for (int x = 0; x < Size.X; x++) {
                for (int y = 0; y < Size.Y; y++) {
                    Cells[x, y] = Buffer[x, y];
                }
            }
        }
    }

    public unsafe void Draw() {
        // Update Matrix Texture
        ClearBackground(DeadColor);
        ImageClearBackground(ref BufferImage, DeadColor);

        for (int x = 0; x < Size.X; x++) {
            for (int y = 0; y < Size.Y; y++) {
                int Cell = Cells[x, y];
                var Color = LiveColor;
                if (Cell < 255) {
                    if (Fade) Color = new Color(LiveColor.r, LiveColor.g, LiveColor.b, Cell);
                    else Color = DeadColor;
                }
                ImageDrawPixel(ref BufferImage, x, y, Color);
            }
        }

        UpdateTexture(BufferTexture, BufferImage.data);
        DrawTexturePro(BufferTexture, new Rectangle(0, 0, Size.X, Size.Y), new Rectangle(0, 0, WindowSize.X, WindowSize.Y), Vector2.Zero, 0, Color.WHITE);

        // Draw Help Text (when paused)
        if (!Active) {
			DrawRectangle(5, 5, 250, 80, new Color(DeadColor.r, DeadColor.g, DeadColor.b, (byte)200));
            DrawText("SPACE - Start/Stop", 15, 15, 20, Color.WHITE);
            DrawText("R - Reset Simulation", 15, 35, 20, Color.WHITE);
			DrawText("F - Speed up (hold)", 15, 55, 20, Color.WHITE);
        }
    }
}