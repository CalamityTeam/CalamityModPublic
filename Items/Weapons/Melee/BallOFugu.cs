using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BallOFugu : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball O' Fugu");
            Tooltip.SetDefault("Throws a fish that spews homing spikes");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.melee = true;
            item.width = 30;
            item.height = 10;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 8f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<BallOFuguProj>();
            item.shootSpeed = 12f;
        }
    }
}
