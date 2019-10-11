using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Fabsol
{
    public class Margarita : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Margarita");
            Description.SetDefault("Immunity to most debuffs, defense and life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().margarita = true;
        }
    }
}
