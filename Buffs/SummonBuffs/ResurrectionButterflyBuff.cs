using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class ResurrectionButterflyBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Resurrection Butterfly");
            Description.SetDefault("Sleep beneath the Cherry Blossoms, Red-White Butterfly");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PinkButterfly>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<PurpleButterfly>()] > 0)
            {
                modPlayer.resButterfly = true;
            }
            if (!modPlayer.resButterfly)
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
