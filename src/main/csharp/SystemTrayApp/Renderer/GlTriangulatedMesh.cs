using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using OpenGL;

namespace SystemTrayApp
{
    public class GlTriangulatedMesh : IDisposable
    {
        private bool disposed = false;

        private string _name;

        private GlVertexDefinition _vertexDefinition;

        private IntPtr _vertexData;
        private uint _vertexCount;
        private IntPtr _indexData;
        private uint _triangleCount;

        private DeviceContext _glContext = null;
        private uint _glVertArray = 0;
        private uint _glVertBuffer = 0;
        private uint _glIndexBuffer = 0;

        public GlTriangulatedMesh(
            string name,
            GlVertexDefinition vertexDefintion,
            IntPtr vertexData, 
            uint vertexCount,
            IntPtr indexData,
            uint triangleCount)
        {
            _name = name;
            _vertexDefinition = vertexDefintion;
            _vertexData= vertexData;
            _vertexCount= vertexCount;
            _indexData= indexData;
            _triangleCount = triangleCount;
        }

        ~GlTriangulatedMesh()
        {
            Dispose(false);
        }

        public void NotifyGLContextUpdated(DeviceContext GlContext)
        {
            if (_glContext == null) {
                CreateGLResources(GlContext);
            }
        }

        public void NotifyGLContextDisposed(DeviceContext GlContext)
        {
            if (GlContext == _glContext) {
                DisposeGLResources();
            }
        }

        public void DrawElements(DeviceContext GlContext)
        {
            if (GlContext == _glContext) {
                Gl.BindVertexArray(_glVertArray);
                Gl.DrawElements(PrimitiveType.Triangles, (int)_triangleCount*3, DrawElementsType.UnsignedShort, IntPtr.Zero);
                Gl.BindVertexArray(0);
            }
        }

        private void CreateGLResources(DeviceContext GlContext)
        {
            _glContext = GlContext;

            if (_vertexData != IntPtr.Zero && _vertexCount > 0 &&
                _indexData != IntPtr.Zero && _triangleCount > 0) {
                int vertexSize = _vertexDefinition.VertexSize;

                // create and bind a Vertex Array Object(VAO) to hold state for this model
                _glVertArray = Gl.GenVertexArray();
                Gl.BindVertexArray(_glVertArray);

                // Populate a vertex buffer
                _glVertBuffer = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ArrayBuffer, _glVertBuffer);
                Gl.BufferData(BufferTarget.ArrayBuffer, (uint)vertexSize * _vertexCount, _vertexData, BufferUsage.StaticDraw);

                // Identify the components in the vertex buffer
                for (uint attribIndex= 0; attribIndex < _vertexDefinition.Attributes.Length; ++attribIndex)
                {
                    GlVertexAttribute attrib = _vertexDefinition.Attributes[attribIndex];

                    Gl.EnableVertexAttribArray(attribIndex);
                    Gl.VertexAttribPointer(attrib.index, attrib.size, attrib.type, attrib.normalized, attrib.stride, attrib.offset);
                }

                // Create and populate the index buffer
                _glIndexBuffer = Gl.GenBuffer();
                Gl.BindBuffer(BufferTarget.ElementArrayBuffer, _glIndexBuffer);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)Marshal.SizeOf(typeof(UInt16)) * _triangleCount * 3, _indexData, BufferUsage.StaticDraw);

                Gl.BindVertexArray(0);
            }
        }

        private void DisposeGLResources()
        {
            if (_glIndexBuffer != 0)
                Gl.DeleteBuffers(_glIndexBuffer);

            if (_glVertArray != 0)
                Gl.DeleteVertexArrays(_glVertArray);

            if (_glVertBuffer != 0)
                Gl.DeleteBuffers(_glVertBuffer);

            _glIndexBuffer = 0;
            _glVertArray = 0;
            _glVertBuffer = 0;
            _vertexCount = 0;

            _glContext = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed) {
                if (disposing) {
                    DisposeGLResources();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
