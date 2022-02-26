using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class LunicEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunic Eye");
            Tooltip.SetDefault("Fires lunic beams that reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once");
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.damage = 9;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4.5f;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            item.autoReuse = true;
            item.noMelee = true;
            item.height = 50;
            item.shoot = ModContent.ProjectileType<LunicBeam>();
            item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        // Lunic Eye scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
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
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 20);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 15);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 15);
            recipe.AddIngredient(ItemID.SunplateBlock, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
