using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseTurret : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 24;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.velocity.Y < 12f)
            {
                Projectile.velocity.Y += 0.5f;
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(850f, player, false);

            if (potentialTarget != null)
            {
                Projectile.spriteDirection = (potentialTarget.Center.X - Projectile.Center.X > 0).ToDirectionInt();
                Projectile.ai[0]++;
                if (Projectile.ai[0] % 40f < 30f)
                {
                    float idealAngle = Projectile.AngleTo(potentialTarget.Center) + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
                    if (Projectile.ai[1] <= 0f)
                    {
                        Projectile.rotation = Projectile.rotation.AngleLerp(idealAngle, MathHelper.TwoPi / 25f);
                    }
                    // Recoil back after firing
                    else
                    {
                        if (Projectile.ai[1] > 13f)
                        {
                            Projectile.rotation -= MathHelper.ToRadians(10f) * Projectile.localAI[0];
                        }
                        Projectile.ai[1]--;
                    }
                }
                if (Projectile.ai[0] % 40f == 39f && Main.myPlayer == Projectile.owner)
                {
                    Texture2D standTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PulseTurretStand").Value;
                    Vector2 shootPosition = Projectile.Center - ((standTexture.Height / 2 + 6f) * Vector2.UnitY);
                    shootPosition += (Projectile.Size * 0.5f).RotatedBy(Projectile.rotation - MathHelper.ToRadians(18f) - (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi);

                    bool aimingAtTarget = Math.Abs(Vector2.Normalize(potentialTarget.Center - shootPosition).ToRotation() - Projectile.rotation) < MathHelper.ToRadians(32f) + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
                    if (aimingAtTarget || Projectile.Distance(potentialTarget.Center) < 45f)
                    {
                        SoundEngine.PlaySound(PulseRifle.FireSound, shootPosition);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition,
                                                 Vector2.Normalize(potentialTarget.Center - shootPosition) * 12f,
                                                 ModContent.ProjectileType<PulseTurretShot>(),
                                                 Projectile.damage,
                                                 Projectile.knockBack,
                                                 Projectile.owner);
                        if (Projectile.ai[0] % 120f == 119f)
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition,
                                                         (potentialTarget.Center - shootPosition).SafeNormalize(Vector2.UnitY).RotatedBy(i * MathHelper.ToRadians(28f)) * 7f,
                                                         ModContent.ProjectileType<PulseTurretShot>(),
                                                         Projectile.damage,
                                                         Projectile.knockBack,
                                                         Projectile.owner,
                                                         0f,
                                                         1f);
                            }
                        }

                        if (!Main.dedServ)
                        {
                            for (int i = 0; i < 12; i++)
                                Dust.NewDustPerfect(shootPosition, 173).scale = Main.rand.NextFloat(1.4f, 1.8f);
                        }

                        Projectile.ai[1] = 15f;
                        Projectile.localAI[0] = Math.Sign(Projectile.SafeDirectionTo(potentialTarget.Center).X);
                    }
                }
            }
            else
            {
                Projectile.rotation = Projectile.rotation.AngleLerp(0f, MathHelper.TwoPi / 50f);
            }

            Projectile.StickToTiles(false, false);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D standTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PulseTurretStand").Value;
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value,
                             Projectile.Center - ((standTexture.Height / 2 + 6f) * Vector2.UnitY) - Main.screenPosition,
                             null,
                             lightColor,
                             Projectile.rotation,
                             Projectile.Size * 0.5f,
                             Projectile.scale,
                             Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                             0);
            Main.EntitySpriteDraw(standTexture,
                             Projectile.Center - Main.screenPosition,
                             null,
                             lightColor,
                             0f,
                             standTexture.Size() * 0.5f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }

        public override bool? CanDamage() => false;
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
