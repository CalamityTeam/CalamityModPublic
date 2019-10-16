using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class FungalClumpBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fungal Clump");
            Description.SetDefault("The fungal clump will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] > 0)
            {
                modPlayer.fClump = true;
            }
            if (!modPlayer.fClump)
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
