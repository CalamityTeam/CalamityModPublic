using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    [LegacyName("DeificThunderbolt")]
    public class TwistingThunder : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 466;
            Item.knockBack = 10f;

            Item.width = 56;
            Item.height = 56;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.DamageType = RogueDamageClass.Instance;

            Item.autoReuse = true;
            Item.shootSpeed = 13.69f;
            Item.shoot = ModContent.ProjectileType<TwistingThunderProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 12;
        public override float StealthVelocityMultiplier => 1.5f;

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (Main.raining)
				velocity = velocity * 1.5f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int thunder = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && thunder.WithinBounds(Main.maxProjectiles)) //setting the stealth strike
            {
                Main.projectile[thunder].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormfrontRazor>().
                AddIngredient<ArmoredShell>(3).
                AddIngredient<UnholyEssence>(15).
                AddIngredient<CoreofSunlight>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
