using CalamityMod.CalPlayer;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class CosmicEnergy : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cosmic Energy");
            Description.SetDefault("The cosmic energy will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CosmicEnergySpiral>()] > 0)
            {
                modPlayer.cEnergy = true;
            }
            if (!modPlayer.cEnergy)
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
