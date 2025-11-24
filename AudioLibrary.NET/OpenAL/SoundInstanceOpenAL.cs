using Silk.NET.OpenAL;
using System;
using System.Numerics;

namespace AudioLibrary.NET.OpenAL
{
    public class SoundInstanceOpenAL : ISoundInstance
    {
        public static AL AL => SoundDeviceOpenAL.AL;
        public static ALContext ALC => SoundDeviceOpenAL.ALC;

        internal readonly uint Soruce;


        public bool Playing
        {
            get
            {
                AL.GetSourceProperty(Soruce, GetSourceInteger.BuffersProcessed, out int processed);
                return processed == 0;
            }
        }

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
                float z = (float)Math.Sqrt(1 - value * value);
                AL.SetSourceProperty(Soruce, SourceVector3.Position, value, 0.0f, z);
            }
        }

        public double Time
        {
            get
            {
                AL.GetSourceProperty(Soruce, SourceFloat.SecOffset, out float value);
                return value;
            }
            set
            {
                AL.SetSourceProperty(Soruce, SourceFloat.SecOffset, (float)value);
            }
        }

        public unsafe SoundInstanceOpenAL()
        {
            Soruce = AL.GenSource();
            AL.SetSourceProperty(Soruce, SourceBoolean.SourceRelative, true);

            Looping = false;
            Pitch = 1.0f;
        }

        public unsafe SoundInstanceOpenAL(uint buffer) : this()
        {
            AL.SetSourceProperty(Soruce, SourceInteger.Buffer, buffer);
        }

        public void Play()
        {
            AL.SourcePlay(Soruce);
        }

        public void Stop()
        {
            AL.SourceStop(Soruce);
        }

        public void Dispose()
        {
            AL.DeleteSource(Soruce);
        }
    }
}
