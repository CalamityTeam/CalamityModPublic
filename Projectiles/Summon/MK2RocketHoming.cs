using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MK2RocketHoming : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rocket");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            NPC potentialTarget = projectile.Center.MinionHoming(1000f, player);

            if (potentialTarget != null)
                projectile.velocity = (projectile.velocity * 24f + projectile.SafeDirectionTo(potentialTarget.Center) * 14f) / 25f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 599)
                return false;
            return true;
        }
    }
}
