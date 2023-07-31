using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class GoldiePet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public Player Owner => Main.player[Projectile.owner];

        public ref float RotationTimer => ref Projectile.ai[0];
        public ref float SparkleTimer => ref Projectile.ai[1];
        public float MaxBloomTime = 30f;
        public float MaxSparkleTime = 150f;
        public static Color GoldColor = Color.Goldenrod;

        public override void SetStaticDefaults()
        {
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
            Lighting.AddLight(Projectile.Center, GoldColor.ToVector3() * (0.9f + 0.45f * SparkleTimer / MaxSparkleTime));

            if (SparkleTimer < MaxBloomTime)
                SparkleTimer++;
            else if (SparkleTimer > MaxBloomTime)
                SparkleTimer--;

            if (Main.rand.NextBool(6))
                Dust.QuickDust(Projectile.Center, GoldColor);

            //Find coins
            Vector2 targetLocation = Projectile.position;
            bool foundCoin = false;
            float range = 800f; //50 block range
            float magnetRange = 128f; //8 block range

            for (int itemIndex = 0; itemIndex < Main.maxItems; itemIndex++)
            {
                Item item = Main.item[itemIndex];
                if (item.active && ItemID.Sets.CommonCoin[item.type] && item.noGrabDelay == 0 && item.playerIndexTheItemIsReservedFor == Projectile.owner && 
                    ItemLoader.CanPickup(item, Main.player[item.playerIndexTheItemIsReservedFor]) && Main.player[item.playerIndexTheItemIsReservedFor].ItemSpace(item).CanTakeItemToPersonalInventory)
                {
                    float itemDist = Vector2.Distance(item.Center, Projectile.Center);
                    float distanceToPotential = Vector2.Distance(Projectile.Center, targetLocation);

                    if (itemDist > range)
                        continue;
                    
                    //Magnetize the items if close enough
                    if (itemDist <= magnetRange)
                        item.velocity = item.SafeDirectionTo(Projectile.Center) * 6f;

                    //Pick it up
                    if (Projectile.owner == Main.myPlayer && Projectile.getRect().Intersects(new Rectangle((int)item.position.X, (int)item.position.Y, item.width, item.height)))
                    {
                        Main.item[itemIndex] = Owner.GetItem(Projectile.owner, item, new GetItemSettings());
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 0f, 0f, 0f, 0, 0, 0);
                        }

                        //Max sparkle on collect
                        SparkleTimer = MaxSparkleTime;
                    }
                    //Otherwise, start chasing
                    if (distanceToPotential < itemDist)
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
                Vector2 destination = Projectile.SafeDirectionTo(targetLocation);
                destination *= 12f;
                Projectile.velocity = (Projectile.velocity * 40f + destination) / 41f;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                RotationTimer = -1f;
            }
            //Return to resting position
            else if (RotationTimer == -1f)
            {
                Vector2 restingSpot = Owner.Center + Vector2.UnitY * -80f;
                Projectile.velocity = (restingSpot - Projectile.Center) / 15f;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                
                //Start spinning again when rested
                if (Vector2.Distance(restingSpot, Projectile.Center) <= 2f)
                    RotationTimer++;
            }
            //Rotate
            else
            {
                Vector2 playerDist = Owner.Center - Projectile.Center;
                Projectile.rotation = playerDist.ToRotation() - MathHelper.PiOver2;
                Projectile.Center = Owner.Center + Vector2.UnitY.RotatedBy(RotationTimer) * -80f;
                RotationTimer += MathHelper.ToRadians(4f);

                if (Main.rand.NextBool(150) && SparkleTimer <= MaxBloomTime)
                    SparkleTimer = MaxSparkleTime * 0.6f; //Slight sparkle randomly
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
            float shinePercent = Utils.SmoothStep(MaxBloomTime, MaxSparkleTime, SparkleTimer);
            float shineScale = (float)Math.Log10(shinePercent + 0.01) + 2f;
            
            Texture2D bloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/Light").Value;
            float bloomPercent = Math.Clamp(SparkleTimer / MaxBloomTime, 0f, 5f); //Max of 500% when collecting coins
            float bloomScale = Math.Clamp(bloomPercent, 0f, 2.5f);

            if (bloomPercent > 0f)
            {
                Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
                
                Main.EntitySpriteDraw(bloomTex, Projectile.Center - Main.screenPosition, null, GoldColor * bloomPercent * 0.2f, Projectile.rotation, bloomTex.Size() / 2f, bloomPercent * Projectile.scale * 0.3f, SpriteEffects.None);
                Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition, null, GoldColor * shinePercent, Projectile.rotation, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None);

                Main.spriteBatch.ExitShaderRegion();
            }

            return true;
        }
    }
}
