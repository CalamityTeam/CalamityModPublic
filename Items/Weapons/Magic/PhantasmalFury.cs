using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PhantasmalFury : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Fury");
            Tooltip.SetDefault("Casts a phantasmal bolt that explodes into more bolts");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 182;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 62;
            Item.height = 60;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.UseSound = SoundID.Item43;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhantasmalFuryProj>();
            Item.shootSpeed = 12f;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreStaff).
                AddIngredient<RuinousSoul>(2).
                AddIngredient<DarkPlasma>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
