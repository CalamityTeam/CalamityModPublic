using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class GammaHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Heart");
            Tooltip.SetDefault("Summons radioactive heads that are bound by your body");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item42;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.damage = 85;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.useTime = Item.useAnimation = 14;
            Item.shoot = ModContent.ProjectileType<GammaHead>();
            Item.shootSpeed = 10f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
