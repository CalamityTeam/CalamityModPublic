using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ManaRose : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Rose");
            Tooltip.SetDefault("Casts a mana flower that explodes into petals");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ManaBolt>();
            Item.shootSpeed = 10f;
        }

        
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NaturesGift).
                AddIngredient(ItemID.JungleRose).
                AddIngredient(ItemID.Moonglow, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
