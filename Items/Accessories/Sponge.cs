using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class Sponge : ModItem
    {
        public override string Texture => (DateTime.Now.Month == 4 && DateTime.Now.Day == 1) ? "CalamityMod/Items/Accessories/SpongeReal" : "CalamityMod/Items/Accessories/Sponge";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Sponge");
            Tooltip.SetDefault("15% increased damage reduction\n" +
                "+30 max life and mana\n" +
                "5% increased movement and jump speed\n" +
                "Standing still boosts life and mana regen\n" +
                "Increased defense, movement speed and damage reduction while submerged in liquid\n" +
                "Enemies take damage when they hit you\n" +
                "You emit a cloud of mushroom spores when you are hit\n" +
                "6.25% of the damage from enemy attacks is absorbed and converted into healing");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 30));
        }

        public override void SetDefaults()
        {
            Item.defense = 20;
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            // Removed Giant Shell speed boost from Sponge
            // modPlayer.gShell = true;
            modPlayer.fCarapace = true;
            modPlayer.seaShell = true;
            modPlayer.absorber = true;
            modPlayer.sponge = true;
            player.statManaMax2 += 30;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Texture == "CalamityMod/Items/Accessories/Sponge")
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/SpongeShield").Value;
                spriteBatch.Draw(tex, Item.Center - Main.screenPosition + new Vector2(0f, 0f), Main.itemAnimations[Item.type].GetFrame(tex), Color.Cyan * 0.5f, 0f, new Vector2(tex.Width / 2f, (tex.Height / 30f ) * 0.8f), 1f, SpriteEffects.None, 0);
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Texture == "CalamityMod/Items/Accessories/Sponge")
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/SpongeShield").Value;
                spriteBatch.Draw(tex, position, Main.itemAnimations[Item.type].GetFrame(tex), Color.Cyan * 0.4f, 0f, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TheAbsorber>().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<CosmiliteBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
