using System;
using System.Collections.Generic;
using System.Text;

using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Gui;
using Sandbox.Game.GameSystems.Electricity;
using Sandbox.Game.World;
using VRage.Components;
using VRage.ObjectBuilders;
using VRageMath;
using Sandbox.ModAPI;

namespace ExtraProgrammingInfo
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ExtraProgrammingInfo : MySessionComponentBase
    {
        static ExtraProgrammingInfo()
        {
            // Backwards compatibility
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.LinearVelocity.Absolute", Vector3.Zero, block => block.CubeGrid.Physics.LinearVelocity);
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.LinearVelocity.Relative", Vector3.Zero, block => GetRelativeDirection(block.CubeGrid.Physics.LinearVelocity, block));
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.LinearAcceleration.Absolute", Vector3.Zero, block => block.CubeGrid.Physics.LinearAcceleration);
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.LinearAcceleration.Relative", Vector3.Zero, block => GetRelativeDirection(block.CubeGrid.Physics.LinearAcceleration, block));
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.AngularVelocity", Vector3.Zero, block => block.CubeGrid.Physics.AngularVelocity);
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.AngularAcceleration", Vector3.Zero, block => block.CubeGrid.Physics.AngularAcceleration);
            RegisterForTerminalBlock<float, MyShipController>("XShipInfo.Mass", 0, block => block.CubeGrid.Physics.Mass);
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.CenterOfMass.Absolute", Vector3.Zero, block => block.CubeGrid.Physics.CenterOfMassWorld);
            RegisterForTerminalBlock<Vector3, MyShipController>("XShipInfo.CenterOfMass.Relative", Vector3.Zero, block => GetRelativePosition(block.CubeGrid.Physics.CenterOfMassWorld, block));

            // Physics info (available on any terminal block)
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.LinearVelocity.Absolute", Vector3D.Zero, block => block.CubeGrid.Physics.LinearVelocity);
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.LinearVelocity.Relative", Vector3D.Zero, block => GetRelativeDirection(block.CubeGrid.Physics.LinearVelocity, (MyCubeBlock) block));
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.LinearAcceleration.Absolute", Vector3D.Zero, block => block.CubeGrid.Physics.LinearAcceleration);
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.LinearAcceleration.Relative", Vector3D.Zero, block => GetRelativeDirection(block.CubeGrid.Physics.LinearAcceleration, (MyCubeBlock) block));
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.AngularVelocity", Vector3D.Zero, block => block.CubeGrid.Physics.AngularVelocity);
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.AngularAcceleration", Vector3D.Zero, block => block.CubeGrid.Physics.AngularAcceleration);
            RegisterForAllTerminalBlocks<double>("XGridInfo.Mass", 0, block => block.CubeGrid.Physics.Mass);
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.CenterOfMass.Absolute", Vector3D.Zero, block => block.CubeGrid.Physics.CenterOfMassWorld);
            RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.CenterOfMass.Relative", Vector3D.Zero, block => GetRelativePosition(block.CubeGrid.Physics.CenterOfMassWorld, (MyCubeBlock) block));

            // Information for ship controller
            // Access denied by scripted mod sandbox
            //RegisterForTerminalBlock<Vector3D, MyShipController>("XShipInfo.Thrust", Vector3D.Zero, block => block.EntityThrustComponent.FinalThrust);
            //RegisterForTerminalBlock<Vector3D, MyShipController>("XShipInfo.Torque", Vector3D.Zero, block => block.GridGyroSystem.Torque);

            // Access denied by scripted mod sandbox
            //RegisterForAllTerminalBlocks<Vector3D>("XGridInfo.Inertia", Vector3D.Zero, block => GetInertia((MyCubeBlock) block));

            // Thruster information
            RegisterForTerminalBlock<double, MyThrust>("XThrusterInfo.Strength", 0, thruster => thruster.CurrentStrength);
            RegisterForTerminalBlock<Vector3D, MyThrust>("XThrusterInfo.Force", Vector3D.Zero, thruster => thruster.ThrustForce);
            RegisterForTerminalBlock<double, MyThrust>("XThrusterInfo.MinPower", 0, thruster => thruster.MinPowerConsumption);
            RegisterForTerminalBlock<double, MyThrust>("XThrusterInfo.MaxPower", 0, thruster => thruster.MaxPowerConsumption);

            // Wheel information
            //RegisterForTerminalBlock<double, MyMotorSuspension>("XWheelInfo.SteerAngle", 0, wheel => wheel.SteerAngle);
            //RegisterForTerminalBlock<bool, MyMotorSuspension>("XWheelInfo.Brake", false, wheel => wheel.Brake);

            // Game information

            // Access denied by scripted mod sandbox
            //RegisterForTerminalBlock<Vector3D, MyShipController>("XGameInfo.SunDirection.Absolute", Vector3D.Zero, shipController => MySector.DirectionToSunNormalized);
            //RegisterForTerminalBlock<Vector3D, MyShipController>("XGameInfo.SunDirection.Relative", Vector3D.Zero, shipController => GetRelativeDirection(MySector.DirectionToSunNormalized, shipController);
            //RegisterForTerminalBlock<TimeSpan, MyShipController>("XGameInfo.ElapsedGameTime", TimeSpan.Zero, shipController => MySession.Static.ElapsedGameTime);
        }

        private static void RegisterForAllTerminalBlocks<TValue>(string name, TValue defaultValue, Func<IMyTerminalBlock, TValue> getter)
        {
            RegisterForTerminalBlock<TValue, MyShipController>(name, defaultValue, getter);
            RegisterForTerminalBlock<TValue, MyThrust>(name, defaultValue, getter);
            RegisterForTerminalBlock<TValue, MyGyro>(name, defaultValue, getter);
            //RegisterForTerminalBlock<TValue, MyMotorSuspension>(name, defaultValue, getter);
            //RegisterForTerminalBlock<TValue, MyMotorBase>(name, defaultValue, getter);
        }

        private static void RegisterForTerminalBlock<TValue, TBlock>(string name, TValue defaultValue, Func<TBlock, TValue> getter) where TBlock:MyTerminalBlock
        {
            var property = new ProgrammingInfoControl<TBlock, TValue>(
                name,
                defaultValue,
                getter);
            MyTerminalControlFactory.AddControl(property);
        }

        private static Vector3D GetRelativeDirection(Vector3D absolute, MyCubeBlock block)
        {
            var transformed = Vector4.Transform(
                new Vector4D(absolute.X, absolute.Y, absolute.Z, 0),
                MatrixD.Transpose(block.CubeGrid.WorldMatrix));
            return new Vector3D(transformed.X, transformed.Y, transformed.Z);
        }

        private static Vector3D GetRelativePosition(Vector3D absolute, MyCubeBlock block)
        {
            return Vector3D.Transform(absolute, MatrixD.Invert(block.CubeGrid.WorldMatrix));
        }

        /*private static Vector3D GetInertia(MyCubeBlock block)
        {
            var invTensor = block.CubeGrid.Physics.RigidBody.InverseInertiaTensor;
            return new Vector3D(1 / invTensor.M11, 1 / invTensor.M22, 1 / invTensor.M33);
        }*/
    }
}
