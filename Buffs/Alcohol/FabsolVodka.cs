using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class FabsolVodka : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fab");
            Description.SetDefault("You feel fabulous");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().fabsolVodka = true;
        }
    }
}
