using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class GoldiePet : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float MovementTimer => ref Projectile.ai[0];
        public ref float ShineLevel => ref Projectile.ai[1];
        public float MaxShineTime = 30f;
        public static Color GoldColor = Color.Goldenrod;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goldie");
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (OwnerCheck())
                return;
        
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];
            Lighting.AddLight(Projectile.Center, GoldColor.ToVector3() * 0.9f);

            if (ShineLevel < MaxShineTime)
                ShineLevel++;
            else if (ShineLevel > MaxShineTime)
                ShineLevel--;

            if (Main.rand.NextBool(6))
                Dust.QuickDust(Projectile.Center, GoldColor);

            //Find coins
            Vector2 targetLocation = Projectile.position;
            bool foundCoin = false;
            float range = 800f; //50 block range

            for (int itemIndex = 0; itemIndex < Main.maxItems; itemIndex++)
            {
                Item item = Main.item[itemIndex];
                if (item.active && ItemID.Sets.CommonCoin[item.type] && item.noGrabDelay == 0 && item.playerIndexTheItemIsReservedFor == Projectile.owner && 
                    ItemLoader.CanPickup(item, Main.player[item.playerIndexTheItemIsReservedFor]) && Main.player[item.playerIndexTheItemIsReservedFor].ItemSpace(item).CanTakeItemToPersonalInventory)
                {
                    float itemDist = Vector2.Distance(item.Center, Projectile.Center);
                    float distanceToPotential = Vector2.Distance(Projectile.Center, targetLocation);
                    //Pick it up
                    if (Projectile.owner == Main.myPlayer && Projectile.getRect().Intersects(new Rectangle((int)item.position.X, (int)item.position.Y, item.width, item.height)))
                    {
                        Main.item[itemIndex] = Owner.GetItem(Projectile.owner, item, new GetItemSettings());
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 0f, 0f, 0f, 0, 0, 0);
                        }

                        //Big shine boost on collect
                        ShineLevel += 30f;
                    }
                    //Otherwise, start chasing
                    if (distanceToPotential < itemDist && itemDist < range)
                    {
                        range = itemDist;
                        targetLocation = item.Center;
                        foundCoin = true;
                    }
                }
            }

            //Chase after coins
            if (foundCoin)
            {
                float itemDist = Vector2.Distance(targetLocation, Projectile.Center);
                Vector2 destination = Projectile.SafeDirectionTo(targetLocation);
                destination *= 12f;
                Projectile.velocity = (Projectile.velocity * 40f + destination) / 41f;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                MovementTimer = -1f;
            }
            //Return to resting position
            else if (MovementTimer == -1f)
            {
                Vector2 restingSpot = Owner.Center + Vector2.UnitY * -80f;
                Projectile.velocity = (restingSpot - Projectile.Center) / 10f;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                
                //Start spinning again when rested
                if (Vector2.Distance(restingSpot, Projectile.Center) <= 2f)
                    MovementTimer++;
            }
            //Rotate
            else
            {
                Vector2 playerDist = Owner.Center - Projectile.Center;
                Projectile.rotation = playerDist.ToRotation() - MathHelper.PiOver2;
                Projectile.Center = Owner.Center + Vector2.UnitY.RotatedBy(MovementTimer) * -80f;
                MovementTimer += MathHelper.ToRadians(4f);
            }
        }

        public bool OwnerCheck()
        {
            // No logic should be run if the player is no longer active in the game.
            if (!Owner.active)
            {
                Projectile.Kill();
                return true;
            }

            if (Owner.dead)
                Owner.Calamity().thiefsDime = false;
            if (Owner.Calamity().thiefsDime)
                Projectile.timeLeft = 2;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Similar coin shining to Midas Prime coins. Thought it'd look pretty neat for a light pet.
            Texture2D shineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
            Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float shinePercent = Math.Clamp(ShineLevel / MaxShineTime, 0f, 3f); //Max of 300% when collecting coins
            Vector2 shineScale = new Vector2(1f, 3f - shinePercent * 2f);
            Color shineColor = GoldColor;

            if (shinePercent > 0f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(bloomTex, Projectile.Center - Main.screenPosition, null, shineColor * shinePercent * 0.15f, Projectile.rotation, bloomTex.Size() / 2f, shineScale * Projectile.scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition, null, shineColor * shinePercent * 0.6f, Projectile.rotation, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return true;
        }
    }
}
