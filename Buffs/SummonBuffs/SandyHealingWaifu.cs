using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class SandyHealingWaifu : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Rare Sand Elemental");
            Description.SetDefault("The sand elemental will heal you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] > 0)
            {
                modPlayer.dWaifu = true;
            }
            if (!modPlayer.dWaifu)
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
