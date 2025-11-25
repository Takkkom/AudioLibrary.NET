using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AudioLibrary.NET
{
    public interface ISoundInstance : ISoundParams, IDisposable
    {
        ISound Sound { get; }

        void Play();
        void Stop();
    }
}
