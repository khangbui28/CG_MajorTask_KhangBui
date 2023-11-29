using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Assimp;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Open_TK_Tut_1;

public static class StaticUtilities
{
    public static readonly string MainDirectory = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent + "\\";
    public static readonly string TextureDirectory = MainDirectory + "Textures\\";
    public static readonly string ShaderDirectory = MainDirectory + "Shaders\\";
    public static readonly string ObjectDirectory = MainDirectory + "Objects\\";

    //This runs as soon as the project loads.
    static StaticUtilities()
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
    }

   

    public static void CheckError(string stage)
    {
        ErrorCode errorCode = GL.GetError();
        if (errorCode != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error ({stage}): {errorCode}");
        }
    }

    public static float[] MergeMeshData(this Mesh myMesh)
    {
        int n = myMesh.Vertices.Count;
        float[] export = new float[n*8];


        for (int i = 0; i < n; ++i)
        {
            int index = i * 8;

            //Verts

            export[index] = myMesh.Vertices[i].X;
            export[index + 1] = myMesh.Vertices[i].Y;
            export[index + 2] = myMesh.Vertices[i].Z;


            // Normals
            export[index + 3] = myMesh.Normals[i].X;
            export[index + 4] = myMesh.Normals[i].Y;
            export[index + 5] = myMesh.Normals[i].Z;

            //UVs

            export[index + 6] = myMesh.TextureCoordinateChannels[0][i].X;
            export[index + 7] = myMesh.TextureCoordinateChannels[0][i].Y;
            

        }

        return export;

    }

    public static readonly float[] QuadVertices =
    {
        -1.0f,  1.0f, 0.0f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f, // Top Left (X, Y, Z, Nx, Ny, Nz, U, V)
        -1.0f, -1.0f, 0.0f,  0.0f, 1.0f, 0.0f, 0.0f, 0.0f, // Bottom Left (X, Y, Z, Nx, Ny, Nz, U, V)
        1.0f, -1.0f, 0.0f,  0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // Bottom Right (X, Y, Z, Nx, Ny, Nz, U, V)
        1.0f,  1.0f, 0.0f,  0.0f, 1.0f, 0.0f, 1.0f, 1.0f  // Top Right (X, Y, Z, Nx, Ny, Nz, U, V)
    };

    public static readonly uint[] QuadIndices =
    {
        0, 1, 2,
        0, 2, 3
    };

    public static readonly float[] BillboardVertices =
    {
        -0.5f, 0.5f, 0.0f,  // top left
        0.5f, 0.5f, 0.0f,   // top right
        -0.5f, -0.5f, 0.0f, // bottom left
        0.5f, -0.5f, 0.0f   // bottom right
    };

    public static readonly uint[] BillboardIndices =
    {
        0, 1, 2,
        1, 3, 2
    };



    public static readonly float[] BoxVertices =
{
    // Front face
    -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // Bottom-left (Position), Front Normal, UV
    0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, // Bottom-right (Position), Front Normal, UV
    0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, // Top-right (Position), Front Normal, UV
    -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, // Top-left (Position), Front Normal, UV

// Right face
    0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // Bottom-left (Position), Right Normal, UV
    0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // Bottom-right (Position), Right Normal, UV
    0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, // Top-right (Position), Right Normal, UV
    0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // Top-left (Position), Right Normal, UV

// Back face
    -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, // Bottom-left (Position), Back Normal, UV
    0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, // Bottom-right (Position), Back Normal, UV
    0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f, // Top-right (Position), Back Normal, UV
    -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f, // Top-left (Position), Back Normal, UV

// Left face
    -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // Bottom-left (Position), Left Normal, UV
    -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // Bottom-right (Position), Left Normal, UV
    -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f, // Top-right (Position), Left Normal, UV
    -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // Top-left (Position), Left Normal, UV

// Top face
    -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, // Bottom-left (Position), Top Normal, UV
    0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // Bottom-right (Position), Top Normal, UV
    0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, // Top-right (Position), Top Normal, UV
    -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, // Top-left (Position), Top Normal, UV

// Bottom face
    -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, // Bottom-left (Position), Bottom Normal, UV
    0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f, // Bottom-right (Position), Bottom Normal, UV
    0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f, // Top-right (Position), Bottom Normal, UV
    -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f // Top-left (Position), Bottom Normal, UV
};

    public static readonly uint[] BoxIndices =
    {
        0, 1, 2,
        2, 3, 0,

        // Right face
        4, 5, 6,
        6, 7, 4,

        // Back face
        8, 10, 9,
        10, 8, 11,

        // Left face
        12, 13, 14,
        14, 15, 12,

        // Top face
        16, 17, 18,
        18, 19, 16,

        // Bottom face
        20, 21, 22,
        22, 23, 20
    };


    public static readonly float[] PlanVertices =
    {
        // Positions            // Normals         // Texture Coords
        -1.0f, 0.0f, -1.0f,    0.0f, 1.0f, 0.0f,  0.0f, 0.0f,
        -1.0f, 0.0f,  1.0f,    0.0f, 1.0f, 0.0f,  0.0f, 1.0f,
        1.0f, 0.0f,  1.0f,    0.0f, 1.0f, 0.0f,  1.0f, 1.0f,
        1.0f, 0.0f, -1.0f,    0.0f, 1.0f, 0.0f,  1.0f, 0.0f,
    };

    public static readonly uint[] PlanIndices =
    {
        0, 1, 2,
        0, 2, 3
    };

}