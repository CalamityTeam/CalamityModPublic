using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summons
{
    public class SilvaCrystal : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Silva Crystal");
            Description.SetDefault("The crystal will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.SilvaCrystal>()] > 0)
            {
                modPlayer.sCrystal = true;
            }
            if (!modPlayer.sCrystal)
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
