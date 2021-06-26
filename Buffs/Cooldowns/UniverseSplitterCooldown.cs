using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class UniverseSplitterCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Universe Splitter Cooldown");
            Description.SetDefault("You cannot use the Universe Splitter");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
    }
}
