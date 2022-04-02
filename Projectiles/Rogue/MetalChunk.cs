using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Rogue
{
    public class MetalChunk : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MetalMonstrosity";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Metal Chunk"); //Metal Chungus - Shucks
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true; //Its hella heavy so ofc
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            //Gravity
            projectile.velocity.Y += 0.11f;
            if (projectile.velocity.Y > 16f)
                projectile.velocity.Y = 16f;
            //Rotation
            projectile.rotation += 0.14f * projectile.direction;
            if (projectile.Calamity().stealthStrike)
            {
                projectile.ai[0]++;
                if(projectile.ai[0] >= 10f)
                {
                    Vector2 speed = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f));
                    Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<MetalShard>(), (int)(projectile.damage * 0.3f), 0f, projectile.owner, 0f, 0f);
                    projectile.ai[0] = 0f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit42, projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Vector2 S1 = new Vector2(-projectile.velocity.X, -projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-45, 46)));
                int proj = Projectile.NewProjectile(projectile.Center, S1, ProjectileID.SpikyBall, (int)(projectile.damage * 0.3), 0f, projectile.owner, 0f, 0f);
                Main.projectile[proj].timeLeft = 600;
                Main.projectile[proj].usesLocalNPCImmunity = true;
                Main.projectile[proj].localNPCHitCooldown = 20;
                S1 = new Vector2(-projectile.velocity.X * 0.7f, -projectile.velocity.Y*0.7f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-45, 46)));
                Projectile.NewProjectile(projectile.Center, S1, ModContent.ProjectileType<MetalShard>(), (int)(projectile.damage * 0.3), 0f, projectile.owner, 0f, 0f);
            }
            //Dust
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(projectile.Center, 1, 1, DustID.Lead, 0f, 0f, 0, default, 1.1f);
            }
        }
    }
}
