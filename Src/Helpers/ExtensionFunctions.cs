using System.Runtime.CompilerServices;
using Godot;

namespace SomeGame.Helpers
{
    public static class ExtensionFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color AverageColorFromTexture(Texture2D texture)
        {
            var image = texture.GetImage();
            var textureData = image.GetData();

            float r = 0;
            float g = 0;
            float b = 0;
            float colorCount = 0;

            for (var i = 0; i < textureData.Length - 1; i += 3)
            {
                r += textureData[i];
                g += textureData[i + 1];
                b += textureData[i + 2];

                colorCount += 1;
            }

            return new Color(r / colorCount / 255, g / colorCount / 255, b / colorCount / 255);
        }
    }
}