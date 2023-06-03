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
            Projectile.minion = true;

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
            Projectile.ai[0]++;
            // Hover to the left because evil is not right or something
            Vector2 idealPos = new Vector2(player.Center.X - 200, player.Center.Y - 50);
            float distanceFromOwner = Projectile.Distance(idealPos);

            NPC target = CalamityUtils.MinionHoming(Projectile.position, 2500, player, true, true);
            if (target != null && Projectile.position.Distance(player.position) <= 2500)
            {
                AttackTarget(target);
            }
            else
            {
                float hoverAcceleration = 0.2f;
                if (distanceFromOwner < 200f)
                    hoverAcceleration = 0.12f;
                if (distanceFromOwner < 140f)
                    hoverAcceleration = 0.06f;

                if (distanceFromOwner > 100f)
                {
                    if (Math.Abs(player.Center.X - Projectile.Center.X) > 20f)
                        Projectile.velocity.X += hoverAcceleration * Math.Sign(idealPos.X - Projectile.Center.X);
                    if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f)
                        Projectile.velocity.Y += hoverAcceleration * Math.Sign(idealPos.Y - Projectile.Center.Y);
                }
                else if (Projectile.velocity.Length() > 1f)
                    Projectile.velocity *= 0.96f;

                if (Math.Abs(Projectile.velocity.Y) < 1f)
                    Projectile.velocity.Y -= 0.1f;

                if (Projectile.velocity.Length() > 25)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 25;
            }
            if (distanceFromOwner > 5000)
            {
                Projectile.Center = player.Center;
            }

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
        }
        internal void AttackTarget(NPC target)
        {
            float idealFlyAcceleration = 0.18f;

            Vector2 destination = target.Center;
            float distanceFromDestination = Projectile.Distance(destination);

            // Get a swerve effect if somewhat far from the target.
            if (Projectile.Distance(destination) > 425f)
            {
                destination += (Projectile.ai[0] % 30f / 30f * MathHelper.TwoPi).ToRotationVector2() * 145f;
                distanceFromDestination = Projectile.Distance(destination);
                idealFlyAcceleration *= 2.5f;
            }

            // Charge if the target is far away.
            if (distanceFromDestination > 1500f)
                idealFlyAcceleration = MathHelper.Min(6f, Projectile.ai[1] + 1f);

            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], idealFlyAcceleration, 0.3f);

            float directionToTargetOrthogonality = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Projectile.SafeDirectionTo(destination));

            // Fly towards the target if it's far.
            if (distanceFromDestination > 320f)
            {
                float speed = Projectile.velocity.Length();
                if (speed < 23f)
                    speed += 0.08f;

                if (speed > 32f)
                    speed -= 0.08f;

                // Go faster if the line of sight is aiming closely at the target.
                if (directionToTargetOrthogonality < 0.85f && directionToTargetOrthogonality > 0.5f)
                    speed += 16f;

                // And go slower otherwise so that the dragon can angle towards the target more accurately.
                if (directionToTargetOrthogonality < 0.5f && directionToTargetOrthogonality > -0.7f)
                    speed -= 16f;

                speed = MathHelper.Clamp(speed, 16f, 34f);

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(destination), Projectile.ai[1]).ToRotationVector2() * speed;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
                        Main.EntitySpriteDraw((i == 5 || i == 12) ? texBody2 : texBody, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texBody.Size() / 2f, segments[i].scale, fx, 0);
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
