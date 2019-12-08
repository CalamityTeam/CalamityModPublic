using CalamityMod.Items.Weapons.Rogue;
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
            projectile.penetrate = -1;
            projectile.timeLeft = 480;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }
        //Jesus christ, why isn't this in the Entity class instead of just NPC???
        //Negative check is so that it doesn't register a bounce as a collision
        public bool collideX => projectile.oldPosition.X == projectile.position.X;
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
            if (collideX && cooldown == 0)
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
            int projectileCount = 12;
            Main.PlaySound(2, projectile.Center, 14);
            //aka can bounce multiple times
            if (stealthS)
            {
                projectileCount += (3 - stealthS.ToInt()) * 2; //more shit the closer we are to death
            }
            for (int i = 0; i < projectileCount; i++)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 shrapnelVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(30f));
                    Projectile.NewProjectile(projectile.Center, projectile.velocity + shrapnelVelocity,
                        ModContent.ProjectileType<BarrelShrapnel>(), projectile.damage, 3f, projectile.owner);
                }
                else
                {
                    Vector2 fireVelocity = (Vector2.UnitY * (-16f + Main.rand.NextFloat(-3, 12f))).RotatedByRandom((double)MathHelper.ToRadians(40f));
                    int fireIndex = Projectile.NewProjectile(projectile.Center, projectile.velocity + fireVelocity,
                        Main.rand.Next(ProjectileID.MolotovFire, ProjectileID.MolotovFire3 + 1),
                        projectile.damage, 1f, projectile.owner);
                    Main.projectile[fireIndex].Calamity().forceRogue = true;
                    Main.projectile[fireIndex].penetrate = -1;
                    Main.projectile[fireIndex].usesLocalNPCImmunity = true;
                    Main.projectile[fireIndex].localNPCHitCooldown = -1;
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
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
