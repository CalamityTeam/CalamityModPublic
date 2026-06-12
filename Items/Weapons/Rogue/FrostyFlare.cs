using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class FrostyFlare : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frostburn2, BuffID.Frozen];
        }
        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 22;
            Item.damage = 37;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityLightPurpleBuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<FrostyFlareProj>();
            Item.shootSpeed = 22f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int flare = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<FrostyFlareStealth>(), damage, knockback, player.whoAmI);
                if (flare.WithinBounds(Main.maxProjectiles))
                    Main.projectile[flare].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
