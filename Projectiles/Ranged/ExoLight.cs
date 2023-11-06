using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Graphics.Metaballs;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator left click side projectile (waving light)
    public class ExoLight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Vector2 InitialCenter;
        public Vector2 Destination;
        public Vector2 NPCDestination;
        public const float MaxRadius = 90f;
        public ref float YDirection => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public Color sparkColor;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 210;
            Projectile.alpha = 127;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(Destination);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Destination = reader.ReadVector2();
        }

        public override void AI()
        {
            PhotoMetaball.SpawnParticle(Projectile.Center, 54);
            PhotoMetaball2.SpawnParticle(Projectile.Center, 50);

            sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };
            
            Lighting.AddLight(Projectile.Center, Color.DarkSlateGray.ToVector3());
            Projectile.scale = MathHelper.Lerp(0.001f, 1f, Utils.GetLerpValue(0f, 25f, Time, true));
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile.GetSource_FromThis(), false))
                        NPCDestination = Main.npc[i].Center + Main.npc[i].velocity * 5f;
                }
                InitialCenter = Projectile.Center;
                Projectile.localAI[0] = 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Destination = NPCDestination;
                    Projectile.netUpdate = true;
                }
            }

            if (Destination == Vector2.Zero)
                return;

            if (Time <= 60f)
            {
                Projectile.Center = Vector2.Lerp(InitialCenter, Destination, Time / 60f);
                Projectile.Center += (Vector2.UnitY * MathF.Sin(Time / 70f * MathHelper.TwoPi) * 75f * YDirection).RotatedBy(Projectile.velocity.ToRotation());
            }
            else if (Time < 120f)
            {
                // For those who haven't seen this yet (namely in the 1.4 source), this allows you to achieve a
                // fade-in and fade-out effect with relative ease. At 90, multiply by 0, and somewhere in-between until 115, where you multiply by 1.
                // And then do the same for the fade-out effect of 180f-165. These inverse linear interpolations cannot overlap by definition because
                // their ranges do not overlap. Overall a really cool trick.
                float radius = MaxRadius * Utils.GetLerpValue(60f, 75f, Time, true) * Utils.GetLerpValue(120f, 105f, Time, true);
                radius *= 1f + MathF.Cos(Main.GlobalTimeWrappedHourly / 24f) * 0.25f;
                if (radius < 5f)
                    radius = 5f;
                Projectile.Center = Destination + ((Time - 60) / 60f * MathHelper.ToRadians(720f) + (YDirection == -1).ToInt() * MathHelper.Pi).ToRotationVector2() * radius;
            }
            else if (Time == 120f)
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            float scaleBonus = Time >= 120f ? Main.rand.NextFloat(3.4f, 4.2f) : Main.rand.NextFloat(0.8f, 1.6f);
            SoundEngine.PlaySound(DeadSunsWind.Explosion with { Volume = 0.7f}, Projectile.Center);
            float numberOfDusts = Time >= 120f ? 30 : 20;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                sparkColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = (Vector2.UnitX * Main.rand.NextFloat(0.2f, 3.1f)).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));

                SquishyLightParticle exoEnergy = new(Projectile.Center + offset, velOffset * (Main.rand.NextFloat(0.5f, 3.5f) + scaleBonus * 0.65f), Time >= 120f ? 0.7f : 0.5f, sparkColor, Time >= 120f ? 50 : 35);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }
            Projectile.scale = 6.5f * scaleBonus;
            // we should probably have a generic util to do this whole thing
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * Projectile.scale * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 360);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 360);
    }
}
