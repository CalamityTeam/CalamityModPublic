using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Aorta : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aorta");
            Tooltip.SetDefault("Fires homing blood when enemies are near\n" +
                "An exceptionally agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 29;
            Item.knockBack = 4.25f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<AortaYoyo>();
            Item.shootSpeed = 8f;

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 4);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodSample>(6).
                AddIngredient(ItemID.Vertebrae, 3).
                AddIngredient(ItemID.CrimtaneBar, 3).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
