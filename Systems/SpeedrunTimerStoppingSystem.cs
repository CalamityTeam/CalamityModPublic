using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SpeedrunTimerStoppingSystem : ModSystem
    {
        public override void PreSaveAndQuit() => CalamityMod.SpeedrunTimer?.Stop();
    }
}
