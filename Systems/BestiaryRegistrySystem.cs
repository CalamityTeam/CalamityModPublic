using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.Astral;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Systems
{
    public class BestiaryRegistrySystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // Manually register variants post-initiailization
            ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<AstralachneaGround>()] = ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<AstralachneaWall>()];
            ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<DevilFishAlt>()] = ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<DevilFish>()];
        }
    }
}
