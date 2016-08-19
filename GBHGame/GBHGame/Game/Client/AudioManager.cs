using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.Codecs.MP3;
using CSCore.Utils;
using CSCore.XAudio2;
using CSCore.XAudio2.X3DAudio;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using AudioVector3 = CSCore.Utils.Vector3;

namespace GBH
{
    static class AudioManager
    {
        private static XAudio2 _xaudio2;
        private static XAudio2MasteringVoice _masteringVoice;
        private static ConVar _snd_volume;

        public static void Initialize()
        {
            _xaudio2 = XAudio2.CreateXAudio2();
            _masteringVoice = _xaudio2.CreateMasteringVoice();

            _snd_volume = ConVar.Register<float>("snd_volume", 1.0f, "Game audio volume", ConVarFlags.Archived);

            _masteringVoice.Volume = _snd_volume.GetValue<float>();

            _snd_volume.Modified += () =>
            {
                _masteringVoice.Volume = _snd_volume.GetValue<float>();
            };
        }

        public static void PlayRepeat(string soundFile, Vector3 position)
        {
            var source = new DmoMp3Decoder(FileSystem.OpenRead(soundFile));
            var streamingSourceVoice = new StreamingSourceVoice(_xaudio2, source);
            StreamingSourceVoiceListener.Default.Add(streamingSourceVoice);
            streamingSourceVoice.Start();

            streamingSourceVoice.Stopped += (s, e) =>
            {
                streamingSourceVoice.Start();
            };
        }

        public static void PlayOneShot(string soundFile, Vector3 position)
        {
            var source = new WaveFileReader(FileSystem.OpenRead(soundFile));
            var streamingSourceVoice = new StreamingSourceVoice(_xaudio2, source);
            StreamingSourceVoiceListener.Default.Add(streamingSourceVoice);
            streamingSourceVoice.Start();

            streamingSourceVoice.Stopped += (s, e) =>
            {
                StreamingSourceVoiceListener.Default.Remove(streamingSourceVoice);
                streamingSourceVoice.Stop();

                streamingSourceVoice.Dispose();
                source.Dispose();
            };
        }
    }
}
