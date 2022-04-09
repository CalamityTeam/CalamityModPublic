using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Quagmire : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quagmire");
            Tooltip.SetDefault("Fires spore clouds");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 52;
            Item.knockBack = 3.5f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<QuagmireYoyo>();
            Item.shootSpeed = 10f;

            Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(gold: 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DraedonBar>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
