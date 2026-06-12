using System;
using CalamityMod.Effects;
using Terraria.Localization;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.PixelationSystem;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Food
{
    public class DeliciousMeat : ModItem, ILocalizedModType
    {
        private static Asset<Texture2D> FadedStarRing;
        private static Asset<Texture2D> BloomFlare;
        private static int DeliciousMeatCooldown = CalamityUtils.MinutesToFrames(15);

        public new string LocalizationCategory => "Items.Potions";
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DeliciousMeatCooldown / 60).FramesToSeconds());

        public override void Load()
        {
            if (!Main.dedServ)
            {
                BloomFlare = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BloomFlare");
                FadedStarRing = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/FadedStarRing");
            }
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.IsFood[Type] = true;
            ItemID.Sets.FoodParticleColors[Type] = new Color[4] {
                new Color(27, 103, 155),
                new Color(164, 255, 253),
                new Color(250, 219, 127),
                new Color(248, 211, 179),
            };
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 40;
            Item.useAnimation = Item.useTime = 17;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);
            Item.maxStack = 1;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.buffType = BuffID.WellFed2;
            Item.buffTime = CalamityUtils.MinutesToFrames(15);
            Item.useTurn = true;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player) => player.GetModPlayer<DeliciousMeatPlayer>().deliciousMeatCooldown <= 0;
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<DeliciousMeatPlayer>().deliciousMeatCooldown = DeliciousMeatCooldown;
            return null;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Spawn some cute sparkles for added effect when dropped in the world.
            if (Main.rand.NextBool(45))
            {
                int sparkleAmt = Main.rand.Next(1, 3);
                for (int i = 0; i < sparkleAmt; i++)
                {
                    Vector2 spawnPosition = Item.Center + Main.rand.NextVector2Circular(Item.width, Item.height);
                    Color drawColorBlue = Color.Lerp(new Color(44, 166, 247), new Color(123, 197, 247), Main.rand.NextFloat());
                    Color drawColorYellow = Color.Lerp(new Color(249, 197, 42), new Color(249, 221, 142), Main.rand.NextFloat());
                    Color sparkleColor = Utils.SelectRandom(Main.rand, drawColorBlue, drawColorYellow);

                    QuickSparkleParticle sparkle = new(spawnPosition, Vector2.Zero, sparkleColor, Main.rand.NextFloat(0.2f, 0.3f), Main.rand.Next(30, 45));
                    GeneralParticleHandler.QueueParticleForNextFrame(sparkle, true);
                }
            }

            // Additional glowy effects similar to Divine Swine itself.
            Main.GetItemDrawFrame(Item.type, out _, out Rectangle itemFrame);
            Vector2 origin = itemFrame.Size() * 0.5f;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0f, origin.Y);
            float sineWave = MathF.Sin((float)Main.timeForVisualEffects / 180f) * 0.5f + 0.5f;
            Color drawColor = Color.White * MathHelper.Lerp(0.7f, 0f, sineWave);

            spriteBatch.End(out var snapshot);

            using var pixelationLease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetDescriptor.Default);
            using (pixelationLease.Scope(clearColor: Color.Transparent))
            {
                Effect chromaAbberShader = CalamityShaders.ChromaticAbberationShader.Value;
                chromaAbberShader.Parameters["abberationStrength"].SetValue(10f);
                chromaAbberShader.Parameters["impactPosition"].SetValue(drawPosition);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, chromaAbberShader, PixelationManager.PixelationMatrix);

                float starCircleScale = MathHelper.Lerp(0.3f, 0.15f, sineWave);
                float starCircleRotation = (float)(Main.timeForVisualEffects / 720f) + whoAmI;
                spriteBatch.Draw(FadedStarRing.Value, drawPosition, null, drawColor with { A = 0 }, starCircleRotation, FadedStarRing.Size() * 0.5f, starCircleScale, 0, 0f);

                float bloomFlareScale = MathHelper.Lerp(0.15f, 0.05f, sineWave);
                float bloomFlareRotation = (float)(Main.timeForVisualEffects / 540f) + whoAmI;
                spriteBatch.Draw(BloomFlare.Value, drawPosition, null, drawColor with { A = 0 }, -bloomFlareRotation, BloomFlare.Size() * 0.5f, bloomFlareScale, 0, 0f);

                spriteBatch.End();
            }

            spriteBatch.Begin(snapshot);
            spriteBatch.Draw(pixelationLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);

            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.LocalPlayer.GetModPlayer<DeliciousMeatPlayer>().deliciousMeatCooldown <= 0)
                return;

            Vector2 barScale = new Vector2(1.25f, 1f);
            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

            Vector2 drawPos = position + new Vector2((frame.Width - barBG.Width * 0.95f) * scale, (frame.Height - 3f) * scale);
            Rectangle frameCrop = new Rectangle(0, 0, (int)((1 - Main.LocalPlayer.GetModPlayer<DeliciousMeatPlayer>().deliciousMeatCooldown / (float)DeliciousMeatCooldown) * barFG.Width), barFG.Height);
            Color colorBG = Color.Black;
            Color colorFG = Color.White;

            spriteBatch.Draw(barBG, drawPos, null, colorBG, 0f, origin, scale * barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, colorFG * 0.8f, 0f, origin, scale * barScale, 0f, 0f);
        }
    }

    public class DeliciousMeatPlayer : ModPlayer
    {
        public int deliciousMeatCooldown = 0;

        public override void PostUpdateMiscEffects()
        {
            if (deliciousMeatCooldown > 0)
                deliciousMeatCooldown--;
        }
    }
}
