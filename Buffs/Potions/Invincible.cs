using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class Invincible : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Invincible");
            Description.SetDefault("Immune to damage and most debuffs");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().invincible = true;
        }
    }
}
