using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SpentFuelContainer : RogueWeapon
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spent Fuel Container");
            Tooltip.SetDefault("War Never Changes\n" + //Fallout reference breh, pls don't fall out with me :cri:
                               "Throws a fuel container with trace amounts of plutonium that causes a nuclear explosion\n" +
                               "The explosion does not occur if there are no tiles below it\n" +
                               "Stealth strikes leave a lingering irradiated zone after the explosion dissipates");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 50;
            item.width = 22;
            item.height = 24;
            item.useAnimation = 35;
            item.useTime = 35;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SpentFuelContainerProjectile>();
            item.shootSpeed = 15f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            bool stealthAvailable = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<SpentFuelContainerProjectile>(), damage, knockBack, player.whoAmI, stealthAvailable ? 1f : 0f);
            if (stealthAvailable && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}
