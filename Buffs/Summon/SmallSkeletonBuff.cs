using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class SmallSkeletonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Skeleton");
            Description.SetDefault("A skeleton is observing you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SmallSkeletonMinion>()] > 0)
            {
                modPlayer.necrosteocytesDudes = true;
            }
            if (!modPlayer.necrosteocytesDudes)
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
