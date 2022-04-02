using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BlastBarrelProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BlastBarrel";

        public float BounceEffectCooldown = 0f;
        public float OldVelocityX = 0f;
        public float RemainingBounces
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public bool CollideX => projectile.oldPosition.X == projectile.position.X;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barrel");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.timeLeft = 480;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                RemainingBounces = projectile.Calamity().stealthStrike ? 3 : 1;
                projectile.localAI[0] = 1f;
            }
            projectile.rotation += Math.Sign(projectile.velocity.X) * MathHelper.ToRadians(8f);
            if (projectile.velocity.Y < 10f)
                projectile.velocity.Y += 0.2f;

            if (CollideX && BounceEffectCooldown == 0)
            {
                BounceEffects();
                projectile.velocity.X = -OldVelocityX;
            }
            else if (BounceEffectCooldown > 0)
                BounceEffectCooldown--;

            if (projectile.velocity.X != 0f)
                OldVelocityX = Math.Sign(projectile.velocity.X) * 12f;
        }
        public void BounceEffects()
        {
            int projectileCount = 12;
            if (projectile.Calamity().stealthStrike)
            {
                projectileCount += 4; // More shit the closer we are to death
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 shrapnelVelocity = (Vector2.UnitY * Main.rand.NextFloat(-19f, -4f)).RotatedByRandom(MathHelper.ToRadians(30f));
                        Projectile.NewProjectile(projectile.Center, projectile.velocity + shrapnelVelocity, ModContent.ProjectileType<BarrelShrapnel>(), projectile.damage, 3f, projectile.owner);
                    }
                    else
                    {
                        Vector2 fireVelocity = (Vector2.UnitY * Main.rand.NextFloat(-19f, -4f)).RotatedByRandom(MathHelper.ToRadians(40f));
                        Projectile fire = Projectile.NewProjectileDirect(projectile.Center, projectile.velocity + fireVelocity, ModContent.ProjectileType<TotalityFire>(), (int)(projectile.damage * 0.75f), 1f, projectile.owner);
                        fire.timeLeft = 300;
                        fire.penetrate = 3;
                    }
                }
            }
            RemainingBounces--;
            BounceEffectCooldown = 15;
            if (RemainingBounces <= 0)
            {
                projectile.Kill();
            }
            Main.PlaySound(SoundID.Item14, projectile.Center);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
