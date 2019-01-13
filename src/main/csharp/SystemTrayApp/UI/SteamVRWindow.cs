using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MaterialSkin.Controls;

using OpenGL;
//using OpenGL.Objects;
//using OpenGL.Objects.Scene;
//using OpenGL.Objects.State;

namespace SystemTrayApp
{
    public partial class SteamVRWindow : MaterialForm
    {
        private float _ViewAzimuth;
        private float _ViewElevation;
        private float _ViewLever = 4.0f;
        private float _ViewStrideLat, _ViewStrideAlt;

        private readonly List<Keys> _PressedKeys = new List<Keys>();
        private System.Drawing.Point? _Mouse;

        //private SceneGraph _CubeScene;

        //private ArrayBuffer<Vertex3f> _CubeArrayPosition;
        //private ArrayBuffer<ColorRGBF> _CubeArrayColor;
        //private ArrayBuffer<Vertex3f> _CubeArrayNormal;
        //private VertexArrays _CubeArrays;

        //private GraphicsContext _Context;
        //private SceneObjectLightSpot spotLight;
        //private SceneObjectLightDirectional _GlobalLightObject;

        public SteamVRWindow()
        {
            InitializeComponent();
        }

        //private SceneObjectGeometry CreatePlane()
        //{
        //    SceneObjectGeometry geometry = new SceneObjectGeometry("Plane");

        //    geometry.VertexArray = VertexArrays.CreatePlane(50.0f, 50.0f, 0.0f, 1, 1);
        //    geometry.ObjectState.DefineState(new CullFaceState(FrontFaceDirection.Ccw, CullFaceMode.Back));
        //    geometry.ObjectState.DefineState(new TransformState());

        //    MaterialState cubeMaterialState = new MaterialState();
        //    cubeMaterialState.FrontMaterial = new MaterialState.Material(ColorRGBAF.ColorWhite * 0.5f);
        //    cubeMaterialState.FrontMaterial.Ambient = ColorRGBAF.ColorBlack;
        //    cubeMaterialState.FrontMaterial.Diffuse = ColorRGBAF.ColorWhite * 0.5f;
        //    cubeMaterialState.FrontMaterial.Specular = ColorRGBAF.ColorBlack;
        //    geometry.ObjectState.DefineState(cubeMaterialState);

        //    geometry.LocalModel.RotateX(-90.0);

        //    geometry.ProgramTag = ShadersLibrary.Instance.CreateProgramTag("OpenGL.Standard+PhongFragment");

        //    return (geometry);
        //}

        private const float _CubeSize = 1.0f;

        private Vertex3f[] ArrayPosition
        {
            get
            {
                Vertex3f[] arrayPosition = new Vertex3f[36];
                int i = 0;

                // +Y
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, +_CubeSize);

                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, -_CubeSize);

                // -Y
                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, +_CubeSize);

                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, +_CubeSize);

                // +X
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, +_CubeSize);

                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, +_CubeSize);

                // -X
                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, +_CubeSize);

                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, -_CubeSize);

                // +Z
                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, +_CubeSize);

                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, +_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, +_CubeSize);

                // -Z
                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(-_CubeSize, +_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, -_CubeSize);

                arrayPosition[i++] = new Vertex3f(-_CubeSize, -_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, +_CubeSize, -_CubeSize);
                arrayPosition[i++] = new Vertex3f(+_CubeSize, -_CubeSize, -_CubeSize);

                return (arrayPosition);
            }
        }

        private Vertex3f[] ArrayNormals
        {
            get
            {
                Vertex3f[] arrayNormal = new Vertex3f[36];
                int v = 0;

                for (int i = 0; i < 6; i++)
                    arrayNormal[v++] = -Vertex3f.UnitY;
                for (int i = 0; i < 6; i++)
                    arrayNormal[v++] = Vertex3f.UnitY;
                for (int i = 0; i < 6; i++)
                    arrayNormal[v++] = -Vertex3f.UnitX;
                for (int i = 0; i < 6; i++)
                    arrayNormal[v++] = Vertex3f.UnitX;
                for (int i = 0; i < 6; i++)
                    arrayNormal[v++] = -Vertex3f.UnitZ;
                for (int i = 0; i < 6; i++)
                    arrayNormal[v++] = Vertex3f.UnitZ;

                return (arrayNormal);
            }
        }

        private ColorRGBF[] ArrayColors
        {
            get
            {
                ColorRGBF[] arrayColor = new ColorRGBF[36];
                int v = 0;

                for (int i = 0; i < 12; i++)
                    arrayColor[v++] = ColorRGBF.ColorRed;
                for (int i = 0; i < 12; i++)
                    arrayColor[v++] = ColorRGBF.ColorGreen;
                for (int i = 0; i < 12; i++)
                    arrayColor[v++] = ColorRGBF.ColorBlue;

                return (arrayColor);
            }
        }

        //private SceneObjectGeometry CreateCubeGeometry()
        //{
        //    SceneObjectGeometry cubeGeometry = new SceneObjectGeometry("Cube");

        //    cubeGeometry.ObjectState.DefineState(new CullFaceState(FrontFaceDirection.Ccw, CullFaceMode.Back));
        //    cubeGeometry.ObjectState.DefineState(new TransformState());

        //    MaterialState cubeMaterialState = new MaterialState();
        //    cubeMaterialState.FrontMaterial = new MaterialState.Material(ColorRGBAF.ColorWhite * 0.5f);
        //    cubeMaterialState.FrontMaterial.Ambient = ColorRGBAF.ColorBlack;
        //    cubeMaterialState.FrontMaterial.Diffuse = ColorRGBAF.ColorWhite * 0.5f;
        //    cubeMaterialState.FrontMaterial.Specular = ColorRGBAF.ColorWhite * 0.5f;
        //    cubeMaterialState.FrontMaterial.Shininess = 10.0f;
        //    cubeGeometry.ObjectState.DefineState(cubeMaterialState);

        //    if (_CubeArrayPosition == null) {
        //        _CubeArrayPosition = new ArrayBuffer<Vertex3f>();
        //        _CubeArrayPosition.Create(ArrayPosition);
        //    }

        //    if (_CubeArrayColor == null) {
        //        _CubeArrayColor = new ArrayBuffer<ColorRGBF>();
        //        _CubeArrayColor.Create(ArrayColors);
        //    }

        //    if (_CubeArrayNormal == null) {
        //        _CubeArrayNormal = new ArrayBuffer<Vertex3f>();
        //        _CubeArrayNormal.Create(ArrayNormals);
        //    }

        //    if (_CubeArrays == null) {
        //        _CubeArrays = new VertexArrays();
        //        _CubeArrays.SetArray(_CubeArrayPosition, VertexArraySemantic.Position);
        //        _CubeArrays.SetArray(_CubeArrayColor, VertexArraySemantic.Color);
        //        _CubeArrays.SetArray(_CubeArrayNormal, VertexArraySemantic.Normal);
        //        _CubeArrays.SetElementArray(PrimitiveType.Triangles);
        //    }

        //    cubeGeometry.VertexArray = _CubeArrays;

        //    cubeGeometry.BoundingVolume = new BoundingBox(-Vertex3f.One * _CubeSize, Vertex3f.One * _CubeSize);

        //    return (cubeGeometry);
        //}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void glControl_ContextCreated(object sender, GlControlEventArgs e)
        {
            //// Wrap GL context with GraphicsContext
            //_Context = new GraphicsContext(e.DeviceContext, e.RenderContext);
            
            //// Scene
            //_CubeScene = new SceneGraph(
            //    SceneGraphFlags.CullingViewFrustum | SceneGraphFlags.StateSorting | SceneGraphFlags.Lighting | SceneGraphFlags.ShadowMaps
            //    //| SceneGraphFlags.BoundingVolumes
            //    );
            //_CubeScene.SceneRoot = new SceneObjectGeometry();
            //_CubeScene.SceneRoot.ObjectState.DefineState(new DepthTestState(DepthFunction.Less));

            //_CubeScene.CurrentView = new SceneObjectCamera();
            //_CubeScene.SceneRoot.Link(_CubeScene.CurrentView);

            //// Global lighting
            //SceneObjectLightZone globalLightZone = new SceneObjectLightZone();

            //_GlobalLightObject = new SceneObjectLightDirectional();
            //_GlobalLightObject.Direction = (-Vertex3f.UnitX + Vertex3f.UnitY - Vertex3f.UnitZ).Normalized;
            //globalLightZone.Link(_GlobalLightObject);

            //_CubeScene.SceneRoot.Link(globalLightZone);

            //// Horizontal plane
            //globalLightZone.Link(CreatePlane());

            //SceneObjectGeometry cubeColored = CreateCubeGeometry();
            //cubeColored.ProgramTag = ShadersLibrary.Instance.CreateProgramTag("OpenGL.Standard+Color");
            //cubeColored.LocalModel.Translate(0.0f, 5.0f, 0.0f);
            //globalLightZone.Link(cubeColored);

            //_CubeScene.Create(_Context);

            Gl.ClearColor(0.1f, 0.1f, 0.1f, 0.0f);
            Gl.Enable(EnableCap.Multisample);
        }

        private void glControl_Render(object sender, GlControlEventArgs e)
        {
            GlControl senderControl = (GlControl)sender;
            float senderAspectRatio = (float)senderControl.Width / senderControl.Height;

            // Clear
            Gl.Viewport(0, 0, senderControl.Width, senderControl.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //_CubeScene.CurrentView.ProjectionMatrix = new PerspectiveProjectionMatrix(45.0f, senderAspectRatio, 0.1f, 100.0f);
            //_CubeScene.CurrentView.LocalModel.SetIdentity();
            //_CubeScene.CurrentView.LocalModel.Translate(_ViewStrideLat, _ViewStrideAlt, 0.0f);
            //_CubeScene.CurrentView.LocalModel.RotateY(_ViewAzimuth);
            //_CubeScene.CurrentView.LocalModel.RotateX(_ViewElevation);
            //_CubeScene.CurrentView.LocalModel.Translate(0.0f, 0.0f, _ViewLever);
            //_CubeScene.UpdateViewMatrix();

            //_CubeScene.Draw(_Context);
        }

        private void glControl_ContextUpdate(object sender, GlControlEventArgs e)
        {
            foreach (Keys pressedKey in _PressedKeys) {
                switch (pressedKey) {
                    case Keys.A:
                        _ViewStrideLat -= 0.1f;
                        break;
                    case Keys.D:
                        _ViewStrideLat += 0.1f;
                        break;
                    case Keys.W:
                        _ViewStrideAlt += 0.1f;
                        break;
                    case Keys.S:
                        _ViewStrideAlt -= 0.1f;
                        break;
                }
            }
        }

        private void glControl_ContextDestroying(object sender, GlControlEventArgs e)
        {
            //_CubeScene.Dispose();
            //_Context.Dispose();
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _Mouse = e.Location;
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Mouse.HasValue) {
                System.Drawing.Point delta = _Mouse.Value - (System.Drawing.Size)e.Location;

                _ViewAzimuth += delta.X * 0.5f;
                _ViewElevation += delta.Y * 0.5f;

                _Mouse = e.Location;
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            _Mouse = null;
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            _ViewLever += e.Delta / 60.0f;
            _ViewLever = Math.Max(2.5f, _ViewLever);
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (_PressedKeys.Contains(e.KeyCode) == false)
                _PressedKeys.Add(e.KeyCode);
        }

        private void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            _PressedKeys.Remove(e.KeyCode);
        }
    }
}
