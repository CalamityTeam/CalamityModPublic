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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = penetrationAmt;
            Projectile.timeLeft = 290;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!initialized)
            {
                startYVelSign = (float)Math.Sign(Projectile.velocity.Y) * 0.35f;
                initialized = true;
            }
            if (Projectile.penetrate == penetrationAmt && Projectile.timeLeft < 245)
            {
                Projectile.velocity.Y -= startYVelSign; //arc on the X axis (the amount of pressure on this tiny detail made me want to tear my head off)
                if (Math.Abs(Projectile.velocity.X) < 30f)
                {
                    Projectile.velocity.X *= 1.04f;
                }
            }
            if (Projectile.velocity.Length() != 0)
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.Pi / 4;
            }
            else
            {
                Projectile.rotation += MathHelper.Pi / 6f / (Projectile.scale * 2.5f);
            }
            Projectile.ai[0]++;
            if (Projectile.ai[1] >= 0)
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.velocity *= -0.9f;
                }
            }

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.785f;
            Lighting.AddLight(Projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);
            if (currentColor == Color.Black)
            {
                int startPoint = Main.rand.Next(6);
                Projectile.localAI[0] = startPoint;
                currentColor = GetStartingColor(startPoint);
            }
            Visuals(Projectile, ref currentColor);
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

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.scale += (maxScale - 1) / (float)penetrationAmt;
            Projectile.ai[1] = 5 + Main.rand.Next(-2, 3);

            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Projectile.scale += (maxScale - 1) / (float)penetrationAmt;
            Projectile.ai[1] = 5 + Main.rand.Next(-2, 3);

            target.ExoDebuffs();
        }
    }
}
