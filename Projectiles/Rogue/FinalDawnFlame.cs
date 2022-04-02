using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnFlame : ModProjectile
    {
        internal struct Flame
        {
            public Vector2 Position;

            public int FrameCounter;

            public int Frame;

            public float Alpha;

            public float Scale;

            public int Direction;
        }
        private Flame[] Flames;
        public const int TotalFlames = 120;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Final Dawn");
            Main.projFrames[projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            projectile.scale = 1f;
            projectile.width = 1000;
            projectile.height = 100;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;

            Flames = new Flame[TotalFlames];
            for (int i = 0; i < Flames.Length; i++)
            {
                Vector2 flamePosition = Main.rand.NextVector2Circular(projectile.width * 0.36f, projectile.height / 2);

                Flames[i].Position = flamePosition;
                Flames[i].Frame = Main.rand.Next(8);
                Flames[i].FrameCounter = Main.rand.Next(5);
                Flames[i].Alpha = 1f;
                Flames[i].Scale = 0.8f + 0.4f * Main.rand.NextFloat();
                Flames[i].Direction = Main.rand.Next(2);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];


            if (Flames.Length > 0)
            {
                for (int i = 0; i < Flames.Length; i++)
                {
                    int frameHeight2 = frameHeight * Flames[i].Frame;
                    spriteBatch.Draw(texture,
                                     projectile.Center + Flames[i].Position - Main.screenPosition,
                                     new Rectangle?(new Rectangle(0, frameHeight2, texture.Width, frameHeight)),
                                     projectile.GetAlpha(Color.White) * Flames[i].Alpha,
                                     projectile.rotation,
                                     new Vector2(texture.Width / 2f, frameHeight / 2f),
                                     Flames[i].Scale,
                                     Flames[i].Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                     0f);
                }
            }
            return false;
        }

        public override void AI()
        {
            if (Flames.Length > 0)
            {
                for (int i = 0; i < Flames.Length; i++)
                {
                    AdjustFlameValues(ref Flames[i]);
                }
            }
            AlphaAdjustments();
        }
        // The internal is to match consistency with the Flame struct. If you want to make this class public, you must also make the struct public.
        internal void AdjustFlameValues(ref Flame flame)
        {
            flame.FrameCounter++;
            if (flame.FrameCounter >= 5)
            {
                flame.Frame++;
                flame.FrameCounter = 0;

                if (flame.Frame >= 8)
                {
                    if (projectile.ai[0] < 570)
                    {
                        Vector2 pos = new Vector2(Main.rand.NextFloat(projectile.width * 0.36f)).RotatedByRandom(MathHelper.TwoPi);
                        float widthHeightRatio = (float)projectile.height / projectile.width;
                        pos.Y *= widthHeightRatio;

                        flame.Position = pos;
                        flame.Scale = 0.8f + 0.4f * Main.rand.NextFloat();
                        flame.Direction = Main.rand.Next(2);
                        flame.Frame = 0;
                    }
                    else
                    {
                        flame.Alpha = 0f;
                    }
                }
            }
        }
        public void AlphaAdjustments()
        {
            projectile.ai[0]++;
            if (projectile.ai[0] < 570)
            {
                projectile.alpha -= 10;
                if (projectile.alpha < 0) projectile.alpha = 0;
            }
            else
            {
                projectile.friendly = false;
                projectile.alpha += 5;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                }
            }
            if (projectile.ai[0] >= 600 && projectile.alpha >= 255)
            {
                projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}
