using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class MidnightSunBeacon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Midnight Sun Beacon");
            Tooltip.SetDefault("Summons a UFO to vaporize enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 91;
            item.mana = 12;
            item.width = item.height = 32;
            item.useTime = item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item90;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MidnightSunBeaconProj>();
            item.shootSpeed = 10f;
            item.summon = true;

			item.value = CalamityGlobalItem.Rarity15BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Violet;
		}
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.XenoStaff);
            recipe.AddIngredient(ItemID.MoonlordTurretStaff);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
