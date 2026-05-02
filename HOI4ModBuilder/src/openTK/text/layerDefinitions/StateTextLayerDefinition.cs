using HOI4ModBuilder.hoiDataObjects.map;
using HOI4ModBuilder.src.hoiDataObjects.history.states;
using QuickFont;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.openTK.text.layerDefinitions
{
    public sealed class StateTextLayerDefinition<TContext> : ITextLayerDefinition<TContext>
    {
        public float Scale { get; }
        public Color Color { get; }
        public QFontAlignment Alignment { get; }
        public TextLayerDependencies Dependencies { get; }
        public Func<State, TContext, bool> IncludePredicate { get; }
        public Func<State, TContext, string> TextFactory { get; }

        public StateTextLayerDefinition(
            float scale,
            Color color,
            QFontAlignment alignment,
            TextLayerDependencies dependencies,
            Func<State, TContext, bool> includePredicate,
            Func<State, TContext, string> textFactory)
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
                .ForEachState(
                    state => IncludePredicate(state, context),
                    (fontRegion, state, pos) =>
                    {
                        string text = TextFactory(state, context);
                        if (text == null)
                            return;

                        fontRegion.SetStateTextMulti(
                            state, TextRenderManager.Instance.FontData64, Scale, text, pos, Alignment, Color, true
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
                    .ForEachState(invalidations, layerDependencies, s => true, (fontRegion, state, pos) =>
                    {
                        if (!IsCurrentState(state))
                        {
                            controller.PushAction(pos, region => region.RemoveStateTextMulti(state));
                            return;
                        }

                        string text = TextFactory(state, context);
                        if (text == null)
                            controller.PushAction(pos, region => region.RemoveStateTextMulti(state));
                        else
                            controller.PushAction(pos, region => region.SetStateTextMulti(
                                state, TextRenderManager.Instance.FontData64, Scale, text, pos, Alignment, Color, true
                            ));
                    })
                    .EndAssembleParallelWithWait();
            };
        }
        public static bool IsCurrentState(State state)
            => state != null &&
               StateManager.TryGet(state.Id.GetValue(), out var currentState) &&
               currentState == state;
    }
}
