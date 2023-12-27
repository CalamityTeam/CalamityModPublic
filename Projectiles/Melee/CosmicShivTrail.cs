using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Contracts;
using Terraria.GameContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicShivTrail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public NPC target = null;
        public const float MaxDistanceToTarget = 540f;
        public const float ExplosionDamageMultiplier = 2f;
        public const int FadeInTime = 12;

        public static List<Color> DustColors = new List<Color>
        {
            new Color(69, 69, 222),
            new Color(99, 66, 212),
            new Color(130, 64, 214),
            new Color(154, 75, 219),
            new Color(165, 62, 201)
        };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 220;
            Projectile.scale = 0.7f;
            Projectile.Opacity = 0f;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            int QuarterSpriteWidth = 48 / 3;
            int QuarterSpriteHeight = 48 / 3;

            int QuarterProjWidth = Projectile.width / 3;
            int QuarterProjHeight = Projectile.height / 3;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(QuarterSpriteWidth - QuarterProjWidth);
            DrawOriginOffsetY = -(QuarterSpriteHeight - QuarterProjHeight);


            Lighting.AddLight(Projectile.Center, Projectile.GetAlpha(Color.White).ToVector3());

            Projectile.rotation += Utils.GetLerpValue(-8f, 12f, Projectile.velocity.Length(), clamped: true);

            if (Projectile.ai[0] > 2f)  // After two frames
            {
                Projectile.Opacity = Utils.GetLerpValue(2f, FadeInTime, Projectile.ai[0], clamped: true);   // Fade in

                if (Main.rand.Next(0, 2) == 0)      // Dust trail
                {
                    float scale = Main.rand.NextFloat(1.6f, 2.1f);
                    Color randomColor = DustColors[Main.rand.Next(0, DustColors.Count)];
                    Dust dust = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.RainbowMk2, 0, 0, 255, randomColor * 0.6f, scale);
                    dust.noGravity = true;
                }
            }

            if (Projectile.ai[0] % 15 == 0) // every 0.25 seconds
            {
                target = Projectile.Center.ClosestNPCAt(MaxDistanceToTarget);
            }

            if (target is not null)
            {
                Vector2 targetVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                targetVelocity *= Projectile.velocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.12f);               
            }

            // lifespan timer
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Summons invisible aura that sticks to enemies, spawning blades
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<CosmicShivAura>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
        }

        // pretty much entirely from the Oracle circular damage code
        private void CircularDamage(float radius)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[Projectile.owner];

            for (int i = 0; i < Main.npc.Length; ++i)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage || target.friendly)
                    continue;

                // Shock any valid target within range. Check all four corners of their hitbox.
                float d1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
                float d2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
                float d3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
                float d4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());
                float dist = MathHelper.Min(d1, d2);
                dist = MathHelper.Min(dist, d3);
                dist = MathHelper.Min(dist, d4);

                if (dist <= radius)
                {
                    int damage = (int)(Projectile.damage * ExplosionDamageMultiplier);
                    bool crit = Main.rand.Next(100) <= owner.GetCritChance<MeleeDamageClass>() + 4;
                    target.StrikeNPC(target.CalculateHitInfo(damage, 0, crit, 0));

                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, damage, 0f, 0f, crit ? 1 : 0, 0, 0);
                }
            }
        }

        // Very similar to CosmicShivBlade PreDraw
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            float rotation = Projectile.rotation;
            Color color = Projectile.GetAlpha(Color.White);
            float scaleMult = 1f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                color *= ((float)(Projectile.oldPos.Length - (i / 4)) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(sourceRectangle), color * 0.4f, rotation * -1f, origin, Projectile.scale * 1.8f * scaleMult, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(sourceRectangle), color, rotation, origin, Projectile.scale * scaleMult, SpriteEffects.None, 0f);
                scaleMult *= 0.9f;
            }
            
            return false; 

        }

        // Lots of trig here, use desmos to comprehend its full power
        public override void OnKill(int timeLeft)
        {
            float rand2PI = Main.rand.NextFloat(0, MathHelper.TwoPi);       // Random rotation offset
            int rosePetalCount = Main.rand.Next(5, 8);
            int bigWavyPetalCount = Main.rand.Next(7, 11);
            int wavyPetalCount = Main.rand.Next(6, 10);
            float speed = Main.rand.Next(6, 11);                            // Size

            int rand = Main.rand.Next(0, 10);   // 10% chance rose, 10% chance big wavy shape, and 80% chance small undetail wavy shape
            if (rand == 0) { 
                for (float k = 0f; k < MathHelper.TwoPi; k += 0.03f)
                {
                    float scale = Main.rand.NextFloat(1.1f, 1.4f);
                    float randomWhitingValue = Main.rand.NextFloat(0.0f, 0.2f);
                    Color color = Color.Lerp(DustColors[Main.rand.Next(0, DustColors.Count)], Color.White, randomWhitingValue);

                    Vector2 velocity = k.ToRotationVector2() * (2f + (float)(Math.Sin((double)(rand2PI + k * (float)rosePetalCount)) + 1.0) * speed) * Main.rand.NextFloat(0.95f, 1.05f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2?(velocity), 0, color, scale);
                    dust.noGravity = true;
                    dust.fadeIn = -1f;

                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2?(velocity) * 0.75f, 0, color, scale);
                    dust2.noGravity = true;
                    dust2.fadeIn = -1f;
                }
            } else if (rand == 1) {
                for (float k = 0f; k < MathHelper.TwoPi; k += 0.08f)
                {
                    float scale = Main.rand.NextFloat(1.2f, 1.6f);
                    float randomWhitingValue = Main.rand.NextFloat(0.0f, 0.2f);
                    Color color = Color.Lerp(DustColors[Main.rand.Next(0, DustColors.Count)], Color.White, randomWhitingValue);

                    Vector2 velocity = k.ToRotationVector2() * (float)(Math.Cos((double)(k * (float)bigWavyPetalCount) + rand2PI) + 5.1f) * speed / 4;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2?(velocity), 0, color, scale);
                    dust.noGravity = true;
                    dust.fadeIn = -1f;

                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2?(velocity) * 0.75f, 0, color, scale);
                    dust2.noGravity = true;
                    dust2.fadeIn = -1f;
                }
            } else {
                for (float k = 0f; k < MathHelper.TwoPi; k += 0.2f)
                {
                    float scale = Main.rand.NextFloat(1f, 1.2f);
                    float randomWhitingValue = Main.rand.NextFloat(0.5f, 0.7f);
                    Color color = Color.Lerp(DustColors[Main.rand.Next(0, DustColors.Count)], Color.White, randomWhitingValue);

                    Vector2 velocity = k.ToRotationVector2() * (0.4f * (float)(Math.Sin((double)(k * (float)wavyPetalCount) + rand2PI) + 2.1f) * (speed / 2));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2?(velocity), 0, color * 0.3f, scale);
                    dust.noGravity = true;
                    dust.fadeIn = -1f;
                }
            }

            CircularDamage(80f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(238, 171, 255, 25) * Projectile.Opacity;
        }
    }
}
