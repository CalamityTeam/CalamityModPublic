using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DynamicPursuer : RogueWeapon
    {
        public override void SetDefaults()
        {
            CalamityGlobalItem modItem = Item.Calamity();

            Item.damage = 2550;
            Item.DamageType = RogueDamageClass.Instance;

            Item.width = 30;
            Item.height = 34;
            Item.useTime = 42;
            Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();

            Item.noUseGraphic = true;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<DynamicPursuerProjectile>();
            Item.shootSpeed = 18f;

            modItem.UsesCharge = true;
            modItem.MaxCharge = 300f; // Tesla Cannon = 250f
            modItem.ChargePerUse = 0.5f; // Tesla Cannon = 0.9f
        }
        public static float StealthDmgMult = 0.3f; //So I can edit it directly via DragonLens instead of having to do math with CalTestHelpers
		public override float StealthDamageMultiplier => StealthDmgMult;
        public override float StealthVelocityMultiplier => 0.8f;

        //Stuff to be used on the projectile, but here for ease of access ingame via DragonLens
        public static float ReturnAcceleration = 0.75f;
        public static float ReturnMaxSpeed = 24f;
        public static float RicochetShootingCooldown = 1000f;
        public static float RicochetVelocityCap = 28f;
        public static float ElectricityDmgMult = 0.3f;
        public static float ElectricityCooldown = 500f;
        public static float LaserDmgMult = 0.25f;
        public static float LaserCooldown = 300f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Eradicator>().
                AddIngredient<TrackingDisk>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
