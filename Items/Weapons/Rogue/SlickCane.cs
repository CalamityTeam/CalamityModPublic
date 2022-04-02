using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SlickCane : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slick Cane");
            Tooltip.SetDefault("Swipes a cane that steals money from enemies.\n" +
                               "Stealth strikes gives a 1 in 15 chance for enemies to drop 1-3 gold coins when hit\n" +
                               "'Economy at its finest'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 36;
            item.damage = 27;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.melee = true;
            item.useAnimation = 26;
            item.useTime = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.shoot = ModContent.ProjectileType<SlickCaneProjectile>();
            item.shootSpeed = 22f;
            item.Calamity().rogue = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai0 = Main.rand.NextFloat() * item.shootSpeed * 0.75f * (float)player.direction;
            int projectileIndex = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, ai0, 0f);
            if (projectileIndex.WithinBounds(Main.maxProjectiles))
                Main.projectile[projectileIndex].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
