using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class CrystylCrusher : ModItem
    {
		private static int PickPower = 5000;
		private static float LaserSpeed = 14f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystyl Crusher");
            Tooltip.SetDefault("Gotta dig faster, gotta go deeper\n" +
				"5000% pickaxe power\n" +
				"Right click to swing normally");
			Item.staff[item.type] = true;
		}

        public override void SetDefaults()
        {
            item.damage = 2000;
            item.melee = true;
			item.noMelee = true;
			item.channel = true;
			item.crit += 25;
            item.width = 70;
            item.height = 70;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9f;
			item.shootSpeed = 14f;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/CrystylCharge");
			item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
        }

		public override Vector2? HoldoutOrigin()
		{
			if (item.pick == PickPower)
				return null;
			return new Vector2(10, 10);
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.pick = PickPower;
				item.shoot = ProjectileID.None;
				item.shootSpeed = 0f;
				item.tileBoost = 50;
				item.UseSound = SoundID.Item1;
				item.useStyle = ItemUseStyleID.SwingThrow;
				item.useTime = 2;
				item.useAnimation = 2;
				item.useTurn = true;
				item.autoReuse = true;
				item.noMelee = false;
				item.channel = false;
			}
			else
			{
				item.pick = 0;
				item.shoot = ModContent.ProjectileType<CrystylCrusherRay>();
				item.shootSpeed = LaserSpeed;
				item.tileBoost = 0;
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/CrystylCharge");
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.useTime = 20;
				item.useAnimation = 20;
				item.useTurn = false;
				item.autoReuse = false;
				item.noMelee = true;
				item.channel = true;
			}
			return base.CanUseItem(player);
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (player.altFunctionUse == 2)
				return;

			if (Main.rand.NextBool(3))
			{
				int num307 = Main.rand.Next(3);
				if (num307 == 0)
				{
					num307 = 173;
				}
				else if (num307 == 1)
				{
					num307 = 57;
				}
				else
				{
					num307 = 58;
				}
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, num307);
			}
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("LunarPickaxe");
            recipe.AddIngredient(ModContent.ItemType<BlossomPickaxe>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
