using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Projectiles.Summon;

namespace CalamityMod.Buffs.Summon
{
    public class BrimstoneWaifu : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Brimstone Elemental");
            Description.SetDefault("The brimstone elemental will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] > 0)
            {
                modPlayer.bWaifu = true;
            }
            if (!modPlayer.bWaifu)
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
