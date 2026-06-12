using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Player;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistException]
    public class LeviathanToothStealth : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/LeviathanTooth";
        public new string LocalizationCategory => "Projectiles.Rogue";
        public ref float time => ref Projectile.ai[0];

        public bool canDamage = false;
        public bool isTopJaw = false;
        public float topJawRot = 0;
        public float bottomJawRot = 0;
        public int jawSlamTime = 25;
        public bool spawnedDust = false;
        public int jawLength = 750;
        public int hitTimer = 0;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.ArmorPenetration = 25;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void AI()
        {
            if (hitTimer > 0)
                hitTimer--;
            if (time == 0)
            {
                topJawRot = -MathHelper.PiOver4;
                bottomJawRot = MathHelper.PiOver4;
            }
            topJawRot *= 0.95f;
            bottomJawRot *= 0.95f;
            float openingRot = 0.5f;
            if (time <= 10)
            {
                float lerpValue = 1 - (float)Math.Pow(Utils.GetLerpValue(10, 0, time, true), 2);
                topJawRot = MathHelper.Lerp(-MathHelper.PiOver4, -MathHelper.PiOver4 - openingRot, lerpValue);
                bottomJawRot = MathHelper.Lerp(MathHelper.PiOver4, MathHelper.PiOver4 + openingRot, lerpValue);
            }
            else
            {
                float lerpValue = (float)Math.Pow(Utils.GetLerpValue(11, jawSlamTime, time, true), 4);
                topJawRot = MathHelper.Lerp(-MathHelper.PiOver4 - openingRot, 0, lerpValue);
                bottomJawRot = MathHelper.Lerp(MathHelper.PiOver4 + openingRot, 0, lerpValue);
            }

            if (time < jawSlamTime * 1.5f)
            {
                Owner.direction = Projectile.direction;
                if (time < jawSlamTime)
                {
                    Projectile.Center = Owner.Center;
                    Projectile.velocity = Owner.Center.DirectionTo(Owner.Calamity().mouseWorld) * 0.01f;
                }
                Owner.itemTime = Owner.itemAnimation = 2;
                Vector2 toMouse = Owner.Center.DirectionTo(Owner.Calamity().mouseWorld);
                Owner.SetCompositeArmFront(true, CompositeArmStretchAmount.Full, toMouse.ToRotation() + topJawRot * Owner.direction + MathHelper.PiOver2 * -Owner.direction + (Owner.direction == -1 ? MathHelper.Pi : 0));
                Owner.SetCompositeArmBack(true, CompositeArmStretchAmount.Full, toMouse.ToRotation() + bottomJawRot * Owner.direction + MathHelper.PiOver2 * -Owner.direction + (Owner.direction == -1 ? MathHelper.Pi : 0));
            }
            if (time == jawSlamTime)
            {
                Owner.SetScreenshake(5f);
                Owner.Calamity().ConsumeStealthByAttacking();
                SoundStyle crunch = new("CalamityMod/Sounds/NPCKilled/PerfSmallDeath");
                for (int i = 0; i < 3; i++)
                    SoundEngine.PlaySound(crunch with { Volume = 0.4f, Pitch = -0.1f * i, MaxInstances = 3 }, Projectile.Center);
                SoundStyle crunch2 = new("CalamityMod/Sounds/NPCHit/RavagerHurt3");
                for (int i = 0; i < 2; i++)
                    SoundEngine.PlaySound(crunch2 with { Volume = 0.8f, Pitch = -0.4f * i, MaxInstances = 2 }, Projectile.Center);
                canDamage = true;
            }

            Projectile.Opacity = (float)Math.Pow(Utils.GetLerpValue(0, 40, Projectile.timeLeft, true), 4);
            if (Projectile.Opacity <= 0.4f)
                canDamage = false;
            time++;
            spawnedDust = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float damageMult = 1;
            if (hitTimer != 0)
                damageMult = 0.2f;
            bool jawsJustSlammed = (time >= jawSlamTime && time <= jawSlamTime + 8);
            modifiers.SourceDamage *= (jawsJustSlammed ? 4 : 1) * damageMult;
            if (hitTimer == 0)
                hitTimer = Projectile.localNPCHitCooldown;

            if (target.CanBeMoved())
                target.velocity *= 0.05f;
        }
        public override bool? CanDamage() => canDamage ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (time == 0)
                return false;

            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 startPoint = Projectile.Center;
            int distance = jawLength;
            int travelLength = 35;
            Vector2 endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * distance;
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);

            for (int t = -1; t <= 1; t += 2)
            {
                SpriteEffects spriteFx = (!isTopJaw) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Color drawColor = Projectile.GetAlpha(lightColor);
                int toothNum = 0;
                for (int i = 0; i < distance; i += travelLength)
                {
                    if (((isTopJaw && Projectile.direction == -1) || (!isTopJaw && Projectile.direction == 1)) && i == 0)
                        i += (int)(travelLength * 0.5f);

                    float jawRot = (isTopJaw ? topJawRot : bottomJawRot);
                    float scale = (Projectile.scale + 0.35f - 0.02f * toothNum);
                    float slamLerp = (float)Math.Pow(Utils.GetLerpValue(0, jawSlamTime * 1.5f, time, true), 4);
                    float sine = ((float)Math.Pow((float)Math.Sin((time + toothNum * 3) / 10), 4)) * slamLerp;
                    Vector2 drawPos = (startPoint + i * direction.RotatedBy(jawRot)) + direction.RotatedBy(MathHelper.PiOver2 * (isTopJaw ? 1 : -1)) * (-25 * scale - 65 * sine * scale);
                    Vector2 toMouse = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 finalDrawPos = drawPos + toMouse.RotatedBy(jawRot) * 5 * (1 - slamLerp) + Main.rand.NextVector2Circular(14, 14) * (time >= jawSlamTime ? Utils.GetLerpValue(jawSlamTime + 30, jawSlamTime, time, true) : 0);
                    float rotation = Projectile.velocity.ToRotation() + (jawRot) + (isTopJaw ? MathHelper.Pi : 0);
                    float toothOpacity = Math.Min((float)Math.Pow(Utils.GetLerpValue(0, toothNum, time, true), 3), Projectile.Opacity);
                    Main.EntitySpriteDraw(tex, finalDrawPos - Main.screenPosition, null, Color.Lerp(drawColor, Color.White, toothOpacity) * toothOpacity, rotation, new Vector2(tex.Width / 2, tex.Height), new Vector2(1f, 1) * scale, spriteFx, 0);
                    toothNum++;

                    if (!spawnedDust && toothNum > 1)
                    {
                        Dust dust = Dust.NewDustPerfect(finalDrawPos, !ChildSafety.Disabled ? DustID.Cloud : DustID.Blood, (rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2)).RotatedByRandom(0.5f).RotatedBy(1.2f * (isTopJaw ? -1 : 1)) * Main.rand.NextFloat(5f, 8f), 100, default, Main.rand.NextFloat(1.1f, 1.9f) * scale);
                        dust.noGravity = true;
                        dust.alpha = (int)(255 * (1 - toothOpacity));
                        Particle blood = new CustomSpark(finalDrawPos, (rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2)).RotatedByRandom(0.4f).RotatedBy(1.2f * (isTopJaw ? -1 : 1)) * Main.rand.NextFloat(5f, 8f), "CalamityMod/Particles/LargeBloom", 
                            false, 8, Main.rand.NextFloat(0.065f, 0.08f) * scale, (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.Lerp(Color.DarkRed, Color.Black, Main.rand.NextFloat(0, 0.5f))) * 0.7f * toothOpacity, new Vector2(1f, 1f), false, shrinkSpeed: 0.9f);
                        GeneralParticleHandler.SpawnParticle(blood);

                        Lighting.AddLight(finalDrawPos, (!ChildSafety.Disabled ? Color.CornflowerBlue : Color.Lerp(Color.Red, Color.White, 0.85f)).ToVector3() * 1.2f * toothOpacity);
                    }
                }
                isTopJaw = (t == -1);
            }
            spawnedDust = true; // Make sure effect spawning isn't happening based on frame rate

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!canDamage)
                return false;

            Vector2 start = Projectile.Center;
            float length = jawLength;
            float size = 165;
            float _ = float.NaN;

            bool hit = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, start + Projectile.velocity.SafeNormalize(Vector2.UnitX) * length, size, ref _);
            return hit;
        }
    }
}
