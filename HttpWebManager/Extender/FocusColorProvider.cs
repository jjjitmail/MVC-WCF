using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace HttpWebManager
{
    [ProvideProperty("FocusColor", typeof(Control))]
    public class FocusColorProvider : Component, IExtenderProvider
    {
        private readonly Dictionary<IntPtr, Color> _focusColors;
        private readonly Dictionary<IntPtr, Color> _backColors;

        public FocusColorProvider()
        {
            _focusColors = new Dictionary<IntPtr, Color>();
            _backColors = new Dictionary<IntPtr, Color>();
        }

        public bool CanExtend(object extendee)
        {
            return (extendee is Control);
        }

        public Color GetFocusColor(Control ctl)
        {
            Color color;
            if (_focusColors.TryGetValue(ctl.Handle, out color))
            {
                return color;
            }
            return ctl.BackColor;
        }

        public void SetFocusColor(Control ctl, Color color)
        {
            Color backColor;
            if (!_backColors.TryGetValue(ctl.Handle, out backColor))
            {
                backColor = ctl.BackColor;
            }

            // Same color as BackColor, disable the behavior
            if (color == backColor)
            {
                RemoveFocusColor(ctl);
                ctl.LostFocus -= ctl_LostFocus;
                ctl.GotFocus -= ctl_GotFocus;
                _focusColors.Remove(ctl.Handle);
            }
            else
            {
                _focusColors[ctl.Handle] = color;
                if (ctl.Focused)
                    ApplyFocusColor(ctl);
                ctl.LostFocus += ctl_LostFocus;
                ctl.GotFocus += ctl_GotFocus;
            }
        }

        void ctl_GotFocus(object sender, EventArgs e)
        {
            ApplyFocusColor((Control)sender);
        }

        void ctl_LostFocus(object sender, EventArgs e)
        {
            RemoveFocusColor((Control)sender);
        }

        void ApplyFocusColor(Control ctl)
        {
            _backColors[ctl.Handle] = ctl.BackColor;
            ctl.BackColor = GetFocusColor(ctl);
        }

        void RemoveFocusColor(Control ctl)
        {
            Color color;
            if (_backColors.TryGetValue(ctl.Handle, out color))
            {
                ctl.BackColor = color;
                _backColors.Remove(ctl.Handle);
            }
        }
    }
}
