using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Invincible : ModBuff
    {
        public override void SetDefaults()
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
