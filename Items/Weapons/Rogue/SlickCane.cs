using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 36;
            Item.damage = 27;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 26;
            Item.useTime = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<SlickCaneProjectile>();
            Item.shootSpeed = 22f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai0 = Main.rand.NextFloat() * Item.shootSpeed * 0.75f * (float)player.direction;
            int projectileIndex = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, ai0, 0f);
            if (projectileIndex.WithinBounds(Main.maxProjectiles))
                Main.projectile[projectileIndex].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
