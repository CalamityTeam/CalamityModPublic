using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    // HUGE credit to Dozezoze for lending his worm projectile code
    public class BlackDragonHead : ModProjectile, ILocalizedModType
    {
        public string LocalizationCategory => "Projectiles.Summon";
        Dictionary<int, Projectile> segments = new Dictionary<int, Projectile>();
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.minionSlots = 2;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<KingofConstellationsBuff>(), 1);
            if (Projectile.type == ModContent.ProjectileType<BlackDragonHead>())
            {
                if (player.dead)
                {
                    player.Calamity().celestialDragons = false;
                }
                if (player.Calamity().celestialDragons)
                {
                    Projectile.timeLeft = 2;
                }
            }

            Projectile.ChargingMinionAI(2000f, 2500f, 3200f, 300f, 0, 45f, 36f, 8f, new Vector2(0f, -60f), 12f, 12f, true, true, 1);
            Projectile.rotation = Projectile.velocity.ToRotation();
            segments.Clear();
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<BlackDragonBody>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<BlackDragonBody>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<BlackDragonBody>().segmentIndex, projectile);
                }
                if (projectile.type == ModContent.ProjectileType<BlackDragonTail>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<BlackDragonTail>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<BlackDragonTail>().segmentIndex, projectile);
                }
            }
            for (var i = 1; i <= segments.Count; i++)
            {
                if (i < segments.Count)
                {
                    if (segments.ContainsKey(i))
                    segments[i].ModProjectile<BlackDragonBody>().SegmentMove();
                }
                else
                {
                    if (segments.ContainsKey(i))
                        segments[i].ModProjectile<BlackDragonTail>().SegmentMove();
                }
            }
            // Fly away from the white dragon
            float pushForce = 0.1f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || k == Projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == ModContent.ProjectileType<WhiteDragonHead>();
                float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                float distancegate = 80f;
                if (sameProjType && taxicabDist < distancegate)
                {
                    if (Projectile.position.X < otherProj.position.X)
                        Projectile.velocity.X -= pushForce;
                    else
                        Projectile.velocity.X += pushForce;

                    if (Projectile.position.Y < otherProj.position.Y)
                        Projectile.velocity.Y -= pushForce;
                    else
                        Projectile.velocity.Y += pushForce;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texBody = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/BlackDragonBody").Value;
            Texture2D texBody2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/BlackDragonBody2").Value;
            Texture2D texTail = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/BlackDragonTail").Value;
            Texture2D texTail2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/BlackDragonTail2").Value;
            for (var i = segments.Count; i > 0; i--)
            {
                if (segments.ContainsKey(i))
                {
                    SpriteEffects fx = Math.Abs(segments[i].rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    if (i < segments.Count - 1)
                    {
                        Main.EntitySpriteDraw((i == 5 || i == 16) ? texBody2 : texBody, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texBody.Size() / 2f, segments[i].scale, fx, 0);
                    }
                    else if (i < segments.Count)
                    {
                        Main.EntitySpriteDraw(texTail, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texBody.Size() / 2f, segments[i].scale, fx, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(texTail2, segments[i].Center - Main.screenPosition - new Vector2(10, 0).RotatedBy(segments[i].rotation), null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texTail.Size() / 2f, segments[i].scale, fx, 0);

                    }
                }
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi / 2f, tex.Size() / 2f, Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 300);
    }
}
