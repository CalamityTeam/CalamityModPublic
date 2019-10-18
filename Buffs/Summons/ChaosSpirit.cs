using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summons
{
    public class ChaosSpirit : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Chaos Spirit");
            Description.SetDefault("The chaos spirit will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.ChaosSpirit>()] > 0)
            {
                modPlayer.cSpirit = true;
            }
            if (!modPlayer.cSpirit)
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
