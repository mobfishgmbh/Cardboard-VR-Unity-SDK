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

    public struct CardboardMesh
    {
        public int[] indices;
        public int n_indices;
        //one dim array, 2 floats per vertex: x, y.
        public float[] vertices;
        public int n_vertices;
        //one dim array, 2 floats per uv: u, v.
        public float[] uvs;
    }
}