using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AudioLibrary.NET
{
    public interface ISoundInstance : IDisposable
    {
        bool Looping { get; set; }
        float Pitch { get; set; }
        float Gain { get; set; }
        float Pan { get; set; }

        void Play();
    }
}
