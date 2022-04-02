using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Brimlance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlance");
            Tooltip.SetDefault("Creates brimstone explosions on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.damage = 58;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 19;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 56;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<BrimlanceProj>();
            item.shootSpeed = 12f;
            item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
