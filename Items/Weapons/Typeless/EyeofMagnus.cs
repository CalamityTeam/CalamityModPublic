using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class EyeofMagnus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Magnus");
            Tooltip.SetDefault("Fires powerful beams that reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once\n" +
                "Heals mana and health on hit");
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.damage = 32;
            item.rare = ItemRarityID.Cyan;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 5f;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            item.autoReuse = true;
            item.noMelee = true;
            item.height = 50;
            item.value = CalamityGlobalItem.Rarity9BuyPrice;
            item.shoot = ModContent.ProjectileType<MagnusBeam>();
            item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        // Eye of Magnus scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float formula = 5f * (player.allDamage - 1f);
            formula += player.meleeDamage - 1f;
            formula += player.rangedDamage - 1f;
            formula += player.magicDamage - 1f;
            formula += player.minionDamage - 1f;
            formula += player.Calamity().throwingDamage - 1f;
            add += formula;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LunicEye>());
            recipe.AddIngredient(ItemID.FragmentNebula, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
