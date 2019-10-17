using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class DarkEnergySpawn : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spawn");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = 1;
            projectile.scale = 1f;
            projectile.penetrate = 1;
            projectile.timeLeft = 20;
            projectile.tileCollide = false;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.ai[1]++;

            if (projectile.ai[1] >= 0)
            {
                NPC.NewNPC((int)projectile.Center.X - 200, (int)projectile.Center.Y - 200, ModContent.NPCType<DarkEnergy>());
                NPC.NewNPC((int)projectile.Center.X + 200, (int)projectile.Center.Y - 200, ModContent.NPCType<DarkEnergy2>());
                NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y + 200, ModContent.NPCType<DarkEnergy3>());
                projectile.ai[1] = -30;
            }
        }
    }
}
