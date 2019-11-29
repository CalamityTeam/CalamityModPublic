using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SandDollar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Dollar");
            Tooltip.SetDefault("Stacks up to 2");
        }

        public override void SafeSetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.damage = 30;
            item.thrown = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.maxStack = 2;
            item.knockBack = 3.5f;
            item.autoReuse = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<SandDollarProj>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }

        public override bool CanUseItem(Player player)
        {
			int UseMax = item.stack - 1;

			if (player.ownedProjectileCounts[item.shoot] > UseMax)
			{
				return false;
			}
			else
			{
				return true;
			}
        }
    }
}
