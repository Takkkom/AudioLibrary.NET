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
            waveStream.Read(bytes);

            BufferFormat bufferFormat = BufferFormat.Mono8;

            switch(waveStream.WaveFormat.BitsPerSample)
            {
                case 8:
                    bufferFormat = waveStream.WaveFormat.Channels == 1 ? BufferFormat.Mono8 : BufferFormat.Stereo8;
                    break;
                case 16:
                    bufferFormat = waveStream.WaveFormat.Channels == 1 ? BufferFormat.Mono16 : BufferFormat.Stereo16;
                    break;
                case 24:
                    {
                        bufferFormat = waveStream.WaveFormat.Channels == 1 ? (BufferFormat)FloatBufferFormat.Mono : (BufferFormat)FloatBufferFormat.Stereo;

                        int baseLength = bytes.Length / 3;
                        byte[] newBytes = new byte[baseLength * 4];

                        int mul = 256;

                        for (int i = 0; i < baseLength; i++)
                        {
                            int shift24 = i * 3;
                            int shift32 = i * 4;
                            byte[] byteArray = new byte[3] { bytes[shift24 + 0], bytes[shift24 + 1], bytes[shift24 + 2] };

                            int og = byteArray[0] + (byteArray[1] << 8) + (byteArray[2] << 16);
                            float val = og / 16777216.0f;
                            val *= 0.5f;

                            byte[] newB = BitConverter.GetBytes(val);
                            newBytes[shift32 + 0] = newB[0];
                            newBytes[shift32 + 1] = newB[1];
                            newBytes[shift32 + 2] = newB[2];
                            newBytes[shift32 + 3] = newB[3];
                        }
                        bytes = newBytes;
                    }
                    break;
                case 32:
                    bufferFormat = waveStream.WaveFormat.Channels == 1 ? (BufferFormat)FloatBufferFormat.Mono : (BufferFormat)FloatBufferFormat.Stereo;
                    break;
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
