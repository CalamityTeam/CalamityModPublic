using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summons
{
    public class DaedalusCrystal : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Daedalus Crystal");
            Description.SetDefault("The daedalus crystal will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.DaedalusCrystal>()] > 0)
            {
                modPlayer.dCrystal = true;
            }
            if (!modPlayer.dCrystal)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
