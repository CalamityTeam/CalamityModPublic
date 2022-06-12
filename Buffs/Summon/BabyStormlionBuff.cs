using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class BabyStormlionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Stormlion");
            Description.SetDefault("The baby stormlion will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<StormjawBaby>()] > 0)
            {
                modPlayer.stormjaw = true;
            }
            if (!modPlayer.stormjaw)
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
