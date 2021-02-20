using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class UrchinStinger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Stinger");
            Tooltip.SetDefault("Stealth strikes stick to enemies while releasing sulfuric bubbles");
        }

        public override void SafeSetDefaults()
        {
            item.width = 10;
            item.damage = 17;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 14;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 26;
            item.maxStack = 999;
            item.value = 200;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<UrchinStingerProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<UrchinStingerProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
            if (player.Calamity().StealthStrikeAvailable())
                Main.projectile[proj].Calamity().stealthStrike = true;
            return false;
        }
    }
}
