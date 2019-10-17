using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Calamari : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Calamari");
            Description.SetDefault("The squid will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Calamari>()] > 0)
            {
                modPlayer.calamari = true;
            }
            if (!modPlayer.calamari)
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
