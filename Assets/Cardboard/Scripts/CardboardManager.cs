using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardManager
    {
        public static RenderTexture viewTextureLeft { get; private set; }
        public static RenderTexture viewTextureRight { get; private set; }
        public static Mesh viewMeshLeft { get; private set; }
        public static Mesh viewMeshRight { get; private set; }

        public static void SetRenderTexture(RenderTexture newLeft, RenderTexture newRight)
        {
            viewTextureLeft = newLeft;
            viewTextureRight = newRight;
        }

        public static void SetEyeMesh(Mesh newLeft, Mesh newRight)
        {
            viewMeshLeft = newLeft;
            viewMeshRight = newRight;
        }
    }
}