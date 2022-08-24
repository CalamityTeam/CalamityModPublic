using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DeificThunderbolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Thunderbolt");
            Tooltip.SetDefault(@"Fires a lightning bolt to electrocute enemies
The lightning bolt travels faster while it is raining
Summons lightning from the sky on impact
Stealth strikes summon more lightning and travel faster");
            SacrificeTotal = 1;
        }

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
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.DamageType = RogueDamageClass.Instance;

            Item.autoReuse = true;
            Item.shootSpeed = 13.69f;
            Item.shoot = ModContent.ProjectileType<DeificThunderboltProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 12;

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float stealthSpeedMult = 1f;
            if (player.Calamity().StealthStrikeAvailable())
                stealthSpeedMult = 1.5f;
            float rainSpeedMult = 1f;
            if (Main.raining)
                rainSpeedMult = 1.5f;

            int thunder = Projectile.NewProjectile(source, position.X, position.Y, velocity.X * rainSpeedMult * stealthSpeedMult, velocity.Y * rainSpeedMult * stealthSpeedMult, type, damage, knockback, player.whoAmI, 0f, 0f);
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
