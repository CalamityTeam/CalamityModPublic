using CalamityMod.CalPlayer;
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
    public class GodsParanoia : RogueWeapon
    {
        private static int damage = 98;
        private static int knockBack = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God's Paranoia");
            Tooltip.SetDefault(@"Hurls up to 10 speedy homing spiky balls
Attaches to enemies and summons a localized storm of god slayer kunai
Stealth strikes home in faster and summon kunai at a faster rate
Right click to delete all existing spiky balls");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = damage;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 20;
            Item.height = 18;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = knockBack;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                return player.ownedProjectileCounts[ModContent.ProjectileType<GodsParanoiaProj>()] > 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();
                return player.ownedProjectileCounts[ModContent.ProjectileType<GodsParanoiaProj>()] < 10;
            }
        }

		public override float StealthDamageMultiplier => 1.345f;

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
                AddIngredient(ItemID.SpikyBall, 200).
                AddIngredient<CosmiliteBar>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
