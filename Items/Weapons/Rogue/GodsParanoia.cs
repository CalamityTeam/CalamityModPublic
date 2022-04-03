using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
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
            Tooltip.SetDefault(@"Shoots a speedy homing spiky ball. Stacks up to 10.
Attaches to enemies and summons a localized storm of god slayer kunai
Stealth strikes home in faster and summon kunai at a faster rate
Right click to delete all existing spiky balls");
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
            Item.value = Item.buyPrice(0, 18, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 10;

            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
                return player.ownedProjectileCounts[ModContent.ProjectileType<GodsParanoiaProj>()] > 0;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<GodsParanoiaProj>();
                Item.shootSpeed = 5f;
                int UseMax = Item.stack;
                return player.ownedProjectileCounts[ModContent.ProjectileType<GodsParanoiaProj>()] < UseMax;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.killSpikyBalls = false;
            if (modPlayer.StealthStrikeAvailable()) //setting the stealth strike
            {
                damage = (int)(damage * 1.345);
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
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
            CreateRecipe(1).AddIngredient(ItemID.SpikyBall, 200).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 1).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
