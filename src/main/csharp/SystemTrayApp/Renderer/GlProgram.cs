using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace SystemTrayApp
{
    public struct GlProgramCode
    {
        public string shaderName;
        public string vertexShaderCode;
        public string fragmentShaderCode;

        public GlProgramCode(string _shaderName, string _vertexShaderCode, string _fragmentShaderCode)
        {
            shaderName = _shaderName;
            vertexShaderCode = _vertexShaderCode;
            fragmentShaderCode = _fragmentShaderCode;
        }
    }

    public class GlProgram : IDisposable
    {
        private bool disposed = false;

        private DeviceContext _glContext = null;

        public string ShaderName
        {
            get { return _code.shaderName; }
        }

        private GlProgramCode _code;
        private uint _programID = 0;
        private int _modelViewProjectionMatrixId = -1;
        private int _modelColorId = -1;

        public GlProgram(GlProgramCode code)
        {
            _code = code;
        }

        ~GlProgram()
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

        public bool UseProgram(DeviceContext GlContext)
        {
            if (_programID != 0 && GlContext == _glContext)
            {
                Gl.UseProgram(_programID);

                return true;
            }

            return false;
        }

        public bool SetModelViewProjectionMatrix(Matrix4x4 ModelViewProjection)
        {
            if (_modelViewProjectionMatrixId != -1) {
                Gl.UniformMatrix4(_modelViewProjectionMatrixId, false, ModelViewProjection.ToArray());
                return true;
            }
            return false;
        }

        public bool SetModelColor(ColorRGBA rgba)
        {
            if (_modelColorId != -1)
            {
                Vertex4f colorUniform = new Vertex4f(rgba.Red, rgba.Green, rgba.Blue, rgba.Alpha);
                Gl.Uniform4f(_modelColorId, 1, ref colorUniform);
                return true;
            }
            return false;
        }

        public void ClearProgram(DeviceContext GlContext)
        {
            if (_programID != 0 && GlContext == _glContext)
            {
                Gl.UseProgram(0);
            }
        }

        private bool CreateGLResources(DeviceContext GlContext)
        {
            _glContext = GlContext;

            if (_code.vertexShaderCode.Length > 0 && _code.fragmentShaderCode.Length > 0) {
                _programID = Gl.CreateProgram();

                uint nSceneVertexShader = Gl.CreateShader(ShaderType.VertexShader);
                Gl.ShaderSource(nSceneVertexShader, new string[] { _code.vertexShaderCode }, null);
                Gl.CompileShader(nSceneVertexShader);

                int vShaderCompiled = 0;
                Gl.GetShader(nSceneVertexShader, ShaderParameterName.CompileStatus, out vShaderCompiled);
                if (vShaderCompiled != 1) 
                {
                    Trace.TraceError(string.Format("{0} - Unable to compile vertex shader {1}!", _code.shaderName, nSceneVertexShader));
                    Gl.DeleteProgram(_programID);
                    Gl.DeleteShader(nSceneVertexShader);
                    _programID = 0;
                    return false;
                }
                Gl.AttachShader(_programID, nSceneVertexShader);
                Gl.DeleteShader(nSceneVertexShader); // the program hangs onto this once it's attached

                uint nSceneFragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
                Gl.ShaderSource(nSceneFragmentShader, new string[] { _code.fragmentShaderCode }, null);
                Gl.CompileShader(nSceneFragmentShader);

                int fShaderCompiled = 0;
                Gl.GetShader(nSceneFragmentShader, ShaderParameterName.CompileStatus, out fShaderCompiled);
                if (fShaderCompiled != 1)
                {
                    Trace.TraceError(string.Format("{0} - Unable to compile fragment shader {1}!", _code.shaderName, nSceneFragmentShader));
                    Gl.DeleteProgram(_programID);
                    Gl.DeleteShader(nSceneFragmentShader);
                    _programID = 0;
                    return false;
                }
                Gl.AttachShader(_programID, nSceneFragmentShader);
                Gl.DeleteShader(nSceneFragmentShader); // the program hangs onto this once it's attached

                Gl.LinkProgram(_programID);

                int programSuccess = 1;
                Gl.GetProgram(_programID, ProgramProperty.LinkStatus, out programSuccess);
                if (programSuccess != 1) 
                {
                    Trace.TraceError(string.Format("{0} - Error linking program {1}!", _code.shaderName, _programID));
                    Gl.DeleteProgram(_programID);
                    _programID = 0;
                    return false;
                }

                Gl.UseProgram(_programID);
                Gl.UseProgram(0);

                _modelViewProjectionMatrixId = Gl.GetUniformLocation(_programID, "matrix");
                if (_modelViewProjectionMatrixId == -1) {
                    Trace.TraceWarning(string.Format("{0} - Unable to find matrix uniform!", _code.shaderName));
                }

                _modelColorId = Gl.GetUniformLocation(_programID, "modelColor");
                if (_modelColorId == -1)
                {
                    Trace.TraceWarning(string.Format("{0} - Unable to find modelColor uniform!", _code.shaderName));
                }

                return true;
            }

            return false;
        }

        private void DisposeGLResources()
        {
            if (_programID != 0) {
                Gl.DeleteProgram(_programID);
                _programID = 0;
            }

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
