using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TriactisHammerFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/ExtraTextures/TinyGreyscaleCircle";

        public ref float FlareType => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float OrbitRadius => ref Projectile.ai[2]; // Repurposed for target once transformed
        public Player Owner => Main.player[Projectile.owner];

        public static Asset<Texture2D> Sparkle;
        public static Asset<Texture2D> Bloom;
        public override void Load()
        {
            Sparkle = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle");
            Bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }
            // You may swap weapons and keep the flares, but only for a short time
            if (Owner.HeldItem.type == ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>())
                Projectile.timeLeft = 180;

            float rotation = Main.GlobalTimeWrappedHourly * 2f + MathHelper.ToRadians(120f) * FlareType;

            // Transform into smashy hammers
            if (Target == -2f)
            {
                Projectile hammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TriactisHammerProj>(), Projectile.damage, 0f, Projectile.owner, 0f, FlareType, OrbitRadius);
                hammer.scale = 0f;
                Projectile.Kill();
                return;
            }
            // Orbit around the player
            else if (!Main.npc.IndexInRange((int)Target))
                Projectile.Center = Owner.MountedCenter + Vector2.UnitX.RotatedBy(rotation) * 96f;
            // Orbit around the enemies
            else
            {
                NPC enemy = Main.npc[(int)Target];
                // Relocate to the player and spawn teleporting particles if said target cannot be stuck on
                if (enemy is null || enemy.life <= 0 || !enemy.active || enemy.dontTakeDamage || enemy.immortal)
                {
                    Target = -1f;
                    Projectile.netUpdate = true;
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 10f);
                        Particle sparkle = new CritSpark(Projectile.Center, velocity, Color.White, GetColor(FlareType), 1f, 24, 0.1f, 2.4f);
                        GeneralParticleHandler.SpawnParticle(sparkle);
                    }
                    return;
                }

                OrbitRadius = MathHelper.Clamp(MathF.Max(enemy.width, enemy.height) + 64f, 64f, 400f);
                Projectile.Center = enemy.Center + Vector2.UnitX.RotatedBy(rotation) * OrbitRadius;
            }

            // Dust trail
            if (Main.rand.NextBool(3))
            {
                Dust trail = Dust.QuickDust(Projectile.Center, GetColor(FlareType));
                trail.position += Main.rand.NextVector2Unit() * Main.rand.NextFloat(0f, 8f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, GetColor(FlareType), Vector2.One, 0f, 0.1f, 0.8f, 10);
            GeneralParticleHandler.SpawnParticle(pulse);
        }

        // Hue-shifted version of the base colours
        public static Color GetColor(float type)
        {
            if (type == 3f)
            {
                Vector3 blue = Main.rgbToHsl(new Color(117, 170, 239));
                return Main.hslToRgb(blue.X + 0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f), blue.Y, blue.Z);
            }
            if (type == 2f)
            {
                Vector3 green = Main.rgbToHsl(new Color(132, 225, 26));
                return Main.hslToRgb(green.X + 0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f), green.Y, green.Z);
            }
            Vector3 red = Main.rgbToHsl(Color.Red);
            return Main.hslToRgb(red.X + 0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f), red.Y, red.Z);
        }

        public override Color? GetAlpha(Color lightColor) => GetColor(FlareType);

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);

            Texture2D sparkleTex = Sparkle.Value;
            Texture2D bloomTex = Bloom.Value;
            float bloomScale = (float)sparkleTex.Height / (float)bloomTex.Height;
            float sparkleScale = 0.7f + CalamityUtils.Convert01To010((Main.GlobalTimeWrappedHourly % 2f) / 2f) * 0.2f;

            Color color = Projectile.GetAlpha(lightColor);
            float rotation = Main.GlobalTimeWrappedHourly * 8f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(bloomTex, drawPos, null, color * 0.5f, 0, bloomTex.Size() * 0.5f, 5f * bloomScale, SpriteEffects.None);
            Main.EntitySpriteDraw(sparkleTex, drawPos, null, Color.Lerp(color, Color.White, 0.7f), rotation, sparkleTex.Size() * 0.5f, 2.2f * sparkleScale, SpriteEffects.None);
            Main.EntitySpriteDraw(sparkleTex, drawPos, null, color, rotation + MathHelper.PiOver4, sparkleTex.Size() * 0.5f, 1.6f * sparkleScale, SpriteEffects.None);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                // The further the smaller
                Color trailColor = Color.Lerp(color, Color.Black, completionRatio);
                float trailScale = MathHelper.Lerp(0.15f, 1f, 1f - completionRatio);
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, 0f, texture.Size() * 0.5f, Projectile.scale * trailScale, SpriteEffects.None);
            }

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
