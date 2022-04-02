using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class ScarletDevilProjectile : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;
        public ref float ShootTimer => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ScarletDevil";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spear the Gungnir");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 45;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 45;
            projectile.width = 108;
            projectile.height = 108;
            projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.55f, 0.25f, 0f);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (!Main.dedServ)
            {
                for (int i = 0; i < (projectile.Calamity().stealthStrike && Main.rand.NextBool(2) ? 2 : 1); i++)
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 130, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.85f);
            }

            ShootTimer++;

            if (!projectile.Calamity().stealthStrike && projectile.oldPos.Length != 6)
                projectile.oldPos = new Vector2[6];

            if ((ShootTimer %= 5f) == 0f && !projectile.Calamity().stealthStrike)
            {
                if (projectile.owner == Main.myPlayer)
                    GenerateSideBullets(2, MathHelper.ToRadians(15f));
            }
        }

        internal void GenerateSideBullets(int totalBullets, float rotationalOffset)
        {
            for (int i = 0; i < totalBullets; i++)
            {
                Vector2 perturbedSpeed = new Vector2(-projectile.velocity.X / 3, -projectile.velocity.Y / 3).RotatedBy(MathHelper.Lerp(-rotationalOffset, rotationalOffset, i / (totalBullets - 1)));
                for (int j = 0; j < 2; j++)
                {
                    Projectile.NewProjectile(projectile.Center, perturbedSpeed, ModContent.ProjectileType<ScarletDevilBullet>(), (int)(projectile.damage * 0.03), 0f, projectile.owner, 0f, 0f);
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
                int bullet = Projectile.NewProjectile(projectile.Center + shootVelocity, shootVelocity, ModContent.ProjectileType<ScarletDevilBullet>(), (int)(projectile.damage * 0.01), 0f, projectile.owner);
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
                        int bullet = Projectile.NewProjectile(projectile.Center + shootVelocity, shootVelocity, ModContent.ProjectileType<ScarletDevilBullet>(), (int)(projectile.damage * 0.01), 0f, projectile.owner);
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

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item122, projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 150);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScarletBlast>(), (int)(projectile.damage * 0.0075), 0f, projectile.owner);
            if (!projectile.Calamity().stealthStrike)
                return;

            if (!Main.player[projectile.owner].moonLeech)
            {
                // Give on-heal effects from stealth strikes.
                Main.player[projectile.owner].statLife += 120;
                Main.player[projectile.owner].HealEffect(120);
            }

            // And spawn a bloom of bullets.
            SpawnOnStealthStrikeBullets();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 150);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScarletBlast>(), (int)(projectile.damage * 0.0075), 0f, projectile.owner);
            if (!projectile.Calamity().stealthStrike)
                return;

            // Give on-heal effects from stealth strikes.
            Main.player[projectile.owner].statLife += 120;
            Main.player[projectile.owner].HealEffect(120);

            // And spawn a bloom of bullets.
            SpawnOnStealthStrikeBullets();
        }

        internal float WidthFunction(float completionRatio)
        {
            float widthRatio = Utils.InverseLerp(0f, 0.1f, completionRatio, true);
            float baseWidth = MathHelper.Lerp(0f, 110f, widthRatio) * MathHelper.Clamp(1f - (float)Math.Pow(completionRatio, 0.4D), 0.37f, 1f);
            return baseWidth;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float colorFade = 1f - Utils.InverseLerp(0.6f, 0.98f, completionRatio, true);
            Color baseColor = CalamityUtils.MulticolorLerp((float)Math.Pow(completionRatio, 1D / 2D), Color.White, Color.DarkRed, Color.Wheat, Color.IndianRed) * MathHelper.Lerp(0f, 1.4f, colorFade);
            return Color.Lerp(baseColor, Color.DarkRed, (float)Math.Pow(completionRatio, 3D));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!projectile.Calamity().stealthStrike)
            {
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], new Color(100, 100, 100));
                return true;
            }
            else
            {
                if (TrailDrawer is null)
                    TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction, GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"]);

                GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));
                TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition + projectile.velocity.SafeNormalize(Vector2.Zero) * 86f, 60);

                Texture2D spearTexture = ModContent.GetTexture(Texture);

                for (int i = 0; i < 7; i++)
                {
                    Color drawColor = Color.Lerp(lightColor, Color.White, 0.8f) * 0.2f;
                    drawColor.A = 0;

                    Vector2 drawOffset = (i / 7f * MathHelper.TwoPi).ToRotationVector2() * 2f;
                    spriteBatch.Draw(spearTexture, projectile.Center - Main.screenPosition + drawOffset, null, drawColor, projectile.rotation, spearTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
    }
}
