using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TotalityBreakers : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Totality Breakers");
            Tooltip.SetDefault(@"Explodes into highly flammable black tar
Tar oils enemies and sets them alight
Stealth strikes leak tar as they fly");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.damage = 55;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 28;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<TotalityFlask>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override float StealthDamageMultiplier => 1.15f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MolotovCocktail, 50).
                AddIngredient<ConsecratedWater>().
                AddIngredient<DesecratedWater>().
                AddIngredient<SpentFuelContainer>().
                AddIngredient<SolarVeil>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
