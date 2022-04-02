using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Norfleet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Norfleet");
            Tooltip.SetDefault("Fire everything!");
        }

        public override void SetDefaults()
        {
            item.damage = 354;
            item.knockBack = 15f;
            item.shootSpeed = 30f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 75;
            item.useTime = 75;
            item.reuseDelay = 0;
            item.width = 140;
            item.height = 42;
            item.UseSound = SoundID.Item92;
            item.shoot = ModContent.ProjectileType<NorfleetCannon>();
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.useTurn = false;
            item.useAmmo = AmmoID.FallenStar;
            item.autoReuse = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<NorfleetCannon>(), 0, 0f, player.whoAmI);
            return false;
        }
    }
}
