using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CrescentMoonProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/Crescent";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crescent Moon");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = 5;
            projectile.timeLeft = 180;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 18;
            aiType = ProjectileID.DeathSickle;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 0.6f);
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 30 + Main.rand.Next(50);
                if (Main.rand.NextBool(10))
                {
                    Main.PlaySound(SoundID.Item9, (int)projectile.position.X, (int)projectile.position.Y);
                }
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, 12f, 20f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
        }
    }
}
