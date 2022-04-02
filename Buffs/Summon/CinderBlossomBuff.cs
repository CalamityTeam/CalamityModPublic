using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class CinderBlossomBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom");
            Description.SetDefault("A hot, searing flower is floating uncomfortably close to you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CinderBlossom>()] > 0)
            {
                modPlayer.cinderBlossom = true;
            }
            if (!modPlayer.cinderBlossom)
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
