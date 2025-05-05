using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Masters_2025_MSS_621_JW.Devices;
using Masters_2025_MSS_621_JW.UserInterface;

namespace Masters_2025_MSS_621_JW {
    public class AudioTV : Audio {
        private CrestronConnected tv;
        public AudioTV(CrestronConnected tv, Xpanel tp) : base(tp) {
            this.tv = tv;

            tv.VolumeChanged += Tv_VolumeChanged;
            tv.MuteChanged += Tv_MuteChanged;
        }

        private void Tv_MuteChanged(bool mute) {
            fbMute(mute);
        }

        private void Tv_VolumeChanged(ushort vol) {
            fbVol(vol);
        }

        override public void VolumeUpRamp() { tv.VolumeUp(); }
        override public void VolumeDownRamp() { tv.VolumeDown(); }
        override public void VolumeStopRamp() { tv.VolumeStop(); }
        override public void MuteToggle() { tv.MuteToggle(); }

    }
}
