using NAudio.Wave;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AudioLibrary.NET.OpenAL
{
    public class SoundOpenAL : ISound
    {
        public static AL AL => SoundDeviceOpenAL.AL;
        public static ALContext ALC => SoundDeviceOpenAL.ALC;

        private readonly uint buffer;

        public unsafe SoundOpenAL(WaveStream waveStream)
        {
            buffer = AL.GenBuffer();

            byte[] bytes = new byte[waveStream.Length];
            waveStream.Read(bytes, 0, bytes.Length);

            BufferFormat bufferFormat = ALUtil.GetBufferFormat(waveStream);

            if (waveStream.WaveFormat.BitsPerSample == 24)
            {
                bytes = ALUtil.Bit24ToBit16(bytes);
            }

            fixed (void* data = bytes)
            {
                AL.BufferData(buffer, bufferFormat, data, bytes.Length, waveStream.WaveFormat.SampleRate);
            }
        }

        public void Dispose()
        {
            AL.DeleteBuffer(buffer);
        }

        public ISoundInstance CreateInstance()
        {
            return new SoundInstanceOpenAL(buffer);
        }
    }
}
