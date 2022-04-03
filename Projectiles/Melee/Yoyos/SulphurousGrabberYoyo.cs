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
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 350f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 99;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (bubbleStronk)
                {
                    ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20f;
                    Projectile.extraUpdates = 2;
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = 10 * Projectile.extraUpdates;
                    bubbleStronkCounter++;
                }
                else
                {
                    ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f;
                    Projectile.extraUpdates = 1;
                    Projectile.usesLocalNPCImmunity = false;
                    bubbleStronkCounter = 0;
                }

                if (bubbleStronkCounter >= 240)
                    bubbleStronk = false;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == ModContent.ProjectileType<SulphurousGrabberBubble2>() && proj.ai[0] >= 40f && proj.owner == Projectile.owner)
                    {
                        if (Projectile.Hitbox.Intersects(proj.Hitbox))
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
                        Projectile.NewProjectile(Projectile.Center, angle.ToRotationVector2() * 8f, projType, Projectile.damage / 4, Projectile.knockBack / 4, Projectile.owner);
                    }
                    bubbleCounter = 0;
                }
            }

            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            if (bubbleStronk)
            {
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SulphurousGrabberYoyoBubble");
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, tex);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
    }
}
