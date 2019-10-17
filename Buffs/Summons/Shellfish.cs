using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class ShellfishBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shellfish");
            Description.SetDefault("The shellfish will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Shellfish>()] > 0)
            {
                modPlayer.shellfish = true;
            }
            if (!modPlayer.shellfish)
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
