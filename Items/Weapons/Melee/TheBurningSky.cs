using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheBurningSky : ModItem
    {
        private const int ProjectilesPerBarrage = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Burning Sky");
            Tooltip.SetDefault("Hold the blade to the sky, and witness Armageddon");

            // Visually a sword, but with no true melee capability. The Burning Sky is held out like a staff.
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 146;
            Item.damage = 244;
            Item.knockBack = 2.5f;

            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item105;

            Item.shoot = ModContent.ProjectileType<BurningMeteor>();
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Every time the item spawns more meteors, play a violent, bass heavy sound to add onto Star Wrath's use sound.
            SoundEngine.PlaySound(SoundID.Item70, player.Center);

            float speed = velocity.Length();
            for (int i = 0; i < ProjectilesPerBarrage; ++i)
            {
                float randomSpeed = speed * Main.rand.NextFloat(0.7f, 1.4f);
                CalamityUtils.ProjectileRain(source, Main.MouseWorld, 290f, 130f, 850f, 1100f, randomSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
        public override void OnHitPvp(Player player, Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
    }
}
