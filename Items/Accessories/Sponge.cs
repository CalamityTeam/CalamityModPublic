using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Sponge : ModItem
    {
		public override string Texture => (DateTime.Now.Month == 4 && DateTime.Now.Day == 1) ? "CalamityMod/Items/Accessories/SpongeReal" : "CalamityMod/Items/Accessories/Sponge";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Sponge");
            Tooltip.SetDefault("50% increased mining speed and you emit light\n" +
                "10% increased damage reduction and increased life regen\n" +
                "Poison, Freeze, Chill, Frostburn, and Venom immunity\n" +
                "Honey-like life regen with no speed penalty, +20 max life and mana\n" +
                "Most bee/hornet enemies and projectiles do 75% damage to you\n" +
                "24% increased jump speed and 12% increased movement speed\n" +
                "Standing still boosts life and mana regen\n" +
                "Increased defense and damage reduction when submerged in liquid\n" +
                "Increased movement speed when submerged in liquid\n" +
                "Enemies take damage when they hit you\n" +
                "Taking a hit will make you move very fast for a short time\n" +
                "You emit a mushroom spore and spark explosion when you are hit\n" +
                "Enemy attacks will have part of their damage absorbed and used to heal you");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 30));
        }

        public override void SetDefaults()
        {
            item.defense = 10;
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			bool autoJump = Main.player[Main.myPlayer].autoJump;
			string jumpAmt = autoJump ? "6" : "24";
			foreach (TooltipLine line2 in list)
			{
				if (line2.mod == "Terraria" && line2.Name == "Tooltip5")
					line2.text = jumpAmt + "% increased jump speed and 12% increased movement speed";
			}

			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip12")
					{
						line2.text = "Enemy attacks will have part of their damage absorbed and used to heal you\n" +
						"Provides cold protection in Death Mode";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.beeResist = true;
            modPlayer.aSpark = true;
            modPlayer.gShell = true;
            modPlayer.fCarapace = true;
            modPlayer.seaShell = true;
            modPlayer.absorber = true;
            modPlayer.aAmpoule = true;
            player.statManaMax2 += 20;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Texture == "CalamityMod/Items/Accessories/Sponge")
            {
                Texture2D tex = ModContent.GetTexture("CalamityMod/Items/Accessories/SpongeShield");
                spriteBatch.Draw(tex, item.Center - Main.screenPosition + new Vector2(0f, 0f), Main.itemAnimations[item.type].GetFrame(tex), Color.Cyan * 0.5f, 0f, new Vector2(tex.Width / 2f, (tex.Height / 30f ) * 0.8f), 1f, SpriteEffects.None, 0);
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Texture == "CalamityMod/Items/Accessories/Sponge")
            {
                Texture2D tex = ModContent.GetTexture("CalamityMod/Items/Accessories/SpongeShield");
                spriteBatch.Draw(tex, position, Main.itemAnimations[item.type].GetFrame(tex), Color.Cyan * 0.4f, 0f, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TheAbsorber>());
            recipe.AddIngredient(ModContent.ItemType<AmbrosialAmpoule>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
