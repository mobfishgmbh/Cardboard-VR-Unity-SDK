using System;
using System.Runtime.InteropServices;

namespace MobfishCardboard
{
    public enum CardboardEye
    {
        kLeft = 0,
        kRight = 1
    }

    public struct CardboardUv
    {
        public float u;
        public float v;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CardboardMesh
    {
        public int[] indices;
        public int n_indices;
        //one dim array, 2 floats per vertex: x, y.
        public float[] vertices;
        //one dim array, 2 floats per uv: u, v.
        public float[] uvs;
        public int n_vertices;
    }

    public struct CardboardEyeTextureDescription
    {
        public IntPtr texture;
        public int layer;
        public float left_u;
        public float right_u;
        public float top_v;
        public float bottom_v;
        public float[] eye_from_head;

    }
}