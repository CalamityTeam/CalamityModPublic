using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class ResilienceCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Resilience Cooldown");
            Description.SetDefault("You cannot re-summon a relic of resilience");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
