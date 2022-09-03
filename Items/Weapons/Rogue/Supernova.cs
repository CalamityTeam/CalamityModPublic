using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Supernova : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supernova");
            Tooltip.SetDefault(@"Creates a massive explosion on impact
Explodes into spikes and homing energy
Stealth strikes release energy as they fly");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.damage = 675;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 24;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.shoot = ModContent.ProjectileType<SupernovaBomb>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;
            Item.rare = ModContent.RarityType<Violet>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                damage = (int)(damage * 1.08f);
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
                AddIngredient<TotalityBreakers>().
                AddIngredient<BallisticPoisonBomb>().
                AddIngredient<ShockGrenade>(200).
                AddIngredient<Penumbra>().
                AddIngredient<StarofDestruction>().
                AddIngredient<SealedSingularity>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
