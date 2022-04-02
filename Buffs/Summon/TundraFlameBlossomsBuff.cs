using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class TundraFlameBlossomsBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Tundra Flame Blossom");
            Description.SetDefault("A perfect unison of balance and beauty");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TundraFlameBlossom>()] > 0)
            {
                modPlayer.tundraFlameBlossom = true;
            }
            if (!modPlayer.tundraFlameBlossom)
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
