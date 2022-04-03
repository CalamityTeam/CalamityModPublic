using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Celestus : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestus");
            Tooltip.SetDefault("Throws a scythe that splits into multiple scythes on enemy hits\n" +
            "Stealth strikes reverse direction and home in on enemies after returning to the player");
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 280;
            Item.knockBack = 6f;
            Item.useAnimation = Item.useTime = 22;
            Item.Calamity().rogue = true;
            Item.autoReuse = true;
            Item.shootSpeed = 25f;
            Item.shoot = ModContent.ProjectileType<CelestusBoomerang>();

            Item.width = Item.height = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 0.4), knockBack, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AccretionDisk>()).AddIngredient(ModContent.ItemType<AlphaVirus>()).AddIngredient(ModContent.ItemType<MoltenAmputator>()).AddIngredient(ModContent.ItemType<FrostcrushValari>()).AddIngredient(ModContent.ItemType<EnchantedAxe>()).AddIngredient(ModContent.ItemType<MiracleMatter>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
