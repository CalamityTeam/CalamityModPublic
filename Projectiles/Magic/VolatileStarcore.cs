using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VolatileStarcore : ModProjectile
    {
        private static int Lifetime = 240;
        private static int NumAnimationFrames = 6;
        private static int AnimationFrameTime = 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volatile Starcore");
            Main.projFrames[projectile.type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 2;
            projectile.timeLeft = Lifetime;
            projectile.alpha = 48;
        }

        public override void AI()
        {
            // Draw offsets
            drawOffsetX = -10;
            drawOriginOffsetY = -10;
            drawOriginOffsetX = 0;

            // Play sound and set rotation on frame 1
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.NPCDeath56, projectile.Center);
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            }

            // Dust only shows up after the first few frames
            if (projectile.localAI[0] >= 5f)
                SpawnDust();

            // Lighting and spin
            Lighting.AddLight(projectile.Center, 1.8f, 1.6f, 0.5f);
            projectile.rotation += 0.11f;

            // Increment frame counter
            projectile.localAI[0] += 1f;

            // Update animation
            projectile.frameCounter++;
            if (projectile.frameCounter > AnimationFrameTime)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= NumAnimationFrames)
                projectile.frame = 0;
        }

        private void SpawnDust()
        {
            int coreDustCount = 2; //3
            int coreDustType = 262;
            for (int i = 0; i < coreDustCount; ++i)
            {
                float scale = Main.rand.NextFloat(1.0f, 1.4f);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, coreDustType);
                Main.dust[idx].velocity *= 0.7f;
                Main.dust[idx].velocity += projectile.velocity * 1.4f;
                Main.dust[idx].scale = scale;
                Main.dust[idx].noGravity = true;
            }

            int trailDustCount = 4; //5
            int trailDustType = 264;
            for (int i = 0; i < trailDustCount; ++i)
            {
                float scale = Main.rand.NextFloat(1.0f, 1.4f);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, trailDustType, 0f, 0f);
                Main.dust[idx].velocity = projectile.velocity * 0.8f;
                Main.dust[idx].scale = scale;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 900);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 900);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 900);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 900);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner != Main.myPlayer)
                return;

            // Spawn a Helium Flash on impact
            int type = ModContent.ProjectileType<HeliumFlashBlast>();
            int damage = (int)(HeliumFlash.ExplosionDamageMultiplier * projectile.damage);
            float kb = 9.5f;
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, type, damage, kb, projectile.owner, 0f, 0f);
        }
    }
}
