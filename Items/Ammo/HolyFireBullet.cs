﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class HolyFireBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        internal const float ExplosionMultiplier = 0.33f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(copper: 24);
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<HolyFireBulletProj>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient(ItemID.ExplodingBullet, 100).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
