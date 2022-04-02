using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class SylvanSlasher : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sylvan Slasher");
            Tooltip.SetDefault(@"Summons a slash attack at the cursor's position
Enemy hits build stealth and cause sword waves to fire from the player in the opposite direction
Does not consume stealth and cannot stealth strike");
        }

        public override void SafeSetDefaults()
        {
            item.width = 72;
            item.damage = 52;
            item.Calamity().rogue = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 5;
            item.knockBack = 3f;
            item.autoReuse = false;
            item.height = 78;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.shoot = ModContent.ProjectileType<SylvanSlashAttack>();
            item.shootSpeed = 24f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
