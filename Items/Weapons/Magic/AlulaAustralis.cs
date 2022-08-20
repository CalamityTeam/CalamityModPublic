using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AlulaAustralis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alula Australis");
            Tooltip.SetDefault("Fires a beautiful aurora trailed by a star shower");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 99;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 52;
            Item.height = 52;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AuroraAustralis>();
            Item.shootSpeed = 13f;
        }
    }
}
