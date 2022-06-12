using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class SeaSnailBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Snail");
            Description.SetDefault("Meow?");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();

            
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VictideSeaSnail>()] > 0)
            {
                modPlayer.victideSnail = true;
            }

            if (!modPlayer.victideSnail)
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
