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
            Item.width = 80;
            Item.damage = 32;
            Item.rare = ItemRarityID.Cyan;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.height = 50;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.shoot = ModContent.ProjectileType<MagnusBeam>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        // Eye of Magnus scales off of all damage types simultaneously (meaning it scales 5x from universal damage boosts).
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            float formula = 5f * (player.allDamage - 1f);
            formula += player.GetDamage(DamageClass.Melee) - 1f;
            formula += player.GetDamage(DamageClass.Ranged) - 1f;
            formula += player.GetDamage(DamageClass.Magic) - 1f;
            formula += player.GetDamage(DamageClass.Summon) - 1f;
            formula += player.Calamity().throwingDamage - 1f;
            add += formula;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<LunicEye>()).AddIngredient(ItemID.FragmentNebula, 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
