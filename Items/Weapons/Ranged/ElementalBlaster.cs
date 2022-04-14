using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ElementalBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Blaster");
            Tooltip.SetDefault("Does not consume ammo\n" +
                "Fires a storm of rainbow blasts");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 104;
            Item.height = 42;
            Item.useTime = 2;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.75f;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/PlasmaBolt");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RainbowBlast>();
            Item.shootSpeed = 18f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SpectralstormCannon>()).AddIngredient(ModContent.ItemType<ClockGatlignum>()).AddIngredient(ModContent.ItemType<PaintballBlaster>()).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5).AddIngredient(ItemID.LunarBar, 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
