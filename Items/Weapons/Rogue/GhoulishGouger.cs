using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class GhoulishGouger : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 68;

            Item.damage = 166;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 6;
            Item.useAnimation = 24;
            Item.reuseDelay = 11;
            Item.useLimitPerAnimation = 4;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GhoulishGougerBoomerang>();
            Item.shootSpeed = 16f;
            Item.DamageType = RogueDamageClass.Instance;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/GhoulishGougerGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
