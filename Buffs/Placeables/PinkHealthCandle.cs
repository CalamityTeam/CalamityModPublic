using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Placeables
{
    public class PinkHealthCandle : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vigor");
            Description.SetDefault("Its brilliant light suffuses those nearby with hope");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().pinkCandle = true;
        }
    }
}
