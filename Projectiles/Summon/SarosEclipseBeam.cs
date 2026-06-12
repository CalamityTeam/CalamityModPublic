using System;
using System.Collections.Generic;
using CalamityMod.Enums;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SarosEclipseBeam : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.MaxUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        Vector2 SarosPos => Owner.Center + Vector2.UnitY * (Owner.gfxOffY + Owner.gravDir * -24f);
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SarosPossession.SpawnSound with {pitch = -0.5f, MaxInstances = 5, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew}, Owner.Center);
            Projectile.rotation = SarosPos.DirectionTo(Owner.Calamity().mouseWorld).ToRotation();
        }
        public override void AI()
        {
            if (Owner.miscCounter % 10 == 0)
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with {Volume = 0.2f }, Owner.Center);
            Projectile.rotation = Projectile.rotation.AngleLerp(SarosPos.DirectionTo(Owner.Calamity().mouseWorld).ToRotation(), 0.1f);
            Projectile.Center = SarosPos + Projectile.rotation.ToRotationVector2() * Utils.Remap(Projectile.timeLeft,5,10,1600,0);
            if (Projectile.timeLeft == 5 && Owner.channel)
                Projectile.timeLeft++;
            Owner.Calamity().sarosEclipseBeamUsage += 2;
            if (Owner.itemTime <= 1)
                Owner.itemTime++;
            }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), SarosPos, Projectile.Center, 80, ref _);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Utils.Remap(Owner.Calamity().sarosEclipseBeamUsage, 0, 300, 2, 1);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 6f;
                float dustScale = Main.rand.NextFloat(3f, 5f);
                Color dustColor = Color.Lerp(Color.OrangeRed, Color.Gold, Main.rand.NextFloat(0.5f, 1f));

                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                dust.noGravity = true;
                dust.noLight = false;
                dust.noLightEmittence = true;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float FireWidthFunction(float completion, Vector2 vertexPos)
        {
            return MathHelper.Min(Utils.Remap(completion, 0, 0.1f, 0, 96), Utils.Remap(completion, 0.9f, 1f, 96, 0)) * Utils.Remap(Projectile.timeLeft, 0, 5, 0, 1) * (0.75f+MathF.Pow(1 -(Owner.Calamity().sarosEclipseBeamUsage / 300f), 3)) * (Projectile.timeLeft > 5 ? 1 - (Projectile.timeLeft - 5) / 5f : 1);
        }

        public Color FireColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = Color.Lerp(new Color(238, 226, 153), new Color(255, 191, 73), (MathF.Sin(completion*MathHelper.TwoPi + Main.GlobalTimeWrappedHourly*5)+1)*0.5f);
            return mainColor * MathF.Pow(1 - completion, 0.5f);
        }

        public float FireCoreWidthFunction(float completion, Vector2 vertexPos)
        {
            return MathHelper.Min(Utils.Remap(completion, 0, 0.1f, 0, 32), Utils.Remap(completion, 0.9f, 1f, 32, 0)) * Utils.Remap(Projectile.timeLeft, 0, 5, 0, 1) * (0.75f+MathF.Pow(1 - (Owner.Calamity().sarosEclipseBeamUsage / 300f), 3)) * (Projectile.timeLeft > 5 ? 1-(Projectile.timeLeft-5)/5f : 1);
        }

        public Color FireCoreColorFunction(float completion, Vector2 vertexPos)
        {
            return Color.Black * MathF.Pow(1 - completion, 0.5f);
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            List<Vector2> posList = [];
            //For the prim to render properly I need to divide the distance between the positions into a couple points. Just using start and end doesn't render.
            for (var i = 0; i <= 10; i++)
            {
                posList.Add(Vector2.Lerp(SarosPos, Projectile.Center, i / 10f));
            }
            var pos = posList.ToArray();

            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(pos, new(FireWidthFunction, FireColorFunction, null, true, true, GameShaders.Misc["CalamityMod:ImpFlameTrail"]), pos.Length+32);
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(pos, new(FireCoreWidthFunction, FireCoreColorFunction, null, true, true, GameShaders.Misc["CalamityMod:ImpFlameTrail"]), pos.Length+24);
        }
    }
}
