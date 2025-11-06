using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AudioLibrary.NET
{
    public static class AudioFile
    {
        public static WaveStream? GetWaveStream(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer);
            string head = Encoding.UTF8.GetString(buffer);

            stream.Position = 8;
            byte[] buffer2 = new byte[4];
            stream.Read(buffer2);
            string sub = Encoding.UTF8.GetString(buffer2);

            stream.Position = 0;

            switch (head)
            {
                case "OggS":
                    return new VorbisWaveReader(stream);
                case "RIFF":
                    if (sub == "WAVE")
                    {
                        return new WaveFileReader(stream);
                    }
                    break;
                case "FORM":
                    if (sub == "AIFF" || sub == "AIFC")
                    {
                        return new AiffFileReader(stream);
                    }
                    break;
                default:
                    {
                        byte[] buffer3 = new byte[3];
                        stream.Read(buffer3);
                        string sub2 = Encoding.UTF8.GetString(buffer3);
                        stream.Position = 0;

                        if (sub2 == "ID3" || (buffer3[0] == 0xff && (buffer3[1] & 0x0a) == 0x0a))
                        {
                            return new Mp3FileReader(stream);
                        }
                    }
                    break;
            }

            return null;
        }
    }
}
