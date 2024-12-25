using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using NAudio.Dsp;
using Rise.Common.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace Rise.Effects
{
    public sealed partial class EqualizerEffect : IBasicAudioEffect, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly static Lazy<EqualizerEffect> _current
            = new Lazy<EqualizerEffect>(() => new EqualizerEffect());
        /// <summary>
        /// Gets the current instance of the effect.
        /// </summary>
        public static EqualizerEffect Current => _current.Value;

        /// <summary>
        /// Whether the effect has been initialized.
        /// </summary>
        public static bool Initialized => _current.IsValueCreated;

        /// <summary>
        /// Bands available for the equalizer.
        /// </summary>
        public IObservableVector<EqualizerBand> Bands { get; private set; }
            = new ObservableVector<EqualizerBand>();

        private bool _isEnabled;
        /// <summary>
        /// Whether the effect should be enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        private static List<AudioEncodingProperties> _supportedEncodingProperties;
        private AudioEncodingProperties currentEncodingProperties;

        private BiQuadFilter[,] filters;
        private int channels;
        private int bandCount;
        private IPropertySet configuration;

        /// <summary>
        /// Initializes the EQ bands with the specified gains.
        /// </summary>
        public void InitializeBands([ReadOnlyArray] float[] gains)
        {
            if (Bands.Count > 0)
                return;

            // Generalize to 0@max
            var max = float.MinValue;
            foreach (var gain in gains)
            {
                if (gain > max)
                    max = gain;
            }

            var bands = new EqualizerBand[]
            {
                new EqualizerBand { Index = 0, Bandwidth = 0.8f, Frequency = 30, Gain = gains[0] - max },
                new EqualizerBand { Index = 1, Bandwidth = 0.8f, Frequency = 75, Gain = gains[1] - max },
                new EqualizerBand { Index = 2, Bandwidth = 0.8f, Frequency = 150, Gain = gains[2] - max },
                new EqualizerBand { Index = 3, Bandwidth = 0.8f, Frequency = 30, Gain = gains[3] - max },
                new EqualizerBand { Index = 4, Bandwidth = 0.8f, Frequency = 600, Gain = gains[4] - max },
                new EqualizerBand { Index = 5, Bandwidth = 0.8f, Frequency = 1250, Gain = gains[5] - max },
                new EqualizerBand { Index = 6, Bandwidth = 0.8f, Frequency = 2500, Gain = gains[6] - max },
                new EqualizerBand { Index = 7, Bandwidth = 0.8f, Frequency = 5000, Gain = gains[7] - max },
                new EqualizerBand { Index = 8, Bandwidth = 0.8f, Frequency = 10000, Gain = gains[8] - max },
                new EqualizerBand { Index = 9, Bandwidth = 0.8f, Frequency = 20000, Gain = gains[9] - max },
            };

            foreach (var band in bands)
                Bands.Add(band);
        }

        /// <summary>
        /// Recreates the filters for the band at the specified index.
        /// </summary>
        public void UpdateBand(int index)
        {
            var band = Bands[index];
            for (int n = 0; n < channels; n++)
            {
                if (filters[n, index] == null)
                    filters[n, index] = BiQuadFilter.PeakingEQ(currentEncodingProperties.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                else
                    filters[n, index].SetPeakingEq(currentEncodingProperties.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
            }
        }

        /// <summary>
        /// Recreates the filters for every band.
        /// </summary>
        public void UpdateAllBands()
        {
            for (int i = 0; i < bandCount; i++)
                UpdateBand(i);
        }

        private void SetEncodingPropertiesImpl(AudioEncodingProperties encodingProperties)
        {
            currentEncodingProperties = encodingProperties;

            if (channels != (int)currentEncodingProperties.ChannelCount || bandCount != Bands.Count)
            {
                channels = (int)currentEncodingProperties.ChannelCount;
                bandCount = Bands.Count;

                filters = new BiQuadFilter[channels, bandCount];
            }

            UpdateAllBands();
        }

        private void ProcessFrameImpl(ProcessAudioFrameContext context)
        {
            // Check if we don't have the effect enabled in the configuration.
            // This is a workaround for the fact that MediaPlayer does not
            // have a way to remove/disable the effect.
            if (!IsEnabled)
                return;

            unsafe
            {
                AudioFrame inputFrame = context.InputFrame;

                using (AudioBuffer inputBuffer = inputFrame.LockBuffer(AudioBufferAccessMode.ReadWrite))
                using (IMemoryBufferReference inputReference = inputBuffer.CreateReference())
                {
                    ((IMemoryBufferByteAccess)inputReference).GetBuffer(out byte* inputDataInBytes, out uint inputCapacity);

                    float* inputDataInFloat = (float*)inputDataInBytes;
                    int dataInFloatLength = (int)inputBuffer.Length / sizeof(float);

                    // Process audio data
                    for (int n = 0; n < dataInFloatLength; n++)
                    {
                        int ch = n % channels;

                        // Cascaded filter to perform EQ
                        for (int band = 0; band < bandCount; band++)
                        {
                            inputDataInFloat[n] = filters[ch, band].Transform(inputDataInFloat[n]);
                        }
                    }
                }
            }
        }

        private void CloseImpl(MediaEffectClosedReason reason)
        {
            switch (reason)
            {
                case MediaEffectClosedReason.Done:
                case MediaEffectClosedReason.UnknownError:
                case MediaEffectClosedReason.UnsupportedEncodingFormat:
                default:
                    break;
                case MediaEffectClosedReason.EffectCurrentlyUnloaded:
                    if (filters != null)
                        for (int i = 0; i < filters.Rank; i++)
                        {
                            for (int j = 0; j < filters.GetLength(i); j++)
                            {
                                filters[i, j] = null;
                            }
                        }
                    channels = 0;
                    bandCount = 0;
                    filters = null;
                    break;
            }
        }
    }

    // IBasicAudioEffect
    public sealed partial class EqualizerEffect : IBasicAudioEffect
    {
        public bool UseInputFrameForOutput => true;

        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties
        {
            get
            {
                if (_supportedEncodingProperties == null)
                {
                    _supportedEncodingProperties = new List<AudioEncodingProperties>();

                    AudioEncodingProperties encodingProps1 = AudioEncodingProperties.CreatePcm(44100, 1, 32);
                    encodingProps1.Subtype = MediaEncodingSubtypes.Float;
                    AudioEncodingProperties encodingProps2 = AudioEncodingProperties.CreatePcm(48000, 1, 32);
                    encodingProps2.Subtype = MediaEncodingSubtypes.Float;

                    AudioEncodingProperties encodingProps3 = AudioEncodingProperties.CreatePcm(44100, 2, 32);
                    encodingProps3.Subtype = MediaEncodingSubtypes.Float;
                    AudioEncodingProperties encodingProps4 = AudioEncodingProperties.CreatePcm(48000, 2, 32);
                    encodingProps4.Subtype = MediaEncodingSubtypes.Float;

                    AudioEncodingProperties encodingProps5 = AudioEncodingProperties.CreatePcm(96000, 2, 32);
                    encodingProps5.Subtype = MediaEncodingSubtypes.Float;
                    AudioEncodingProperties encodingProps6 = AudioEncodingProperties.CreatePcm(192000, 2, 32);
                    encodingProps6.Subtype = MediaEncodingSubtypes.Float;

                    _supportedEncodingProperties.Add(encodingProps1);
                    _supportedEncodingProperties.Add(encodingProps2);
                    _supportedEncodingProperties.Add(encodingProps3);
                    _supportedEncodingProperties.Add(encodingProps4);
                    _supportedEncodingProperties.Add(encodingProps5);
                    _supportedEncodingProperties.Add(encodingProps6);
                }

                return _supportedEncodingProperties;
            }
        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties)
        {
            Current?.SetEncodingPropertiesImpl(encodingProperties);
        }

        public void ProcessFrame(ProcessAudioFrameContext context)
        {
            Current?.ProcessFrameImpl(context);
        }

        public void DiscardQueuedFrames()
        {
        }

        public void Close(MediaEffectClosedReason reason)
        {
            Current?.CloseImpl(reason);
        }

        public void SetProperties(IPropertySet configuration)
        {
            if (Current != null)
                Current.configuration = configuration;
        }
    }
}
