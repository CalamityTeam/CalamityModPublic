using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 325;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 48;
            Item.height = 30;
            Item.useTime = 3;
            Item.reuseDelay = 8;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;

            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shootSpeed = 24f;
            Item.shoot = ModContent.ProjectileType<CardHeart>();
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Revolver).
                AddIngredient<ClaretCannon>().
                AddIngredient<FantasyTalisman>(52).
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int card = Utils.SelectRandom(Main.rand, new int[]
            {
                ModContent.ProjectileType<CardHeart>(),
                ModContent.ProjectileType<CardSpade>(),
                ModContent.ProjectileType<CardDiamond>(),
                ModContent.ProjectileType<CardClub>()
            });

            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, card, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
