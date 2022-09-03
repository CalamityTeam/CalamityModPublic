using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HarvestStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvest Staff");
            Tooltip.SetDefault("Casts flaming pumpkins");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 46;
            Item.height = 44;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlamingPumpkin>();
            Item.shootSpeed = 10f;
            Item.scale = 0.9f;
        }

        
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Pumpkin, 20).
                AddIngredient(ItemID.FallenStar, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
