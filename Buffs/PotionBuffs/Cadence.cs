using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.PotionBuffs
{
    public class Cadence : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cadance");
            Description.SetDefault("Your heart is pure");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().cadence = true;
        }
    }
}
