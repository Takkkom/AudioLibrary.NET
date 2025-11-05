using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.NET
{
    public interface ISoundParams
    {
        bool Playing { get; }
        bool Looping { get; set; }
        float Pitch { get; set; }
        float Gain { get; set; }
        float Pan { get; set; }
        double Time { get; set; }
    }
}
