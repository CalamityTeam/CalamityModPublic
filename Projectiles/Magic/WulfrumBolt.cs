using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static CalamityMod.CalamityUtils;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;
using ReLogic.Content;
using CalamityMod.Items.Weapons.Magic;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Projectiles.Magic
{
    public class WulfrumBolt : ModProjectile
    {
        public ref float OriginalRotation => ref Projectile.ai[0];
        public NPC Target
        {
            get
            {
                if (Projectile.ai[1] < 0 || Projectile.ai[1] > Main.maxNPCs)
                    return null;

                return Main.npc[(int)Projectile.ai[1]];
            }
            set
            {
                if (value == null)
                    Projectile.ai[1] = -1;
                else
                    Projectile.ai[1] = value.whoAmI;
            }
        }

        public static float MaxDeviationAngle = MathHelper.PiOver4;
        public static float HomingRange = 200;

        internal PrimitiveTrail TrailDrawer;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 140;
            Projectile.extraUpdates = 2;
            HomingRange = 200f;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 140)
            {
                if (OriginalRotation == 0)
                {
                    OriginalRotation = Projectile.velocity.ToRotation();
                    Projectile.rotation = OriginalRotation;
                }
                Target = null;
            }
            else
            {
                if (Target == null)
                {
                    Target = Projectile.Center.ClosestNPCAt(HomingRange, false);
                }
            }

            Lighting.AddLight(Projectile.Center, (Color.GreenYellow * 0.8f).ToVector3() * 0.5f);

            if (Target != null)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards((Target.Center - Projectile.Center).ToRotation(), 0.04f);

                //float angleBetween = Projectile.rotation.ToRotationVector2().AngleBetween(OriginalRotation.ToRotationVector2());
                //if (Math.Abs(angleBetween) > MaxDeviationAngle) 
                //    Projectile.rotation += MaxDeviationAngle * Math.Sign(angleBetween);
            }

            Projectile.velocity *= 0.98f;
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();

            //for (int num151 = 0; num151 < 3; num151++)
            //{
            //    int num154 = 14;
            //    int num155 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 61, 0f, 0f, 100, default, 3f);
            //    Main.dust[num155].noGravity = true;
            //    Main.dust[num155].noLight = true;
            //}
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = (float)Math.Sqrt(1 - completionRatio);
            return Color.YellowGreen * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return (1 - completionRatio * 0.4f) * 6.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/BasicTrail"));
            TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 30);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(WulfrumProthesis.HitSound, Projectile.Center);
        }
    }
}
