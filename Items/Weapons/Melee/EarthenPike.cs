using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EarthenPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earthen Pike");
            Tooltip.SetDefault("Crushes enemy defenses\n" +
                "Sprays fossil shards on use");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 90;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 25;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 60;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<EarthenPikeSpear>();
            item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
