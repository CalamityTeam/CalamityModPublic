using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    // TODO -- Expand this ModSystem to contain the Speedrun Timer itself, as well as the UI.
    public class SpeedrunTimerStoppingSystem : ModSystem
    {
        public override void PreSaveAndQuit() => CalamityMod.SpeedrunTimer?.Stop();
    }
}
