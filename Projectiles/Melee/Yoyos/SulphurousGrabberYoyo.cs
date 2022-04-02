using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class SulphurousGrabberYoyo : ModProjectile
    {
        private int bubbleCounter = 0;
        private bool bubbleStronk = false;
        private int bubbleStronkCounter = 0;
        private float arbitraryTimer = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Grabber Yoyo");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 350f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 16f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 18;
            projectile.height = 18;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.owner == Main.myPlayer)
            {
                if (bubbleStronk)
                {
                    ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 20f;
                    projectile.extraUpdates = 2;
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = 10 * projectile.extraUpdates;
                    bubbleStronkCounter++;
                }
                else
                {
                    ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 16f;
                    projectile.extraUpdates = 1;
                    projectile.usesLocalNPCImmunity = false;
                    bubbleStronkCounter = 0;
                }

                if (bubbleStronkCounter >= 240)
                    bubbleStronk = false;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == ModContent.ProjectileType<SulphurousGrabberBubble2>() && proj.ai[0] >= 40f && proj.owner == projectile.owner)
                    {
                        if (projectile.Hitbox.Intersects(proj.Hitbox))
                        {
                            proj.Kill();
                            bubbleStronk = true;
                            bubbleStronkCounter = 0;
                            break;
                        }
                    }
                }

                arbitraryTimer += bubbleStronk ? 0.5f : 1f;

                bubbleCounter++;
                if (bubbleCounter >= 60)
                {
                    int bubbleAmt = 7;
                    for (float i = 0; i < bubbleAmt; i++)
                    {
                        int projType = ModContent.ProjectileType<SulphurousGrabberBubble>();
                        if (Main.rand.NextBool(10))
                            projType = ModContent.ProjectileType<SulphurousGrabberBubble2>();
                        float angle = MathHelper.TwoPi / bubbleAmt * i + (float)Math.Sin(arbitraryTimer / 20f) * MathHelper.PiOver2;
                        Projectile.NewProjectile(projectile.Center, angle.ToRotationVector2() * 8f, projType, projectile.damage / 4, projectile.knockBack / 4, projectile.owner);
                    }
                    bubbleCounter = 0;
                }
            }

            if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f) //200 blocks
                projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            if (bubbleStronk)
            {
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Melee/Yoyos/SulphurousGrabberYoyoBubble");
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1, tex);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
