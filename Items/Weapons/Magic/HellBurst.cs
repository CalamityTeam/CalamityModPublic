using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HellBurst : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Burst");
            Tooltip.SetDefault("Casts a beam of flame");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.magic = true;
            item.mana = 14;
            item.width = 52;
            item.height = 52;
            item.useTime = 31;
            item.useAnimation = 31;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<FlameBeamTip>();
            item.shootSpeed = 32f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Flamelash);
            recipe.AddIngredient(ItemID.CrystalVileShard);
            recipe.AddIngredient(ItemID.DarkShard, 2);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
