using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Sounds;

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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = AverageDamageClass.Instance;
            Item.width = 80;
            Item.damage = 32;
            Item.rare = ItemRarityID.Cyan;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5f;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.height = 50;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.shoot = ModContent.ProjectileType<MagnusBeam>();
            Item.shootSpeed = 12f;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ClasslessWeapon;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LunicEye>().
                AddIngredient(ItemID.FragmentNebula, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
