using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.NET
{
    public interface ISoundDevice : IDisposable
    {
        ISound CreateSound(WaveStream waveStream);
        ISoundStream CreateSoundStream(WaveStream waveStream);
    }
}
