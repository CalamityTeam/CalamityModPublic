using System;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.Particles;
using System.IO;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    // Although normally yoyos are automatically exempted from pierce resist,
    // the damage logic for the buzzsaw briefly clobbers aiStyle, necessitating a manual pierce resist exception.
    [PierceResistException]
    public class LaceratorYoyo : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Lacerator>();
        public const int MaxUpdates = 3;

        public float chargeProgress = 0;
        private bool sawHit = false;
        private bool spawnedBlood = false;
        private int sawDir = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = Lacerator.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = Lacerator.Speed / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6 * MaxUpdates;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(chargeProgress);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            chargeProgress = reader.ReadSingle();
        }

        public override void AI()
        {
            if (sawDir == 0)
                sawDir = Projectile.velocity.X < 0 ? -1 : 1;
            if (chargeProgress > 1)
                chargeProgress = 1;
            if (chargeProgress > 0)
            {
                // Handles damage logic for the buzzsaw visual
                if (Main.player[Projectile.owner].miscCounter % 5 == 0 && Projectile.FinalExtraUpdate() && chargeProgress > 0.05f)
                {
                    spawnedBlood = false;
                    Projectile.position = Projectile.Center;
                    Projectile.width = 196;
                    Projectile.height = 196;
                    Projectile.Center = Projectile.position;
                    Projectile.originalDamage = Projectile.damage;
                    Projectile.damage = (int)(Projectile.damage * MathHelper.Lerp(0, 1f, chargeProgress));
                    sawHit = true;
                    Projectile.usesIDStaticNPCImmunity = true;
                    Projectile.aiStyle = -1;
                    Projectile.Damage();
                    Projectile.aiStyle = ProjAIStyleID.Yoyo;
                    Projectile.usesIDStaticNPCImmunity = false;
                    sawHit = false;
                    Projectile.damage = Projectile.originalDamage;
                    Projectile.position = Projectile.Center;
                    Projectile.width = 16;
                    Projectile.height = 16;
                    Projectile.Center = Projectile.position;

                }
                if (chargeProgress > 0.05f)
                {
                    float particleSize = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    particleSize *= 0.5f;
                    var particlePos = Projectile.Center + new Vector2(0, 84).RotatedByRandom(MathHelper.TwoPi);
                    var particleVel = particlePos.DirectionTo(Projectile.Center).RotatedBy(-sawDir * MathHelper.PiOver2 * 0.9f).RotatedByRandom(0.1f) * Main.rand.NextFloat(15f, 25f);
                    Particle blood = new CustomSpark(particlePos, particleVel, "CalamityMod/Particles/PearlParticleGlow", false, (int)MathHelper.Lerp(7, 17, chargeProgress), particleSize, Color.DarkRed, new Vector2(0.5f, 1), false, false, 0, false, false, 0f);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
                chargeProgress -= 0.003f / Projectile.extraUpdates;
                if (chargeProgress < 0)
                    chargeProgress = 0;
            }

            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (sawHit)
            {
                if (damageDone > 2 && !target.Calamity().IsArmored())
                    Projectile.damage = (int)(Projectile.damage * 0.85f);
                Vector2 bloodpos = Projectile.Center + Projectile.DirectionTo(target.Center) * 84;
                if (!spawnedBlood && chargeProgress > Main.rand.NextFloat() && target.Hitbox.Contains(bloodpos.ToPoint()))
                {
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), bloodpos, Projectile.DirectionTo(target.Center).RotatedBy(sawDir * MathHelper.PiOver2 * 0.9f).RotatedByRandom(0.1f) * Main.rand.NextFloat(3f, 5f), ModContent.ProjectileType<BloodstoneHealOrb>(), 8, 0f, Projectile.owner);
                    spawnedBlood = true;
                }
                return;
            }
            var baseYoyo = Main.projectile.First(x => x.active && x.type == ModContent.ProjectileType<LaceratorYoyo>() && x.owner == Projectile.owner);
            baseYoyo.ModProjectile<LaceratorYoyo>().chargeProgress += (Main.player[Projectile.owner].yoyoGlove ? 0.05f : 0.1f);
            Projectile.netUpdate = true;
            target.AddBuff(ModContent.BuffType<Laceration>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Laceration>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            if (chargeProgress > 0)
            {
                var owner = Main.player[Projectile.owner];
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/LaceratorSaw").Value;
                Texture2D WindTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/BlazingPhantomBlade").Value;
                float rot = MathHelper.TwoPi * 30 * sawDir * (owner.miscCounter / 300f);
                var color = new Color(200, 200, 200);
                var frame = WindTexture.Frame(1, 4, 0, 0);
                Main.EntitySpriteDraw(WindTexture, Projectile.Center - Main.screenPosition + rot.ToRotationVector2() * 30, frame, color * MathF.Pow(chargeProgress, 0.5f) * 0.33f, rot - 0.2f * sawDir, new Vector2(frame.Width, frame.Height) * 0.5f, 1, sawDir == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
                Main.EntitySpriteDraw(WindTexture, Projectile.Center - Main.screenPosition + (rot + MathHelper.Pi).ToRotationVector2() * 30, frame, color * MathF.Pow(chargeProgress, 0.5f) * 0.33f, (rot + MathHelper.Pi) - 0.2f * sawDir, new Vector2(frame.Width, frame.Height) * 0.5f, 1, sawDir == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
                color = new Color(255, 10, 10);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * MathF.Pow(chargeProgress, 0.5f) * 1f, rot, texture.Size() * 0.5f, 1, sawDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}
