using Masters_2025_MSS_621_JW.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masters_2025_MSS_621_JW {
    public class Audio {
        Xpanel tp;
        public Audio(Xpanel tp) {
            this.tp = tp;
            tp.ConnectAudio(this);
        }

        protected void fbMute(bool mute) {
            tp.UpdateMute(mute);
        }

        protected void fbVol(ushort vol) {
            tp.UpdateVolume(vol);
        }

        virtual public void VolumeUpRamp() { throw new NotImplementedException(); }
        virtual public void VolumeDownRamp() { throw new NotImplementedException(); }
        virtual public void VolumeStopRamp() { throw new NotImplementedException(); }
        virtual public void MuteToggle() { throw new NotImplementedException(); }
    }
}
