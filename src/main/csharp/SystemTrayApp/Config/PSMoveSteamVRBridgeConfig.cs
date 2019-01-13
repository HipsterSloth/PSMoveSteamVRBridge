using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class PSMoveSteamVRBridgeConfig : ConfigBase
    {
        public PSMoveSteamVRBridgeConfig() : base("PSMoveSteamVRBridgeConfig")
        {
            world_from_driver_quat = new OpenGL.Quaternion(OpenGL.Quaternion.Identity);
            world_from_driver_pos = new OpenGL.Vertex3f();
        }

        public static PSMoveSteamVRBridgeConfig Instance
        {
            get { return ConfigManager.Instance.PSMSteamVRBridgeConfig; }
        }

        private string filter_virtual_hmd_serial = "";
        public string FilterVirtualHmdSerial
        {
            get { return filter_virtual_hmd_serial; }
            set { filter_virtual_hmd_serial = value; IsDirty = true; }
        }

        private string server_address = "localhost";
        public string ServerAddress
        {
            get { return server_address; }
            set { server_address = value; IsDirty = true; }
        }

        private string server_port = "9512";
        public string ServerPort
        {
            get { return server_port; }
            set { server_port = value; IsDirty = true; }
        }

        private bool use_installation_path = false;
        public bool UseInstallationPath
        {
            get { return use_installation_path; }
            set { use_installation_path = value; IsDirty = true; }
        }

        private bool auto_launch_psmove_service = false;
        public bool AutoLaunchPSMoveService
        {
            get { return auto_launch_psmove_service; }
            set { auto_launch_psmove_service = value; IsDirty = true; }
        }

        private bool has_calibrated_world_from_driver_pose = false;
        public bool HasCalibratedWorldFromDriverPose
        {
            get { return has_calibrated_world_from_driver_pose; }
            set { has_calibrated_world_from_driver_pose = value; IsDirty = true; }
        }

        private OpenGL.Quaternion world_from_driver_quat;
        public OpenGL.Quaternion WorldFromDriverQuat
        {
            get { return world_from_driver_quat; }
            set { world_from_driver_quat = value; IsDirty = true; }
        }

        private OpenGL.Vertex3f world_from_driver_pos;
        public OpenGL.Vertex3f WorldFromDriverPos
        {
            get { return world_from_driver_pos; }
            set { world_from_driver_pos = value; IsDirty = true; }
        }

        public override void WriteToJSON(JsonValue pt)
        {
            pt["filter_virtual_hmd_serial"] = filter_virtual_hmd_serial;
            pt["server_address"] = server_address;
            pt["server_port"] = server_port;
            pt["auto_launch_psmove_service"] = auto_launch_psmove_service;
            pt["has_calibrated_world_from_driver_pose"] = has_calibrated_world_from_driver_pose;
            pt["use_installation_path"] = use_installation_path;
            pt["world_from_driver_pose.orientation.w"] = world_from_driver_quat.W;
            pt["world_from_driver_pose.orientation.x"] = world_from_driver_quat.X;
            pt["world_from_driver_pose.orientation.y"] = world_from_driver_quat.Y;
            pt["world_from_driver_pose.orientation.z"] = world_from_driver_quat.Z;
            pt["world_from_driver_pose.position.x"] = world_from_driver_pos.x;
            pt["world_from_driver_pose.position.y"] = world_from_driver_pos.y;
            pt["world_from_driver_pose.position.z"] = world_from_driver_pos.z;
        }

        public override bool ReadFromJSON(JsonValue pt)
        {
            if (base.ReadFromJSON(pt)) {
                if (pt.ContainsKey("filter_virtual_hmd_serial")) {
                    filter_virtual_hmd_serial = pt["filter_virtual_hmd_serial"];
                }
                if (pt.ContainsKey("server_address")) {
                    server_address = pt["server_address"];
                }
                if (pt.ContainsKey("server_port")) {
                    server_port = pt["server_port"];
                }
                if (pt.ContainsKey("auto_launch_psmove_service")) {
                    auto_launch_psmove_service = pt["auto_launch_psmove_service"];
                }
                if (pt.ContainsKey("use_installation_path")) {
                    use_installation_path = pt["use_installation_path"];
                }
                if (pt.ContainsKey("has_calibrated_world_from_driver_pose")) {
                    has_calibrated_world_from_driver_pose = pt["has_calibrated_world_from_driver_pose"];
                }
                if (pt.ContainsKey("world_from_driver_pose.orientation.w") &&
                    pt.ContainsKey("world_from_driver_pose.orientation.x") &&
                    pt.ContainsKey("world_from_driver_pose.orientation.y") &&
                    pt.ContainsKey("world_from_driver_pose.orientation.z") &&
                    pt.ContainsKey("world_from_driver_pose.position.x") &&
                    pt.ContainsKey("world_from_driver_pose.position.y") &&
                    pt.ContainsKey("world_from_driver_pose.position.z")) {
                    world_from_driver_quat.W = pt["world_from_driver_pose.orientation.w"];
                    world_from_driver_quat.X = pt["world_from_driver_pose.orientation.x"];
                    world_from_driver_quat.Y = pt["world_from_driver_pose.orientation.y"];
                    world_from_driver_quat.Z = pt["world_from_driver_pose.orientation.z"];
                    world_from_driver_pos.x = pt["world_from_driver_pose.position.x"];
                    world_from_driver_pos.y = pt["world_from_driver_pose.position.y"];
                    world_from_driver_pos.z = pt["world_from_driver_pose.position.z"];
                }

                return true;
            }

            return false;
        }
    }
}
