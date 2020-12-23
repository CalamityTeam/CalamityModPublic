using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MoltenAmputator : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Amputator");
            Tooltip.SetDefault("Throws a scythe that emits molten globs on enemy hits\n" +
                "Stealth strikes spawn molten globs periodically in flight and more on-hit");
        }

        public override void SafeSetDefaults()
        {
            item.width = 60;
            item.damage = 120;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.height = 60;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.shoot = ModContent.ProjectileType<MoltenAmputatorProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int boomer = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
				if (boomer.WithinBounds(Main.maxProjectiles))
					Main.projectile[boomer].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
