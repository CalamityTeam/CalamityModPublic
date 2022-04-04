using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CarnageRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Carnage Ray");
            Tooltip.SetDefault("Fires a blood ray\n" +
                "The farther along the ray hit enemies are, the more damage they take");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item72;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodRay>();
            Item.shootSpeed = 6f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(12, 12);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.CrimsonRod).AddIngredient(ItemID.MagicMissile).AddIngredient(ItemID.WandofSparking).AddIngredient(ItemID.AmberStaff).AddIngredient(ModContent.ItemType<BloodSample>(), 15).AddIngredient(ModContent.ItemType<PurifiedGel>(), 10).AddTile(TileID.DemonAltar).Register();
        }
    }
}
