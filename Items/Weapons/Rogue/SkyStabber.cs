using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SkyStabber : RogueWeapon
    {
        private static int damage = 50;
        private static int knockBack = 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Stabber");
            Tooltip.SetDefault("Shoots a gravity-defying spiky ball. Stacks up to 4.\n" +
                "Stealth strikes make the balls rain feathers onto enemies when they hit\n" +
                "Right click to delete all existing spiky balls");
        }

        public override void SafeSetDefaults()
        {
            item.damage = damage;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.width = 1;
            item.height = 1;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = knockBack;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.maxStack = 4;

            item.shootSpeed = 2f;
            item.shoot = ModContent.ProjectileType<SkyStabberProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 4;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.shoot = ProjectileID.None;
                item.shootSpeed = 0f;
                return player.ownedProjectileCounts[ModContent.ProjectileType<SkyStabberProj>()] > 0;
            }
            else
            {
                item.shoot = ModContent.ProjectileType<SkyStabberProj>();
                item.shootSpeed = 2f;
                int UseMax = item.stack;
                return player.ownedProjectileCounts[ModContent.ProjectileType<SkyStabberProj>()] < UseMax;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<SkyStabberProj>(), damage, knockBack, player.whoAmI);
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
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.SpikyBall, 100);
            recipe.AddIngredient(ItemID.Cloud, 10);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 4);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

    }
}
