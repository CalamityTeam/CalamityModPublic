using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class BensUmbrella : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Temporal Umbrella");
            Tooltip.SetDefault("Surprisingly sturdy, I reckon this could defeat the Mafia in a single blow\n" +
                "Summons a magic hat to hover above your head\n" +
                "The hat will release a variety of objects to assault your foes\n" +
                "Requires 5 minion slots to use and there can only be one hat");
        }

        public override void SetDefaults()
        {
            item.damage = 193;
            item.knockBack = 1f;
            item.mana = 99;
            item.useTime = item.useAnimation = 10;
            item.summon = true;
            item.shootSpeed = 0f;
            item.shoot = ModContent.ProjectileType<MagicHat>();

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 74;
            item.height = 72;
            item.noMelee = true;
            item.UseSound = SoundID.Item68;
            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && player.maxMinions >= 5;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SpikecragStaff>());
            recipe.AddIngredient(ModContent.ItemType<SarosPossession>());
            recipe.AddIngredient(ItemID.Umbrella);
            recipe.AddIngredient(ItemID.TopHat);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
