using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RefractionRotorProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public const int EnergyShotCount = 6;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RefractionRotor";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 142;
            Projectile.height = 126;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 18, 0, 255);
            Projectile.rotation += Projectile.velocity.Length() * Math.Sign(Projectile.velocity.X) * 0.036f;
        }

        public override bool? CanDamage() => Projectile.alpha <= 128 ? null : false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 20)
                Projectile.timeLeft = 20;
        }

        public override void OnKill(int timeLeft)
        {
            // Release a puff of rainbow dust and some blades.
            // If this projectile is a stealth strike, don't create the blades as a gore-- create them as a projectile instead.
            if (!Main.dedServ)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust rainbowBurst = Dust.NewDustPerfect(Projectile.Center, 267);
                    rainbowBurst.color = Main.hslToRgb(i / 80f, 0.9f, 0.6f);
                    rainbowBurst.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 5.5f);
                    rainbowBurst.scale = Main.rand.NextFloat(1.4f, 2.4f);
                    rainbowBurst.fadeIn = Main.rand.NextFloat(0.8f, 1.6f);
                    rainbowBurst.noGravity = true;
                }

                if (!Projectile.Calamity().stealthStrike)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        int goreType = Mod.Find<ModGore>("PrismShurikenBlade").Type;
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 shootDirection = (MathHelper.TwoPi * i / 6f + Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
                            Vector2 spawnPosition = Projectile.Center + Projectile.Size * 0.5f * Projectile.scale * shootDirection * 0.85f;
                            if (!WorldGen.SolidTile((int)spawnPosition.X / 16, (int)spawnPosition.Y / 16))
                                Gore.NewGorePerfect(Projectile.GetSource_Death(), spawnPosition, Projectile.velocity * 0.5f + shootDirection * 7f, goreType, Projectile.scale);
                        }
                    }
                }
            }

            int shootType = ModContent.ProjectileType<PrismRocket>();
            if (Main.myPlayer != Projectile.owner)
                return;

            // Release a circle of damaging blades if this projectile is a stealth strike.
            if (Projectile.Calamity().stealthStrike)
            {
                int bladeType = ModContent.ProjectileType<PrismShurikenBlade>();
                int rocketDamage = (int)(Projectile.damage * 0.8);
                for (int i = 0; i < 6; i++)
                {
                    Vector2 shootDirection = (MathHelper.TwoPi * i / 6f + Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
                    Vector2 spawnPosition = Projectile.Center + Projectile.Size * 0.5f * Projectile.scale * shootDirection * 0.85f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Projectile.velocity * 0.5f + shootDirection * 7f, bladeType, rocketDamage, Projectile.knockBack, Projectile.owner);
                }
            }

            if (CalamityUtils.CountProjectiles(shootType) > 24)
                return;

            int energyDamage = (int)(Projectile.damage * 0.495);
            float baseDirectionRotation = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < EnergyShotCount; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / EnergyShotCount + baseDirectionRotation).ToRotationVector2() * 9f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + shootVelocity, shootVelocity, shootType, energyDamage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/RefractionRotorGlowmask").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = glowmask.Size() * 0.5f;
            Main.EntitySpriteDraw(glowmask, drawPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
