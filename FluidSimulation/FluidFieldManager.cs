using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.FluidSimulation
{
    public class FluidFieldManager : ModSystem
    {
        internal static List<FluidField> Fields = new();

        public static FluidField CreateField(int size, float scale, float viscosity, float diffusionFactor, float dissipationFactor)
        {
            var field = new FluidField(size, scale, viscosity, diffusionFactor, dissipationFactor);
            Fields.Add(field);
            return field;
        }

        public static void AdjustSizeRelativeToGraphicsQuality(ref int size, int min = 245, int max = 530)
        {
            float graphicsQuality = MathHelper.Clamp(Main.gfxQuality, 0f, 1f);

            // Use a quality of 0.5 for quality if it automatically updates. This is done to prevent having small changes reset the size and
            // require the creation of a new field altogether.
            if (Main.qaStyle == 0)
                graphicsQuality = 0.5f;
            graphicsQuality = (float)Math.Pow(graphicsQuality, 2.3);

            int lowerBound = (int)MathHelper.Lerp(size, min, 1f - graphicsQuality);
            int upperBound = (int)MathHelper.Lerp(size, max, graphicsQuality);
            size = (upperBound + lowerBound) / 2;
        }

        // Update logic SHOULD NOT be called manually in external parts.
        // A should update and update action field exist to allow for modularity with that.
        // The reason you should not update manually in arbitrary parts of code is because the update loop
        // involves heavy manipulation of render targets, which will fuck up the draw logic of the game
        // if not done at an appropriate point in time.
        public static void Update()
        {
            var old = Main.GameViewMatrix.Zoom;
            Main.GameViewMatrix.Zoom = Vector2.One;
            foreach (FluidField field in Fields)
                field.Update();
            Main.GameViewMatrix.Zoom = old;
        }

        public override void OnModUnload()
        {
            Main.QueueMainThreadAction(() =>
            {
                // The dispose method automatically pops fields from the list.
                while (Fields.Count > 0)
                    Fields[0].Dispose();
            });
        }
    }
}
