using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.NET
{
    public interface ISound : IDisposable
    {
        ISoundInstance CreateInstance();
    }
}
