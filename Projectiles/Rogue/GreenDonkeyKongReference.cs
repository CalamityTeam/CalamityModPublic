using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class GreenDonkeyKongReference : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AcidicRainBarrel";

        public float cooldown = 0f;
        public float oldVelocityX = 0f;

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
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public bool CollideX => Projectile.oldPosition.X == Projectile.position.X;

        public override void AI()
        {
            bool stealthS = Projectile.Calamity().stealthStrike;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] = stealthS.ToInt() == 0 ? 1 : 3;
                Projectile.localAI[0] = 1f;
            }
            Projectile.rotation += Math.Sign(Projectile.velocity.X) * MathHelper.ToRadians(8f);
            if (Projectile.velocity.Y < 15f)
            {
                Projectile.velocity.Y += 0.3f;
            }
            if (CollideX && cooldown == 0)
            {
                BounceEffects();
                Projectile.velocity.X = -oldVelocityX;
            }
            else if (cooldown > 0)
            {
                cooldown -= 1f;
            }
            if (Projectile.velocity.X != 0f)
            {
                oldVelocityX = Math.Sign(Projectile.velocity.X) * 12f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public void BounceEffects()
        {
            bool stealthS = Projectile.Calamity().stealthStrike;
            int projectileCount = 8;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (stealthS)
            {
                projectileCount += 5; //more shit the closer we are to death
            }
            for (int i = 0; i < projectileCount; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(30f));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity + shrapnelVelocity,
                        ModContent.ProjectileType<AcidShrapnel>(), Projectile.damage, 3f, Projectile.owner);
                }
                else
                {
                    Vector2 acidVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int acidIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity + acidVelocity,
                        ModContent.ProjectileType<AcidBarrelDrop>(),
                        (int)(Projectile.damage * 0.75f), 1f, Projectile.owner);
                    if (acidIndex.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[acidIndex].DamageType = RogueDamageClass.Instance;
                        Main.projectile[acidIndex].timeLeft = 300;
                        Main.projectile[acidIndex].usesLocalNPCImmunity = true;
                        Main.projectile[acidIndex].localNPCHitCooldown = -1;
                    }
                }
            }

            if (stealthS)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 acidVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int acidIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity + acidVelocity,
                        ModContent.ProjectileType<AcidBarrelDrop>(),
                        (int)(Projectile.damage * 0.667f), 1f, Projectile.owner);
                    if (acidIndex.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[acidIndex].DamageType = RogueDamageClass.Instance;
                        Main.projectile[acidIndex].timeLeft = 420;
                    }
                }
            }

            Projectile.ai[1]--;
            cooldown = 15;
            if (Projectile.ai[1] <= 0)
            {
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
