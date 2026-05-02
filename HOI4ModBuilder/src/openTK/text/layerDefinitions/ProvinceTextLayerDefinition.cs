using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.managers;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text.layerDefinitions
{
    public sealed class ProvinceTextLayerDefinition<TContext> : ITextLayerDefinition<TContext>
    {
        public float Scale { get; }
        public Color Color { get; }
        public QFontAlignment Alignment { get; }
        public TextLayerDependencies Dependencies { get; }
        public Func<Province, TContext, bool> IncludePredicate { get; }
        public Func<Province, TContext, string> TextFactory { get; }

        public ProvinceTextLayerDefinition(
            float scale,
            Color color,
            QFontAlignment alignment,
            TextLayerDependencies dependencies,
            Func<Province, TContext, bool> includePredicate,
            Func<Province, TContext, string> textFactory)
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
                .ForEachProvince(
                    province => IncludePredicate(province, context),
                    (fontRegion, province, pos) =>
                    {
                        string text = TextFactory(province, context);
                        if (text == null)
                            return;

                        fontRegion.SetProvinceTextMulti(
                            province, TextRenderManager.Instance.FontData64, Scale, text, pos, Alignment, Color, true
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
                    .ForEachProvince(invalidations, layerDependencies, p => true, (fontRegion, province, pos) =>
                    {
                        if (!IsCurrentProvince(province))
                        {
                            controller.PushAction(pos, region => region.RemoveProvinceTextMulti(province));
                            return;
                        }

                        string text = TextFactory(province, context);
                        if (text == null)
                            controller.PushAction(pos, region => region.RemoveProvinceTextMulti(province));
                        else
                            controller.PushAction(pos, region => region.SetProvinceTextMulti(
                                province, TextRenderManager.Instance.FontData64, Scale, text, pos, Alignment, Color, true
                            ));
                    })
                    .EndAssembleParallelWithWait();
            };
        }
        public static bool IsCurrentProvince(Province province)
            => province != null &&
               ProvinceManager.TryGet(province.Id, out var currentProvince) &&
               currentProvince == province;
    }
}
