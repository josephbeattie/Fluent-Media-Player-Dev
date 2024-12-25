using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace Rise.Common.Extensions
{
    /// <summary>
    /// A set of methods for composition related tasks.
    /// </summary>
    public static class CompositionExtensions
    {
        /// <summary>
        /// Creates a mask containing an image that fades to transpaarent.
        /// </summary>
        /// <param name="imageSurface">The image surface to use for the mask.</param>
        /// <param name="opacity">The mask's opacity.</param>
        /// <returns>The created <see cref="CompositionMaskBrush"/>.</returns>
        public static CompositionMaskBrush CreateImageGradientMask(this Compositor compositor, LoadedImageSurface imageSurface, float opacity)
        {
            var imageBrush = compositor.CreateSurfaceBrush(imageSurface);
            imageBrush.HorizontalAlignmentRatio = 0.5f;
            imageBrush.VerticalAlignmentRatio = 0.5f;
            imageBrush.Stretch = CompositionStretch.UniformToFill;

            var gradient = compositor.CreateLinearGradientBrush();
            gradient.EndPoint = new Vector2(0, 1);
            gradient.MappingMode = CompositionMappingMode.Relative;
            gradient.ColorStops.Add(compositor.CreateColorGradientStop(opacity, Colors.White));
            gradient.ColorStops.Add(compositor.CreateColorGradientStop(1, Colors.Transparent));

            var maskBrush = compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = gradient;

            return maskBrush;
        }

        /// <summary>
        /// Creates an image gradient with parallax for the provided list's <see cref="ScrollViewer"/>.
        /// </summary>
        /// <param name="surface">The image surface to use for the gradient.</param>
        /// <param name="visualHost">The element that will host the <see cref="SpriteVisual"/>.</param>
        /// <returns>A tuple containing the <see cref="ScrollViewer"/>'s manipulation property set and
        /// a <see cref="SpriteVisual"/> with a parallax animation.</returns>
        public static (CompositionPropertySet, SpriteVisual) CreateParallaxGradientVisual(this ListViewBase scrollViewer, LoadedImageSurface surface, FrameworkElement visualHost)
            => CreateParallaxGradientVisual(scrollViewer.FindVisualChild<ScrollViewer>(), surface, visualHost);

        /// <summary>
        /// Creates an image gradient with parallax for the provided <see cref="ScrollViewer"/>.
        /// </summary>
        /// <param name="surface">The image surface to use for the gradient.</param>
        /// <param name="visualHost">The element that will host the <see cref="SpriteVisual"/>.</param>
        /// <returns>A tuple containing the <see cref="ScrollViewer"/>'s manipulation property set and
        /// a <see cref="SpriteVisual"/> with a parallax animation.</returns>
        public static (CompositionPropertySet, SpriteVisual) CreateParallaxGradientVisual(this ScrollViewer scrollViewer, LoadedImageSurface surface, FrameworkElement visualHost)
        {
            var propSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
            var visual = CreateParallaxGradientVisual(propSet.Compositor, surface, visualHost, propSet);

            return (propSet, visual);
        }

        /// <summary>
        /// Creates an image gradient with parallax based on the provided
        /// <see cref="CompositionPropertySet"/>.
        /// </summary>
        /// <param name="surface">The image surface to use for the gradient.</param>
        /// <param name="visualHost">The element that will host the <see cref="SpriteVisual"/>.</param>
        /// <param name="scrollPropertySet">A property set to use for the parallax.</param>
        /// <returns>A <see cref="SpriteVisual"/> with a parallax animation.</returns>
        public static SpriteVisual CreateParallaxGradientVisual(this Compositor compositor, LoadedImageSurface surface, FrameworkElement visualHost, CompositionPropertySet scrollPropertySet)
        {
            var maskBrush = CreateImageGradientMask(compositor, surface, 0.6f);

            var spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Size = new Vector2((float)visualHost.ActualWidth, (float)visualHost.ActualHeight);
            spriteVisual.Opacity = 0.15f;
            spriteVisual.Brush = maskBrush;

            var parallax = CreateTranslationParallaxAnimation(compositor, 0.8f, scrollPropertySet);
            var translation = CreateTranslationAnimation(compositor, scrollPropertySet);

            maskBrush.Source.StartAnimation("Offset.Y", parallax);
            spriteVisual.StartAnimation("Offset.Y", translation);

            ElementCompositionPreview.SetElementChildVisual(visualHost, spriteVisual);
            return spriteVisual;
        }

        /// <summary>
        /// Creates a simple translation animation based on a <see cref="CompositionPropertySet"/>.
        /// </summary>
        /// <returns>A <see cref="ExpressionAnimation"/> that changes during translation.</returns>
        public static ExpressionAnimation CreateTranslationAnimation(this Compositor compositor, CompositionPropertySet propertySet)
        {
            var expression = compositor.CreateExpressionAnimation();
            expression.SetReferenceParameter("ScrollManipulation", propertySet);
            expression.Expression = "ScrollManipulation.Translation.Y";

            return expression;
        }

        /// <summary>
        /// Creates a simple parallax animation based on a <see cref="CompositionPropertySet"/>
        /// and the provided parallax value.
        /// </summary>
        /// <returns>A <see cref="ExpressionAnimation"/> with parallax during translation.</returns>
        public static ExpressionAnimation CreateTranslationParallaxAnimation(this Compositor compositor, float parallax, CompositionPropertySet propertySet)
        {
            var expression = compositor.CreateExpressionAnimation();
            expression.SetScalarParameter("ParallaxValue", parallax);

            expression.SetReferenceParameter("ScrollManipulation", propertySet);
            expression.Expression = "-(ScrollManipulation.Translation.Y) * ParallaxValue";

            return expression;
        }
    }
}
