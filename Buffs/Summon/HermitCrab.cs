using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Projectiles.Summon;

namespace CalamityMod.Buffs.Summon
{
    public class HermitCrab : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Hermit Crab");
            Description.SetDefault("The hermit crab will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<HermitCrabMinion>()] > 0)
            {
                modPlayer.hCrab = true;
            }

            if (!modPlayer.hCrab)
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
