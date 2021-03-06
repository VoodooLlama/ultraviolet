﻿using System;
using System.Runtime.InteropServices;
using TwistedLogik.Nucleus;
using TwistedLogik.Ultraviolet.Audio;
using TwistedLogik.Ultraviolet.BASS.Native;
using TwistedLogik.Ultraviolet.Platform;

namespace TwistedLogik.Ultraviolet.BASS.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the SoundEffect class.
    /// </summary>
    public sealed class BASSSoundEffect : SoundEffect
    {
        /// <summary>
        /// Initializes a new instance of the BASSSoundEffect class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="filename">The filename of the sample to load.</param>
        public BASSSoundEffect(UltravioletContext uv, String filename)
            : base(uv)
        {
            var fileSystemService = FileSystemService.Create();
            var fileData          = default(Byte[]);

            using (var stream = fileSystemService.OpenRead(filename))
            {
                fileData = new Byte[stream.Length];
                stream.Read(fileData, 0, fileData.Length);
            }

            sample = BASSNative.SampleLoad(fileData, 0, (UInt32)fileData.Length, UInt16.MaxValue, 0);
            if (!BASSUtil.IsValidHandle(sample))
                throw new BASSException();

            if (!BASSNative.SampleGetInfo(sample, out this.sampleInfo))
                throw new BASSException();

            this.data = Marshal.AllocHGlobal((int)sampleInfo.length);
            if (!BASSNative.SampleGetData(sample, this.data))
                throw new BASSException();
        }

        /// <inheritdoc/>
        public override void Play()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var channel = BASSNative.SampleGetChannel(sample, false);
            if (!BASSUtil.IsValidHandle(channel))
                throw new BASSException();

            if (!BASSNative.ChannelPlay(channel, true))
                throw new BASSException();
        }

        /// <inheritdoc/>
        public override void Play(Single volume, Single pitch, Single pan)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var channel = 0u;

            if (pitch == 0)
            {
                channel = BASSNative.SampleGetChannel(sample, false);
                if (!BASSUtil.IsValidHandle(channel))
                    throw new BASSException();
            }
            else
            {
                var stream = BASSNative.StreamCreate(sampleInfo.freq, sampleInfo.chans, sampleInfo.flags | BASSNative.BASS_STREAM_DECODE, BASSNative.STREAMPROC_PUSH, IntPtr.Zero);
                if (!BASSUtil.IsValidHandle(stream))
                    throw new BASSException();

                var pushed = BASSNative.StreamPutData(stream, data, sampleInfo.length);
                if (!BASSUtil.IsValidValue(pushed))
                    throw new BASSException();

                stream = BASSFXNative.TempoCreate(stream, BASSNative.BASS_FX_FREESOURCE | BASSNative.BASS_STREAM_AUTOFREE);
                if (!BASSUtil.IsValidHandle(stream))
                    throw new BASSException();

                channel = stream;

                BASSUtil.SetPitch(channel, MathUtil.Clamp(pitch, -1f, 1f));
            }

            BASSUtil.SetVolume(channel, MathUtil.Clamp(volume, 0f, 1f));
            BASSUtil.SetPan(channel, MathUtil.Clamp(pan, -1f, 1f));

            if (!BASSNative.ChannelPlay(channel, false))
                throw new BASSException();
        }

        /// <inheritdoc/>
        public override TimeSpan Duration
        {
            get
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var duration = BASSUtil.GetDurationInSeconds(sample);
                return TimeSpan.FromSeconds(duration);
            }
        }

        /// <summary>
        /// Gets the sound effect's sample data.
        /// </summary>
        /// <param name="data">The sound effect's raw PCM sample data.</param>
        /// <param name="info">The sound effect's sample info.</param>
        /// <returns>The handle to the sound effect's BASS sample.</returns>
        internal UInt32 GetSampleData(out IntPtr data, out BASS_SAMPLE info)
        {
            data = this.data;
            info = this.sampleInfo;
            return sample;
        }
        
        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;
            
            if (!BASSNative.SampleFree(sample))
                throw new BASSException();

            if (this.data != IntPtr.Zero)
                Marshal.FreeHGlobal(this.data);

            this.sample = 0;
            this.data = IntPtr.Zero;

            base.Dispose(disposing);
        }

        // The sound effect's sample data.
        private IntPtr data;
        private UInt32 sample;
        private BASS_SAMPLE sampleInfo;
    }
}
