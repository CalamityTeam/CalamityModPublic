using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AcesHigh : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ace's High");
            Tooltip.SetDefault("Fires a string of cards with varying effects based on card type\n" +
			"Hearts grant lifesteal. Spades pierce and ignore immunity frames.\n" +
			"Diamonds explode. Clubs split into three.");
        }

        public override void SetDefaults()
        {
            item.damage = 1599;
            item.ranged = true;
            item.width = 48;
            item.height = 30;
            item.useTime = 3;
            item.reuseDelay = 8;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<CardHeart>();
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ClaretCannon>());
            recipe.AddIngredient(ModContent.ItemType<Spyker>());
            recipe.AddIngredient(ModContent.ItemType<Fungicide>());
            recipe.AddIngredient(ModContent.ItemType<FantasyTalisman>(), 52);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 6);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int card = Utils.SelectRandom(Main.rand, new int[]
			{
				ModContent.ProjectileType<CardHeart>(),
				ModContent.ProjectileType<CardSpade>(),
				ModContent.ProjectileType<CardDiamond>(),
				ModContent.ProjectileType<CardClub>()
			});

            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, card, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
