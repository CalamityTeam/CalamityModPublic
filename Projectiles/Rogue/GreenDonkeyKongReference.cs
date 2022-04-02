using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 480;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }
        public bool CollideX => projectile.oldPosition.X == projectile.position.X;
        public override void AI()
        {
            bool stealthS = projectile.Calamity().stealthStrike;
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[1] = stealthS.ToInt() == 0 ? 1 : 3;
                projectile.localAI[0] = 1f;
            }
            projectile.rotation += Math.Sign(projectile.velocity.X) * MathHelper.ToRadians(8f);
            if (projectile.velocity.Y < 15f)
            {
                projectile.velocity.Y += 0.3f;
            }
            if (CollideX && cooldown == 0)
            {
                BounceEffects();
                projectile.velocity.X = -oldVelocityX;
            }
            else if (cooldown > 0)
            {
                cooldown -= 1f;
            }
            if (projectile.velocity.X != 0f)
            {
                oldVelocityX = Math.Sign(projectile.velocity.X) * 12f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public void BounceEffects()
        {
            bool stealthS = projectile.Calamity().stealthStrike;
            int projectileCount = 8;
            Main.PlaySound(SoundID.Item14, projectile.Center);
            if (stealthS)
            {
                projectileCount += 5; //more shit the closer we are to death
            }
            for (int i = 0; i < projectileCount; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(30f));
                    Projectile.NewProjectile(projectile.Center, projectile.velocity + shrapnelVelocity,
                        ModContent.ProjectileType<AcidShrapnel>(), projectile.damage, 3f, projectile.owner);
                }
                else
                {
                    Vector2 acidVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int acidIndex = Projectile.NewProjectile(projectile.Center, projectile.velocity + acidVelocity,
                        ModContent.ProjectileType<AcidBarrelDrop>(),
                        (int)(projectile.damage * 0.75f), 1f, projectile.owner);
                    if (acidIndex.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[acidIndex].Calamity().forceRogue = true;
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
                    int acidIndex = Projectile.NewProjectile(projectile.Center, projectile.velocity + acidVelocity,
                        ModContent.ProjectileType<AcidBarrelDrop>(),
                        (int)(projectile.damage * 0.667f), 1f, projectile.owner);
                    if (acidIndex.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[acidIndex].Calamity().forceRogue = true;
                        Main.projectile[acidIndex].timeLeft = 420;
                    }
                }
            }

            projectile.ai[1]--;
            cooldown = 15;
            if (projectile.ai[1] <= 0)
            {
                projectile.Kill();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
