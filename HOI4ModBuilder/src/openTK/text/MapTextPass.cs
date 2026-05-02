using HOI4ModBuilder.src.utils.structs;
using OpenTK;
using System;

namespace HOI4ModBuilder.src.openTK.text
{
    public class MapTextPass
    {
        public bool EnableZoomInvariantTextScaling { get; set; } = true;
        public double ZoomInvariantTextScalingStartZoom { get; set; } = 0.01d;
        public float MinZoomInvariantTextScale { get; set; } = 0.05f;

        public void Render(
            FontRenderController controller,
            ViewportInfo viewportInfo,
            Bounds4F viewportBounds,
            double zoomFactor,
            double mapDifX,
            double mapDifY,
            bool isTextLayerVisible,
            bool renderDebugChunks,
            bool disableDistanceCutoff,
            bool enableZoomInvariantTextScaling,
            double zoomInvariantTextScalingStartZoom,
            float minZoomInvariantTextScale)
        {
            if (controller == null)
                return;

            if (renderDebugChunks)
                controller.RenderDebug();

            if (!isTextLayerVisible)
                return;

            float textScale = controller.TextScale;
            if (textScale <= 0f)
                return;

            bool distanceTextCutoff = zoomFactor < (1 / textScale * 0.00015f * (viewportInfo.height / (float)viewportInfo.max));
            if (!disableDistanceCutoff && distanceTextCutoff)
                return;

            var projection = Matrix4.CreateOrthographicOffCenter(
                viewportInfo.x,
                -viewportInfo.x + viewportInfo.width,
                viewportInfo.y,
                -viewportInfo.y + viewportInfo.height,
                -1f, 1f
            );

            float scaleHalf = textScale / 2f;
            float factor = (float)zoomFactor * viewportInfo.max;

            var viewMatrix =
                Matrix4.CreateScale(scaleHalf, scaleHalf, scaleHalf) *
                Matrix4.CreateScale(factor, factor, factor) *
                Matrix4.CreateTranslation(
                    viewportInfo.width / 2f + (float)(mapDifX * factor / 2f),
                    viewportInfo.height / 2f + (float)(-mapDifY * factor / 2f),
                    0f
                );

            EnableZoomInvariantTextScaling = enableZoomInvariantTextScaling;
            ZoomInvariantTextScalingStartZoom = zoomInvariantTextScalingStartZoom;
            MinZoomInvariantTextScale = minZoomInvariantTextScale;

            controller.Render(
                viewMatrix * projection,
                viewportBounds,
                CalculateGeometryScale(zoomFactor)
            );
        }

        private float CalculateGeometryScale(double zoomFactor)
        {
            if (!EnableZoomInvariantTextScaling)
                return 1f;

            if (ZoomInvariantTextScalingStartZoom <= 0d || zoomFactor <= ZoomInvariantTextScalingStartZoom)
                return 1f;

            float geometryScale = (float)(ZoomInvariantTextScalingStartZoom / zoomFactor);
            if (geometryScale < MinZoomInvariantTextScale)
                return MinZoomInvariantTextScale;

            return geometryScale;
        }
    }
}
