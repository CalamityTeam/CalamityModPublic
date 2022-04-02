using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Viscera : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Viscera");
            Tooltip.SetDefault("Fires a blood beam that heals you on enemy hits\n" +
                "The more tiles and enemies the beam bounces off of or travels through the more healing the beam does");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 100;
            item.magic = true;
            item.mana = 15;
            item.width = 50;
            item.height = 52;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VisceraBeam>();
            item.shootSpeed = 6f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
