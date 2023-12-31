﻿using CalamityMod.Projectiles.Typeless;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class StarStruckWater : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Typeless";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
			ItemID.Sets.SortingPriorityTerraforming[Type] = 92; // Blood Water
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 14f;
            Item.rare = ItemRarityID.Orange;
            Item.damage = 20;
            Item.shoot = ModContent.ProjectileType<StarStruckWaterBottle>();
            Item.width = 18;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = 200;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.BottledWater, 10).
                AddIngredient<Stardust>(2).
                AddIngredient<AstralGrassSeeds>().
                Register();
        }
    }
}
