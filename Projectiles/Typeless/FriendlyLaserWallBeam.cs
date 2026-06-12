using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class FriendlyLaserWallBeam : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectiles.Typeless";
        public float time = 0;
        public ref float attackSpeed => ref Projectile.ai[0];
        public ref float laserType => ref Projectile.ai[1];
        public bool canDamage => doneAttack && laserFX >= 1f;
        public bool doneAttack = false;
        public int attackTime = 30;
        public float laserLength => laserType == 0 ? 4000 : 2000;
        public float laserFX = 0;
        public float storedTime = 0;
        public Color drawColor = Color.Magenta;
        public float sine = 0;
        public float laserRot = 0;
        Vector2 beamStart = Vector2.Zero;
        Vector2 directionToTarget = Vector2.Zero;
        public Vector2 targetPos => Projectile.Center;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6000;
            Projectile.scale = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if (laserFX > 0)
                laserFX = MathHelper.Lerp(laserFX, 0, time > 15 ? 0.07f : 0.01f);
            sine = (float)Math.Sin(time * 4f / MathHelper.Pi);
            if (time == 0)
            {
                laserRot = Projectile.velocity.ToRotation();

                beamStart = targetPos + Vector2.UnitX.RotatedBy(laserRot) * laserLength;
                directionToTarget = beamStart.DirectionTo(targetPos);

                //Projectile.Center += Main.rand.NextVector2CircularEdge(400, 400);
                // Some default values for if the projectile spawns without them set
                if (attackSpeed == 0)
                {
                    attackSpeed = 3f;
                }
                //Negative speed causes instant attack with the set speed
                if (attackSpeed < 0)
                {
                    attackSpeed = -attackSpeed;
                    time = attackTime;
                }
                Projectile.velocity = Vector2.Zero;
                laserFX = 1f;
                Projectile.ForceNetUpdate();
            }
            if (time >= attackTime && !doneAttack)
            {
                SoundStyle attack = new("CalamityMod/Sounds/Custom/DoGLaserWallLightAttack");
                if (Projectile.scale > 3) {
                    attack = new("CalamityMod/Sounds/Custom/DoGLaserWallBigAttack");
                }
                SoundEngine.PlaySound(attack with { Volume = 0.4f, Pitch = 0, MaxInstances =  -1}, Vector2.Lerp(targetPos, Main.player[Projectile.owner].Center, laserType == 0 ? 0 : 0.7f));
                laserFX = 2.5f;
                doneAttack = true;
                storedTime = time;
                Projectile.ForceNetUpdate();
                if (Main.LocalPlayer.Distance(Projectile.Center) < 1600)
                    Main.LocalPlayer.SetScreenshake(Projectile.ai[2]);
            }
            float endTime = storedTime + 10;
            if (time >= endTime && doneAttack)
            {
                Projectile.Kill();
                return;
            }
            else if (doneAttack)
            {
                drawColor = Color.Lerp(Color.Magenta, Color.Cyan, (float)Math.Pow(Utils.GetLerpValue(endTime, storedTime, time, true), 2));
            }
            time += attackSpeed;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (canDamage)
                return null;
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            if (canDamage)
                return true;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!canDamage)
                return false;
            else
            {
                float _ = float.NaN;
                Vector2 start = beamStart;
                Vector2 end = beamStart + directionToTarget * laserLength * 2;
                bool hitCheck = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 30 * Projectile.scale, ref _);

                return hitCheck;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.CountsAsClass(DamageClass.Ranged)) //Thread of Eradication
                modifiers.ApplyScalingForcedCrit(Projectile);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (laserFX == 0)
                return false;
            Texture2D beam = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            Texture2D bBeam = ModContent.Request<Texture2D>("CalamityMod/Particles/LineThick").Value;
            Texture2D angleBeam = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowBlade").Value;
            Texture2D angleBeamInside = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowBladeNoBloom").Value;
            float opacity = (doneAttack ? 0.65f : 0.35f) * (float)Math.Pow(Math.Min(laserFX, 1), 2);
            Color beamColor = drawColor with { A = 0 };

            if (CalamityClientConfig.Instance.Photosensitivity)
                opacity = 0.2f;
            if (laserType == 0)
            {
                for (int t = 0; t < (!doneAttack ? 1 : 5); t++)
                {
                    bool black = (t > 0);
                    Texture2D usedTex = (black ? bBeam : beam);
                    float beamThickness = 0.03f * (black ? (0.8f - 0.15f * t) : 1f) * (laserFX <= 1 ? (float)Math.Pow(Math.Min(laserFX, 1), 2) : laserFX) * Utils.Remap(sine, -1, 1, 0.8f, 1.1f);
                    float rot = beamStart.DirectionTo(targetPos).ToRotation() + (MathHelper.PiOver2);
                    Main.EntitySpriteDraw(usedTex, beamStart - Main.screenPosition, null, (black ? Color.Black * opacity : beamColor * opacity) * (black ? (0.2f + 0.15f * t) : 1), rot, new Vector2(beam.Width / 2, beam.Height), new Vector2(beamThickness * Projectile.scale, laserLength / 975 * (usedTex == beam ? 1 : 0.8277f)), SpriteEffects.None);
                }
            }
            else
            {
                opacity = 1f;

                if (CalamityClientConfig.Instance.Photosensitivity)
                    opacity = 0.2f;
                for (int t = 0; t < (!doneAttack ? 1 : 5); t++)
                {
                    bool notFirstDrawn = (t > (Projectile.scale > 3 ? 1 : 0));
                    float beamThickness = 16/1960f * Projectile.scale * (notFirstDrawn ? (1f - 0.8f * (t-1) / 4f) : 1) * (laserFX <= 1 ? (float)Math.Pow(Math.Min(laserFX, 1), 2) : laserFX) * Utils.Remap(sine, -1, 1, 0.8f, 1.1f);
                    Main.EntitySpriteDraw(notFirstDrawn ? angleBeamInside : angleBeam, beamStart - Main.screenPosition, null, (notFirstDrawn ? Color.Black * opacity : beamColor * opacity) * (notFirstDrawn ? (0.2f + 0.8f * (t - 1) / 4f) : 1), directionToTarget.ToRotation() + MathHelper.PiOver2, new Vector2(angleBeam.Width / 2, angleBeam.Height), new Vector2(beamThickness * Projectile.scale, laserLength / 975 * 0.8277f), SpriteEffects.None);
                }
            }
            return false;
        }
    }
}
