using System;

using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Screens.Terminal.Controls;

namespace ExtraProgrammingInfo
{
    class ProgrammingInfoControl<TBlock, TValue> : MyTerminalValueControl<TBlock, TValue>
        where TBlock:MyTerminalBlock
    {
        private TValue DefaultValue;
        private Func<TBlock, TValue> Getter;

        public ProgrammingInfoControl(string id, TValue defaultValue, Func<TBlock, TValue> getter) : base(id)
        {
            DefaultValue = defaultValue;
            Getter = getter;
            Visible = block => false;
        }

        protected override Sandbox.Graphics.GUI.MyGuiControlBase CreateGui()
        {
            return null;
        }

        public override TValue GetValue(TBlock block)
        {
            return Getter(block);
        }

        public override TValue GetDefaultValue(TBlock block)
        {
            return DefaultValue;
        }

        public override TValue GetMininum(TBlock block)
        {
            return DefaultValue;
        }

        public override TValue GetMaximum(TBlock block)
        {
            return DefaultValue;
        }
    }
}
