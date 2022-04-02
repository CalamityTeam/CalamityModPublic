using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class AstralScytheProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/MantisRing";

        private int tileCounter = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Ring");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 8;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (tileCounter > 0)
                tileCounter--;
            if (tileCounter <= 0)
                projectile.tileCollide = true;

            projectile.velocity *= 1.03f;

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame > 2)
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item1, projectile.Center);

            for (int i = 0; i < 60; i++)
            {
                float angle = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                Vector2 angleVec = angle.ToRotationVector2();
                float distance = Main.rand.NextFloat(14f, 36f);
                Vector2 off = angleVec * distance;
                off.Y *= (float)projectile.height / projectile.width;
                Vector2 pos = projectile.Center + off;
                Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<AstralBlue>(), angleVec * Main.rand.NextFloat(2f, 4f));
                d.customData = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
