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

    //https://github.com/googlevr/cardboard/blob/master/sdk/distortion_mesh.cc
    [StructLayout(LayoutKind.Sequential)]
    public struct CardboardMesh
    {
        [MarshalAs(UnmanagedType.LPArray,
            SizeConst = (2 * CardboardUtility.kResolution * (CardboardUtility.kResolution - 1) + CardboardUtility.kResolution - 2),
            ArraySubType = UnmanagedType.I4)]
        public int[] indices;
        public int n_indices;
        //one dim array, 2 floats per vertex: x, y.
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2 * CardboardUtility.kResolution * CardboardUtility.kResolution, ArraySubType = UnmanagedType.R4)]
        public float[] vertices;
        //one dim array, 2 floats per uv: u, v.
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2 * CardboardUtility.kResolution * CardboardUtility.kResolution, ArraySubType = UnmanagedType.R4)]
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