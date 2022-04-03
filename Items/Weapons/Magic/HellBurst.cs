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
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 31;
            Item.useAnimation = 31;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlameBeamTip>();
            Item.shootSpeed = 32f;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Flamelash).AddIngredient(ItemID.CrystalVileShard).AddIngredient(ItemID.DarkShard, 2).AddIngredient(ItemID.SoulofNight, 10).AddIngredient(ItemID.SoulofFright, 5).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
