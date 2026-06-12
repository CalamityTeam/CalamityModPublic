using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Sounds;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class GalileoGladiusProj : BaseSwordHoldoutProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<GalileoGladius>();
        public Player Owner => Main.player[Projectile.owner];
        public override int swingWidth => 200;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<GalileoGladius>()).Item;
        public override string Texture => BaseItem.ModItem.Texture;
        public override int AfterImageLength => 0;
        public override int OffsetDistance { get; set; } = 90;
        public override bool drawSwordTrail => false;
        public override bool AlternateSwings => false;

        public override bool useAttackSpeed => true;

        public override int swingTime { get; set; } = 8;

        public override SoundStyle? UseSound => SoundID.Item71 with { Volume = 0.5f };

        public override void Defaults()
        {
            Projectile.extraUpdates = 3;
        }

        public override void Spawn()
        {
            angle = angle.RotatedByRandom(0.25f);
        }

        public override void AdditionalAI()
        {
            OffsetDistance = (int)MathHelper.Lerp(15, 45, SwingCompletion);
            //Spawn the large glow V
            var sparkAngle = angle.RotatedBy(MathHelper.Pi);
            int dir = MathF.Sign(sparkAngle.X);
            var color = Color.LightSkyBlue;
            if (Projectile.FinalExtraUpdate())
            {
                for (float i = -1; i <= 1; i += 2f)
                {
                    Vector2 velocity = -sparkAngle.RotatedBy(i * -0.3f) * 10f;
                    Vector2 position = Projectile.Center + new Vector2(MathHelper.Lerp(20, 93 * Projectile.scale, SwingCompletion), i * 15).RotatedBy(sparkAngle.ToRotation());
                    Particle spark = new CustomSpark(position, velocity, "CalamityMod/Particles/BloomCircle", false, 3, 0.3f, color, new Vector2(0.3f, 3f),noShrink:true);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            Lighting.AddLight(Main.player[Projectile.owner].Center, 0.96f, 0.91f, 1f);
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
        }

        public override float SwingFunction()
        {
            return 0; //Galileo stabs, not swings.
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowBlade").Value;
            var sparkAngle = angle.RotatedBy(MathHelper.Pi);

            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(tex, Projectile.Center + sparkAngle * -10 - Main.screenPosition, null, Color.SkyBlue * 0.75f, sparkAngle.ToRotation() + MathHelper.PiOver2, new Vector2(tex.Width * 0.5f,tex.Height), new Vector2(0.85f, MathHelper.Lerp(0.1f, 1.55f, SwingCompletion)) * Projectile.scale * 0.04f, SpriteEffects.None, 1);

                Main.spriteBatch.End();
            }

            var tex2 = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(tex2, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, Color.SkyBlue * 1f, Projectile.rotation, tex2.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);
            return false ;
        }

        public override void PostDraw(Color lightColor)
        {
            var tex2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/GalileoGladiusGlow").Value;
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(tex2, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, Color.SkyBlue * 0.75f, Projectile.rotation, tex2.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);
                Main.spriteBatch.End();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0)
                Owner.Calamity().StratusStarburst++;
            else
                Owner.Calamity().StarburstSpawnFrameCounter += 0.25f;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var lineCollisionLength = 192;
            var player = Main.player[Projectile.owner];
            var armcenter = player.Center - new Vector2(5 * player.direction, 2);
            var swordDir = armcenter.DirectionTo(Projectile.Center);
            var collisionline = new Vector2(lineCollisionLength / 2f, 0).RotatedBy(swordDir.ToRotation()) * Projectile.scale;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + collisionline);
            if (c && !float.IsNaN(collisionline.X) && !float.IsNaN(collisionline.Y))
                return true;
            return base.Colliding(projHitbox, targetHitbox);
        }

    }
}
