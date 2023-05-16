using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace GOL;

class Program {
    static void Main(string[] args) {
        ////
        // Setup
        // var WindowSize = new Vector2i(1920, 1080);
        var WindowSize = new Vector2i(1280, 720);

        InitWindow(WindowSize.X, WindowSize.Y, "Game of Life");
        SetTargetFPS(99999);

        var Matrix = new Matrix(WindowSize);


        ////
        // Main Loop
        while (!WindowShouldClose()) {
            ////
            // Update
            Matrix.Update();

            ////
            // Draw
            BeginDrawing();
            Matrix.Draw();
            EndDrawing();
        }

        ////
        // Exit
        CloseWindow();
    }
}
