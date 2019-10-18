using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Projectiles.Summon;

namespace CalamityMod.Buffs.Summon
{
    public class MiniSlimeGodBuff : ModBuff
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
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CrimsonSlimeGodMinion>()] > 0)
            {
                modPlayer.sGod = true;
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<CorruptionSlimeGodMinion>()] > 0)
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
