using HOI4ModBuilder.src.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.map.strategicRegion;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text.layerDefinitions
{
    public sealed class RegionTextLayerDefinition<TContext> : ITextLayerDefinition<TContext>
    {
        public float Scale { get; }
        public Color Color { get; }
        public QFontAlignment Alignment { get; }
        public TextLayerDependencies Dependencies { get; }
        public Func<StrategicRegion, TContext, bool> IncludePredicate { get; }
        public Func<StrategicRegion, TContext, string> TextFactory { get; }

        public RegionTextLayerDefinition(
            float scale,
            Color color,
            QFontAlignment alignment,
            TextLayerDependencies dependencies,
            Func<StrategicRegion, TContext, bool> includePredicate,
            Func<StrategicRegion, TContext, string> textFactory)
        {
            Scale = scale;
            Color = color;
            Alignment = alignment;
            Dependencies = dependencies;
            IncludePredicate = includePredicate;
            TextFactory = textFactory;
        }

        public bool Rebuild(FontRenderController controller, TContext context)
        {
            controller.TryStart(out var result)?
                .SetEventsHandler(Dependencies, CreateEventsHandler(controller, context))
                .SetScale(Scale)
                .ClearAllMulti()
                .ForEachRegion(
                    region => IncludePredicate(region, context),
                    (fontRegion, region, pos) =>
                    {
                        string text = TextFactory(region, context);
                        if (text == null)
                            return;

                        fontRegion.SetRegionTextMulti(
                            region, TextRenderManager.Instance.FontData64, Scale, text, pos, Alignment, Color, true
                        );
                    })
                .EndAssembleParallel();

            return result;
        }

        private Action<TextLayerDependencies, TextRenderInvalidationBatch> CreateEventsHandler(FontRenderController controller, TContext context)
        {
            return (layerDependencies, invalidations) =>
            {
                controller.TryStart(layerDependencies, out var eventResult)?
                    .ForEachRegion(invalidations, layerDependencies, r => true, (fontRegion, region, pos) =>
                    {
                        if (!IsCurrentRegion(region))
                        {
                            controller.PushAction(pos, currentRegion => currentRegion.RemoveRegionTextMulti(region));
                            return;
                        }

                        string text = TextFactory(region, context);
                        if (text == null)
                            controller.PushAction(pos, currentRegion => currentRegion.RemoveRegionTextMulti(region));
                        else
                            controller.PushAction(pos, currentRegion => currentRegion.SetRegionTextMulti(
                                region, TextRenderManager.Instance.FontData64, Scale, text, pos, Alignment, Color, true
                            ));
                    })
                    .EndAssembleParallelWithWait();
            };
        }
        public static bool IsCurrentRegion(StrategicRegion region)
            => region != null &&
               StrategicRegionManager.TryGet(region.Id, out var currentRegion) &&
               currentRegion == region;
    }
}
