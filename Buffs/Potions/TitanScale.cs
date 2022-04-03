using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class TitanScale : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Scale");
            Description.SetDefault("You feel tanky");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().tScale = true;
        }
    }
}
