using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 4;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = damage;
            Item.Calamity().rogue = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = knockBack;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 4;

            Item.shootSpeed = 2f;
            Item.shoot = ModContent.ProjectileType<SkyStabberProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
                return player.ownedProjectileCounts[ModContent.ProjectileType<SkyStabberProj>()] > 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<SkyStabberProj>();
                Item.shootSpeed = 2f;
                int UseMax = Item.stack;
                return player.ownedProjectileCounts[ModContent.ProjectileType<SkyStabberProj>()] < UseMax;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SkyStabberProj>(), damage, knockback, player.whoAmI);
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
                AddIngredient(ItemID.SpikyBall, 100).
                AddIngredient(ItemID.Cloud, 10).
                AddIngredient<AerialiteBar>(4).
                AddTile(TileID.SkyMill).
                Register();
        }

    }
}
