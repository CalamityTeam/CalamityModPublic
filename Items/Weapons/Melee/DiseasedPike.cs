using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DiseasedPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diseased Pike");
            Tooltip.SetDefault("Fires plague seekers on hit");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 65;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 20;
            item.knockBack = 8.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.shoot = ModContent.ProjectileType<DiseasedPikeSpear>();
            item.shootSpeed = 10f;
            item.Calamity().trueMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
