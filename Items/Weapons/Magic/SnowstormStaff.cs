using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SnowstormStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snowstorm Staff");
            Tooltip.SetDefault("Fires a snowflake that follows the mouse cursor");
        }

        public override void SetDefaults()
        {
            Item.damage = 53;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.mana = 13;
            Item.width = 66;
            Item.height = 66;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item46;
            Item.shoot = ModContent.ProjectileType<Snowflake>();
            Item.shootSpeed = 7f;
        }
    }
}
