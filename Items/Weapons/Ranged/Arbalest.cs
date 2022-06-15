using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Arbalest : ModItem
    {
        private int totalProjectiles = 1;
        private float arrowScale = 0.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arbalest");
            Tooltip.SetDefault("Fires a volley of 10 high-speed arrows\n" +
                "Arrows start off small and grow in size with continuous fire\n" +
                "Arrow damage, spread and knockback scale with arrow size");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 82;
            Item.height = 34;
            Item.useTime = 7;
            Item.reuseDelay = 30;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item5, player.Center);

            if (totalProjectiles > 4)
            {
                totalProjectiles = 1;

                if (arrowScale < 1.5f)
                    arrowScale += 0.05f;
            }

            float spreadScale = arrowScale * arrowScale;
            int spread = (int)(30f * spreadScale);
            for (int i = 0; i < totalProjectiles; i++)
            {
                float SpeedX = velocity.X + Main.rand.Next(-spread, spread + 1) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-spread, spread + 1) * 0.05f;
                int proj = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * arrowScale), knockback * arrowScale, player.whoAmI);
                Main.projectile[proj].scale = arrowScale;
                Main.projectile[proj].extraUpdates += 1;
                Main.projectile[proj].noDropItem = true;
            }

            totalProjectiles++;

            if (arrowScale >= 1.5f)
                arrowScale = 0.5f;

            return false;
        }
    }
}
