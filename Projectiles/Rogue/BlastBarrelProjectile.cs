using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class BlastBarrelProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BlastBarrel";

        public float BounceEffectCooldown = 0f;
        public float OldVelocityX = 0f;
        public float RemainingBounces
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public bool CollideX => Projectile.oldPosition.X == Projectile.position.X;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barrel");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                RemainingBounces = Projectile.Calamity().stealthStrike ? 3 : 1;
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation += Math.Sign(Projectile.velocity.X) * MathHelper.ToRadians(8f);
            if (Projectile.velocity.Y < 10f)
                Projectile.velocity.Y += 0.2f;

            if (CollideX && BounceEffectCooldown == 0)
            {
                BounceEffects();
                Projectile.velocity.X = -OldVelocityX;
            }
            else if (BounceEffectCooldown > 0)
                BounceEffectCooldown--;

            if (Projectile.velocity.X != 0f)
                OldVelocityX = Math.Sign(Projectile.velocity.X) * 12f;
        }
        public void BounceEffects()
        {
            int projectileCount = 12;
            if (Projectile.Calamity().stealthStrike)
            {
                projectileCount += 4; // More shit the closer we are to death
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 shrapnelVelocity = (Vector2.UnitY * Main.rand.NextFloat(-19f, -4f)).RotatedByRandom(MathHelper.ToRadians(30f));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity + shrapnelVelocity, ModContent.ProjectileType<BarrelShrapnel>(), Projectile.damage, 3f, Projectile.owner);
                    }
                    else
                    {
                        Vector2 fireVelocity = (Vector2.UnitY * Main.rand.NextFloat(-19f, -4f)).RotatedByRandom(MathHelper.ToRadians(40f));
                        Projectile fire = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity + fireVelocity, ModContent.ProjectileType<TotalityFire>(), (int)(Projectile.damage * 0.75f), 1f, Projectile.owner);
                        fire.timeLeft = 300;
                        fire.penetrate = 3;
                    }
                }
            }
            RemainingBounces--;
            BounceEffectCooldown = 15;
            if (RemainingBounces <= 0)
            {
                Projectile.Kill();
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
