using NAudio.Wave;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.NET.OpenAL
{
    public class SoundDeviceOpenAL : ISoundDevice
    {
        public static readonly AL AL = AL.GetApi();
        public static readonly ALContext ALC = ALContext.GetApi();

        internal readonly unsafe Device* Device;
        internal readonly unsafe Context* Context;

        public unsafe SoundDeviceOpenAL()
        {
            Device = ALC.OpenDevice(null);

            if (Device == null)
            {
                throw new Exception();
            }

            Context = ALC.CreateContext(Device, null);
            ALC.MakeContextCurrent(Context);
        }

        public unsafe void Dispose()
        {
            ALC.DestroyContext(Context);
            ALC.CloseDevice(Device);
        }

        public ISound CreateSound(WaveStream waveStream)
        {
            return new SoundOpenAL(waveStream);
        }

        public ISoundStream CreateSoundStream(WaveStream waveStream)
        {
            return new SoundStreamOpenAL(waveStream);
        }
    }
}
