using System.Collections.Generic;

namespace CalamityMod.FluidSimulation
{
    public static class FluidFieldManager
    {
        internal static List<FluidField> Fields = new();

        public static FluidField CreateField(int size, float viscosity, float diffusionFactor, float dissipationFactor)
        {
            var field = new FluidField(size, viscosity, diffusionFactor, dissipationFactor);
            Fields.Add(field);
            return field;
        }

        public static void DestroyField(ref FluidField field)
        {
            Fields.Remove(field);
            field.Dispose();
            field = null;
        }

        // Update logic SHOULD NOT be called manually in external parts.
        // A should update and update action field exist to allow for modularity with that.
        // The reason you should not update manually in arbitrary parts of code is because the update loop
        // involves heavy manipulation of render targets, which will fuck up the draw logic of the game
        // if not done at an appropriate point in time.
        public static void Update()
        {
            foreach (FluidField field in Fields)
                field.Update();
        }
    }
}
