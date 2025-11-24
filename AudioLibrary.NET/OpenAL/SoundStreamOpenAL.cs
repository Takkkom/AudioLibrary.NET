using NAudio.Wave;
using Silk.NET.OpenAL;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AudioLibrary.NET.OpenAL
{
    public class SoundStreamOpenAL : ISoundStream
    {
        public static AL AL => SoundDeviceOpenAL.AL;
        public static ALContext ALC => SoundDeviceOpenAL.ALC;

        private readonly int secondLength;
        private readonly int bufferSize;

        //!!
        private readonly uint buffer;
        private readonly SoundInstanceOpenAL instanceOpenAL;
        private readonly WaveStream stream;
        private readonly BufferFormat format;
        private bool forceUpdate;

        public ISoundInstance Instance => instanceOpenAL;


        public bool Looping { get; set; }

        public float Pitch
        {
            get => Instance.Pitch;
            set => Instance.Pitch = value;
        }

        public float Gain
        {
            get => Instance.Gain;
            set => Instance.Gain = value;
        }

        public float Pan
        {
            get => Instance.Pan;
            set => Instance.Pan = value;
        }

        private double baseStreamTime;
        private double stopInstanceTime;
        public double Time
        {
            get
            {
                double instTime = instanceOpenAL.Time;
                if (!Playing) instTime = stopInstanceTime;

                return baseStreamTime + instTime;
            }
            set
            {
                long pos = (long)(value * secondLength);
                pos = Math.Min(pos, stream.Length);
                stream.Seek(pos, SeekOrigin.Begin);

                forceUpdate = true;
            }
        }

        public bool Playing { get; private set; }

        public unsafe SoundStreamOpenAL(WaveStream waveStream)
        {
            secondLength = waveStream.WaveFormat.BitsPerSample * waveStream.WaveFormat.Channels * waveStream.WaveFormat.SampleRate / 8;
            bufferSize = secondLength * 10;
            format = ALUtil.GetBufferFormat(waveStream);
            stream = waveStream;
            buffer = AL.GenBuffer();

            instanceOpenAL = new SoundInstanceOpenAL();


            byte[] bytes = new byte[waveStream.WaveFormat.BitsPerSample];
            fixed (void* data = bytes)
            {
                AL.BufferData(buffer, format, data, bytes.Length, stream.WaveFormat.SampleRate);
            }

            AL.SourceQueueBuffers(instanceOpenAL.Soruce, new uint[1] { buffer });
        }

        public unsafe void Dispose()
        {
            Instance.Dispose();
            AL.DeleteBuffer(buffer);
        }

        public void Play()
        {
            if (Playing) return;
            Playing = true;

            instanceOpenAL.Play();
            instanceOpenAL.Time = stopInstanceTime;
            Task.Run(ProcessLoop);
        }

        public void Stop()
        {
            if (!Playing) return;
            stopInstanceTime = Instance.Time;
            Playing = false;
        }

        private unsafe void ReadBuffer()
        {
            AL.SourceUnqueueBuffers(instanceOpenAL.Soruce, new uint[1] { buffer });

            if (Looping && stream.Position == stream.Length)
            {
                stream.Seek(stream.Position % stream.Length, SeekOrigin.Begin);
            }

            baseStreamTime = stream.CurrentTime.TotalSeconds;

            byte[] bytes = new byte[Math.Min(bufferSize, stream.Length)];
            stream.Read(bytes, 0, bytes.Length);

            if (stream.WaveFormat.BitsPerSample == 24)
            {
                bytes = ALUtil.Bit24ToBit16(bytes);
            }

            fixed (void* data = bytes)
            {
                AL.BufferData(buffer, format, data, bytes.Length, stream.WaveFormat.SampleRate);
            }

            AL.SourceQueueBuffers(instanceOpenAL.Soruce, new uint[1] { buffer });
            instanceOpenAL.Play();
        }


        private void ProcessLoop()
        {
            while (Playing)
            {
                AL.GetSourceProperty(instanceOpenAL.Soruce, GetSourceInteger.BuffersProcessed, out int processed);

                while (processed > 0 || forceUpdate)
                {
                    instanceOpenAL.Stop();
                    ReadBuffer();

                    forceUpdate = false;
                    processed--;
                }
            }

            instanceOpenAL.Stop();
        }
    }
}
