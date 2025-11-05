using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.NET
{
    public interface ISoundStream : ISoundParams, IDisposable
    {
        void Play();
        void Stop();
    }
}
