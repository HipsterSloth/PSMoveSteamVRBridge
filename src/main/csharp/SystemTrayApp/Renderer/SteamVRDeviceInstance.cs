using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using Valve.VR;

namespace SystemTrayApp
{
    public class SteamVRDeviceInstance
    {
        private static GlProgramCode _shaderCode =
            new GlProgramCode(
                "render model",
                // vertex shader
                @"#version 410
                uniform mat4 matrix;
                layout(location = 0) in vec4 position;
                layout(location = 1) in vec3 v3NormalIn;
                layout(location = 2) in vec2 v2TexCoordsIn;
                out vec2 v2TexCoord;
                void main()
                {
                	v2TexCoord = v2TexCoordsIn;
                	gl_Position = matrix * vec4(position.xyz, 1);
                }",
                //fragment shader
                @"#version 410 core
                uniform sampler2D diffuse;
                uniform vec4 modelColor;
                in vec2 v2TexCoord;
                out vec4 outputColor;
                void main()
                {
                   outputColor = texture( diffuse, v2TexCoord) * modelColor;
                }");
        
        private struct ChildComponent
        {
            public string componentName;
            public string renderModelName;
            public GlModelInstance glComponentInstance;
            public VRControllerState_t controllerState;
            public RenderModel_ComponentState_t componentState;
            public RenderModel_ControllerMode_State_t componentModeState;

            public ChildComponent(
                string _componentName,
                string _renderModelName,
                GlModelInstance _glComponentInstance)
            {
                componentName = _componentName;
                renderModelName = _renderModelName;
                glComponentInstance = _glComponentInstance;
                controllerState = new VRControllerState_t();
                componentState = new RenderModel_ComponentState_t();
                componentModeState = new RenderModel_ControllerMode_State_t();
            }
        }
        private List<ChildComponent> _componentStateList;

        private ModelMatrix _modelMatrix;
        public ModelMatrix ModelMatrix
        {
            get { return _modelMatrix; }
            set { _modelMatrix = value; }
        }

        private string _renderModelName;

        public SteamVRDeviceInstance()
        {
            _renderModelName = "";
            _modelMatrix = new ModelMatrix();
            _componentStateList = new List<ChildComponent>();
        }

        public void AddToScene(GlScene glScene, SteamVRTrackedDevice device)
        {
            if (device.RenderModel == null)
                return;

            _renderModelName = device.RenderModel.RenderModelName;

            foreach (SteamVRRenderModelComponent steamVRComponent in device.RenderModel.Components)
            {
                string instanceName = string.Format("SteamVRDevice_{0}_{1}", device.DeviceID, steamVRComponent.ComponentName);
                GlModelInstance glInstance = glScene.ResourceManager.AllocateGlModel(instanceName, steamVRComponent, _shaderCode);

                glInstance.ModelMatrix.Set(device.Transform);
                glScene.AddInstance(glInstance);

                _componentStateList.Add(
                    new ChildComponent(
                        steamVRComponent.ComponentName,
                        steamVRComponent.RenderModelName, 
                        glInstance));
            }
        }

        public void RemoveFromScene(GlScene glScene)
        {
            foreach(ChildComponent state in _componentStateList)
            {
                glScene.RemoveInstance(state.glComponentInstance);
            }
            _componentStateList.Clear();
        }

        public void PollComponentState()
        {
            for (int componentStateIndex = 0; componentStateIndex < _componentStateList.Count; ++componentStateIndex)
            {
                ChildComponent component = _componentStateList[componentStateIndex];

                if (component.componentName.Length > 0 &&
                    OpenVR.RenderModels.GetComponentState(
                            _renderModelName,
                            component.componentName,
                            ref component.controllerState,
                            ref component.componentModeState,
                            ref component.componentState))
                {
                    ModelMatrix componentMat = 
                        SteamVRUtility.ConvertToGlModelMatrix(component.componentState.mTrackingToComponentRenderModel);

                    component.glComponentInstance.ModelMatrix.Set(_modelMatrix * componentMat);
                    component.glComponentInstance.Visible = 
                        (component.componentState.uProperties & (uint)EVRComponentProperty.IsVisible) != 0;
                }
                else
                {
                    component.glComponentInstance.ModelMatrix.Set(_modelMatrix);
                }
            }
        }

        public void SetDiffuseColor(ColorRGBA diffuseColor)
        {
            foreach (ChildComponent childComponent in _componentStateList)
            {
                if (childComponent.glComponentInstance != null)
                {
                    childComponent.glComponentInstance.MaterialInstance.DiffuseColor = diffuseColor;
                }
            }
        }
    }
}
