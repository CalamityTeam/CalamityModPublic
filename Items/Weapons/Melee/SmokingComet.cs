using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SmokingComet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smoking Comet");
            Tooltip.SetDefault("Rains stars from the sky\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 40;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 14;
            Item.knockBack = 1.5f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<SmokingCometYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Green;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Amethyst, 5).
                AddIngredient(ItemID.PinkGel, 10).
                AddIngredient(ItemID.FallenStar, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
