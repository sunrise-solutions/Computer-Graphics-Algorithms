namespace Models
{
    // List of texture coordinates, in (u, [v ,w]) coordinates, these will vary between 0 and 1, v and w are optional and default to 0.
    // vt 0.500 1 [0]
    public class VertexTexture
    {
        public float U { get; set; }

        public float V { get; set; }

        public float W { get; set; }

        public VertexTexture()
        {
        }

        public VertexTexture(float u, float v, float w)
        {
            U = u;
            V = v;
            W = w;
        }

        public object Clone()
        {
            return new VertexTexture(this.U, this.V, this.W);
        }
    }
}
