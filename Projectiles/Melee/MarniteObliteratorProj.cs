using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.GameContent;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Melee
{
    public class MarniteObliteratorProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Obliterator");
        }

        public override string Texture => "CalamityMod/Items/Tools/MarniteObliterator";
        public static Asset<Texture2D> SheenTex;
        public static Asset<Texture2D> BloomTex;

        public Player Owner => Main.player[Projectile.owner];
        public ref float MoveInIntervals => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item132, Projectile.Center);
                Projectile.soundDelay = 23;
            }

            if ((Owner.Center - Projectile.Center).Length() >= 5f)
            {
                if ((Owner.Center - Projectile.Center).Length() >= 30f)
                {
                    DelegateMethods.v3_1 = Color.Blue.ToVector3() * 0.5f;
                    Utils.PlotTileLine(Owner.Center + Owner.Center.DirectionTo(Projectile.Center) * 30f, Projectile.Center, 8f, DelegateMethods.CastLightOpen);
                }

                Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.7f);
            }

                if (MoveInIntervals > 0f)
                    MoveInIntervals -= 1f;

            if (!Owner.channel || Owner.noItems || Owner.CCed)
                Projectile.Kill();

            else if (MoveInIntervals <= 0f)
            {
                Vector2 newVelocity = Owner.Calamity().mouseWorld - Owner.Center;

                if (Main.tile[Player.tileTargetX, Player.tileTargetY].HasTile)
                {
                    newVelocity = new Vector2(Player.tileTargetX, Player.tileTargetY) * 16f + Vector2.One * 8f - Owner.Center;
                    MoveInIntervals = 2f;
                }

                newVelocity = Vector2.Lerp(newVelocity, Projectile.velocity, 0.7f);
                if (float.IsNaN(newVelocity.X) || float.IsNaN(newVelocity.Y))
                    newVelocity = -Vector2.UnitY;

                if (newVelocity.Length() < 30f)
                    newVelocity = newVelocity.SafeNormalize(-Vector2.UnitY) * 30f;

                //Tile reach is a fucking square in terraria. Can you believe that?
                int tileBoost = Owner.inventory[Owner.selectedItem].tileBoost;
                int fullRangeX = (Player.tileRangeX + tileBoost) * 16 + 11; //Why are those 2 separate variables? Whatever
                int fullRangeY = (Player.tileRangeY + tileBoost) * 16 + 11;

                newVelocity.X = Math.Clamp(newVelocity.X, -fullRangeX, fullRangeX);
                newVelocity.Y = Math.Clamp(newVelocity.Y, -fullRangeY -1, fullRangeY);

                if (newVelocity != Projectile.velocity)
                    Projectile.netUpdate = true;

                Projectile.velocity = newVelocity;
            }

            //Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Owner.gravDir - MathHelper.PiOver2);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() * Owner.gravDir - MathHelper.PiOver2 - MathHelper.PiOver4 * 0.5f * Owner.direction);

            Owner.SetDummyItemTime(2);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.MountedCenter + Projectile.velocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.active)
                return false;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            if (SheenTex == null)
                SheenTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar");
            Texture2D shineTex = SheenTex.Value;

            if (BloomTex == null)
                BloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            Texture2D bloomTex = BloomTex.Value;

            Vector2 shineScale = new Vector2(1f, 2f);
            Main.EntitySpriteDraw(bloomTex, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * 0.3f, MathHelper.PiOver2, bloomTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * 0.7f, MathHelper.PiOver2, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);



            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(9f, tex.Height / 2f);
            SpriteEffects effect = SpriteEffects.None;
            if (Owner.direction * Owner.gravDir < 0)
                effect = SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(tex, Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 10f - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effect, 0);
            return false;
        }
    }
}
