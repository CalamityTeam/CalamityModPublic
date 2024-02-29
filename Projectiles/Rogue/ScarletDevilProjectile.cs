using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScarletDevilProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public ref float ShootTimer => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ScarletDevil";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 45;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 45;
            Projectile.width = 108;
            Projectile.height = 108;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.55f, 0.25f, 0f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (!Main.dedServ)
            {
                for (int i = 0; i < (Projectile.Calamity().stealthStrike && Main.rand.NextBool() ? 2 : 1); i++)
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 130, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.85f);
            }

            ShootTimer++;

            if (!Projectile.Calamity().stealthStrike && Projectile.oldPos.Length != 6)
                Projectile.oldPos = new Vector2[6];

            if ((ShootTimer %= 5f) == 0f && !Projectile.Calamity().stealthStrike)
            {
                if (Projectile.owner == Main.myPlayer)
                    GenerateSideBullets(2, MathHelper.ToRadians(15f));
            }
        }

        internal void GenerateSideBullets(int totalBullets, float rotationalOffset)
        {
            for (int i = 0; i < totalBullets; i++)
            {
                Vector2 perturbedSpeed = new Vector2(-Projectile.velocity.X / 3, -Projectile.velocity.Y / 3).RotatedBy(MathHelper.Lerp(-rotationalOffset, rotationalOffset, i / (totalBullets - 1)));
                for (int j = 0; j < 2; j++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<ScarletDevilBullet>(), (int)(Projectile.damage * 0.03), 0f, Projectile.owner, 0f, 0f);
                    perturbedSpeed *= 1.05f;
                }
            }
        }

        internal void SpawnOnStealthStrikeBullets()
        {
            float starSpeed = 25f;

            // Spawn a circle of fast bullets.
            for (int i = 0; i < 40; i++)
            {
                Vector2 shootVelocity = (MathHelper.TwoPi * i / 40f).ToRotationVector2() * starSpeed;
                int bullet = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + shootVelocity, shootVelocity, ModContent.ProjectileType<ScarletDevilBullet>(), (int)(Projectile.damage * 0.01), 0f, Projectile.owner);
                if (Main.projectile.IndexInRange(bullet))
                    Main.projectile[bullet].Calamity().stealthStrike = true;
            }

            // Spawn a pair of stars, one slow, one fast.
            int pointsOnStar = 6;
            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < pointsOnStar; i++)
                {
                    float angle = MathHelper.Pi * 1.5f - i * MathHelper.TwoPi / pointsOnStar;
                    float nextAngle = MathHelper.Pi * 1.5f - (i + 3) % pointsOnStar * MathHelper.TwoPi / pointsOnStar;
                    if (k == 1)
                        nextAngle = MathHelper.Pi * 1.5f - (i + 2) * MathHelper.TwoPi / pointsOnStar;
                    Vector2 start = angle.ToRotationVector2();
                    Vector2 end = nextAngle.ToRotationVector2();
                    int pointsOnStarSegment = 18;
                    for (int j = 0; j < pointsOnStarSegment; j++)
                    {
                        Vector2 shootVelocity = Vector2.Lerp(start, end, j / (float)pointsOnStarSegment) * starSpeed;
                        int bullet = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + shootVelocity, shootVelocity, ModContent.ProjectileType<ScarletDevilBullet>(), (int)(Projectile.damage * 0.01), 0f, Projectile.owner);
                        if (Main.projectile.IndexInRange(bullet))
                            Main.projectile[bullet].Calamity().stealthStrike = true;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ExpandHitboxBy(150);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScarletBlast>(), (int)(Projectile.damage * 0.0075), 0f, Projectile.owner);
            if (!Projectile.Calamity().stealthStrike)
                return;

            if (!Main.player[Projectile.owner].moonLeech)
            {
                // Give on-heal effects from stealth strikes.
                Main.player[Projectile.owner].statLife += 90;
                Main.player[Projectile.owner].HealEffect(90);
            }

            // And spawn a bloom of bullets.
            SpawnOnStealthStrikeBullets();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.ExpandHitboxBy(150);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScarletBlast>(), (int)(Projectile.damage * 0.0075), 0f, Projectile.owner);
            if (!Projectile.Calamity().stealthStrike)
                return;

            // Give on-heal effects from stealth strikes.
            Main.player[Projectile.owner].statLife += 120;
            Main.player[Projectile.owner].HealEffect(120);

            // And spawn a bloom of bullets.
            SpawnOnStealthStrikeBullets();
        }

        internal float WidthFunction(float completionRatio)
        {
            float widthRatio = Utils.GetLerpValue(0f, 0.1f, completionRatio, true);
            float baseWidth = MathHelper.Lerp(0f, 110f, widthRatio) * MathHelper.Clamp(1f - (float)Math.Pow(completionRatio, 0.4D), 0.37f, 1f);
            return baseWidth;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float colorFade = 1f - Utils.GetLerpValue(0.6f, 0.98f, completionRatio, true);
            Color baseColor = CalamityUtils.MulticolorLerp((float)Math.Pow(completionRatio, 1D / 2D), Color.White, Color.DarkRed, Color.Wheat, Color.IndianRed) * MathHelper.Lerp(0f, 1.4f, colorFade);
            return Color.Lerp(baseColor, Color.DarkRed, (float)Math.Pow(completionRatio, 3D));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], new Color(100, 100, 100));
                return true;
            }
            else
            {
                GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * 86f, false,
                    shader: GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"]), 60);

                Texture2D spearTexture = ModContent.Request<Texture2D>(Texture).Value;

                for (int i = 0; i < 7; i++)
                {
                    Color drawColor = Color.Lerp(lightColor, Color.White, 0.8f) * 0.2f;
                    drawColor.A = 0;

                    Vector2 drawOffset = (i / 7f * MathHelper.TwoPi).ToRotationVector2() * 2f;
                    Main.EntitySpriteDraw(spearTexture, Projectile.Center - Main.screenPosition + drawOffset, null, drawColor, Projectile.rotation, spearTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return false;
        }
    }
}
