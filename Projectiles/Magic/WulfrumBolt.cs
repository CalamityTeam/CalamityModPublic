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
        public static float HomingRange = 250;
        public static float HomingAngle = MathHelper.PiOver4 * 1.65f;

        internal PrimitiveTrail TrailDrawer;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
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
        }

        public NPC FindTarget()
        {
            float bestScore = 0;
            NPC bestTarget = null;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC potentialTarget = Main.npc[i];

                if (!potentialTarget.CanBeChasedBy(null, false))
                    continue;

                float distance = potentialTarget.Distance(Projectile.Center);
                float angle = Projectile.velocity.AngleBetween((potentialTarget.Center - Projectile.Center));

                float extraDistance = potentialTarget.width / 2 + potentialTarget.height / 2;

                if (distance - extraDistance < HomingRange && angle < HomingAngle / 2f)
                {
                    if (!Collision.CanHit(Projectile.Center, 1, 1, potentialTarget.Center, 1, 1) && extraDistance < distance)
                        continue;

                    float attemptedScore = EvaluatePotentialTarget(distance - extraDistance, angle / 2f);
                    if (attemptedScore > bestScore)
                    {
                        bestTarget = potentialTarget;
                        bestScore = attemptedScore;
                    }
                }
            }
            return bestTarget;

        }

        public float EvaluatePotentialTarget(float distance, float angle)
        {
            float score = 1 - distance / HomingRange * 0.5f;

            score += (1 - Math.Abs(angle) / (HomingAngle / 2f)) * 0.5f;

            return score;
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
                Target = FindTarget();
            }

            Lighting.AddLight(Projectile.Center, (Color.GreenYellow * 0.8f).ToVector3() * 0.5f);

            if (Target != null)
            {
                float distanceFromTarget = (Target.Center - Projectile.Center).Length();

                Projectile.rotation = Projectile.rotation.AngleTowards((Target.Center - Projectile.Center).ToRotation(), 0.07f * (float)Math.Pow(( 1 - distanceFromTarget / HomingRange), 2));

                /*
                float angleBetween = Projectile.rotation - ((Target.Center - Projectile.Center).ToRotation());
                angleBetween += (angleBetween + 180).Modulo(360) - 180;

                if (Math.Abs(angleBetween) > MaxDeviationAngle) 
                    Projectile.rotation += MaxDeviationAngle * Math.Sign(angleBetween);
                */
            }

            Projectile.velocity *= 0.98f;
            Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();

            if (Projectile.timeLeft == 140)
            {
                Vector2 dustCenter = Projectile.Center + Projectile.velocity * 1f;

                for (int i = 0; i < 5; i++)
                {
                    Dust chust = Dust.NewDustPerfect(dustCenter, 15, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.2f, 0.5f), Scale: Main.rand.NextFloat(1.2f, 1.8f));
                    chust.noGravity = true;
                }
            }

            if (Projectile.timeLeft <= 137)
            {
                for (int num151 = 0; num151 < 3; num151++)
                {
                    int num154 = 14;
                    int num155 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, 61, 0f, 0f, 100, default, 3f);
                    Main.dust[num155].noGravity = true;
                    Main.dust[num155].noLight = true;
                }
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = (float)Math.Sqrt(1 - completionRatio);
            return Color.GreenYellow * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            return  7.4f;
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            return base.OnTileCollide(oldVelocity);
        }
    }
}
