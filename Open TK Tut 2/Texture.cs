using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Open_TK_Tut_1;

public class Texture
{
    public readonly int Handle; // Binding ID into program

    public Texture(string filePath)
    {
        //Generate our handle ID on a blank texture
        Handle = GL.GenTexture();

        Use();

        using (Stream stream = File.OpenRead(StaticUtilities.TextureDirectory + filePath))
        {
            ImageResult img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);
        }
        
        //Filtering Neighbor by Neighbour (Pixel) / point filter || Linear filtering
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        
        //Wrapping mode
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat); // X
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat); // Y
        
        
        //Mip Mapping
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

    }

    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    

}