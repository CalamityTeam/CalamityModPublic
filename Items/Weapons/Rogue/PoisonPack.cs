using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class PoisonPack : ModItem
    {
        private static int baseDamage = 20;
        private static float baseKnockback = 1.8f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Pack");
            Tooltip.SetDefault("Throws a poisonous spiky ball. Stacks up to 3.\n" +
                "Stealth strikes cause the balls to release spore clouds\n" +
                "Right click to delete all existing spiky balls");
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.damage = baseDamage;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 14;
            Item.height = 14;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = baseKnockback;
            Item.value = Item.buyPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 3;

            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<PoisonBol>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
                return player.ownedProjectileCounts[ModContent.ProjectileType<PoisonBol>()] > 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<PoisonBol>();
                Item.shootSpeed = 7f;
                int UseMax = Item.stack;
                return player.ownedProjectileCounts[ModContent.ProjectileType<PoisonBol>()] < UseMax;
            }
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = true;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpikyBall, 50).
                AddIngredient(ItemID.JungleSpores, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
