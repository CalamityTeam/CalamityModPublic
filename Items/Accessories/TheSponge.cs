using System;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("Sponge")]
    public class TheSponge : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override string Texture => (DateTime.Now.Month == 4 && DateTime.Now.Day == 1) ? "CalamityMod/Items/Accessories/TheSpongeReal" : "CalamityMod/Items/Accessories/TheSponge";

        // TODO -- Unique sounds for The Sponge
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ShieldDurabilityMax = 180;
        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(6);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(6);

        // While active, The Sponge gives 30 defense and 30% DR
        public static int ShieldActiveDefense = 30;
        public static float ShieldActiveDamageReduction = 0.3f;


        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 30));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sponge = true;
            modPlayer.spongeShieldVisible = !hideVisual;

            if (modPlayer.SpongeShieldDurability > 0)
            {
                player.statDefense += ShieldActiveDefense;
                player.endurance += ShieldActiveDamageReduction;
            }
        }

        // In vanity, provides a visual shield but no actual functionality
        public override void UpdateVanity(Player player) => player.Calamity().spongeShieldVisible = true;

        // Renders the bubble shield over the item in the world.
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            // Will not render a shield if the April Fool's sprite is active.
            if (Texture == "CalamityMod/Items/Accessories/TheSponge")
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/TheSpongeShield").Value;
                spriteBatch.Draw(tex, Item.Center - Main.screenPosition + new Vector2(0f, 0f), Main.itemAnimations[Item.type].GetFrame(tex), Color.Cyan * 0.5f, 0f, new Vector2(tex.Width / 2f, (tex.Height / 30f ) * 0.8f), 1f, SpriteEffects.None, 0);
            }
        }

        // TODO -- Is this necessary to block the shield in-inventory on April first?
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return Texture != "CalamityMod/Items/Accessories/TheSponge";
        }

        // Renders the bubble shield over the item in a player's inventory.
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Will not render a shield if the April Fool's sprite is active.
            if (Texture == "CalamityMod/Items/Accessories/TheSponge")
            {
                float wantedScale = 0.85f;
                Vector2 drawOffset = new(-2f, -1f);

                CalamityUtils.DrawInventoryCustomScale(
                    spriteBatch,
                    texture: TextureAssets.Item[Type].Value,
                    position,
                    frame,
                    drawColor,
                    itemColor,
                    origin,
                    scale,
                    wantedScale,
                    drawOffset
                );
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/TheSpongeShield").Value;
                CalamityUtils.DrawInventoryCustomScale(
                    spriteBatch,
                    texture: tex,
                    position,
                    Main.itemAnimations[Item.type].GetFrame(tex),
                    Color.Cyan * 0.4f,
                    itemColor,
                    origin,
                    scale,
                    wantedScale,
                    drawOffset
                );
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RoverDrive>().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<CosmiliteBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
