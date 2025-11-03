using NAudio.Wave;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AudioLibrary.NET.OpenAL
{
    public class SoundInstanceOpenAL : ISoundInstance
    {
        public static AL AL => SoundDeviceOpenAL.AL;
        public static ALContext ALC => SoundDeviceOpenAL.ALC;

        internal readonly uint Soruce;

        public bool Looping
        {
            get
            {
                AL.GetSourceProperty(Soruce, SourceBoolean.Looping, out bool value);
                return value;
            }
            set => AL.SetSourceProperty(Soruce, SourceBoolean.Looping, value);
        }

        public float Pitch
        {
            get
            {
                AL.GetSourceProperty(Soruce, SourceFloat.Pitch, out float value);
                return value;
            }
            set => AL.SetSourceProperty(Soruce, SourceFloat.Pitch, value);
        }

        public float Gain
        {
            get
            {
                AL.GetSourceProperty(Soruce, SourceFloat.Gain, out float value);
                return value;
            }
            set => AL.SetSourceProperty(Soruce, SourceFloat.Gain, value);
        }

        public float Pan
        {
            get
            {
                AL.GetSourceProperty(Soruce, SourceVector3.Position, out Vector3 value);
                return value.X;
            }
            set
            {
                float z = MathF.Sqrt(1 - value * value);
                AL.SetSourceProperty(Soruce, SourceVector3.Position, value, 0.0f, z);
            }
        }

        public unsafe SoundInstanceOpenAL()
        {
            Soruce = AL.GenSource();
            AL.SetSourceProperty(Soruce, SourceBoolean.SourceRelative, true);

            Looping = false;
            Pitch = 1.0f;
            Gain = 0.5f;
            Pan = 0.5f;
        }

        public unsafe SoundInstanceOpenAL(uint buffer) : this()
        {
            AL.SetSourceProperty(Soruce, SourceInteger.Buffer, buffer);
        }

        public void Play()
        {
            AL.SourcePlay(Soruce);
        }

        public void Dispose()
        {
            AL.DeleteSource(Soruce);
        }
    }
}
