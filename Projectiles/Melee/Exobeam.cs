using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Melee
{
    public class Exobeam : ModProjectile
    {
        public int TargetIndex = -1;

        public static float MaxWidth = 30;
        public ref float Time => ref Projectile.ai[0];
        public PrimitiveTrail TrailDrawer = null;
        public PrimitiveTrail MiniTrailDrawer = null;

        //public static TrailTexture

        public float TrailLenght
        {
            get
            {
                if (Projectile.localAI[0] == -1)
                {
                    float totalLenght = 0;
                    for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                    {
                        if (Projectile.oldPos[i] == Vector2.Zero)
                            break;

                        totalLenght += (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).Length();
                    }

                    Projectile.localAI[0] = totalLenght;
                }
                
                return Projectile.localAI[0];
            }
        }

        public float TrailCompletionAtWhichTheDomeEnds => TrailLenght < MaxWidth ? TrailLenght : MaxWidth / TrailLenght;

        public static Asset<Texture2D> BloomTex;
        public static Asset<Texture2D> SlashTex;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exobeam");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 12;
        }

        public override void AI()
        {
            // Aim very, very quickly at targets.
            // This takes a small amount of time to happen, to allow the blade to go in its intended direction before immediately racing
            // towards the nearest target.
            if (Time >= Exoblade.BeamNoHomeTime)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f, false);
                if (TargetIndex == -1 && potentialTarget != null)
                    TargetIndex = potentialTarget.whoAmI;

                if (TargetIndex >= 0 && Main.npc[TargetIndex].active && Main.npc[TargetIndex].CanBeChasedBy())
                {
                    Vector2 idealVelocity = Projectile.SafeDirectionTo(Main.npc[TargetIndex].Center) * (Projectile.velocity.Length() + 3.5f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.08f);
                }

                else
                {
                    Projectile.velocity *= 0.99f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool())
            {
                Color dustColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.9f);
                Dust must = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f) + Projectile.velocity, 267, Projectile.velocity * -2.6f, 0, dustColor);
                must.scale = 0.3f;
                must.fadeIn = Main.rand.NextFloat() * 1.2f;
                must.noGravity = true;
            }

            Projectile.scale = Utils.GetLerpValue(0f, 0.1f, Projectile.timeLeft / 600f, true);

            if (Projectile.FinalExtraUpdate())
                Time++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(Exoblade.BeamHitSound, target.Center);
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity * 0.1f, ModContent.ProjectileType<ExobeamSlashCreator>(), Projectile.damage, 0f, Projectile.owner, target.whoAmI);

            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White with { A = 0 } * Projectile.Opacity;


        public float TrailWidth(float completionRatio)
        {
            float width;
            //else
                //width = 1 - (completionRatio - TrailCompletionAtWhichTheDomeEnds) / (1f - TrailCompletionAtWhichTheDomeEnds);

            width = Utils.GetLerpValue(1f, 0.4f, completionRatio, true) * (float)Math.Sin(Math.Acos(1 - Utils.GetLerpValue(0f, 0.15f, completionRatio, true)));

            //if (completionRatio <= TrailCompletionAtWhichTheDomeEnds)
            //    width = (float)Math.Sin(Math.Acos(1 - completionRatio / TrailCompletionAtWhichTheDomeEnds));


            width *= Utils.GetLerpValue(0f, 0.1f, Projectile.timeLeft / 600f, true);

            return width * MaxWidth;
        }
        public Color TrailColor(float completionRatio)
        {
            Color baseColor = Color.Lerp(Color.Cyan, new Color(0, 0, 255), completionRatio);

            return baseColor;
        }

        public float MiniTrailWidth(float completionRatio) => TrailWidth(completionRatio) * 0.8f;
        public Color MiniTrailColor(float completionRatio) => Color.White;


        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            if (BloomTex == null)
                BloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            Texture2D texture = BloomTex.Value;

            Color mainColor = MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI * 0.12f) % 1, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
            Color secondaryColor = MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI * 0.12f + 0.2f) % 1, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);

            //Draw the bloom unde the trail
            Main.EntitySpriteDraw(texture, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, (mainColor * 0.1f) with { A = 0 }, 0, texture.Size() / 2f, 1.3f * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.oldPos[1] + Projectile.Size / 2f - Main.screenPosition, null, (mainColor * 0.5f) with { A = 0 }, 0, texture.Size() / 2f, 0.34f * Projectile.scale, 0, 0);


            TrailDrawer ??= new(TrailWidth, TrailColor, null, GameShaders.Misc["CalamityMod:ExobladePierce"]);
            MiniTrailDrawer ??= new(MiniTrailWidth, MiniTrailColor, null, GameShaders.Misc["CalamityMod:ExobladePierce"]);

            Main.spriteBatch.EnterShaderRegion();

            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/BasicTrail"));
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();

            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            
            TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 30, Projectile.oldRot);

            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Color.White);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Color.White);

            MiniTrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 30, Projectile.oldRot);

            //Code put in cold storage. It looks cool but not the right time for it.
            /*
            
            if (SlashTex == null)
                SlashTex = ModContent.Request<Texture2D>("CalamityMod/Particles/SlashSmear");
            Texture2D slashTex = SlashTex.Value;

            float swingOpacity = (595 - Projectile.timeLeft) / 595f;
            swingOpacity = (float)Math.Pow(Utils.GetLerpValue(0.8f, 0.6f, swingOpacity, true), 0.3);

            if (swingOpacity > 0)
            {
                float rotation = Main.GlobalTimeWrappedHourly * 7f;
                rotation += Projectile.whoAmI * MathHelper.Pi * 1.33f;

                Effect swingFX = Filters.Scene["RotateSprite"].GetShader().Shader;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, swingFX, Main.GameViewMatrix.TransformationMatrix);

                swingFX.Parameters["rotation"].SetValue(rotation);
                Main.EntitySpriteDraw(slashTex, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, secondaryColor * swingOpacity, Projectile.rotation + MathHelper.PiOver4 * 0.6f, slashTex.Size() / 2f, new Vector2(1f, 2f) * 0.5f * Projectile.scale, 0, 0);

                swingFX.Parameters["rotation"].SetValue(rotation + MathHelper.PiOver4);
                Main.EntitySpriteDraw(slashTex, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, secondaryColor * swingOpacity, Projectile.rotation - MathHelper.PiOver4 * 0.6f, slashTex.Size() / 2f, new Vector2(1f, 2f) * 0.5f * Projectile.scale, 0, 0);
            }
            */

            Main.spriteBatch.ExitShaderRegion();

            //Draw the bloom above the trail
            Main.EntitySpriteDraw(texture, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, (Color.White * 0.2f) with { A = 0 }, 0, texture.Size() / 2f, 0.78f * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.oldPos[1] + Projectile.Size / 2f - Main.screenPosition, null, (Color.White * 0.5f) with { A = 0 }, 0, texture.Size() / 2f, 0.2f * Projectile.scale, 0, 0);
            
            
            
            return false;
        }
    }
}
