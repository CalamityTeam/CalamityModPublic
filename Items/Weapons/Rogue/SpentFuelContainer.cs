using Terraria.DataStructures;
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
            Item.damage = 50;
            Item.width = 22;
            Item.height = 24;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpentFuelContainerProjectile>();
            Item.shootSpeed = 15f;
            Item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            bool stealthAvailable = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpentFuelContainerProjectile>(), damage, knockback, player.whoAmI, stealthAvailable ? 1f : 0f);
            if (stealthAvailable && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }
    }
}
