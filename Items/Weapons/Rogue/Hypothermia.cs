using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Hypothermia : RogueWeapon
    {
		private int counter = 0;
		private bool justStealthStriked = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hypothermia");
            Tooltip.SetDefault("Fires a constant barrage of black ice shards\n" +
                               "Stealth strikes launch a chain short ranged ice chunks that shatter into ice shards");
        }

        public override void SafeSetDefaults()
        {
            item.width = 46;
            item.height = 32;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item9;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;

            item.damage = 269;
            item.useAnimation = 21;
            item.useTime = 3;
            item.crit = 16;
            item.knockBack = 3f;
            item.shoot = ModContent.ProjectileType<HypothermiaShard>();
            item.shootSpeed = 8f;

            item.Calamity().postMoonLordRarity = 14;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			counter++;
			if (counter >= 7)
				counter = 0;

            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<HypothermiaChunk>(), damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
				justStealthStriked = true;
                return false;
            }
			if (justStealthStriked == true) //this is Ben, no I don't know how what I did translates to what happened in game but we're rolling with it
			{
				if (counter == 1 || counter == 2 || counter == 3 || counter == 4 || counter == 5 || counter == 6)
				{
					if (counter == 6)
					{
						justStealthStriked = false;
					}
					return false;
				}
			}
			else
			{
				int num6 = Main.rand.Next(1, 3);
				for (int index = 0; index < num6; ++index)
				{
					float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
					float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
					Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
				}
			}	
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 24);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 6);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
