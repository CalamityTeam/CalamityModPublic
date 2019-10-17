using CalamityMod.CalPlayer;
using Terraria;
using CalamityMod.Projectiles;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class Mechworm : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            Description.SetDefault("The mechworm will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MechwormHead>()] > 0)
            {
                modPlayer.mWorm = true;
            }
            if (!modPlayer.mWorm)
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
