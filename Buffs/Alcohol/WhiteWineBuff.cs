using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Alcohol
{
    public class WhiteWineBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("White Wine");
            Description.SetDefault("Magic damage boosted, life regen and defense reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().whiteWine = true;
        }
    }
}
