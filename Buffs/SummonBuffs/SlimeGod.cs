using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class SlimeGod : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Baby Slime God");
            Description.SetDefault("The slime god will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SlimeGodAlt>()] > 0)
            {
                modPlayer.sGod = true;
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<SlimeGod>()] > 0)
            {
                modPlayer.sGod = true;
            }
            if (!modPlayer.sGod)
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
