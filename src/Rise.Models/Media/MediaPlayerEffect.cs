using System;
using Windows.Foundation.Collections;

namespace Rise.Models
{
    /// <summary>
    /// Contains the data necessary to add custom effects to a
    /// <see cref="Windows.Media.Playback.MediaPlayer"/>.
    /// </summary>
    public sealed class MediaPlayerEffect
    {
        /// <summary>
        /// The underlying type of this effect.
        /// </summary>
        public readonly Type EffectClassType;

        /// <summary>
        /// Whether this effect is optional.
        /// </summary>
        public readonly bool IsOptional;

        /// <summary>
        /// Whether this effect is an audio effect.
        /// </summary>
        public readonly bool IsAudioEffect;

        /// <summary>
        /// Configuration settings for this effect.
        /// </summary>
        public readonly IPropertySet Configuration;

        public MediaPlayerEffect(Type effectClassType, bool isOptional, bool isAudioEffect, IPropertySet configuration)
        {
            EffectClassType = effectClassType;
            IsOptional = isOptional;
            IsAudioEffect = isAudioEffect;
            Configuration = configuration;
        }
    }
}
