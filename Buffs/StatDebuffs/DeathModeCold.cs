using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class DeathModeCold : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Freezing Weather");
            Description.SetDefault("The weather slows your movement as you freeze to death");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }
    }
}
