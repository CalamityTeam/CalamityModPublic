using System;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EvilSmasher : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<EvilSmasherProjectile>();

        public bool IsAnimating = false;
        public int AnimationTime = 0;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 64;
            Item.height = 66;
            Item.damage = 200;
            Item.DamageType = AverageDamageClass.Instance;
            Item.useAnimation = Item.useTime = 120;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateInventory(Player player)
        {
            AnimationTime = 300;
            base.UpdateInventory(player);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (!IsAnimating)
            {
                if (AnimationTime < 300)
                    AnimationTime++;
                return;
            }
            float completion = AnimationTime / 240f;
            Item.noGrabDelay = 120;
            Item.velocity = new Vector2(0, -1f);
            int dotAmount = 0;
            for (var i = 0; i < dotAmount; i++)
            {
                var ringDir = getRingOffset(i / (float)dotAmount * -MathHelper.TwoPi + MathHelper.TwoPi * completion * 3, ySquish: 3f);
                Dust.NewDustPerfect(Item.Center + (ringDir * (completion - MathF.Pow(completion, 2)) * 256), i % 2 == 0 ? DustID.Crimson : DustID.Corruption, ringDir);
            }
            if (Main.rand.NextFloat() < completion)
            Dust.NewDustPerfect(Item.Center, Main.rand.NextBool() ? DustID.CursedTorch : DustID.IchorTorch,Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 5);
            if (completion >= 1)
            {
                for (var i = 0; i < 50; i++)
                {
                    Dust.NewDustPerfect(Item.Center, Main.rand.NextBool() ? DustID.CursedTorch : DustID.IchorTorch, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 5);
                    Dust.NewDustPerfect(Item.Center, Main.rand.NextBool() ? DustID.Crimson : DustID.Corruption, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 5);
                }
                IsAnimating = false;
                Item.velocity = new Vector2(0, -5);
            }
            AnimationTime++;
        }

        #region Textures
        private static Asset<Texture2D> GlowTex = null;
        private static Texture2D GetGlowTex()
        {
            if (GlowTex == null)
            {
                GlowTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            }
            return GlowTex.Value;
        }

        private static Asset<Texture2D> GlowBeamTex = null;
        private static Texture2D GetGlowBeamTex()
        {
            if (GlowBeamTex == null)
            {
                GlowBeamTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineFade");
            }
            return GlowBeamTex.Value;
        }
        private static Asset<Texture2D> GlowBeamCapTex = null;
        private static Texture2D GetGlowBeamCapTex()
        {
            if (GlowBeamCapTex == null)
            {
                GlowBeamCapTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineCap");
            }
            return GlowBeamCapTex.Value;
        }
        #endregion
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (IsAnimating)
            {
                var color = Color.Lerp(Color.Purple, Color.Red, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);

                using (Main.spriteBatch.Scope())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.Additive,
                        SamplerState.PointClamp,
                        DepthStencilState.None,
                        Main.Rasterizer,
                        null,
                        Main.GameViewMatrix.TransformationMatrix
                    );

                    Main.spriteBatch.Draw(GetGlowTex(), Item.Center - Main.screenPosition, null, color, 0, GetGlowTex().Size() * 0.5f, 0.1f + 0.7f * (AnimationTime / 240f), SpriteEffects.None, 0);
                    int rep = 0;
                    for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.166f)
                    {
                        Main.spriteBatch.Draw(GetGlowBeamTex(), Item.Center - Main.screenPosition, null, color, MathHelper.WrapAngle(Main.GlobalTimeWrappedHourly) + i, new(GetGlowBeamTex().Size().X * 0.5f, 0), 0f + 0.06f * (AnimationTime / 240f) * (rep == 0 ? 1 : 0.75f), SpriteEffects.None, 0);
                        rep = (rep + 1) % 2;
                    }
                    
                    Main.spriteBatch.End();
                }

                var tex = TextureAssets.Item[ItemID.Pwnhammer].Value;
                Main.spriteBatch.Draw(tex, Item.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.Black, AnimationTime / 240f), 0, tex.Size() * 0.5f, MathHelper.Lerp(1, 1.75f, AnimationTime / 240f), SpriteEffects.None, 0);
                return false;

            }
            else if (AnimationTime < 300)
            {
                float completion = 1 - (AnimationTime - 240) / 60f;
                var color = Color.Lerp(Color.Purple, Color.Red, MathF.Sin(Main.GlobalTimeWrappedHourly * 6) * 0.5f + 0.5f);

                using (Main.spriteBatch.Scope())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.Additive,
                        SamplerState.PointClamp,
                        DepthStencilState.None,
                        Main.Rasterizer,
                        null,
                        Main.GameViewMatrix.TransformationMatrix
                    );

                    Main.spriteBatch.Draw(GetGlowTex(), Item.Center - Main.screenPosition, null, color, 0, GetGlowTex().Size() * 0.5f, 0.1f + 0.7f * completion, SpriteEffects.None, 0);
                    int rep = 0;
                    for (float i = 0; i <= MathHelper.TwoPi; i += MathHelper.TwoPi * 0.166f)
                    {
                        Main.spriteBatch.Draw(GetGlowBeamTex(), Item.Center - Main.screenPosition, null, color, MathHelper.WrapAngle(Main.GlobalTimeWrappedHourly) + i, new(GetGlowBeamTex().Size().X * 0.5f, 0), 0f + 0.06f * completion * (rep == 0 ? 1 : 0.75f), SpriteEffects.None, 0);
                        rep = (rep + 1) % 2;
                    }
                    
                    Main.spriteBatch.End();
                }
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        private Vector2 getRingOffset(float rad, float xSquish = 1, float ySquish = 1)
        {
            return new Vector2((1 / xSquish) * MathF.Cos(rad), (1 / ySquish) * MathF.Sin(rad));
        }
    }

    public class EvilSmasherSacrifice : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.Pwnhammer;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (item.noGrabDelay > 0) return; //Only run once the item is fully settled
            for (var i = 0; i < 5; i++) //Loop a couple times to check different vertical positions for the altar, improving accuracy.
            {
                var tileCheckPos = item.Center.ToTileCoordinates() + new Point(0, i);
                if (Main.tile[tileCheckPos.X, tileCheckPos.Y].TileType == TileID.DemonAltar)
                {
                    if (!(Main.netMode == NetmodeID.MultiplayerClient))
                    {
                        var newItem = Main.item[Item.NewItem(item.GetSource_FromThis(), item.Hitbox, ModContent.ItemType<EvilSmasher>())];
                        newItem.noGrabDelay = 120;
                        newItem.prefix = item.prefix;
                        (newItem.ModItem as EvilSmasher).IsAnimating = true;
                    }
                    item.stack -= 1;
                    if (item.stack <= 0)
                        item.active = false;
                    return;
                }
            }
        }
    }
}
