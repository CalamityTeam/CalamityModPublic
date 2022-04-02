using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CosmicDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Discharge");
            Tooltip.SetDefault("Striking an enemy with the whip causes glacial explosions and grants the player the cosmic freeze buff\n" +
                "This buff gives the player increased life regen while standing still and freezes enemies near the player");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 52;
            item.damage = 400;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 0.5f;
            item.UseSound = SoundID.Item122;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<CosmicDischargeFlail>();

            item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, ai3);
            return false;
        }
    }
}
