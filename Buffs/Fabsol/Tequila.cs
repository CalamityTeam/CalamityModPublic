using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Fabsol
{
    public class Tequila : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Tequila");
            Description.SetDefault("Damage, critical strike chance, damage reduction, defense, and knockback boosted during daytime, life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().tequila = true;
        }
    }
}
