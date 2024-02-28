using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Melee
{
    public class Exobeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public int TargetIndex = -1;

        public static float MaxWidth = 30;
        public ref float Time => ref Projectile.ai[0];

        public static Asset<Texture2D> BloomTex;
        public static Asset<Texture2D> SlashTex;
        public static Asset<Texture2D> TrailTex;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360;
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
                if (TargetIndex >= 0)
                {
                    if (!Main.npc[TargetIndex].active || !Main.npc[TargetIndex].CanBeChasedBy())
                        TargetIndex = -1;

                    else
                    {
                        Vector2 idealVelocity = Projectile.SafeDirectionTo(Main.npc[TargetIndex].Center) * (Projectile.velocity.Length() + 3.5f);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, 0.08f);
                    }
                }

                if (TargetIndex == -1)
                {
                    NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f, false);
                    if (potentialTarget != null)
                        TargetIndex = potentialTarget.whoAmI;

                    else
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Exoblade.BeamHitSound, target.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                int slash = Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity * 0.1f, ModContent.ProjectileType<ExobeamSlashCreator>(), Projectile.damage, 0f, Projectile.owner, target.whoAmI, Projectile.velocity.ToRotation());
                if (Main.projectile.IndexInRange(slash))
                    Main.projectile[slash].timeLeft = 20;
            }

            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White with { A = 0 } * Projectile.Opacity;


        public float TrailWidth(float completionRatio)
        {
            float width = Utils.GetLerpValue(1f, 0.4f, completionRatio, true) * (float)Math.Sin(Math.Acos(1 - Utils.GetLerpValue(0f, 0.15f, completionRatio, true)));

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

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            float bladeScale = Utils.GetLerpValue(3f, 13f, Projectile.velocity.Length(), true) * 1.2f;

            //Draw the blade.
            Main.EntitySpriteDraw(texture, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation + MathHelper.PiOver4, texture.Size() / 2f, bladeScale * Projectile.scale, 0, 0);

            if (BloomTex == null)
                BloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            Texture2D bloomTex = BloomTex.Value;

            Color mainColor = MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI * 0.12f) % 1, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
            Color secondaryColor = MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI * 0.12f + 0.2f) % 1, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);

            //Draw the bloom unde the trail
            Main.EntitySpriteDraw(bloomTex, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, (mainColor * 0.1f) with { A = 0 }, 0, bloomTex.Size() / 2f, 1.3f * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(bloomTex, Projectile.oldPos[1] + Projectile.Size / 2f - Main.screenPosition, null, (mainColor * 0.5f) with { A = 0 }, 0, bloomTex.Size() / 2f, 0.34f * Projectile.scale, 0, 0);

            Main.spriteBatch.EnterShaderRegion();

            if (TrailTex == null)
                TrailTex = Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail");

            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(TrailTex);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();

            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            
            PrimitiveSet.Prepare(Projectile.oldPos, new(TrailWidth, TrailColor, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Color.White);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Color.White);

            PrimitiveSet.Prepare(Projectile.oldPos, new(MiniTrailWidth, MiniTrailColor, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            Main.spriteBatch.ExitShaderRegion();

            //Draw the bloom above the trail
            Main.EntitySpriteDraw(bloomTex, Projectile.oldPos[2] + Projectile.Size / 2f - Main.screenPosition, null, (Color.White * 0.2f) with { A = 0 }, 0, bloomTex.Size() / 2f, 0.78f * Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(bloomTex, Projectile.oldPos[1] + Projectile.Size / 2f - Main.screenPosition, null, (Color.White * 0.5f) with { A = 0 }, 0, bloomTex.Size() / 2f, 0.2f * Projectile.scale, 0, 0);         
            return false;
        }
    }
}
