using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Open_TK_Tut_1;

public class Game : GameWindow
{
    private float n;
    private Shader shader;
    private Shader litShader;

    private Shader bulletShader;

    private Texture ground;


    public int score;
    public float timer;
    private bool gameRunning = true;


    public static readonly List<GameObject> LitObjects = new();
    public static readonly List<GameObject> UnLitObjects = new();
    public static readonly List<GameObject> transparentUnlitObjects = new();
    public static readonly List<GameObject> ShootObjects = new();
    public static readonly List<PointLight> Lights = new();

    public static Matrix4 view;
    public static Matrix4 projection;

    public static Camera gameCam;
    private Vector2 previousMousePos;


    private readonly string[] PointLightDefinition =
    {
        "pointLights[",
        "INDEX",
        "]."
    };

    public Game(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings { Title = title, Size = (width, height) })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.2f, 0.2f, 0);

        previousMousePos = new Vector2(MouseState.X, MouseState.Y);
        CursorState = CursorState.Grabbed;
        score = 0;


        //Enable blending
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);

        shader = new Shader("shader.vert", "shader.frag");
        litShader = new Shader("shader.vert", "Lit_shader.frag");
        bulletShader = new Shader("BulletShader\\bullet.vert", "BulletShader\\bullet.frag");

        SpawnRandomCubes(3);

        gameCam = new Camera(Vector3.UnitZ * 3, (float)Size.X / Size.Y);



            /*water sin wave*/
        transparentUnlitObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices,
            new Shader("shader.vert", "WaveTut//Water.frag")));
        transparentUnlitObjects[0].transform.Position = Vector3.UnitZ * 15 + Vector3.UnitY * -0.8f;
        transparentUnlitObjects[0].transform.Scale = new Vector3(5, 5, 5);
        transparentUnlitObjects[0].transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);

        
        StaticUtilities.CheckError("C");


          /*floor tile*/
        var importer = new AssimpContext();
        var postProcessSteps = PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs;
        var scene = importer.ImportFile(StaticUtilities.ObjectDirectory + "plane.fbx", postProcessSteps);

        LitObjects.Add(new GameObject(scene.Meshes[0].MergeMeshData(), scene.Meshes[0].GetUnsignedIndices(),
            new Shader("GroundShader\\hill.vert", "GroundShader\\ground.frag")));
        LitObjects[0].transform.Position = gameCam.Position - new Vector3(0, 1, 0);
        LitObjects[0].transform.Scale = new Vector3(50, 50, 50);
        LitObjects[0].transform.Rotation = new Vector3(-MathHelper.PiOver2, 0, 0);

        ground = new Texture("ground.png");
        ground.Use(TextureUnit.Texture1);

        LitObjects[0].MyShader.Use();
        GL.Uniform1(LitObjects[0].MyShader.GetUniformLocation("groundTexture"), 1);

        /*explosion*/
        transparentUnlitObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices,
            new Shader("UVTut\\flipbook.vert", "UVTut\\flipbook.frag")));
        transparentUnlitObjects[1].transform.Position = new Vector3(100, 100, 100);

    
        var explosion = new Texture("Explosion00_5x5.png");
        explosion.Use(TextureUnit.Texture2);

        transparentUnlitObjects[1].MyShader.Use();
        GL.Uniform1(transparentUnlitObjects[1].MyShader.GetUniformLocation("tex3"), 2);

            /*tree billboard*/
        transparentUnlitObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices,
            new Shader("Billboard\\Billboard.vert", "Billboard\\Billboard.frag")));
        transparentUnlitObjects[2].transform.Position = new Vector3(5, 0 , 0);
        


        var tree = new Texture("Tree.png");
        tree.Use(TextureUnit.Texture3);

        transparentUnlitObjects[2].MyShader.Use();
        GL.Uniform1(transparentUnlitObjects[2].MyShader.GetUniformLocation("tex1"), 3);


        /*reticle */
        transparentUnlitObjects.Add(new GameObject(StaticUtilities.QuadVertices, StaticUtilities.QuadIndices,
            new Shader("UVTut\\reticle.vert", "UVTut\\reticle.frag")));

        transparentUnlitObjects[3].MyShader.Use();
        GL.Uniform1(transparentUnlitObjects[3].MyShader.GetUniformLocation("aspectRatio"), (float)Size.X / Size.Y);


        Lights.Add(new PointLight(new Vector3(1, 1, 1), 0.8f));
        Lights[0].Transform.Position = Vector3.UnitX + Vector3.UnitY * 6 + Vector3.UnitZ * 3;


         bulletShader.Use();
        int id = bulletShader.GetUniformLocation("bulletColor");
        GL.Uniform4(id, new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
    }

    protected override void OnUnload()
    {
        //Free GPU RAM
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.UseProgram(0);

        foreach (var gameObjectList in new List<List<GameObject>>
                     { UnLitObjects, LitObjects, ShootObjects, transparentUnlitObjects })
        foreach (var gameObject in gameObjectList)
            gameObject.Dispose();

        foreach (var gameObject in ShootObjects) gameObject.Dispose();

        shader.Dispose();

        base.OnUnload();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        //MUST BE FIRST
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        view = gameCam.GetViewMatrix();
        projection = gameCam.GetProjectionMatrix();


     
     


        transparentUnlitObjects[0].MyShader.Use();
        var idx = transparentUnlitObjects[0].MyShader.GetUniformLocation("time");
        GL.Uniform1(idx, n);


        foreach (var unlit in UnLitObjects) unlit.Render();


        foreach (var lit in LitObjects)
        {
            int id;
            lit.MyShader.Use();
            for (var i = 0; i < Lights.Count; ++i)
            {
                var currentLight = Lights[i];
                PointLightDefinition[1] = i.ToString();
                var merged = string.Concat(PointLightDefinition);


                id = lit.MyShader.GetUniformLocation(merged + "lightColor");
                GL.Uniform3(id, currentLight.Color);
                id = lit.MyShader.GetUniformLocation(merged + "lightPosition");
                GL.Uniform3(id, currentLight.Transform.Position);
                id = lit.MyShader.GetUniformLocation(merged + "lightIntensity");
                GL.Uniform1(id, currentLight.Intensity);
            }

            id = lit.MyShader.GetUniformLocation("numPointLights");
            GL.Uniform1(id, Lights.Count);


            id = lit.MyShader.GetUniformLocation("time");
            GL.Uniform1(id, n);

            lit.Render();
        }


        foreach (var bullet in ShootObjects)
        {
            bullet.Render();
        }

        foreach (var unlit in transparentUnlitObjects) unlit.Render();

        transparentUnlitObjects[1].MyShader.Use();
        GL.Uniform1(transparentUnlitObjects[1].MyShader.GetUniformLocation("time"), n);


       


        //MUST BE LAST
        SwapBuffers();
    }

    private bool leftMouseButtonPressed;
    private const float bulletSpeed = 0.5f;
    private const float bulletLifetime = 2f;
    private const float gameTime = 30;


    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);


        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        n += (float)args.Time;
        timer += (float)args.Time;


        Lights[0].Transform.Position = Vector3.UnitY * MathF.Sin(n * 0.2f) * 5 + Vector3.UnitZ * 5;
        Lights[0].Transform.Position = Quaternion.FromAxisAngle(Vector3.UnitY, n * 0.2f) * Lights[0].Transform.Position;


        if (MouseState.IsButtonDown(MouseButton.Left) && !leftMouseButtonPressed)
        {
            leftMouseButtonPressed = true;

            // Create and add a new game object
            var bullet = new GameObject(StaticUtilities.BoxVertices, StaticUtilities.BoxIndices, bulletShader);
            bullet.transform.Position = gameCam.Position + gameCam.Forward * 1.0f;
            bullet.transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            bullet.Lifetime = 0.0f; // Initialize the lifetime
            
            ShootObjects.Add(bullet);
        }
        else if (!MouseState.IsButtonDown(MouseButton.Left))
        {
            leftMouseButtonPressed = false;
        }

        // Update and remove laser bolts
        for (var i = ShootObjects.Count - 1; i >= 0; i--)
        {
            var bullet = ShootObjects[i];
            var bulletDirection = Vector3.Normalize(gameCam.Forward);

            bullet.transform.Position += bulletDirection * bulletSpeed * (float)args.Time * 10;

            var collided = false;
            GameObject collidedCube = null;

            foreach (var cube in UnLitObjects)
                if (CheckCollision(bullet, cube))
                {
                    collided = true;
                    collidedCube = cube;
                    break;
                }

            if (collided)
            {
                // Handle collision
                Console.WriteLine("Hit");


                // Remove collided objects and perform cleanup
                ShootObjects.RemoveAt(i);
                UnLitObjects.Remove(collidedCube);
                transparentUnlitObjects[1].transform.Position = bullet.transform.Position;

                var directionToCamera = Vector3.Normalize(gameCam.Position - bullet.transform.Position);

                // Calculate the rotation needed to look at the camera
                var lookRotation = Vector3.Normalize(new Vector3(directionToCamera.X, 0, directionToCamera.Z));

                // Calculate the angle between the direction and the forward vector
                var angle = (float)Math.Atan2(lookRotation.Z, lookRotation.X);

                // Set the rotation of the object to look at the camera
                transparentUnlitObjects[1].transform.Rotation = new Vector3(0, -angle + MathHelper.PiOver2, 0);
                score++;
                continue; // Move to the next bullet
            }

            bullet.Lifetime += (float)args.Time;

            if (bullet.Lifetime >= bulletLifetime)
            {
                transparentUnlitObjects[1].transform.Position = bullet.transform.Position;
                // Remove expired bullets and perform cleanup
                ShootObjects.RemoveAt(i);
            }
        }


        if (UnLitObjects.Count <= 0) SpawnRandomCubes(3);


        const float cameraSpeed = 10f;
        const float sensitivity = 0.2f;


        if (KeyboardState.IsKeyDown(Keys.W))
            gameCam.Position += gameCam.Forward * cameraSpeed * (float)args.Time; // Forward

        if (KeyboardState.IsKeyDown(Keys.S))
            gameCam.Position -= gameCam.Forward * cameraSpeed * (float)args.Time; // Backwards

        if (KeyboardState.IsKeyDown(Keys.A)) gameCam.Position -= gameCam.Right * cameraSpeed * (float)args.Time; // Left

        if (KeyboardState.IsKeyDown(Keys.D))
            gameCam.Position += gameCam.Right * cameraSpeed * (float)args.Time; // Right


        // Get the mouse state

        // Calculate the offset of the mouse position
        var deltaX = MouseState.X - previousMousePos.X;
        var deltaY = MouseState.Y - previousMousePos.Y;
        previousMousePos = new Vector2(MouseState.X, MouseState.Y);

        // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
        gameCam.Yaw += deltaX * sensitivity;
        gameCam.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top

        if (timer >= gameTime)
        {
            EndGame();
            // Pause any further game updates
            timer = 0;

        }
    }


    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        gameCam.Fov -= e.OffsetY;
    }


    private void SpawnRandomCubes(int count)
    {
        var rand = new Random();

        for (var i = 0; i < count; i++)
        {
            var x = (float)(rand.NextDouble() * 10 - 5); // Example range: -5 to 5
            var y = (float)(rand.NextDouble() * 5); // Adjust the range as needed
            var z = (float)(rand.NextDouble() * 10 - 5);

            var cube = new GameObject(StaticUtilities.BoxVertices, StaticUtilities.BoxIndices,
                new Shader("shader.vert", "shader.frag"));
            cube.transform.Position = new Vector3(x, y, z);

          
            var target = new Texture("Target.jpg");
            target.Use(TextureUnit.Texture4);

            cube.MyShader.Use();
            GL.Uniform1(cube.MyShader.GetUniformLocation("tex0"), 4);

            UnLitObjects.Add(cube); // Assuming UnLitObjects is a list for unlit cubes
        }
    }

    private bool CheckCollision(GameObject laserBolt, GameObject otherObject)
    {
        // Assuming the objects are axis-aligned bounding boxes (AABB)
        return laserBolt.transform.Position.X < otherObject.transform.Position.X + otherObject.transform.Scale.X &&
               laserBolt.transform.Position.X + laserBolt.transform.Scale.X > otherObject.transform.Position.X &&
               laserBolt.transform.Position.Y < otherObject.transform.Position.Y + otherObject.transform.Scale.Y &&
               laserBolt.transform.Position.Y + laserBolt.transform.Scale.Y > otherObject.transform.Position.Y &&
               laserBolt.transform.Position.Z < otherObject.transform.Position.Z + otherObject.transform.Scale.Z &&
               laserBolt.transform.Position.Z + laserBolt.transform.Scale.Z > otherObject.transform.Position.Z;
    }


    private void EndGame()
    {
        Console.WriteLine("Game Over!!");
        Console.WriteLine("Your score is " + score);
        Console.WriteLine("1. Restart");
        Console.WriteLine("2. Exit");
        Console.Write("\nEnter your choice: ");


       

        switch (Console.ReadLine())
        {
            case "1":
                Console.WriteLine("You've selected Option 1!");
                return ; 
            case "2":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.Write("\nEnter your choice: ");
                Console.ReadLine();
                return;
        }
    }

}