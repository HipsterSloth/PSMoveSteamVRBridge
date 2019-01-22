using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;


namespace SystemTrayApp
{
    public struct GlVertexAttribute
    {
        public uint index;
        public int size;
        public VertexAttribType type;
        public bool normalized;
        public int stride;
        public IntPtr offset;

        public GlVertexAttribute(uint _index, int _size, VertexAttribType _type, bool _normalized, int _stride, IntPtr _offset)
        {
            index = _index;
            size = _size;
            type = _type;
            normalized = _normalized;
            stride = _stride;
            offset = _offset;
        }
    }

    public class GlVertexDefinition
    {
        private GlVertexAttribute[] _attributes = null;
        public GlVertexAttribute[] Attributes
        {
            get { return _attributes; }
        }

        private int _vertexSize = 0;
        public int VertexSize
        {
            get { return _vertexSize; }
        }

        public GlVertexDefinition(List<GlVertexAttribute> attribtes, int vertexSize)
        {
            _attributes = attribtes.ToArray();
            _vertexSize = vertexSize;
        }
    }
}
