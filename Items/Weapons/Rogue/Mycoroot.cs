using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Mycoroot : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 12;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<MycorootProj>();
            Item.shootSpeed = 20f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile root = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable())
            {
                root.Calamity().stealthStrike = true;
                for (int i = 0; i < 12; i++)
                {
                    Vector2 spreadVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5f)) * Main.rand.NextFloat(0.8f, 1.2f);
                    Projectile.NewProjectile(source, player.Center, spreadVelocity, ModContent.ProjectileType<ShroomerangSpore>(), damage, knockback, player.whoAmI, 0f, 1f, 1f);
                }
                foreach (Player other in Main.ActivePlayers)
                {
                    if (other.dead)
                        continue;

                    if ((other.team == player.team && player.team != 0) || player.whoAmI == other.whoAmI)
                    {
                        if (player.Distance(other.Center) <= 800f)
                            other.AddBuff(ModContent.BuffType<Mushy>(), 900, quiet: false);
                    }
                }
            }
            return false;
        }
    }
}
