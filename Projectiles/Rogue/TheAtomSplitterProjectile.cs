using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TheAtomSplitterProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        // Atom splitting is cool and all, but actual thermonuclear meltdown levels of DPS is unacceptable.
        // DO NOT increase this unless you are ABSOLUTELY SURE you know what will happen.
        private const float NormalSplitMultiplier = 0.7f;
        private const float StealthSplitMultiplier = 0.144f;

        public ref float HitTargetIndex => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];

        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheAtomSplitter";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 124;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // timeLeft is not synced by default. However, it is changed during an operation that is only
            // performed from one client's perspective, therefore it must be synced in this context.
            writer.Write(Projectile.timeLeft);
            writer.Write((byte)Projectile.alpha);
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadInt32();
            Projectile.alpha = reader.ReadByte();
            Projectile.scale = reader.ReadSingle();
        }

        public override void AI()
        {
            // Accelerate until at a certain speed.
            if (Projectile.velocity.Length() < 20f)
                Projectile.velocity *= 1.02f;

            if (Projectile.alpha < 10)
                EmitDustFromTip();

            // Stick to the target for a while and release a bunch of duplicates.
            if (Main.npc.IndexInRange((int)HitTargetIndex) && Main.npc[(int)HitTargetIndex].active)
            {
                if (Projectile.alpha < 255)
                    Projectile.alpha = Utils.Clamp(Projectile.alpha + 30, 0, 255);

                bool stealth = Projectile.Calamity().stealthStrike;
                int shootRate = stealth ? 2 : 4;
                if (Projectile.timeLeft % shootRate == shootRate - 1)
                {
                    NPC target = Main.npc[(int)HitTargetIndex];
                    FireDuplicateAtTarget(target, stealth ? 305f : 200f, stealth);
                    if (stealth)
                        FireExtraDuplicatesAtTarget(target);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Time++;
        }

        public void EmitDustFromTip()
        {
            if (Main.dedServ)
                return;

            float dustVelocityArcOffset = 1.5f + (float)Math.Sin(MathHelper.TwoPi * Projectile.timeLeft / 35f) * 0.25f;

            float colorInterpolant = (float)Math.Cos(MathHelper.TwoPi * Projectile.timeLeft / 75f) * 0.5f + 0.5f;
            Color dustColor = CalamityUtils.MulticolorLerp(colorInterpolant, CalamityUtils.ExoPalette);
            Vector2 currentDirection = Projectile.velocity.SafeNormalize(-Vector2.UnitY);
            Vector2 tipPosition = Projectile.Center + currentDirection * (Projectile.height * 0.67f - 3f);
            tipPosition += Main.rand.NextVector2CircularEdge(0.35f, 0.35f);

            for (float direction = -1f; direction <= 1f; direction += 2f)
            {
                Dust prismaticEnergy = Dust.NewDustPerfect(tipPosition, 267);
                prismaticEnergy.velocity = currentDirection.RotatedBy(direction * dustVelocityArcOffset) * -7f + Projectile.velocity;
                prismaticEnergy.scale = 1.2f;
                prismaticEnergy.color = dustColor;
                prismaticEnergy.noGravity = true;

                prismaticEnergy = Dust.CloneDust(prismaticEnergy);
                prismaticEnergy.scale *= 0.6f;
            }
        }

        public void FireDuplicateAtTarget(NPC target, float baseOutwardness, bool stealth)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 spawnPosition = target.Center + Main.rand.NextVector2CircularEdge(baseOutwardness, baseOutwardness) * Main.rand.NextFloat(0.9f, 1.15f);
            Vector2 shootVelocity = (target.Center - spawnPosition).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(12f, 14f);
            int damage = (int)(Projectile.damage * (stealth ? StealthSplitMultiplier : NormalSplitMultiplier));
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, shootVelocity, ModContent.ProjectileType<TheAtomSplitterDuplicate>(), damage, Projectile.knockBack, Projectile.owner, 0f, baseOutwardness / 9f);
        }

        public void FireExtraDuplicatesAtTarget(NPC target)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 spawnPosition = target.Center + Vector2.UnitY * Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(200f, 270f);
            spawnPosition.X += Main.rand.NextFloatDirection() * target.width * 0.45f;
            int damage = (int)(Projectile.damage * StealthSplitMultiplier);
            Vector2 shootVelocity = Vector2.UnitY * (target.Center.Y - spawnPosition.Y > 0f).ToDirectionInt() * Main.rand.NextFloat(14f, 16.5f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, shootVelocity, ModContent.ProjectileType<TheAtomSplitterDuplicate>(), damage, Projectile.knockBack, Projectile.owner, 0f, 24f);
        }

        public void ReleaseHitDust(Vector2 spawnPosition)
        {
            if (Main.dedServ)
                return;

            int dustCount = Projectile.Calamity().stealthStrike ? 60 : 40;
            Vector2 baseDustVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * 1.4f;
            Vector2 outwardFireSpeedFactor = new Vector2(2.1f, 2f);

            for (float i = 0f; i < dustCount; i++)
            {
                Color dustColor = CalamityUtils.MulticolorLerp(Main.rand.NextFloat(), CalamityUtils.ExoPalette);
                Dust explosionDust = Dust.NewDustDirect(spawnPosition, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                explosionDust.position = spawnPosition;
                explosionDust.velocity = baseDustVelocity.RotatedBy(MathHelper.TwoPi * i / dustCount) * outwardFireSpeedFactor * Main.rand.NextFloat(0.8f, 1.2f);
                explosionDust.velocity += Projectile.velocity * Main.rand.NextFloat(0.6f, 0.85f);
                if (Projectile.Calamity().stealthStrike)
                    explosionDust.velocity *= 1.6f;

                explosionDust.noGravity = true;
                explosionDust.scale = 1.1f;
                explosionDust.fadeIn = Main.rand.NextFloat(1.4f, 2.4f);

                Dust.CloneDust(explosionDust).velocity *= Main.rand.NextFloat(0.8f);

                explosionDust = Dust.CloneDust(explosionDust);
                explosionDust.scale /= 2f;
                explosionDust.fadeIn /= 2f;
                explosionDust.color = new Color(255, 255, 255, 255);
            }
        }

        public override bool? CanDamage() => Projectile.alpha < 80 ? null : false;

        public override bool? CanHitNPC(NPC target)
        {
            if (HitTargetIndex >= 0 && target.whoAmI != HitTargetIndex)
                return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ReleaseHitDust(target.Center - Projectile.velocity * 3f);
            if (!Main.npc.IndexInRange((int)HitTargetIndex))
            {
                HitTargetIndex = target.whoAmI;
                Projectile.timeLeft = Projectile.Calamity().stealthStrike ? 100 : 60;
                Projectile.netUpdate = true;
            }
        }
    }
}
