using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class ExoGladiusBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/ExoGladius";

        private int penetrationAmt = 6;
        private Color currentColor = Color.Black;
        public const float maxScale = 1.8f;
        public bool initialized = false;
        public float startYVelSign = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladius Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = penetrationAmt;
            projectile.timeLeft = 290;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!initialized)
            {
                startYVelSign = (float)Math.Sign(projectile.velocity.Y) * 0.35f;
                initialized = true;
            }
            if (projectile.penetrate == penetrationAmt && projectile.timeLeft < 245)
            {
                projectile.velocity.Y -= startYVelSign; //arc on the X axis (the amount of pressure on this tiny detail made me want to tear my head off)
                if (Math.Abs(projectile.velocity.X) < 30f)
                {
                    projectile.velocity.X *= 1.04f;
                }
            }
            if (projectile.velocity.Length() != 0)
            {
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.Pi / 4;
            }
            else
            {
                projectile.rotation += MathHelper.Pi / 6f / (projectile.scale * 2.5f);
            }
            projectile.ai[0]++;
            if (projectile.ai[1] >= 0)
            {
                projectile.ai[1]--;
                if (projectile.ai[1] == 0)
                {
                    projectile.velocity *= -0.9f;
                }
            }

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);
            if (currentColor == Color.Black)
            {
                int startPoint = Main.rand.Next(6);
                projectile.localAI[0] = startPoint;
                currentColor = GetStartingColor(startPoint);
            }
            Visuals(projectile, ref currentColor);
        }

        internal static void Visuals(Projectile projectile, ref Color c)
        {
            CalamityUtils.IterateDisco(ref c, ref projectile.localAI[0], 15);
            Vector3 compositeColor = 0.1f * Color.White.ToVector3() + 0.05f * c.ToVector3();
            Lighting.AddLight(projectile.Center, compositeColor);
        }

        internal static Color GetStartingColor(int startPoint = 0)
        {
            switch (startPoint)
            {
                default: return new Color(1f, 0f, 0f, 0f); // Color.Red
                case 1: return new Color(1f, 1f, 0f, 0f); // Color.Yellow
                case 2: return new Color(0f, 1f, 0f, 0f); // Color.Green
                case 3: return new Color(0f, 1f, 1f, 0f); // Color.Turquoise
                case 4: return new Color(0f, 0f, 1f, 0f); // Color.Blue
                case 5: return new Color(1f, 0f, 1f, 0f); // Color.Violet
            }
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return currentColor;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.scale += (maxScale - 1) / (float)penetrationAmt;
            projectile.ai[1] = 5 + Main.rand.Next(-2, 3);

            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.scale += (maxScale - 1) / (float)penetrationAmt;
            projectile.ai[1] = 5 + Main.rand.Next(-2, 3);

            target.ExoDebuffs();
        }
    }
}
