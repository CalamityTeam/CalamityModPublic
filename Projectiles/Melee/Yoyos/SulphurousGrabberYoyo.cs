using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class SulphurousGrabberYoyo : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int bubbleCounter = 0;
        private bool bubbleStronk = false;
        private int bubbleStronkCounter = 0;
        private float arbitraryTimer = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f; // 32 effective, 48 bubbled

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            // Hit cooldown set in AI
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (bubbleStronk)
                {
                    Projectile.MaxUpdates = 3;
                    Projectile.localNPCHitCooldown = 12 * Projectile.MaxUpdates;
                    bubbleStronkCounter++;
                }
                else
                {
                    Projectile.MaxUpdates = 2;
                    Projectile.localNPCHitCooldown = 15 * Projectile.MaxUpdates;
                    bubbleStronkCounter = 0;
                }

                if (bubbleStronkCounter >= 180)
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
                    int bubbleAmt = 3;
                    for (float i = 0; i < bubbleAmt; i++)
                    {
                        int projType = ModContent.ProjectileType<SulphurousGrabberBubble>();
                        if (Main.rand.NextBool(8))
                            projType = ModContent.ProjectileType<SulphurousGrabberBubble2>();
                        float angle = MathHelper.TwoPi / bubbleAmt * i + (float)Math.Sin(arbitraryTimer / 20f) * MathHelper.PiOver2;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, angle.ToRotationVector2() * 10f, projType, Projectile.damage / 2, Projectile.knockBack / 4, Projectile.owner);
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
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            if (bubbleStronk)
            {
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SulphurousGrabberYoyoBubble").Value;
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, tex);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}
