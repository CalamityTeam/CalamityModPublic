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
            item.damage = 53;
            item.magic = true;
            item.channel = true;
            item.mana = 13;
            item.width = 66;
            item.height = 66;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item46;
            item.shoot = ModContent.ProjectileType<Snowflake>();
            item.shootSpeed = 7f;
        }
    }
}
