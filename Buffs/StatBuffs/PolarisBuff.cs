using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class PolarisBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Polaris Boost");
            Description.SetDefault("The Northern Star empowers your weapon");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().polarisBoost = true;
        }
    }
}
