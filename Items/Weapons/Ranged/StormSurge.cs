using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StormSurge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Surge");
            Tooltip.SetDefault("Fear the storm\n" +
                "Does not consume ammo");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 22;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item122;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true; //GRRRRRRRRRRRRRRRR false begone
            Item.shoot = ModContent.ProjectileType<StormSurgeTornado>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StormlionMandible>().
                AddIngredient<PearlShard>(3).
                AddIngredient<SeaPrism>(7).
                AddIngredient<Navystone>(10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
