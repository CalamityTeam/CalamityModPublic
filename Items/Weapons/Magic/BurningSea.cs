using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BurningSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Sea");
            Tooltip.SetDefault("Fires a bouncing brimstone fireball that splits into homing fireballs upon collision with water");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 72;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneFireball>();
            Item.shootSpeed = 15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient<UnholyCore>(5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
